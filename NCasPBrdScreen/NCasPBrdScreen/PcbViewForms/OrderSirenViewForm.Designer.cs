namespace NCasPBrdScreen
{
    partial class OrderSirenViewForm
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
            this.sirenKindLabel = new System.Windows.Forms.Label();
            this.timeProgressBar = new System.Windows.Forms.ProgressBar();
            this.perLabel = new System.Windows.Forms.Label();
            this.sirenOffBtn = new NCASFND.NCasCtrl.NCasButton();
            this.SuspendLayout();
            // 
            // sirenKindLabel
            // 
            this.sirenKindLabel.BackColor = System.Drawing.Color.Transparent;
            this.sirenKindLabel.Font = new System.Drawing.Font("맑은 고딕", 24.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.sirenKindLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(18)))), ((int)(((byte)(129)))));
            this.sirenKindLabel.Location = new System.Drawing.Point(15, 10);
            this.sirenKindLabel.Name = "sirenKindLabel";
            this.sirenKindLabel.Size = new System.Drawing.Size(360, 53);
            this.sirenKindLabel.TabIndex = 0;
            this.sirenKindLabel.Text = "재난경계 사이렌";
            this.sirenKindLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeProgressBar
            // 
            this.timeProgressBar.BackColor = System.Drawing.Color.White;
            this.timeProgressBar.Location = new System.Drawing.Point(53, 99);
            this.timeProgressBar.Name = "timeProgressBar";
            this.timeProgressBar.Size = new System.Drawing.Size(284, 24);
            this.timeProgressBar.Step = 1;
            this.timeProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.timeProgressBar.TabIndex = 1;
            // 
            // perLabel
            // 
            this.perLabel.BackColor = System.Drawing.Color.Transparent;
            this.perLabel.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.perLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.perLabel.Location = new System.Drawing.Point(155, 65);
            this.perLabel.Name = "perLabel";
            this.perLabel.Size = new System.Drawing.Size(80, 30);
            this.perLabel.TabIndex = 2;
            this.perLabel.Text = "0 %";
            this.perLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sirenOffBtn
            // 
            this.sirenOffBtn.AnimationInterval = 300;
            this.sirenOffBtn.CheckedValue = false;
            this.sirenOffBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sirenOffBtn.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.sirenOffBtn.ForeColor = System.Drawing.Color.White;
            this.sirenOffBtn.Image = global::NCasPBrdScreen.NCasPBrdScreenRsc.btnSirenNormal;
            this.sirenOffBtn.ImgDisable = global::NCasPBrdScreen.NCasPBrdScreenRsc.btnSirenPress;
            this.sirenOffBtn.ImgHover = null;
            this.sirenOffBtn.ImgSelect = global::NCasPBrdScreen.NCasPBrdScreenRsc.btnSirenPress;
            this.sirenOffBtn.ImgStatusEvent = null;
            this.sirenOffBtn.ImgStatusNormal = null;
            this.sirenOffBtn.ImgStatusOffsetX = 2;
            this.sirenOffBtn.ImgStatusOffsetY = 0;
            this.sirenOffBtn.IsStatusNormal = true;
            this.sirenOffBtn.Location = new System.Drawing.Point(138, 140);
            this.sirenOffBtn.Name = "sirenOffBtn";
            this.sirenOffBtn.Size = new System.Drawing.Size(115, 44);
            this.sirenOffBtn.TabIndex = 3;
            this.sirenOffBtn.Text = "사이렌 OFF";
            this.sirenOffBtn.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.sirenOffBtn.UseAutoWrap = false;
            this.sirenOffBtn.UseCheck = false;
            this.sirenOffBtn.UseImgStretch = false;
            this.sirenOffBtn.Click += new System.EventHandler(this.sirenOffBtn_Click);
            // 
            // OrderSirenViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NCasPBrdScreen.NCasPBrdScreenRsc.bgLoading;
            this.ClientSize = new System.Drawing.Size(390, 208);
            this.ControlBox = false;
            this.Controls.Add(this.sirenOffBtn);
            this.Controls.Add(this.perLabel);
            this.Controls.Add(this.timeProgressBar);
            this.Controls.Add(this.sirenKindLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderSirenViewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label sirenKindLabel;
        private System.Windows.Forms.ProgressBar timeProgressBar;
        private System.Windows.Forms.Label perLabel;
        private NCASFND.NCasCtrl.NCasButton sirenOffBtn;
    }
}