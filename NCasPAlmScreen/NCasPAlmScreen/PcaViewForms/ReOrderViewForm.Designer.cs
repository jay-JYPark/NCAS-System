namespace NCasPAlmScreen
{
    partial class ReOrderViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReOrderViewForm));
            this.labelTitle = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.Panel();
            this.reOrderListView = new NCASFND.NCasCtrl.NCasListView();
            this.orderResultImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnReOrder = new NCASFND.NCasCtrl.NCasButton();
            this.btnClose = new NCASFND.NCasCtrl.NCasButton();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("나눔바른고딕", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(8, 21);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(300, 19);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "아래 단말을 대상으로 재발령을 진행합니다.";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topPanel
            // 
            this.topPanel.BackgroundImage = global::NCasPAlmScreen.NCasPAlmScreenRsc.popBgTop;
            this.topPanel.Controls.Add(this.labelTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1357, 56);
            this.topPanel.TabIndex = 5;
            // 
            // reOrderListView
            // 
            this.reOrderListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reOrderListView.AntiAlias = false;
            this.reOrderListView.AutoFit = false;
            this.reOrderListView.BackColor = System.Drawing.Color.White;
            this.reOrderListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.reOrderListView.ColumnHeight = 24;
            this.reOrderListView.ColumnOffset = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.reOrderListView.ColumnStyle = NCASFND.NCasListViewColumnStyle.DefaultStyle;
            this.reOrderListView.FocusedItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.reOrderListView.FrozenColumnIndex = -1;
            this.reOrderListView.FullRowSelect = true;
            this.reOrderListView.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            this.reOrderListView.GridLineStyle = NCASFND.NCasListViewGridLine.GridNone;
            this.reOrderListView.HideColumnCheckBox = false;
            this.reOrderListView.HideSelection = false;
            this.reOrderListView.HoverSelection = false;
            this.reOrderListView.IconOffset = new System.Drawing.Point(1, 0);
            this.reOrderListView.InteraceColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.reOrderListView.InteraceColor2 = System.Drawing.Color.White;
            this.reOrderListView.ItemHeight = 18;
            this.reOrderListView.ItemOffset = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.reOrderListView.Location = new System.Drawing.Point(0, 56);
            this.reOrderListView.MultiSelect = false;
            this.reOrderListView.Name = "reOrderListView";
            this.reOrderListView.ScrollType = NCASFND.NCasListViewScrollType.ScrollBoth;
            this.reOrderListView.SelectioinItemBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(232)))), ((int)(((byte)(250)))));
            this.reOrderListView.Size = new System.Drawing.Size(1357, 711);
            this.reOrderListView.StateImageList = this.orderResultImageList;
            this.reOrderListView.TabIndex = 6;
            this.reOrderListView.UseFocusBar = true;
            this.reOrderListView.UseInteraceColor = true;
            this.reOrderListView.UseItemFitBox = false;
            this.reOrderListView.UseSelFocusedBar = false;
            // 
            // orderResultImageList
            // 
            this.orderResultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("orderResultImageList.ImageStream")));
            this.orderResultImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.orderResultImageList.Images.SetKeyName(0, "alertIcon1.png");
            this.orderResultImageList.Images.SetKeyName(1, "alertIcon2.png");
            this.orderResultImageList.Images.SetKeyName(2, "alertIcon3.png");
            this.orderResultImageList.Images.SetKeyName(3, "alertIcon4.png");
            this.orderResultImageList.Images.SetKeyName(4, "alertIcon5.png");
            this.orderResultImageList.Images.SetKeyName(5, "alertIcon6.png");
            this.orderResultImageList.Images.SetKeyName(6, "alertIcon7.png");
            // 
            // btnReOrder
            // 
            this.btnReOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReOrder.AnimationInterval = 300;
            this.btnReOrder.CheckedValue = false;
            this.btnReOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReOrder.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReOrder.ForeColor = System.Drawing.Color.White;
            this.btnReOrder.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSmallNormal;
            this.btnReOrder.ImgDisable = null;
            this.btnReOrder.ImgHover = null;
            this.btnReOrder.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSmallPress;
            this.btnReOrder.ImgStatusEvent = null;
            this.btnReOrder.ImgStatusNormal = null;
            this.btnReOrder.ImgStatusOffsetX = 2;
            this.btnReOrder.ImgStatusOffsetY = 0;
            this.btnReOrder.IsStatusNormal = true;
            this.btnReOrder.Location = new System.Drawing.Point(1170, 773);
            this.btnReOrder.Name = "btnReOrder";
            this.btnReOrder.Size = new System.Drawing.Size(89, 39);
            this.btnReOrder.TabIndex = 8;
            this.btnReOrder.Text = "재발령";
            this.btnReOrder.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnReOrder.UseAutoWrap = false;
            this.btnReOrder.UseCheck = false;
            this.btnReOrder.UseImgStretch = false;
            this.btnReOrder.Click += new System.EventHandler(this.btnReOrder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AnimationInterval = 300;
            this.btnClose.CheckedValue = false;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Image = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSmallNormal;
            this.btnClose.ImgDisable = null;
            this.btnClose.ImgHover = null;
            this.btnClose.ImgSelect = global::NCasPAlmScreen.NCasPAlmScreenRsc.btnSmallPress;
            this.btnClose.ImgStatusEvent = null;
            this.btnClose.ImgStatusNormal = null;
            this.btnClose.ImgStatusOffsetX = 2;
            this.btnClose.ImgStatusOffsetY = 0;
            this.btnClose.IsStatusNormal = true;
            this.btnClose.Location = new System.Drawing.Point(1259, 773);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(89, 39);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "닫기";
            this.btnClose.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnClose.UseAutoWrap = false;
            this.btnClose.UseCheck = false;
            this.btnClose.UseImgStretch = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ReOrderViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(1357, 820);
            this.ControlBox = false;
            this.Controls.Add(this.btnReOrder);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.reOrderListView);
            this.Controls.Add(this.topPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "ReOrderViewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel topPanel;
        private NCASFND.NCasCtrl.NCasListView reOrderListView;
        private NCASFND.NCasCtrl.NCasButton btnReOrder;
        private NCASFND.NCasCtrl.NCasButton btnClose;
        private System.Windows.Forms.ImageList orderResultImageList;
    }
}