namespace INESPRE.Desktop.UI.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSide;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelBody;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnLogout;

        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnEventos;
        private System.Windows.Forms.Button btnProductos;
        private System.Windows.Forms.Button btnUsuarios;
        private System.Windows.Forms.Button btnProducers;
        private System.Windows.Forms.Button btnRoles;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnPurchases;
        private System.Windows.Forms.Button btnPayments;
        private System.Windows.Forms.Button btnSales;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Designer generated code
        private void InitializeComponent()
        {
            this.panelSide = new System.Windows.Forms.Panel();
            this.btnUsuarios = new System.Windows.Forms.Button();
            this.btnProductos = new System.Windows.Forms.Button();
            this.btnEventos = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelBody = new System.Windows.Forms.Panel();
            this.panelSide.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSide
            // 
            this.panelSide.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.panelSide.Controls.Add(this.btnUsuarios);
            this.panelSide.Controls.Add(this.btnProductos);
            this.panelSide.Controls.Add(this.btnEventos);
            this.panelSide.Controls.Add(this.btnDashboard);
            this.panelSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSide.Location = new System.Drawing.Point(0, 56);
            this.panelSide.Name = "panelSide";
            this.panelSide.Size = new System.Drawing.Size(220, 604);
            this.panelSide.TabIndex = 0;
            // 
            // btnUsuarios
            // 
            this.btnUsuarios.Location = new System.Drawing.Point(12, 212);
            this.btnUsuarios.Name = "btnUsuarios";
            this.btnUsuarios.Size = new System.Drawing.Size(196, 36);
            this.btnUsuarios.TabIndex = 4;
            this.btnUsuarios.Text = "Usuarios (ADMIN)";
            this.btnUsuarios.UseVisualStyleBackColor = true;
            // 
            // btnCaja
            // 
            //this.btnCaja.Location = new System.Drawing.Point(12, 168);
            //this.btnCaja.Name = "btnCaja";
            //this.btnCaja.Size = new System.Drawing.Size(196, 36);
            //this.btnCaja.TabIndex = 3;
            //this.btnCaja.Text = "Caja";
            //this.btnCaja.UseVisualStyleBackColor = true;
            // 
            // btnProductos
            // 
            this.btnProductos.Location = new System.Drawing.Point(12, 124);
            this.btnProductos.Name = "btnProductos";
            this.btnProductos.Size = new System.Drawing.Size(196, 36);
            this.btnProductos.TabIndex = 2;
            this.btnProductos.Text = "Productos";
            this.btnProductos.UseVisualStyleBackColor = true;
            // 
            // btnEventos
            // 
            this.btnEventos.Location = new System.Drawing.Point(12, 80);
            this.btnEventos.Name = "btnEventos";
            this.btnEventos.Size = new System.Drawing.Size(196, 36);
            this.btnEventos.TabIndex = 1;
            this.btnEventos.Text = "Eventos";
            this.btnEventos.UseVisualStyleBackColor = true;
            // 
            // btnDashboard
            // 
            this.btnDashboard.Location = new System.Drawing.Point(12, 36);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(196, 36);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.btnLogout);
            this.panelHeader.Controls.Add(this.lblUser);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(984, 56);
            this.panelHeader.TabIndex = 1;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.Location = new System.Drawing.Point(884, 14);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(88, 28);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Salir";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // lblUser
            // 
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.Location = new System.Drawing.Point(640, 18);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(236, 20);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "usuario · rol";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "INESPRE";
            // 
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.White;
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(220, 56);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(764, 604);
            this.panelBody.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 660);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelSide);
            this.Controls.Add(this.panelHeader);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INESPRE – Panel";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelSide.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);

            // ...
            this.btnProducers = new System.Windows.Forms.Button();
            this.btnProducers.Text = "Productores";
            this.btnProducers.Dock = DockStyle.Top;

            this.btnRoles = new System.Windows.Forms.Button();
            this.btnRoles.Text = "Roles";
            this.btnRoles.Dock = DockStyle.Top;

            this.btnInventory = new System.Windows.Forms.Button();
            this.btnInventory.Text = "Inventario";
            this.btnInventory.Dock = DockStyle.Top;

            this.btnPurchases = new System.Windows.Forms.Button();
            this.btnPurchases.Text = "Órdenes compra";
            this.btnPurchases.Dock = DockStyle.Top;

            this.btnPayments = new System.Windows.Forms.Button();
            this.btnPayments.Text = "Pagos";
            this.btnPayments.Dock = DockStyle.Top;

            this.btnSales = new System.Windows.Forms.Button();
            this.btnSales.Text = "Ventas";
            this.btnSales.Dock = DockStyle.Top;

            // agregarlos al panel menú
            this.panelSide.Controls.Add(this.btnSales);
            this.panelSide.Controls.Add(this.btnPayments);
            this.panelSide.Controls.Add(this.btnPurchases);
            this.panelSide.Controls.Add(this.btnInventory);
            this.panelSide.Controls.Add(this.btnProducers);
            this.panelSide.Controls.Add(this.btnRoles);


        }
        #endregion
    }
}
