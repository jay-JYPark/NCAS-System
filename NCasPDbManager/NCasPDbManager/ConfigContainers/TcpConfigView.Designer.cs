namespace NCasPDbManager
{
    partial class TcpConfigView
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvTcpProfile = new NCASFND.NCasCtrl.NCasListView();
            this.btnProfileDelete = new NCASFND.NCasCtrl.NCasButton();
            this.tbxServerIp = new System.Windows.Forms.TextBox();
            this.tbxServerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnProfileregister = new NCASFND.NCasCtrl.NCasButton();
            this.btnProfileNew = new NCASFND.NCasCtrl.NCasButton();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.tbxTcpPort = new System.Windows.Forms.TextBox();
            this.tbxTcpIp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvTcpProfile
            // 
            this.lvTcpProfile.AntiAlias = false;
            this.lvTcpProfile.AutoFit = false;
            this.lvTcpProfile.BackColor = System.Drawing.Color.White;
            this.lvTcpProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvTcpProfile.ColumnHeight = 24;
            this.lvTcpProfile.ColumnOffset = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.lvTcpProfile.ColumnStyle = NCASFND.NCasListViewColumnStyle.DefaultStyle;
            this.lvTcpProfile.FocusedItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.lvTcpProfile.FrozenColumnIndex = -1;
            this.lvTcpProfile.FullRowSelect = true;
            this.lvTcpProfile.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.lvTcpProfile.GridLineStyle = NCASFND.NCasListViewGridLine.GridNone;
            this.lvTcpProfile.HideColumnCheckBox = false;
            this.lvTcpProfile.HideSelection = false;
            this.lvTcpProfile.HoverSelection = false;
            this.lvTcpProfile.IconOffset = new System.Drawing.Point(1, 0);
            this.lvTcpProfile.InteraceColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.lvTcpProfile.InteraceColor2 = System.Drawing.Color.White;
            this.lvTcpProfile.ItemHeight = 18;
            this.lvTcpProfile.ItemOffset = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.lvTcpProfile.Location = new System.Drawing.Point(16, 89);
            this.lvTcpProfile.MultiSelect = false;
            this.lvTcpProfile.Name = "lvTcpProfile";
            this.lvTcpProfile.ScrollType = NCASFND.NCasListViewScrollType.ScrollBoth;
            this.lvTcpProfile.SelectioinItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(232)))), ((int)(((byte)(250)))));
            this.lvTcpProfile.Size = new System.Drawing.Size(436, 209);
            this.lvTcpProfile.TabIndex = 0;
            this.lvTcpProfile.UseInteraceColor = true;
            this.lvTcpProfile.UseSelFocusedBar = false;
            this.lvTcpProfile.Click += new System.EventHandler(this.lvTcpProfile_Click);
            this.lvTcpProfile.DoubleClick += new System.EventHandler(this.lvTcpProfile_DoubleClick);
            // 
            // btnProfileDelete
            // 
            this.btnProfileDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileDelete.AnimationInterval = 300;
            this.btnProfileDelete.CheckedValue = false;
            this.btnProfileDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfileDelete.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnProfileDelete.ForeColor = System.Drawing.Color.White;
            this.btnProfileDelete.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintNormal;
            this.btnProfileDelete.ImgDisable = null;
            this.btnProfileDelete.ImgHover = null;
            this.btnProfileDelete.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintPress;
            this.btnProfileDelete.ImgStatusEvent = null;
            this.btnProfileDelete.ImgStatusNormal = null;
            this.btnProfileDelete.ImgStatusOffsetX = 2;
            this.btnProfileDelete.ImgStatusOffsetY = 0;
            this.btnProfileDelete.IsStatusNormal = true;
            this.btnProfileDelete.Location = new System.Drawing.Point(363, 356);
            this.btnProfileDelete.Name = "btnProfileDelete";
            this.btnProfileDelete.Size = new System.Drawing.Size(89, 39);
            this.btnProfileDelete.TabIndex = 26;
            this.btnProfileDelete.Text = "삭제";
            this.btnProfileDelete.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnProfileDelete.UseAutoWrap = false;
            this.btnProfileDelete.UseCheck = false;
            this.btnProfileDelete.UseImgStretch = false;
            this.btnProfileDelete.Click += new System.EventHandler(this.btnProfileDelete_Click);
            // 
            // tbxServerIp
            // 
            this.tbxServerIp.Location = new System.Drawing.Point(327, 6);
            this.tbxServerIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerIp.MaxLength = 15;
            this.tbxServerIp.Name = "tbxServerIp";
            this.tbxServerIp.Size = new System.Drawing.Size(102, 23);
            this.tbxServerIp.TabIndex = 28;
            // 
            // tbxServerName
            // 
            this.tbxServerName.Location = new System.Drawing.Point(84, 6);
            this.tbxServerName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerName.MaxLength = 20;
            this.tbxServerName.Name = "tbxServerName";
            this.tbxServerName.Size = new System.Drawing.Size(146, 23);
            this.tbxServerName.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(272, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 17);
            this.label2.TabIndex = 31;
            this.label2.Text = "IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(18, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 17);
            this.label1.TabIndex = 30;
            this.label1.Text = "서버명";
            // 
            // btnProfileregister
            // 
            this.btnProfileregister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileregister.AnimationInterval = 300;
            this.btnProfileregister.CheckedValue = false;
            this.btnProfileregister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfileregister.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnProfileregister.ForeColor = System.Drawing.Color.White;
            this.btnProfileregister.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintNormal;
            this.btnProfileregister.ImgDisable = null;
            this.btnProfileregister.ImgHover = null;
            this.btnProfileregister.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintPress;
            this.btnProfileregister.ImgStatusEvent = null;
            this.btnProfileregister.ImgStatusNormal = null;
            this.btnProfileregister.ImgStatusOffsetX = 2;
            this.btnProfileregister.ImgStatusOffsetY = 0;
            this.btnProfileregister.IsStatusNormal = true;
            this.btnProfileregister.Location = new System.Drawing.Point(269, 356);
            this.btnProfileregister.Name = "btnProfileregister";
            this.btnProfileregister.Size = new System.Drawing.Size(89, 39);
            this.btnProfileregister.TabIndex = 33;
            this.btnProfileregister.Text = "등록/수정";
            this.btnProfileregister.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnProfileregister.UseAutoWrap = false;
            this.btnProfileregister.UseCheck = false;
            this.btnProfileregister.UseImgStretch = false;
            this.btnProfileregister.Click += new System.EventHandler(this.btnProfileregister_Click);
            // 
            // btnProfileNew
            // 
            this.btnProfileNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProfileNew.AnimationInterval = 300;
            this.btnProfileNew.CheckedValue = false;
            this.btnProfileNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProfileNew.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnProfileNew.ForeColor = System.Drawing.Color.White;
            this.btnProfileNew.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintNormal;
            this.btnProfileNew.ImgDisable = null;
            this.btnProfileNew.ImgHover = null;
            this.btnProfileNew.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintPress;
            this.btnProfileNew.ImgStatusEvent = null;
            this.btnProfileNew.ImgStatusNormal = null;
            this.btnProfileNew.ImgStatusOffsetX = 2;
            this.btnProfileNew.ImgStatusOffsetY = 0;
            this.btnProfileNew.IsStatusNormal = true;
            this.btnProfileNew.Location = new System.Drawing.Point(174, 356);
            this.btnProfileNew.Name = "btnProfileNew";
            this.btnProfileNew.Size = new System.Drawing.Size(89, 39);
            this.btnProfileNew.TabIndex = 34;
            this.btnProfileNew.Text = "신규";
            this.btnProfileNew.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnProfileNew.UseAutoWrap = false;
            this.btnProfileNew.UseCheck = false;
            this.btnProfileNew.UseImgStretch = false;
            this.btnProfileNew.Click += new System.EventHandler(this.btnProfileNew_Click);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(16, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 17);
            this.label3.TabIndex = 35;
            this.label3.Text = "TCP접속 리스트";
            // 
            // tbxTcpPort
            // 
            this.tbxTcpPort.Location = new System.Drawing.Point(327, 6);
            this.tbxTcpPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTcpPort.MaxLength = 5;
            this.tbxTcpPort.Name = "tbxTcpPort";
            this.tbxTcpPort.Size = new System.Drawing.Size(102, 23);
            this.tbxTcpPort.TabIndex = 37;
            // 
            // tbxTcpIp
            // 
            this.tbxTcpIp.Location = new System.Drawing.Point(84, 6);
            this.tbxTcpIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxTcpIp.MaxLength = 15;
            this.tbxTcpIp.Name = "tbxTcpIp";
            this.tbxTcpIp.Size = new System.Drawing.Size(146, 23);
            this.tbxTcpIp.TabIndex = 36;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(248, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 39;
            this.label4.Text = "수신PORT";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(18, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 17);
            this.label5.TabIndex = 38;
            this.label5.Text = "수신IP";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.bgTxtDbMngTcp;
            this.panel1.Controls.Add(this.tbxTcpIp);
            this.panel1.Controls.Add(this.tbxTcpPort);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(16, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(436, 36);
            this.panel1.TabIndex = 40;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.bgTxtDbMngTcp;
            this.panel2.Controls.Add(this.tbxServerName);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.tbxServerIp);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(16, 304);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(436, 36);
            this.panel2.TabIndex = 41;
            // 
            // TcpConfigView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnProfileNew);
            this.Controls.Add(this.btnProfileregister);
            this.Controls.Add(this.btnProfileDelete);
            this.Controls.Add(this.lvTcpProfile);
            this.Name = "TcpConfigView";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NCASFND.NCasCtrl.NCasListView lvTcpProfile;
        private NCASFND.NCasCtrl.NCasButton btnProfileDelete;
        private System.Windows.Forms.TextBox tbxServerIp;
        private System.Windows.Forms.TextBox tbxServerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private NCASFND.NCasCtrl.NCasButton btnProfileregister;
        private NCASFND.NCasCtrl.NCasButton btnProfileNew;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxTcpPort;
        private System.Windows.Forms.TextBox tbxTcpIp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}
