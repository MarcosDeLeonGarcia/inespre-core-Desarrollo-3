namespace INESPRE.Desktop.UI.Forms
{
    partial class ChangePasswordForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            layout = new TableLayoutPanel();
            lblTitle = new Label();
            lblUserLabel = new Label();
            lblUser = new Label();
            labelNew = new Label();
            txtNew = new TextBox();
            labelRepeat = new Label();
            txtRepeat = new TextBox();
            panelButtons = new FlowLayoutPanel();
            btnOk = new Button();
            btnCancel = new Button();
            lblMsg = new Label();
            layout.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // layout
            // 
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(lblUserLabel, 0, 1);
            layout.Controls.Add(lblUser, 1, 1);
            layout.Controls.Add(labelNew, 0, 2);
            layout.Controls.Add(txtNew, 1, 2);
            layout.Controls.Add(labelRepeat, 0, 3);
            layout.Controls.Add(txtRepeat, 1, 3);
            layout.Controls.Add(panelButtons, 1, 4);
            layout.Controls.Add(lblMsg, 1, 5);
            layout.Dock = DockStyle.Fill;
            layout.Location = new Point(0, 0);
            layout.Margin = new Padding(0);
            layout.Name = "layout";
            layout.Padding = new Padding(14, 16, 14, 16);
            layout.RowCount = 6;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 53F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Size = new Size(579, 393);
            layout.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            layout.SetColumnSpan(lblTitle, 2);
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(17, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Cambiar contraseña";
            // 
            // lblUserLabel
            // 
            lblUserLabel.AutoSize = true;
            lblUserLabel.Location = new Point(17, 59);
            lblUserLabel.Name = "lblUserLabel";
            lblUserLabel.Size = new Size(62, 20);
            lblUserLabel.TabIndex = 1;
            lblUserLabel.Text = "Usuario:";
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(177, 59);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(30, 20);
            lblUser.TabIndex = 2;
            lblUser.Text = "(…)";
            // 
            // labelNew
            // 
            labelNew.AutoSize = true;
            labelNew.Location = new Point(17, 96);
            labelNew.Name = "labelNew";
            labelNew.Size = new Size(130, 20);
            labelNew.TabIndex = 3;
            labelNew.Text = "Nueva contraseña:";
            // 
            // txtNew
            // 
            txtNew.Dock = DockStyle.Fill;
            txtNew.Location = new Point(177, 100);
            txtNew.Margin = new Padding(3, 4, 3, 4);
            txtNew.Name = "txtNew";
            txtNew.PlaceholderText = "Nueva contraseña";
            txtNew.Size = new Size(385, 27);
            txtNew.TabIndex = 0;
            txtNew.UseSystemPasswordChar = true;
            // 
            // labelRepeat
            // 
            labelRepeat.AutoSize = true;
            labelRepeat.Location = new Point(17, 139);
            labelRepeat.Name = "labelRepeat";
            labelRepeat.Size = new Size(131, 20);
            labelRepeat.TabIndex = 5;
            labelRepeat.Text = "Repite contraseña:";
            // 
            // txtRepeat
            // 
            txtRepeat.Dock = DockStyle.Fill;
            txtRepeat.Location = new Point(177, 143);
            txtRepeat.Margin = new Padding(3, 4, 3, 4);
            txtRepeat.Name = "txtRepeat";
            txtRepeat.PlaceholderText = "Repite contraseña";
            txtRepeat.Size = new Size(385, 27);
            txtRepeat.TabIndex = 1;
            txtRepeat.UseSystemPasswordChar = true;
            // 
            // panelButtons
            // 
            panelButtons.AutoSize = true;
            panelButtons.Controls.Add(btnOk);
            panelButtons.Controls.Add(btnCancel);
            panelButtons.Location = new Point(177, 186);
            panelButtons.Margin = new Padding(3, 4, 3, 0);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(264, 45);
            panelButtons.TabIndex = 7;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(3, 4);
            btnOk.Margin = new Padding(3, 4, 3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(126, 37);
            btnOk.TabIndex = 2;
            btnOk.Text = "Guardar";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(135, 4);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(126, 37);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.ForeColor = Color.Firebrick;
            lblMsg.Location = new Point(177, 239);
            lblMsg.Margin = new Padding(3, 4, 3, 0);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(0, 20);
            lblMsg.TabIndex = 8;
            // 
            // ChangePasswordForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(579, 393);
            Controls.Add(layout);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangePasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cambiar contraseña";
            layout.ResumeLayout(false);
            layout.PerformLayout();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUserLabel;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label labelNew;
        private System.Windows.Forms.TextBox txtNew;
        private System.Windows.Forms.Label labelRepeat;
        private System.Windows.Forms.TextBox txtRepeat;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMsg;
    }
}
