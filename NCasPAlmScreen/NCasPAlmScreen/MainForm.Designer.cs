namespace NCasPAlmScreen
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
            this.topMainPanel = new NCASFND.NCasCtrl.NCasPanel();
            this.reOrderMenu = new NCASFND.NCasCtrl.NCasButton();
            this.labelMainTime = new System.Windows.Forms.Label();
            this.labelDevStatus9 = new System.Windows.Forms.Label();
            this.devStatusImageList = new System.Windows.Forms.ImageList(this.components);
            this.topLeftNameLabel = new System.Windows.Forms.Label();
            this.labelDevStatus8 = new System.Windows.Forms.Label();
            this.topRightLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelDevStatus4 = new System.Windows.Forms.Label();
            this.btnBroadTextMenu = new NCASFND.NCasCtrl.NCasButton();
            this.labelDevStatus1 = new System.Windows.Forms.Label();
            this.btnDevMonMenu = new NCASFND.NCasCtrl.NCasButton();
            this.labelDevStatus2 = new System.Windows.Forms.Label();
            this.btnOrderResultMenu = new NCASFND.NCasCtrl.NCasButton();
            this.labelDevStatus3 = new System.Windows.Forms.Label();
            this.btnOrderMenu = new NCASFND.NCasCtrl.NCasButton();
            this.labelDevStatus5 = new System.Windows.Forms.Label();
            this.labelDevStatus6 = new System.Windows.Forms.Label();
            this.labelDevStatus7 = new System.Windows.Forms.Label();
            this.logoImageList = new System.Windows.Forms.ImageList(this.components);
            this.middlePanel = new NCASFND.NCasCtrl.NCasPanel();
            this.topMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topRightLogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // topMainPanel
            // 
            this.topMainPanel.BackColor = System.Drawing.Color.Black;
            this.topMainPanel.BackgroundImage = global::NCasPAlmScreen.NCasPAlmScreenRsc.bgMainUp;
            this.topMainPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.topMainPanel.Controls.Add(this.reOrderMenu);
            this.topMainPanel.Controls.Add(this.labelMainTime);
            this.topMainPanel.Controls.Add(this.labelDevStatus9);
            this.topMainPanel.Controls.Add(this.topLeftNameLabel);
            this.topMainPanel.Controls.Add(this.labelDevStatus8);
            this.topMainPanel.Controls.Add(this.topRightLogoPictureBox);
            this.topMainPanel.Controls.Add(this.labelDevStatus4);
            this.topMainPanel.Controls.Add(this.btnBroadTextMenu);
            this.topMainPanel.Controls.Add(this.labelDevStatus1);
            this.topMainPanel.Controls.Add(this.btnDevMonMenu);
            this.topMainPanel.Controls.Add(this.labelDevStatus2);
            this.topMainPanel.Controls.Add(this.btnOrderResultMenu);
            this.topMainPanel.Controls.Add(this.labelDevStatus3);
            this.topMainPanel.Controls.Add(this.btnOrderMenu);
            this.topMainPanel.Controls.Add(this.labelDevStatus5);
            this.topMainPanel.Controls.Add(this.labelDevStatus6);
            this.topMainPanel.Controls.Add(this.labelDevStatus7);
            this.topMainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topMainPanel.Location = new System.Drawing.Point(0, 0);
            this.topMainPanel.Name = "topMainPanel";
            this.topMainPanel.Size = new System.Drawing.Size(1920, 165);
            this.topMainPanel.TabIndex = 0;
            // 
            // reOrderMenu
            // 
            this.reOrderMenu.AnimationInterval = 300;
            this.reOrderMenu.CheckedValue = false;
            this.reOrderMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.reOrderMenu.Font = new System.Drawing.Font("나눔바른고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.reOrderMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.reOrderMenu.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Up210;
            this.reOrderMenu.ImgDisable = null;
            this.reOrderMenu.ImgHover = null;
            this.reOrderMenu.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Down210;
            this.reOrderMenu.ImgStatusEvent = null;
            this.reOrderMenu.ImgStatusNormal = null;
            this.reOrderMenu.ImgStatusOffsetX = 2;
            this.reOrderMenu.ImgStatusOffsetY = 0;
            this.reOrderMenu.IsStatusNormal = true;
            this.reOrderMenu.Location = new System.Drawing.Point(669, 81);
            this.reOrderMenu.Name = "reOrderMenu";
            this.reOrderMenu.Size = new System.Drawing.Size(210, 69);
            this.reOrderMenu.TabIndex = 28;
            this.reOrderMenu.Text = "재발령";
            this.reOrderMenu.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.reOrderMenu.UseAutoWrap = false;
            this.reOrderMenu.UseCheck = true;
            this.reOrderMenu.UseImgStretch = false;
            this.reOrderMenu.Click += new System.EventHandler(this.reOrderMenu_Click);
            // 
            // labelMainTime
            // 
            this.labelMainTime.AutoSize = true;
            this.labelMainTime.BackColor = System.Drawing.Color.Transparent;
            this.labelMainTime.Font = new System.Drawing.Font("나눔바른고딕 옛한글", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMainTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.labelMainTime.Location = new System.Drawing.Point(1450, 10);
            this.labelMainTime.Name = "labelMainTime";
            this.labelMainTime.Size = new System.Drawing.Size(0, 42);
            this.labelMainTime.TabIndex = 27;
            // 
            // labelDevStatus9
            // 
            this.labelDevStatus9.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus9.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus9.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus9.ImageIndex = 1;
            this.labelDevStatus9.ImageList = this.devStatusImageList;
            this.labelDevStatus9.Location = new System.Drawing.Point(1372, 122);
            this.labelDevStatus9.Name = "labelDevStatus9";
            this.labelDevStatus9.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus9.TabIndex = 26;
            this.labelDevStatus9.Tag = "8";
            this.labelDevStatus9.Text = "        방송대#2";
            this.labelDevStatus9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // devStatusImageList
            // 
            this.devStatusImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("devStatusImageList.ImageStream")));
            this.devStatusImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.devStatusImageList.Images.SetKeyName(0, "iconStateGreen.png");
            this.devStatusImageList.Images.SetKeyName(1, "iconStateRed.png");
            // 
            // topLeftNameLabel
            // 
            this.topLeftNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.topLeftNameLabel.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.topLeftNameLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.topLeftNameLabel.Location = new System.Drawing.Point(776, 6);
            this.topLeftNameLabel.Name = "topLeftNameLabel";
            this.topLeftNameLabel.Size = new System.Drawing.Size(368, 35);
            this.topLeftNameLabel.TabIndex = 1;
            this.topLeftNameLabel.Tag = "";
            this.topLeftNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.topLeftNameLabel_MouseDown);
            // 
            // labelDevStatus8
            // 
            this.labelDevStatus8.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus8.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus8.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus8.ImageIndex = 1;
            this.labelDevStatus8.ImageList = this.devStatusImageList;
            this.labelDevStatus8.Location = new System.Drawing.Point(1245, 122);
            this.labelDevStatus8.Name = "labelDevStatus8";
            this.labelDevStatus8.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus8.TabIndex = 24;
            this.labelDevStatus8.Tag = "8";
            this.labelDevStatus8.Text = "        방송대#1";
            this.labelDevStatus8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topRightLogoPictureBox
            // 
            this.topRightLogoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.topRightLogoPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.topRightLogoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.topRightLogoPictureBox.Location = new System.Drawing.Point(154, 4);
            this.topRightLogoPictureBox.Name = "topRightLogoPictureBox";
            this.topRightLogoPictureBox.Size = new System.Drawing.Size(247, 56);
            this.topRightLogoPictureBox.TabIndex = 0;
            this.topRightLogoPictureBox.TabStop = false;
            // 
            // labelDevStatus4
            // 
            this.labelDevStatus4.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus4.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus4.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus4.ImageIndex = 1;
            this.labelDevStatus4.ImageList = this.devStatusImageList;
            this.labelDevStatus4.Location = new System.Drawing.Point(1501, 88);
            this.labelDevStatus4.Name = "labelDevStatus4";
            this.labelDevStatus4.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus4.TabIndex = 18;
            this.labelDevStatus4.Tag = "4";
            this.labelDevStatus4.Text = "        2중앙경보대#2";
            this.labelDevStatus4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBroadTextMenu
            // 
            this.btnBroadTextMenu.AnimationInterval = 300;
            this.btnBroadTextMenu.CheckedValue = false;
            this.btnBroadTextMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBroadTextMenu.Font = new System.Drawing.Font("나눔바른고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBroadTextMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.btnBroadTextMenu.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn003Up210;
            this.btnBroadTextMenu.ImgDisable = null;
            this.btnBroadTextMenu.ImgHover = null;
            this.btnBroadTextMenu.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn003Down210;
            this.btnBroadTextMenu.ImgStatusEvent = null;
            this.btnBroadTextMenu.ImgStatusNormal = null;
            this.btnBroadTextMenu.ImgStatusOffsetX = 2;
            this.btnBroadTextMenu.ImgStatusOffsetY = 0;
            this.btnBroadTextMenu.IsStatusNormal = true;
            this.btnBroadTextMenu.Location = new System.Drawing.Point(879, 81);
            this.btnBroadTextMenu.Name = "btnBroadTextMenu";
            this.btnBroadTextMenu.Size = new System.Drawing.Size(210, 69);
            this.btnBroadTextMenu.TabIndex = 5;
            this.btnBroadTextMenu.Text = "TTS편집";
            this.btnBroadTextMenu.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnBroadTextMenu.UseAutoWrap = false;
            this.btnBroadTextMenu.UseCheck = false;
            this.btnBroadTextMenu.UseImgStretch = false;
            this.btnBroadTextMenu.Click += new System.EventHandler(this.btnBroadTextMenu_Click);
            // 
            // labelDevStatus1
            // 
            this.labelDevStatus1.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus1.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus1.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus1.ImageIndex = 1;
            this.labelDevStatus1.ImageList = this.devStatusImageList;
            this.labelDevStatus1.Location = new System.Drawing.Point(1117, 88);
            this.labelDevStatus1.Name = "labelDevStatus1";
            this.labelDevStatus1.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus1.TabIndex = 10;
            this.labelDevStatus1.Tag = "1";
            this.labelDevStatus1.Text = "        중앙경보대#1";
            this.labelDevStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDevMonMenu
            // 
            this.btnDevMonMenu.AnimationInterval = 300;
            this.btnDevMonMenu.CheckedValue = false;
            this.btnDevMonMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDevMonMenu.Font = new System.Drawing.Font("나눔바른고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDevMonMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.btnDevMonMenu.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Up210;
            this.btnDevMonMenu.ImgDisable = null;
            this.btnDevMonMenu.ImgHover = null;
            this.btnDevMonMenu.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Down210;
            this.btnDevMonMenu.ImgStatusEvent = null;
            this.btnDevMonMenu.ImgStatusNormal = null;
            this.btnDevMonMenu.ImgStatusOffsetX = 2;
            this.btnDevMonMenu.ImgStatusOffsetY = 0;
            this.btnDevMonMenu.IsStatusNormal = true;
            this.btnDevMonMenu.Location = new System.Drawing.Point(459, 81);
            this.btnDevMonMenu.Name = "btnDevMonMenu";
            this.btnDevMonMenu.Size = new System.Drawing.Size(210, 69);
            this.btnDevMonMenu.TabIndex = 4;
            this.btnDevMonMenu.Text = "장비감시";
            this.btnDevMonMenu.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnDevMonMenu.UseAutoWrap = false;
            this.btnDevMonMenu.UseCheck = true;
            this.btnDevMonMenu.UseImgStretch = false;
            this.btnDevMonMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnOrderMenu_MouseDown);
            // 
            // labelDevStatus2
            // 
            this.labelDevStatus2.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus2.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus2.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus2.ImageIndex = 1;
            this.labelDevStatus2.ImageList = this.devStatusImageList;
            this.labelDevStatus2.Location = new System.Drawing.Point(1245, 88);
            this.labelDevStatus2.Name = "labelDevStatus2";
            this.labelDevStatus2.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus2.TabIndex = 14;
            this.labelDevStatus2.Tag = "2";
            this.labelDevStatus2.Text = "        중앙경보대#2";
            this.labelDevStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOrderResultMenu
            // 
            this.btnOrderResultMenu.AnimationInterval = 300;
            this.btnOrderResultMenu.CheckedValue = false;
            this.btnOrderResultMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOrderResultMenu.Font = new System.Drawing.Font("나눔바른고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOrderResultMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.btnOrderResultMenu.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Up210;
            this.btnOrderResultMenu.ImgDisable = null;
            this.btnOrderResultMenu.ImgHover = null;
            this.btnOrderResultMenu.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn002Down210;
            this.btnOrderResultMenu.ImgStatusEvent = null;
            this.btnOrderResultMenu.ImgStatusNormal = null;
            this.btnOrderResultMenu.ImgStatusOffsetX = 2;
            this.btnOrderResultMenu.ImgStatusOffsetY = 0;
            this.btnOrderResultMenu.IsStatusNormal = true;
            this.btnOrderResultMenu.Location = new System.Drawing.Point(249, 81);
            this.btnOrderResultMenu.Name = "btnOrderResultMenu";
            this.btnOrderResultMenu.Size = new System.Drawing.Size(210, 69);
            this.btnOrderResultMenu.TabIndex = 3;
            this.btnOrderResultMenu.Text = "발령결과";
            this.btnOrderResultMenu.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnOrderResultMenu.UseAutoWrap = false;
            this.btnOrderResultMenu.UseCheck = true;
            this.btnOrderResultMenu.UseImgStretch = false;
            this.btnOrderResultMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnOrderMenu_MouseDown);
            // 
            // labelDevStatus3
            // 
            this.labelDevStatus3.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus3.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus3.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus3.ImageIndex = 1;
            this.labelDevStatus3.ImageList = this.devStatusImageList;
            this.labelDevStatus3.Location = new System.Drawing.Point(1372, 88);
            this.labelDevStatus3.Name = "labelDevStatus3";
            this.labelDevStatus3.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus3.TabIndex = 15;
            this.labelDevStatus3.Tag = "3";
            this.labelDevStatus3.Text = "        2중앙경보대#1";
            this.labelDevStatus3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOrderMenu
            // 
            this.btnOrderMenu.AnimationInterval = 300;
            this.btnOrderMenu.CheckedValue = false;
            this.btnOrderMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOrderMenu.Font = new System.Drawing.Font("나눔바른고딕", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOrderMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.btnOrderMenu.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn001Up210;
            this.btnOrderMenu.ImgDisable = null;
            this.btnOrderMenu.ImgHover = null;
            this.btnOrderMenu.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btn001Down210;
            this.btnOrderMenu.ImgStatusEvent = null;
            this.btnOrderMenu.ImgStatusNormal = null;
            this.btnOrderMenu.ImgStatusOffsetX = 2;
            this.btnOrderMenu.ImgStatusOffsetY = 0;
            this.btnOrderMenu.IsStatusNormal = true;
            this.btnOrderMenu.Location = new System.Drawing.Point(39, 81);
            this.btnOrderMenu.Name = "btnOrderMenu";
            this.btnOrderMenu.Size = new System.Drawing.Size(210, 69);
            this.btnOrderMenu.TabIndex = 2;
            this.btnOrderMenu.Text = "경보발령";
            this.btnOrderMenu.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnOrderMenu.UseAutoWrap = false;
            this.btnOrderMenu.UseCheck = true;
            this.btnOrderMenu.UseImgStretch = false;
            this.btnOrderMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnOrderMenu_MouseDown);
            // 
            // labelDevStatus5
            // 
            this.labelDevStatus5.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus5.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus5.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus5.ImageIndex = 1;
            this.labelDevStatus5.ImageList = this.devStatusImageList;
            this.labelDevStatus5.Location = new System.Drawing.Point(1630, 88);
            this.labelDevStatus5.Name = "labelDevStatus5";
            this.labelDevStatus5.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus5.TabIndex = 20;
            this.labelDevStatus5.Tag = "5";
            this.labelDevStatus5.Text = "        PLC";
            this.labelDevStatus5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelDevStatus6
            // 
            this.labelDevStatus6.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus6.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus6.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus6.ImageIndex = 1;
            this.labelDevStatus6.ImageList = this.devStatusImageList;
            this.labelDevStatus6.Location = new System.Drawing.Point(1758, 88);
            this.labelDevStatus6.Name = "labelDevStatus6";
            this.labelDevStatus6.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus6.TabIndex = 22;
            this.labelDevStatus6.Tag = "6";
            this.labelDevStatus6.Text = "        주제어";
            this.labelDevStatus6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelDevStatus7
            // 
            this.labelDevStatus7.BackColor = System.Drawing.Color.Transparent;
            this.labelDevStatus7.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDevStatus7.ForeColor = System.Drawing.Color.Red;
            this.labelDevStatus7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDevStatus7.ImageIndex = 1;
            this.labelDevStatus7.ImageList = this.devStatusImageList;
            this.labelDevStatus7.Location = new System.Drawing.Point(1117, 122);
            this.labelDevStatus7.Name = "labelDevStatus7";
            this.labelDevStatus7.Size = new System.Drawing.Size(118, 20);
            this.labelDevStatus7.TabIndex = 23;
            this.labelDevStatus7.Tag = "7";
            this.labelDevStatus7.Text = "        DUAL";
            this.labelDevStatus7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logoImageList
            // 
            this.logoImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("logoImageList.ImageStream")));
            this.logoImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.logoImageList.Images.SetKeyName(0, "1670.png");
            this.logoImageList.Images.SetKeyName(1, "1671.png");
            this.logoImageList.Images.SetKeyName(2, "1672.png");
            this.logoImageList.Images.SetKeyName(3, "1673.png");
            this.logoImageList.Images.SetKeyName(4, "1674.png");
            this.logoImageList.Images.SetKeyName(5, "1675.png");
            this.logoImageList.Images.SetKeyName(6, "1676.png");
            this.logoImageList.Images.SetKeyName(7, "1677.png");
            this.logoImageList.Images.SetKeyName(8, "1678.png");
            this.logoImageList.Images.SetKeyName(9, "1679.png");
            this.logoImageList.Images.SetKeyName(10, "1680.png");
            this.logoImageList.Images.SetKeyName(11, "1681.png");
            this.logoImageList.Images.SetKeyName(12, "1682.png");
            this.logoImageList.Images.SetKeyName(13, "1683.png");
            this.logoImageList.Images.SetKeyName(14, "1684.png");
            this.logoImageList.Images.SetKeyName(15, "1685.png");
            this.logoImageList.Images.SetKeyName(16, "1686.png");
            // 
            // middlePanel
            // 
            this.middlePanel.BackColor = System.Drawing.Color.Black;
            this.middlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.middlePanel.Location = new System.Drawing.Point(0, 165);
            this.middlePanel.Name = "middlePanel";
            this.middlePanel.Size = new System.Drawing.Size(1920, 915);
            this.middlePanel.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.ControlBox = false;
            this.Controls.Add(this.middlePanel);
            this.Controls.Add(this.topMainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1280, 1024);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.topMainPanel.ResumeLayout(false);
            this.topMainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topRightLogoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NCASFND.NCasCtrl.NCasPanel topMainPanel;
        private System.Windows.Forms.Label topLeftNameLabel;
        private System.Windows.Forms.PictureBox topRightLogoPictureBox;
        private NCASFND.NCasCtrl.NCasButton btnBroadTextMenu;
        private NCASFND.NCasCtrl.NCasButton btnDevMonMenu;
        private NCASFND.NCasCtrl.NCasButton btnOrderResultMenu;
        private NCASFND.NCasCtrl.NCasButton btnOrderMenu;
        private System.Windows.Forms.ImageList logoImageList;
        private System.Windows.Forms.ImageList devStatusImageList;
        private System.Windows.Forms.Label labelDevStatus9;
        private System.Windows.Forms.Label labelDevStatus8;
        private System.Windows.Forms.Label labelDevStatus7;
        private System.Windows.Forms.Label labelDevStatus6;
        private System.Windows.Forms.Label labelDevStatus5;
        private System.Windows.Forms.Label labelDevStatus3;
        private System.Windows.Forms.Label labelDevStatus2;
        private System.Windows.Forms.Label labelDevStatus1;
        private System.Windows.Forms.Label labelDevStatus4;
        private NCASFND.NCasCtrl.NCasPanel middlePanel;
        private System.Windows.Forms.Label labelMainTime;
        private NCASFND.NCasCtrl.NCasButton reOrderMenu;
    }
}

