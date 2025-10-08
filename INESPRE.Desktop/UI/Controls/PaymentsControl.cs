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
    public class PaymentsControl : UserControl
    {
        private readonly PaymentsApi api = new();
        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        private readonly TextBox txtPO = new() { PlaceholderText = "PO Id", Width = 80 };
        private readonly ComboBox cbStatusF = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        private readonly TextBox txtPOId = new() { PlaceholderText = "PO Id", Width = 80 };
        private readonly NumericUpDown numAmount = new() { DecimalPlaces = 2, Maximum = 1000000, Width = 100 };
        private readonly DateTimePicker dtPay = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly ComboBox cbMethod = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly ComboBox cbStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly TextBox txtNotes = new() { PlaceholderText = "Notas", Width = 220 };
        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<PaymentDto> _all = new();
        private int? _id = null;

        public PaymentsControl()
        {
            cbStatusF.Items.AddRange(new[] { "(Todos)", "PENDIENTE", "CONFIRMADO" }); cbStatusF.SelectedIndex = 0;
            cbMethod.Items.AddRange(new[] { "EFECTIVO", "TRANSFERENCIA" }); cbMethod.SelectedIndex = 0;
            cbStatus.Items.AddRange(new[] { "PENDIENTE", "CONFIRMADO" }); cbStatus.SelectedIndex = 0;

            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] { new Label { Text = "PO:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) }, txtPO, cbStatusF, btnBuscar, btnLimpiar });
            Controls.Add(grid);
            Controls.Add(tools);

            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] { txtPOId, numAmount, dtPay, cbMethod, cbStatus, txtNotes, btnCreate, btnUpdate, btnDelete });
            Controls.Add(editor);

            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PaymentId", HeaderText = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "POId", HeaderText = "PO Id", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Amount", HeaderText = "Monto", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PaymentDate", HeaderText = "Fecha", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Method", HeaderText = "Método", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Notes", HeaderText = "Notas", Width = 240 });

            grid.SelectionChanged += (_, __) => FromSelection();
            btnBuscar.Click += (_, __) => ApplyFilter();
            btnLimpiar.Click += async (_, __) => { txtPO.Text = ""; cbStatusF.SelectedIndex = 0; await LoadData(); };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) => { if (Visible && !loaded) { loaded = true; await LoadData(); } };
        }

        private async Task LoadData() { _all = await api.GetAllAsync(); ApplyFilter(); }

        private void ApplyFilter()
        {
            IEnumerable<PaymentDto> q = _all;
            if (int.TryParse(txtPO.Text.Trim(), out var po)) q = q.Where(x => x.POId == po);
            if (cbStatusF.SelectedIndex > 0)
                q = q.Where(x => string.Equals(x.Status, cbStatusF.SelectedItem!.ToString(), StringComparison.OrdinalIgnoreCase));
            bs.DataSource = q.ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is PaymentDto d)
            {
                _id = d.PaymentId;
                txtPOId.Text = d.POId.ToString();
                numAmount.Value = d.Amount;
                dtPay.Value = d.PaymentDate;
                cbMethod.SelectedItem = d.Method.ToUpperInvariant();
                cbStatus.SelectedItem = d.Status.ToUpperInvariant();
                txtNotes.Text = d.Notes ?? "";
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                await api.CreateAsync(new PaymentCreateRequest
                {
                    POId = int.Parse(txtPOId.Text),
                    Amount = numAmount.Value,
                    PaymentDate = dtPay.Value.Date,
                    Method = cbMethod.SelectedItem!.ToString()!,
                    Status = cbStatus.SelectedItem!.ToString()!,
                    Notes = txtNotes.Text.Trim()
                });
                await LoadData();
                MessageBox.Show("Pago creado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un pago."); return; }
            try
            {
                await api.UpdateAsync(_id.Value, new PaymentUpdateRequest
                {
                    PaymentId = _id.Value,
                    POId = int.Parse(txtPOId.Text),
                    Amount = numAmount.Value,
                    PaymentDate = dtPay.Value.Date,
                    Method = cbMethod.SelectedItem!.ToString()!,
                    Status = cbStatus.SelectedItem!.ToString()!,
                    Notes = txtNotes.Text.Trim()
                });
                await LoadData();
                MessageBox.Show("Pago actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un pago."); return; }
            if (MessageBox.Show("¿Borrar pago?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { await api.DeleteAsync(_id.Value); await LoadData(); }
            catch (Exception ex) { MessageBox.Show("Error al borrar: " + ex.Message); }
        }
    }
}
