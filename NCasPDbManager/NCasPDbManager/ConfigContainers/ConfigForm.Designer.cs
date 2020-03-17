namespace NCasPDbManager
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnUdpTab = new NCASFND.NCasCtrl.NCasButton();
            this.btnTcpTab = new NCASFND.NCasCtrl.NCasButton();
            this.btnDbTab = new NCASFND.NCasCtrl.NCasButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.plViewMain = new System.Windows.Forms.Panel();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.btnSave = new NCASFND.NCasCtrl.NCasButton();
            this.btnClose = new NCASFND.NCasCtrl.NCasButton();
            this.환경설정 = new System.IO.Ports.SerialPort(this.components);
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.popBgTop;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 56);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.popIconSetting;
            this.panel2.Location = new System.Drawing.Point(448, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(32, 32);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "시스템 환경을 설정합니다.";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 56);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(494, 10);
            this.panel3.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.popTabBarRight;
            this.panel4.Controls.Add(this.btnUdpTab);
            this.panel4.Controls.Add(this.btnTcpTab);
            this.panel4.Controls.Add(this.btnDbTab);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 66);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(494, 40);
            this.panel4.TabIndex = 2;
            // 
            // btnUdpTab
            // 
            this.btnUdpTab.AnimationInterval = 300;
            this.btnUdpTab.CheckedValue = false;
            this.btnUdpTab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUdpTab.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnUdpTab.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUdpTab.ForeColor = System.Drawing.Color.White;
            this.btnUdpTab.Image = global::NCasPDbManager.NCasPDbManagerRsc.popTabNormal;
            this.btnUdpTab.ImgDisable = null;
            this.btnUdpTab.ImgHover = null;
            this.btnUdpTab.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.popTabSelected;
            this.btnUdpTab.ImgStatusEvent = null;
            this.btnUdpTab.ImgStatusNormal = null;
            this.btnUdpTab.ImgStatusOffsetX = 2;
            this.btnUdpTab.ImgStatusOffsetY = 0;
            this.btnUdpTab.IsStatusNormal = true;
            this.btnUdpTab.Location = new System.Drawing.Point(206, 0);
            this.btnUdpTab.Name = "btnUdpTab";
            this.btnUdpTab.Size = new System.Drawing.Size(103, 40);
            this.btnUdpTab.TabIndex = 2;
            this.btnUdpTab.Text = "UDP설정";
            this.btnUdpTab.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnUdpTab.UseAutoWrap = false;
            this.btnUdpTab.UseCheck = true;
            this.btnUdpTab.UseImgStretch = false;
            this.btnUdpTab.Click += new System.EventHandler(this.TabButton_Click);
            // 
            // btnTcpTab
            // 
            this.btnTcpTab.AnimationInterval = 300;
            this.btnTcpTab.CheckedValue = false;
            this.btnTcpTab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTcpTab.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnTcpTab.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTcpTab.ForeColor = System.Drawing.Color.White;
            this.btnTcpTab.Image = global::NCasPDbManager.NCasPDbManagerRsc.popTabNormal;
            this.btnTcpTab.ImgDisable = null;
            this.btnTcpTab.ImgHover = null;
            this.btnTcpTab.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.popTabSelected;
            this.btnTcpTab.ImgStatusEvent = null;
            this.btnTcpTab.ImgStatusNormal = null;
            this.btnTcpTab.ImgStatusOffsetX = 2;
            this.btnTcpTab.ImgStatusOffsetY = 0;
            this.btnTcpTab.IsStatusNormal = true;
            this.btnTcpTab.Location = new System.Drawing.Point(103, 0);
            this.btnTcpTab.Name = "btnTcpTab";
            this.btnTcpTab.Size = new System.Drawing.Size(103, 40);
            this.btnTcpTab.TabIndex = 1;
            this.btnTcpTab.Text = "TCP설정";
            this.btnTcpTab.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnTcpTab.UseAutoWrap = false;
            this.btnTcpTab.UseCheck = true;
            this.btnTcpTab.UseImgStretch = false;
            this.btnTcpTab.Click += new System.EventHandler(this.TabButton_Click);
            // 
            // btnDbTab
            // 
            this.btnDbTab.AnimationInterval = 300;
            this.btnDbTab.CheckedValue = false;
            this.btnDbTab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDbTab.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnDbTab.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDbTab.ForeColor = System.Drawing.Color.White;
            this.btnDbTab.Image = global::NCasPDbManager.NCasPDbManagerRsc.popTabNormal;
            this.btnDbTab.ImgDisable = null;
            this.btnDbTab.ImgHover = null;
            this.btnDbTab.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.popTabSelected;
            this.btnDbTab.ImgStatusEvent = null;
            this.btnDbTab.ImgStatusNormal = null;
            this.btnDbTab.ImgStatusOffsetX = 2;
            this.btnDbTab.ImgStatusOffsetY = 0;
            this.btnDbTab.IsStatusNormal = true;
            this.btnDbTab.Location = new System.Drawing.Point(0, 0);
            this.btnDbTab.Name = "btnDbTab";
            this.btnDbTab.Size = new System.Drawing.Size(103, 40);
            this.btnDbTab.TabIndex = 0;
            this.btnDbTab.Text = "DB설정";
            this.btnDbTab.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnDbTab.UseAutoWrap = false;
            this.btnDbTab.UseCheck = true;
            this.btnDbTab.UseImgStretch = false;
            this.btnDbTab.Click += new System.EventHandler(this.TabButton_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.panel5.Controls.Add(this.plViewMain);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 106);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(494, 433);
            this.panel5.TabIndex = 3;
            // 
            // plViewMain
            // 
            this.plViewMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.plViewMain.Location = new System.Drawing.Point(12, 10);
            this.plViewMain.Name = "plViewMain";
            this.plViewMain.Size = new System.Drawing.Size(470, 410);
            this.plViewMain.TabIndex = 0;
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AnimationInterval = 300;
            this.btnSave.CheckedValue = false;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnSaveNormal;
            this.btnSave.ImgDisable = null;
            this.btnSave.ImgHover = null;
            this.btnSave.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnSavePress;
            this.btnSave.ImgStatusEvent = null;
            this.btnSave.ImgStatusNormal = null;
            this.btnSave.ImgStatusOffsetX = 2;
            this.btnSave.ImgStatusOffsetY = 0;
            this.btnSave.IsStatusNormal = true;
            this.btnSave.Location = new System.Drawing.Point(244, 553);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 43);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "저 장";
            this.btnSave.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnSave.UseAutoWrap = false;
            this.btnSave.UseCheck = false;
            this.btnSave.UseImgStretch = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AnimationInterval = 300;
            this.btnClose.CheckedValue = false;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnSaveNormal;
            this.btnClose.ImgDisable = null;
            this.btnClose.ImgHover = null;
            this.btnClose.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnSavePress;
            this.btnClose.ImgStatusEvent = null;
            this.btnClose.ImgStatusNormal = null;
            this.btnClose.ImgStatusOffsetX = 2;
            this.btnClose.ImgStatusOffsetY = 0;
            this.btnClose.IsStatusNormal = true;
            this.btnClose.Location = new System.Drawing.Point(365, 553);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(115, 43);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "닫 기";
            this.btnClose.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnClose.UseAutoWrap = false;
            this.btnClose.UseCheck = false;
            this.btnClose.UseImgStretch = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(494, 608);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.Text = "ConfigForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private NCASFND.NCasCtrl.NCasButton btnUdpTab;
        private NCASFND.NCasCtrl.NCasButton btnTcpTab;
        private NCASFND.NCasCtrl.NCasButton btnDbTab;
        private System.Windows.Forms.Panel panel5;
        private NCASFND.NCasCtrl.NCasButton btnClose;
        private NCASFND.NCasCtrl.NCasButton btnSave;
        private System.Windows.Forms.Panel plViewMain;
        private System.Windows.Forms.ImageList imgList;
        private System.IO.Ports.SerialPort 환경설정;
    }
}