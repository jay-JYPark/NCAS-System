namespace NCasPBrdScreen
{
    partial class CoverView
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
            this.imagePanel = new System.Windows.Forms.Panel();
            this.verLabel = new System.Windows.Forms.Label();
            this.imagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.BackColor = System.Drawing.Color.White;
            this.imagePanel.BackgroundImage = global::NCasPBrdScreen.NCasPBrdScreenRsc.mainBgBottom;
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imagePanel.Controls.Add(this.verLabel);
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(1920, 914);
            this.imagePanel.TabIndex = 1;
            // 
            // verLabel
            // 
            this.verLabel.BackColor = System.Drawing.Color.Transparent;
            this.verLabel.Font = new System.Drawing.Font("나눔고딕", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.verLabel.ForeColor = System.Drawing.Color.White;
            this.verLabel.Location = new System.Drawing.Point(1271, 834);
            this.verLabel.Name = "verLabel";
            this.verLabel.Size = new System.Drawing.Size(602, 50);
            this.verLabel.TabIndex = 1;
            this.verLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.verLabel.Visible = false;
            // 
            // CoverView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.imagePanel);
            this.Name = "CoverView";
            this.Size = new System.Drawing.Size(1920, 914);
            this.imagePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.Label verLabel;

    }
}
