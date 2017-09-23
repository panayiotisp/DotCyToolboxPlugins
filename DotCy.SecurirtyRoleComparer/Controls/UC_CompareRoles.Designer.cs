namespace DotCyToolboxPlugins.Controls {
    partial class UC_CompareRoles {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.scRoleControls = new System.Windows.Forms.SplitContainer();
            this.cblRoles = new System.Windows.Forms.CheckedListBox();
            this.pnCompareRoles = new System.Windows.Forms.Panel();
            this.wbSecurityRolePriv = new System.Windows.Forms.WebBrowser();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPermissionGroup = new System.Windows.Forms.ComboBox();
            this.tsMainToolbar = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbutClose = new System.Windows.Forms.ToolStripButton();
            this.tbutLoadRoles = new System.Windows.Forms.ToolStripButton();
            this.tbutCompareSelected = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.scRoleControls)).BeginInit();
            this.scRoleControls.Panel1.SuspendLayout();
            this.scRoleControls.Panel2.SuspendLayout();
            this.scRoleControls.SuspendLayout();
            this.pnCompareRoles.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tsMainToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // scRoleControls
            // 
            this.scRoleControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scRoleControls.Location = new System.Drawing.Point(3, 28);
            this.scRoleControls.Name = "scRoleControls";
            // 
            // scRoleControls.Panel1
            // 
            this.scRoleControls.Panel1.Controls.Add(this.cblRoles);
            this.scRoleControls.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // scRoleControls.Panel2
            // 
            this.scRoleControls.Panel2.Controls.Add(this.pnCompareRoles);
            this.scRoleControls.Size = new System.Drawing.Size(829, 618);
            this.scRoleControls.SplitterDistance = 275;
            this.scRoleControls.TabIndex = 0;
            // 
            // cblRoles
            // 
            this.cblRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cblRoles.FormattingEnabled = true;
            this.cblRoles.Location = new System.Drawing.Point(3, 3);
            this.cblRoles.Name = "cblRoles";
            this.cblRoles.Size = new System.Drawing.Size(269, 612);
            this.cblRoles.Sorted = true;
            this.cblRoles.TabIndex = 0;
            // 
            // pnCompareRoles
            // 
            this.pnCompareRoles.Controls.Add(this.wbSecurityRolePriv);
            this.pnCompareRoles.Controls.Add(this.webBrowser1);
            this.pnCompareRoles.Controls.Add(this.panel1);
            this.pnCompareRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCompareRoles.Location = new System.Drawing.Point(0, 0);
            this.pnCompareRoles.Name = "pnCompareRoles";
            this.pnCompareRoles.Padding = new System.Windows.Forms.Padding(3);
            this.pnCompareRoles.Size = new System.Drawing.Size(550, 618);
            this.pnCompareRoles.TabIndex = 0;
            // 
            // wbSecurityRolePriv
            // 
            this.wbSecurityRolePriv.AllowWebBrowserDrop = false;
            this.wbSecurityRolePriv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSecurityRolePriv.IsWebBrowserContextMenuEnabled = false;
            this.wbSecurityRolePriv.Location = new System.Drawing.Point(3, 40);
            this.wbSecurityRolePriv.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSecurityRolePriv.Name = "wbSecurityRolePriv";
            this.wbSecurityRolePriv.ScriptErrorsSuppressed = true;
            this.wbSecurityRolePriv.Size = new System.Drawing.Size(544, 575);
            this.wbSecurityRolePriv.TabIndex = 2;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 40);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(544, 575);
            this.webBrowser1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbPermissionGroup);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 37);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter";
            // 
            // cbPermissionGroup
            // 
            this.cbPermissionGroup.BackColor = System.Drawing.Color.White;
            this.cbPermissionGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPermissionGroup.FormattingEnabled = true;
            this.cbPermissionGroup.Location = new System.Drawing.Point(42, 6);
            this.cbPermissionGroup.Name = "cbPermissionGroup";
            this.cbPermissionGroup.Size = new System.Drawing.Size(324, 21);
            this.cbPermissionGroup.Sorted = true;
            this.cbPermissionGroup.TabIndex = 1;
            this.cbPermissionGroup.SelectedIndexChanged += new System.EventHandler(this.cbPermissionGroup_SelectedIndexChanged);
            // 
            // tsMainToolbar
            // 
            this.tsMainToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbutClose,
            this.toolStripSeparator1,
            this.tbutLoadRoles,
            this.tbutCompareSelected});
            this.tsMainToolbar.Location = new System.Drawing.Point(3, 3);
            this.tsMainToolbar.Name = "tsMainToolbar";
            this.tsMainToolbar.Size = new System.Drawing.Size(829, 25);
            this.tsMainToolbar.TabIndex = 2;
            this.tsMainToolbar.Text = "Main Toolbar";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbutClose
            // 
            this.tbutClose.AutoToolTip = false;
            this.tbutClose.Image = global::DotCyToolboxPlugins.Properties.Resources.close16;
            this.tbutClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbutClose.Name = "tbutClose";
            this.tbutClose.Size = new System.Drawing.Size(56, 22);
            this.tbutClose.Text = "Close";
            this.tbutClose.ToolTipText = "Close Plugin tool";
            this.tbutClose.Click += new System.EventHandler(this.tbutClose_Click);
            // 
            // tbutLoadRoles
            // 
            this.tbutLoadRoles.Image = global::DotCyToolboxPlugins.Properties.Resources.refresh16;
            this.tbutLoadRoles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbutLoadRoles.Name = "tbutLoadRoles";
            this.tbutLoadRoles.Size = new System.Drawing.Size(90, 22);
            this.tbutLoadRoles.Text = "Load Roles..";
            this.tbutLoadRoles.Click += new System.EventHandler(this.tbutLoadRoles_Click);
            // 
            // tbutCompareSelected
            // 
            this.tbutCompareSelected.Enabled = false;
            this.tbutCompareSelected.Image = global::DotCyToolboxPlugins.Properties.Resources.compare16;
            this.tbutCompareSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbutCompareSelected.Name = "tbutCompareSelected";
            this.tbutCompareSelected.Size = new System.Drawing.Size(154, 22);
            this.tbutCompareSelected.Text = "Compare Selected Roles";
            this.tbutCompareSelected.ToolTipText = "Compare Selected Roles";
            this.tbutCompareSelected.Click += new System.EventHandler(this.tbutCompareSelected_Click);
            // 
            // UC_CompareRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.scRoleControls);
            this.Controls.Add(this.tsMainToolbar);
            this.Name = "UC_CompareRoles";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(835, 649);
            this.scRoleControls.Panel1.ResumeLayout(false);
            this.scRoleControls.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scRoleControls)).EndInit();
            this.scRoleControls.ResumeLayout(false);
            this.pnCompareRoles.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tsMainToolbar.ResumeLayout(false);
            this.tsMainToolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer scRoleControls;
        private System.Windows.Forms.CheckedListBox cblRoles;
        private System.Windows.Forms.Panel pnCompareRoles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.WebBrowser wbSecurityRolePriv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbPermissionGroup;
        private System.Windows.Forms.ToolStrip tsMainToolbar;
        private System.Windows.Forms.ToolStripButton tbutLoadRoles;
        private System.Windows.Forms.ToolStripButton tbutCompareSelected;
        private System.Windows.Forms.ToolStripButton tbutClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
