using System;
using System.Windows.Forms;
using INESPRE.Desktop.Api;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.UI.Forms
{
    public partial class ChangePasswordForm : Form
    {
        public ChangePasswordForm()
        {
            InitializeComponent();
            lblUser.Text = Session.Username;

            btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;
            btnOk.Click += async (_, __) => await SaveAsync();
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            lblMsg.Text = "";
            if (string.IsNullOrWhiteSpace(txtNew.Text) || txtNew.Text.Length < 6)
            { lblMsg.Text = "La contraseña debe tener al menos 6 caracteres."; return; }
            if (txtNew.Text != txtRepeat.Text)
            { lblMsg.Text = "Las contraseñas no coinciden."; return; }

            try
            {
                var api = new AuthApi();
                var ok = await api.ChangePasswordAsync(Session.UserId, txtNew.Text);
                if (!ok) { lblMsg.Text = "No se pudo cambiar la contraseña."; return; }
                MessageBox.Show("Contraseña actualizada.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { lblMsg.Text = "Error: " + ex.Message; }
        }
    }
}
