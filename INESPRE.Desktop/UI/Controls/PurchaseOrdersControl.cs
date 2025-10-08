using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using INESPRE.Desktop.Api;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.UI.Controls
{
    public class PurchaseOrdersControl : UserControl
    {
        private readonly PurchaseOrdersApi api = new();
        private readonly ProducersApi producersApi = new();
        private readonly EventsApi eventsApi = new();

        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        // Filtros
        private readonly TextBox txtProducerId = new() { PlaceholderText = "ProducerId", Width = 90 };
        private readonly ComboBox cbProducerF = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly ComboBox cbStatusF = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        // Editor
        private readonly TextBox txtProd = new() { PlaceholderText = "ProducerId", Width = 90 };
        private readonly ComboBox cbProducer = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly ComboBox cbEvent = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly DateTimePicker dtExpected = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly TextBox txtTotal = new() { PlaceholderText = "Total (0.00)", Width = 100 };
        private readonly TextBox txtNotes = new() { PlaceholderText = "Notas", Width = 220 };
        private readonly ComboBox cbStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<PurchaseOrderDto> _all = new();
        private List<ProducerDto> _producers = new();
        private List<EventDto> _events = new();
        private int? _id = null;

        public PurchaseOrdersControl()
        {
            // combos de estado
            cbStatusF.Items.AddRange(new[] { "(Todos)", "PENDIENTE", "PAGADA", "CERRADA" }); cbStatusF.SelectedIndex = 0;
            cbStatus.Items.AddRange(new[] { "PENDIENTE", "PAGADA", "CERRADA" }); cbStatus.SelectedIndex = 0;

            // tool/filtro bar
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] {
                new Label{Text="Prod.",AutoSize=true,Padding=new Padding(0,6,0,0)},
                cbProducerF,
                txtProducerId,
                new Label{Text="Desde",AutoSize=true,Padding=new Padding(8,6,0,0)}, dtFrom,
                new Label{Text="Hasta",AutoSize=true,Padding=new Padding(8,6,0,0)}, dtTo,
                cbStatusF, btnBuscar, btnLimpiar
            });
            Controls.Add(grid);
            Controls.Add(tools);

            // editor bar
            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] {
                cbProducer, txtProd,
                new Label { Text="F. Esperada", AutoSize=true, Padding=new Padding(8,6,0,0)}, dtExpected,
                txtTotal, txtNotes,
                new Label { Text="Evento", AutoSize=true, Padding=new Padding(8,6,0,0)}, cbEvent,
                cbStatus, btnCreate, btnUpdate, btnDelete
            });
            Controls.Add(editor);

            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            // grid
            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "POId", HeaderText = "PO", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProducerId", HeaderText = "ProducerId", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventId", HeaderText = "Evento", Width = 80 }); // <— visible
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisplayDate", HeaderText = "Fecha", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StatusText", HeaderText = "Status", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

            // events
            grid.SelectionChanged += (_, __) => FromSelection();
            btnBuscar.Click += (_, __) => ApplyFilter();
            btnLimpiar.Click += async (_, __) =>
            {
                txtProducerId.Text = ""; cbProducerF.SelectedIndex = 0; cbStatusF.SelectedIndex = 0;
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                await LoadData();
            };

            cbProducerF.SelectedIndexChanged += (_, __) =>
            {
                if (cbProducerF.SelectedItem is Item pi)
                    txtProducerId.Text = pi.Id == 0 ? "" : pi.Id.ToString();
                ApplyFilter();
            };
            cbProducer.SelectedIndexChanged += (_, __) =>
            {
                if (cbProducer.SelectedItem is Item pi)
                    txtProd.Text = pi.Id == 0 ? "" : pi.Id.ToString();
            };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) => { if (Visible && !loaded) { loaded = true; await LoadData(); } };
        }

        private sealed record Item(int Id, string Text) { public override string ToString() => Text; }
        private static DateTime Clamp(DateTime value, DateTime min, DateTime max)
            => value < min ? min : (value > max ? max : value);

        private async Task LoadData()
        {
            var poTask = api.GetAllAsync();
            var prTask = producersApi.GetAllAsync();
            var evTask = eventsApi.GetAllAsync();
            await Task.WhenAll(poTask, prTask, evTask);

            _all = poTask.Result ?? new List<PurchaseOrderDto>();
            _producers = prTask.Result ?? new List<ProducerDto>();
            _events = evTask.Result ?? new List<EventDto>();

            // productores
            var prodFilter = new List<Item> { new Item(0, "(Todos)") };
            prodFilter.AddRange(_producers.Select(p => new Item(p.ProducerId, string.IsNullOrWhiteSpace(p.Name) ? $"#{p.ProducerId}" : p.Name!)));
            cbProducerF.DataSource = prodFilter; cbProducerF.SelectedIndex = 0;

            var prodEditor = _producers.Select(p => new Item(p.ProducerId, string.IsNullOrWhiteSpace(p.Name) ? $"#{p.ProducerId}" : p.Name!)).ToList();
            if (prodEditor.Count == 0) prodEditor.Add(new Item(0, "(sin productores)"));
            cbProducer.DataSource = prodEditor;

            // eventos
            var evEditor = new List<Item> { new Item(0, "(Sin evento)") };
            evEditor.AddRange(_events.Select(e => new Item(e.EventId, string.IsNullOrWhiteSpace(e.Name) ? $"Evento {e.EventId}" : e.Name!)));
            cbEvent.DataSource = evEditor; cbEvent.SelectedIndex = 0;

            // rango de fechas según datos (usando DisplayDate)
            var capMin = dtFrom.MinDate.Date; var capMax = dtFrom.MaxDate.Date;
            var validDates = _all.Select(x => x.DisplayDate.Date).Where(d => d >= capMin && d <= capMax).ToList();
            if (validDates.Count > 0)
            {
                var min = validDates.Min(); var max = validDates.Max();
                dtFrom.Value = Clamp(min, dtFrom.MinDate, dtFrom.MaxDate);
                dtTo.Value   = Clamp(max, dtTo.MinDate, dtTo.MaxDate);
                if (dtFrom.Value > dtTo.Value) (dtFrom.Value, dtTo.Value) = (dtTo.Value, dtFrom.Value);
            }
            else
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value   = DateTime.Today;
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<PurchaseOrderDto> q = _all;

            // Producer
            if (int.TryParse(txtProducerId.Text.Trim(), out var pid) && pid > 0)
                q = q.Where(x => x.ProducerId == pid);

            // Fecha (usamos DisplayDate)
            var from = dtFrom.Value.Date;
            var to = dtTo.Value.Date;
            if (from > to) (from, to) = (to, from);
            q = q.Where(x => x.DisplayDate.Date >= from && x.DisplayDate.Date <= to);

            // Status
            if (cbStatusF.SelectedIndex > 0)
            {
                var wanted = cbStatusF.SelectedItem!.ToString();
                q = q.Where(x => string.Equals(x.StatusText, wanted, StringComparison.OrdinalIgnoreCase));
            }

            bs.DataSource = q.OrderByDescending(x => x.POId).ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is PurchaseOrderDto d)
            {
                _id = d.POId;

                var prodItem = cbProducer.Items.Cast<Item>().FirstOrDefault(i => i.Id == d.ProducerId);
                if (prodItem != null) cbProducer.SelectedItem = prodItem;
                txtProd.Text = d.ProducerId.ToString();

                cbStatus.SelectedItem = d.StatusText;
                txtTotal.Text = d.Total.ToString("0.00");

                // F. esperada del registro (si viene)
                if (d.ExpectedDate.HasValue)
                    dtExpected.Value = d.ExpectedDate.Value.Date;
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                int prodId = 0;
                if (cbProducer.SelectedItem is Item pi) prodId = pi.Id;
                if (prodId <= 0 && (!int.TryParse(txtProd.Text, out prodId) || prodId <= 0))
                {
                    MessageBox.Show("Selecciona un productor."); return;
                }

                decimal? total = null;
                if (decimal.TryParse(txtTotal.Text, out var tot) && tot >= 0) total = tot;

                int? eventId = null;
                if (cbEvent.SelectedItem is Item ei && ei.Id > 0) eventId = ei.Id;

                await api.CreateAsync(new PurchaseOrderCreateRequest
                {
                    ProducerId = prodId,
                    CreatedBy = Session.UserId,
                    EventId = eventId,
                    ExpectedDate = dtExpected.Value.Date,
                    Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                    Total = total
                });

                await LoadData();
                MessageBox.Show("OC creada.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona una OC."); return; }
            try
            {
                int prodId = 0;
                if (cbProducer.SelectedItem is Item pi) prodId = pi.Id;
                if (prodId <= 0 && (!int.TryParse(txtProd.Text, out prodId) || prodId <= 0))
                {
                    MessageBox.Show("Selecciona un productor."); return;
                }

                if (!decimal.TryParse(txtTotal.Text, out var total) || total < 0)
                {
                    MessageBox.Show("Total inválido."); return;
                }

                int? eventId = null;
                if (cbEvent.SelectedItem is Item ei && ei.Id > 0) eventId = ei.Id;

                await api.UpdateAsync(_id.Value, new PurchaseOrderUpdateRequest
                {
                    POId = _id.Value,
                    ProducerId = prodId,
                    EventId = eventId,
                    ExpectedDate = dtExpected.Value.Date,
                    Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                    Status = cbStatus.SelectedItem!.ToString()!,
                    Total = total
                });

                await LoadData();
                MessageBox.Show("OC actualizada.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona una OC."); return; }
            if (MessageBox.Show("¿Borrar OC?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { await api.DeleteAsync(_id.Value); await LoadData(); }
            catch (Exception ex) { MessageBox.Show("Error al borrar: " + ex.Message); }
        }
    }
}
