namespace NCasPDbManager
{
    partial class MainForm
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.lvClientAdmin = new NCASFND.NCasCtrl.NCasListView();
            this.plClientAdminTitle = new NCASFND.NCasCtrl.NCasPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.lvEvent = new NCASFND.NCasCtrl.NCasListView();
            this.plEventTitle = new NCASFND.NCasCtrl.NCasPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.plDownStatus = new NCASFND.NCasCtrl.NCasPanel();
            this.panel5 = new NCASFND.NCasCtrl.NCasPanel();
            this.panel4 = new NCASFND.NCasCtrl.NCasPanel();
            this.lbStandbyCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbProcessingCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pltopMenu = new NCASFND.NCasCtrl.NCasPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnResetTcRecvCount = new System.Windows.Forms.Button();
            this.btnConfig = new NCASFND.NCasCtrl.NCasButton();
            this.panel2 = new NCASFND.NCasCtrl.NCasPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.plClientAdminTitle.SuspendLayout();
            this.plEventTitle.SuspendLayout();
            this.plDownStatus.SuspendLayout();
            this.pltopMenu.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcMain.Location = new System.Drawing.Point(0, 44);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.lvClientAdmin);
            this.spcMain.Panel1.Controls.Add(this.plClientAdminTitle);
            this.spcMain.Panel1MinSize = 438;
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.lvEvent);
            this.spcMain.Panel2.Controls.Add(this.plEventTitle);
            this.spcMain.Size = new System.Drawing.Size(1181, 618);
            this.spcMain.SplitterDistance = 438;
            this.spcMain.TabIndex = 2;
            // 
            // lvClientAdmin
            // 
            this.lvClientAdmin.AntiAlias = false;
            this.lvClientAdmin.AutoFit = false;
            this.lvClientAdmin.BackColor = System.Drawing.Color.White;
            this.lvClientAdmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvClientAdmin.ColumnHeight = 24;
            this.lvClientAdmin.ColumnOffset = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.lvClientAdmin.ColumnStyle = NCASFND.NCasListViewColumnStyle.DefaultStyle;
            this.lvClientAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvClientAdmin.FocusedItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.lvClientAdmin.FrozenColumnIndex = -1;
            this.lvClientAdmin.FullRowSelect = true;
            this.lvClientAdmin.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.lvClientAdmin.GridLineStyle = NCASFND.NCasListViewGridLine.GridNone;
            this.lvClientAdmin.HideColumnCheckBox = false;
            this.lvClientAdmin.HideSelection = false;
            this.lvClientAdmin.HoverSelection = false;
            this.lvClientAdmin.IconOffset = new System.Drawing.Point(1, 0);
            this.lvClientAdmin.InteraceColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.lvClientAdmin.InteraceColor2 = System.Drawing.Color.White;
            this.lvClientAdmin.ItemHeight = 18;
            this.lvClientAdmin.ItemOffset = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.lvClientAdmin.Location = new System.Drawing.Point(0, 29);
            this.lvClientAdmin.MultiSelect = false;
            this.lvClientAdmin.Name = "lvClientAdmin";
            this.lvClientAdmin.ScrollType = NCASFND.NCasListViewScrollType.ScrollBoth;
            this.lvClientAdmin.SelectioinItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(232)))), ((int)(((byte)(250)))));
            this.lvClientAdmin.Size = new System.Drawing.Size(438, 589);
            this.lvClientAdmin.TabIndex = 1;
            this.lvClientAdmin.UseInteraceColor = true;
            this.lvClientAdmin.UseSelFocusedBar = false;
            // 
            // plClientAdminTitle
            // 
            this.plClientAdminTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(146)))), ((int)(((byte)(213)))));
            this.plClientAdminTitle.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.bgSubBarCom;
            this.plClientAdminTitle.Controls.Add(this.label6);
            this.plClientAdminTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plClientAdminTitle.Location = new System.Drawing.Point(0, 0);
            this.plClientAdminTitle.Name = "plClientAdminTitle";
            this.plClientAdminTitle.Size = new System.Drawing.Size(438, 29);
            this.plClientAdminTitle.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(4, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "클라이언트 관리";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lvEvent
            // 
            this.lvEvent.AntiAlias = false;
            this.lvEvent.AutoFit = false;
            this.lvEvent.BackColor = System.Drawing.Color.White;
            this.lvEvent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvEvent.ColumnHeight = 24;
            this.lvEvent.ColumnOffset = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.lvEvent.ColumnStyle = NCASFND.NCasListViewColumnStyle.DefaultStyle;
            this.lvEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEvent.FocusedItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.lvEvent.FrozenColumnIndex = -1;
            this.lvEvent.FullRowSelect = true;
            this.lvEvent.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.lvEvent.GridLineStyle = NCASFND.NCasListViewGridLine.GridNone;
            this.lvEvent.HideColumnCheckBox = false;
            this.lvEvent.HideSelection = false;
            this.lvEvent.HoverSelection = false;
            this.lvEvent.IconOffset = new System.Drawing.Point(1, 0);
            this.lvEvent.InteraceColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.lvEvent.InteraceColor2 = System.Drawing.Color.White;
            this.lvEvent.ItemHeight = 18;
            this.lvEvent.ItemOffset = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.lvEvent.Location = new System.Drawing.Point(0, 29);
            this.lvEvent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lvEvent.MultiSelect = false;
            this.lvEvent.Name = "lvEvent";
            this.lvEvent.ScrollType = NCASFND.NCasListViewScrollType.ScrollBoth;
            this.lvEvent.SelectioinItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(232)))), ((int)(((byte)(250)))));
            this.lvEvent.Size = new System.Drawing.Size(739, 589);
            this.lvEvent.TabIndex = 2;
            this.lvEvent.UseInteraceColor = true;
            this.lvEvent.UseSelFocusedBar = false;
            // 
            // plEventTitle
            // 
            this.plEventTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(146)))), ((int)(((byte)(213)))));
            this.plEventTitle.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.bgSubBarCom;
            this.plEventTitle.Controls.Add(this.label7);
            this.plEventTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plEventTitle.Location = new System.Drawing.Point(0, 0);
            this.plEventTitle.Name = "plEventTitle";
            this.plEventTitle.Size = new System.Drawing.Size(739, 29);
            this.plEventTitle.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(4, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "이벤트";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // plDownStatus
            // 
            this.plDownStatus.BackColor = System.Drawing.Color.Transparent;
            this.plDownStatus.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.footerBg;
            this.plDownStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plDownStatus.Controls.Add(this.panel5);
            this.plDownStatus.Controls.Add(this.panel4);
            this.plDownStatus.Controls.Add(this.lbStandbyCount);
            this.plDownStatus.Controls.Add(this.label4);
            this.plDownStatus.Controls.Add(this.lbProcessingCount);
            this.plDownStatus.Controls.Add(this.label2);
            this.plDownStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plDownStatus.ForeColor = System.Drawing.Color.White;
            this.plDownStatus.Location = new System.Drawing.Point(0, 662);
            this.plDownStatus.Name = "plDownStatus";
            this.plDownStatus.Size = new System.Drawing.Size(1181, 28);
            this.plDownStatus.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.iconFooter2;
            this.panel5.Location = new System.Drawing.Point(216, 6);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(18, 18);
            this.panel5.TabIndex = 6;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.iconFooter1;
            this.panel4.Location = new System.Drawing.Point(7, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(18, 18);
            this.panel4.TabIndex = 5;
            // 
            // lbStandbyCount
            // 
            this.lbStandbyCount.BackColor = System.Drawing.Color.Transparent;
            this.lbStandbyCount.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStandbyCount.ForeColor = System.Drawing.Color.White;
            this.lbStandbyCount.Location = new System.Drawing.Point(327, 6);
            this.lbStandbyCount.Name = "lbStandbyCount";
            this.lbStandbyCount.Size = new System.Drawing.Size(80, 17);
            this.lbStandbyCount.TabIndex = 4;
            this.lbStandbyCount.Text = "-";
            this.lbStandbyCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(238, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "대기카운트 :";
            // 
            // lbProcessingCount
            // 
            this.lbProcessingCount.BackColor = System.Drawing.Color.Transparent;
            this.lbProcessingCount.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbProcessingCount.ForeColor = System.Drawing.Color.White;
            this.lbProcessingCount.Location = new System.Drawing.Point(123, 6);
            this.lbProcessingCount.Name = "lbProcessingCount";
            this.lbProcessingCount.Size = new System.Drawing.Size(80, 17);
            this.lbProcessingCount.TabIndex = 2;
            this.lbProcessingCount.Text = "-";
            this.lbProcessingCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbProcessingCount.Click += new System.EventHandler(this.lbProcessingCount_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(29, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "처리 카운트 :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pltopMenu
            // 
            this.pltopMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.pltopMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pltopMenu.Controls.Add(this.btnClose);
            this.pltopMenu.Controls.Add(this.btnResetTcRecvCount);
            this.pltopMenu.Controls.Add(this.btnConfig);
            this.pltopMenu.Controls.Add(this.panel2);
            this.pltopMenu.Controls.Add(this.lbDate);
            this.pltopMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pltopMenu.Location = new System.Drawing.Point(0, 0);
            this.pltopMenu.Name = "pltopMenu";
            this.pltopMenu.Size = new System.Drawing.Size(1181, 44);
            this.pltopMenu.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(529, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "종료";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnResetTcRecvCount
            // 
            this.btnResetTcRecvCount.Location = new System.Drawing.Point(610, 11);
            this.btnResetTcRecvCount.Name = "btnResetTcRecvCount";
            this.btnResetTcRecvCount.Size = new System.Drawing.Size(144, 23);
            this.btnResetTcRecvCount.TabIndex = 4;
            this.btnResetTcRecvCount.Text = "TC수신카운트초기화";
            this.btnResetTcRecvCount.UseVisualStyleBackColor = true;
            this.btnResetTcRecvCount.Visible = false;
            this.btnResetTcRecvCount.Click += new System.EventHandler(this.btnResetTcRecvCount_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.AnimationInterval = 300;
            this.btnConfig.CheckedValue = false;
            this.btnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfig.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnConfig.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnMenuNormal;
            this.btnConfig.ImgDisable = null;
            this.btnConfig.ImgHover = global::NCasPDbManager.NCasPDbManagerRsc.btnMenuSelected;
            this.btnConfig.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnMenuSelected;
            this.btnConfig.ImgStatusEvent = null;
            this.btnConfig.ImgStatusNormal = null;
            this.btnConfig.ImgStatusOffsetX = 2;
            this.btnConfig.ImgStatusOffsetY = 0;
            this.btnConfig.IsStatusNormal = true;
            this.btnConfig.Location = new System.Drawing.Point(234, 0);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(159, 44);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "환경설정";
            this.btnConfig.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnConfig.UseAutoWrap = false;
            this.btnConfig.UseCheck = false;
            this.btnConfig.UseImgStretch = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.menu00Title;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(234, 44);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(33, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB 매니저";
            // 
            // lbDate
            // 
            this.lbDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDate.BackColor = System.Drawing.Color.Transparent;
            this.lbDate.Font = new System.Drawing.Font("나눔바른고딕 옛한글", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lbDate.Location = new System.Drawing.Point(928, 10);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(245, 25);
            this.lbDate.TabIndex = 0;
            this.lbDate.Text = "-";
            this.lbDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1181, 690);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.plDownStatus);
            this.Controls.Add(this.pltopMenu);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "시도DB매니저프로그램";
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            this.plClientAdminTitle.ResumeLayout(false);
            this.plClientAdminTitle.PerformLayout();
            this.plEventTitle.ResumeLayout(false);
            this.plEventTitle.PerformLayout();
            this.plDownStatus.ResumeLayout(false);
            this.plDownStatus.PerformLayout();
            this.pltopMenu.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private  NCASFND.NCasCtrl.NCasPanel pltopMenu;
        private NCASFND.NCasCtrl.NCasButton btnConfig;
        private  NCASFND.NCasCtrl.NCasPanel panel2;
        private System.Windows.Forms.Label label1;
        private  NCASFND.NCasCtrl.NCasPanel plDownStatus;
        private System.Windows.Forms.Label lbStandbyCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbProcessingCount;
        private System.Windows.Forms.Label label2;
        private  NCASFND.NCasCtrl.NCasPanel panel5;
        private  NCASFND.NCasCtrl.NCasPanel panel4;
        private System.Windows.Forms.SplitContainer spcMain;
        private NCASFND.NCasCtrl.NCasListView lvClientAdmin;
        private  NCASFND.NCasCtrl.NCasPanel plClientAdminTitle;
        private NCASFND.NCasCtrl.NCasListView lvEvent;
        private  NCASFND.NCasCtrl.NCasPanel plEventTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Button btnResetTcRecvCount;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbDate;

    }
}

