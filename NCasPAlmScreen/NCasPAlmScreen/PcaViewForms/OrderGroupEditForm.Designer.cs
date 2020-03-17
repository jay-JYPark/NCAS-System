namespace NCasPAlmScreen
{
    partial class OrderGroupEditForm
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
            this.topPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.titlelabel = new System.Windows.Forms.Label();
            this.btnClose = new NCASFND.NCasCtrl.NCasButton();
            this.btnSave = new NCASFND.NCasCtrl.NCasButton();
            this.mainPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.groupMemberListView = new NCASFND.NCasCtrl.NCasListView();
            this.groupMemberTopPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.groupMemberTopLabel = new System.Windows.Forms.Label();
            this.topMainPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.groupNamePanel = new NCASFND.NCasCtrl.NCasPanel();
            this.groupSelectComboBox = new System.Windows.Forms.ComboBox();
            this.groupNameLabel1 = new System.Windows.Forms.Label();
            this.mainLabelPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.groupNameLabel = new System.Windows.Forms.Label();
            this.middlePanel = new System.Windows.Forms.Panel();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.groupMemberTopPanel.SuspendLayout();
            this.topMainPanel.SuspendLayout();
            this.groupNamePanel.SuspendLayout();
            this.mainLabelPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackgroundImage = global::NCasPAlmScreen.NCasPAlmScreenRsc.popBgTop;
            this.topPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.topPanel.Controls.Add(this.mainPictureBox);
            this.topPanel.Controls.Add(this.titlelabel);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(698, 56);
            this.topPanel.TabIndex = 0;
            // 
            // mainPictureBox
            // 
            this.mainPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.mainPictureBox.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.popGroupEditIcon;
            this.mainPictureBox.Location = new System.Drawing.Point(654, 12);
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.Size = new System.Drawing.Size(32, 32);
            this.mainPictureBox.TabIndex = 3;
            this.mainPictureBox.TabStop = false;
            // 
            // titlelabel
            // 
            this.titlelabel.AutoSize = true;
            this.titlelabel.BackColor = System.Drawing.Color.Transparent;
            this.titlelabel.Font = new System.Drawing.Font("나눔바른고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.titlelabel.ForeColor = System.Drawing.Color.White;
            this.titlelabel.Location = new System.Drawing.Point(17, 21);
            this.titlelabel.Name = "titlelabel";
            this.titlelabel.Size = new System.Drawing.Size(138, 19);
            this.titlelabel.TabIndex = 2;
            this.titlelabel.Text = "그룹을 편집합니다.";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AnimationInterval = 300;
            this.btnClose.CheckedValue = false;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("나눔바른고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSaveNormal;
            this.btnClose.ImgDisable = null;
            this.btnClose.ImgHover = null;
            this.btnClose.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSavePress;
            this.btnClose.ImgStatusEvent = null;
            this.btnClose.ImgStatusNormal = null;
            this.btnClose.ImgStatusOffsetX = 2;
            this.btnClose.ImgStatusOffsetY = 0;
            this.btnClose.IsStatusNormal = true;
            this.btnClose.Location = new System.Drawing.Point(571, 726);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(115, 43);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "닫기";
            this.btnClose.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnClose.UseAutoWrap = true;
            this.btnClose.UseCheck = false;
            this.btnClose.UseImgStretch = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AnimationInterval = 300;
            this.btnSave.CheckedValue = false;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("나눔바른고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSaveNormal;
            this.btnSave.ImgDisable = null;
            this.btnSave.ImgHover = null;
            this.btnSave.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSavePress;
            this.btnSave.ImgStatusEvent = null;
            this.btnSave.ImgStatusNormal = null;
            this.btnSave.ImgStatusOffsetX = 2;
            this.btnSave.ImgStatusOffsetY = 0;
            this.btnSave.IsStatusNormal = true;
            this.btnSave.Location = new System.Drawing.Point(456, 726);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 43);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "저장";
            this.btnSave.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnSave.UseAutoWrap = true;
            this.btnSave.UseCheck = false;
            this.btnSave.UseImgStretch = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.mainPanel.Controls.Add(this.middlePanel);
            this.mainPanel.Controls.Add(this.groupMemberListView);
            this.mainPanel.Controls.Add(this.groupMemberTopPanel);
            this.mainPanel.Controls.Add(this.topMainPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPanel.Location = new System.Drawing.Point(0, 56);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(698, 656);
            this.mainPanel.TabIndex = 3;
            // 
            // groupMemberListView
            // 
            this.groupMemberListView.AntiAlias = false;
            this.groupMemberListView.AutoFit = false;
            this.groupMemberListView.BackColor = System.Drawing.Color.White;
            this.groupMemberListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.groupMemberListView.ColumnHeight = 24;
            this.groupMemberListView.ColumnOffset = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.groupMemberListView.ColumnStyle = NCASFND.NCasListViewColumnStyle.DefaultStyle;
            this.groupMemberListView.FocusedItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.groupMemberListView.FrozenColumnIndex = -1;
            this.groupMemberListView.FullRowSelect = true;
            this.groupMemberListView.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.groupMemberListView.GridLineStyle = NCASFND.NCasListViewGridLine.GridNone;
            this.groupMemberListView.HideColumnCheckBox = false;
            this.groupMemberListView.HideSelection = false;
            this.groupMemberListView.HoverSelection = false;
            this.groupMemberListView.IconOffset = new System.Drawing.Point(1, 0);
            this.groupMemberListView.InteraceColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.groupMemberListView.InteraceColor2 = System.Drawing.Color.White;
            this.groupMemberListView.ItemHeight = 18;
            this.groupMemberListView.ItemOffset = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.groupMemberListView.Location = new System.Drawing.Point(0, 157);
            this.groupMemberListView.MultiSelect = false;
            this.groupMemberListView.Name = "groupMemberListView";
            this.groupMemberListView.ScrollType = NCASFND.NCasListViewScrollType.ScrollBoth;
            this.groupMemberListView.SelectioinItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(232)))), ((int)(((byte)(250)))));
            this.groupMemberListView.Size = new System.Drawing.Size(698, 496);
            this.groupMemberListView.TabIndex = 4;
            this.groupMemberListView.UseInteraceColor = true;
            this.groupMemberListView.UseSelFocusedBar = false;
            this.groupMemberListView.ItemChecked += new NCASFND.NCasItemCheckedEventHandler(this.groupMemberListView_ItemChecked);
            // 
            // groupMemberTopPanel
            // 
            this.groupMemberTopPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.groupMemberTopPanel.Controls.Add(this.groupMemberTopLabel);
            this.groupMemberTopPanel.Location = new System.Drawing.Point(0, 130);
            this.groupMemberTopPanel.Name = "groupMemberTopPanel";
            this.groupMemberTopPanel.Size = new System.Drawing.Size(698, 27);
            this.groupMemberTopPanel.TabIndex = 3;
            // 
            // groupMemberTopLabel
            // 
            this.groupMemberTopLabel.AutoSize = true;
            this.groupMemberTopLabel.BackColor = System.Drawing.Color.Transparent;
            this.groupMemberTopLabel.Font = new System.Drawing.Font("나눔바른고딕 옛한글", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupMemberTopLabel.ForeColor = System.Drawing.Color.White;
            this.groupMemberTopLabel.Location = new System.Drawing.Point(7, 5);
            this.groupMemberTopLabel.Name = "groupMemberTopLabel";
            this.groupMemberTopLabel.Size = new System.Drawing.Size(104, 17);
            this.groupMemberTopLabel.TabIndex = 1;
            this.groupMemberTopLabel.Text = "시군/단말 선택";
            // 
            // topMainPanel
            // 
            this.topMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.topMainPanel.Controls.Add(this.groupNamePanel);
            this.topMainPanel.Controls.Add(this.mainLabelPanel);
            this.topMainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topMainPanel.Location = new System.Drawing.Point(0, 0);
            this.topMainPanel.Name = "topMainPanel";
            this.topMainPanel.Size = new System.Drawing.Size(698, 110);
            this.topMainPanel.TabIndex = 2;
            // 
            // groupNamePanel
            // 
            this.groupNamePanel.BackgroundImage = global::NCasPAlmScreen.NCasPAlmScreenRsc.bgTxtGroupName;
            this.groupNamePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.groupNamePanel.Controls.Add(this.groupSelectComboBox);
            this.groupNamePanel.Controls.Add(this.groupNameLabel1);
            this.groupNamePanel.Location = new System.Drawing.Point(37, 50);
            this.groupNamePanel.Name = "groupNamePanel";
            this.groupNamePanel.Size = new System.Drawing.Size(624, 36);
            this.groupNamePanel.TabIndex = 1;
            // 
            // groupSelectComboBox
            // 
            this.groupSelectComboBox.BackColor = System.Drawing.Color.White;
            this.groupSelectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupSelectComboBox.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupSelectComboBox.FormattingEnabled = true;
            this.groupSelectComboBox.Location = new System.Drawing.Point(158, 7);
            this.groupSelectComboBox.Name = "groupSelectComboBox";
            this.groupSelectComboBox.Size = new System.Drawing.Size(408, 23);
            this.groupSelectComboBox.TabIndex = 1;
            this.groupSelectComboBox.SelectedIndexChanged += new System.EventHandler(this.groupSelectComboBox_SelectedIndexChanged);
            // 
            // groupNameLabel1
            // 
            this.groupNameLabel1.AutoSize = true;
            this.groupNameLabel1.BackColor = System.Drawing.Color.Transparent;
            this.groupNameLabel1.Font = new System.Drawing.Font("나눔바른고딕 옛한글", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupNameLabel1.ForeColor = System.Drawing.Color.White;
            this.groupNameLabel1.Location = new System.Drawing.Point(32, 11);
            this.groupNameLabel1.Name = "groupNameLabel1";
            this.groupNameLabel1.Size = new System.Drawing.Size(43, 15);
            this.groupNameLabel1.TabIndex = 0;
            this.groupNameLabel1.Text = "그룹명";
            this.groupNameLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainLabelPanel
            // 
            this.mainLabelPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.mainLabelPanel.Controls.Add(this.groupNameLabel);
            this.mainLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainLabelPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLabelPanel.Name = "mainLabelPanel";
            this.mainLabelPanel.Size = new System.Drawing.Size(698, 27);
            this.mainLabelPanel.TabIndex = 0;
            // 
            // groupNameLabel
            // 
            this.groupNameLabel.AutoSize = true;
            this.groupNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.groupNameLabel.Font = new System.Drawing.Font("나눔바른고딕 옛한글", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupNameLabel.ForeColor = System.Drawing.Color.White;
            this.groupNameLabel.Location = new System.Drawing.Point(7, 5);
            this.groupNameLabel.Name = "groupNameLabel";
            this.groupNameLabel.Size = new System.Drawing.Size(68, 17);
            this.groupNameLabel.TabIndex = 1;
            this.groupNameLabel.Text = "그룹 선택";
            // 
            // middlePanel
            // 
            this.middlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.middlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.middlePanel.Location = new System.Drawing.Point(0, 110);
            this.middlePanel.Name = "middlePanel";
            this.middlePanel.Size = new System.Drawing.Size(698, 20);
            this.middlePanel.TabIndex = 5;
            // 
            // OrderGroupEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(698, 781);
            this.ControlBox = false;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.topPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderGroupEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "그룹 편집";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.groupMemberTopPanel.ResumeLayout(false);
            this.groupMemberTopPanel.PerformLayout();
            this.topMainPanel.ResumeLayout(false);
            this.groupNamePanel.ResumeLayout(false);
            this.groupNamePanel.PerformLayout();
            this.mainLabelPanel.ResumeLayout(false);
            this.mainLabelPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private NCASFND.NCasCtrl.NCasPanel topPanel;
        private NCASFND.NCasCtrl.NCasButton btnClose;
        private NCASFND.NCasCtrl.NCasButton btnSave;
        private NCASFND.NCasCtrl.NCasPanel mainPanel;
        private NCASFND.NCasCtrl.NCasPanel mainLabelPanel;
        private System.Windows.Forms.Label groupNameLabel;
        private NCASFND.NCasCtrl.NCasListView groupMemberListView;
        private NCASFND.NCasCtrl.NCasPanel groupMemberTopPanel;
        private System.Windows.Forms.Label groupMemberTopLabel;
        private NCASFND.NCasCtrl.NCasPanel topMainPanel;
        private NCASFND.NCasCtrl.NCasPanel groupNamePanel;
        private System.Windows.Forms.Label groupNameLabel1;
        private System.Windows.Forms.ComboBox groupSelectComboBox;
        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.Label titlelabel;
        private System.Windows.Forms.Panel middlePanel;
    }
}