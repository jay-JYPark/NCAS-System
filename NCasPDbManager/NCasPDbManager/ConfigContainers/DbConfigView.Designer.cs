namespace NCasPDbManager
{
    partial class DbConfigView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDbConnectTest = new NCASFND.NCasCtrl.NCasButton();
            this.lbStatus = new System.Windows.Forms.Label();
            this.tbxServerUserPw = new System.Windows.Forms.TextBox();
            this.tbxServerUserId = new System.Windows.Forms.TextBox();
            this.tbxServerSid = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxServerIp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::NCasPDbManager.NCasPDbManagerRsc.bgSettingInfoTbl;
            this.panel1.Controls.Add(this.btnDbConnectTest);
            this.panel1.Controls.Add(this.lbStatus);
            this.panel1.Controls.Add(this.tbxServerUserPw);
            this.panel1.Controls.Add(this.tbxServerUserId);
            this.panel1.Controls.Add(this.tbxServerSid);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbxServerIp);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 187);
            this.panel1.TabIndex = 0;
            // 
            // btnDbConnectTest
            // 
            this.btnDbConnectTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDbConnectTest.AnimationInterval = 300;
            this.btnDbConnectTest.CheckedValue = false;
            this.btnDbConnectTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDbConnectTest.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDbConnectTest.ForeColor = System.Drawing.Color.White;
            this.btnDbConnectTest.Image = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintNormal;
            this.btnDbConnectTest.ImgDisable = null;
            this.btnDbConnectTest.ImgHover = null;
            this.btnDbConnectTest.ImgSelect = global::NCasPDbManager.NCasPDbManagerRsc.btnPrintPress;
            this.btnDbConnectTest.ImgStatusEvent = null;
            this.btnDbConnectTest.ImgStatusNormal = null;
            this.btnDbConnectTest.ImgStatusOffsetX = 2;
            this.btnDbConnectTest.ImgStatusOffsetY = 0;
            this.btnDbConnectTest.IsStatusNormal = true;
            this.btnDbConnectTest.Location = new System.Drawing.Point(331, 152);
            this.btnDbConnectTest.Name = "btnDbConnectTest";
            this.btnDbConnectTest.Size = new System.Drawing.Size(89, 39);
            this.btnDbConnectTest.TabIndex = 5;
            this.btnDbConnectTest.Text = "테스트";
            this.btnDbConnectTest.TxtAlignment = System.Drawing.StringAlignment.Center;
            this.btnDbConnectTest.UseAutoWrap = false;
            this.btnDbConnectTest.UseCheck = false;
            this.btnDbConnectTest.UseImgStretch = false;
            this.btnDbConnectTest.Click += new System.EventHandler(this.btnDbConnectTest_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.BackColor = System.Drawing.Color.Transparent;
            this.lbStatus.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(55)))), ((int)(((byte)(115)))));
            this.lbStatus.Location = new System.Drawing.Point(118, 161);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(13, 17);
            this.lbStatus.TabIndex = 32;
            this.lbStatus.Text = "-";
            // 
            // tbxServerUserPw
            // 
            this.tbxServerUserPw.Location = new System.Drawing.Point(118, 113);
            this.tbxServerUserPw.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerUserPw.MaxLength = 10;
            this.tbxServerUserPw.Name = "tbxServerUserPw";
            this.tbxServerUserPw.PasswordChar = '*';
            this.tbxServerUserPw.Size = new System.Drawing.Size(219, 23);
            this.tbxServerUserPw.TabIndex = 4;
            // 
            // tbxServerUserId
            // 
            this.tbxServerUserId.Location = new System.Drawing.Point(118, 78);
            this.tbxServerUserId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerUserId.MaxLength = 10;
            this.tbxServerUserId.Name = "tbxServerUserId";
            this.tbxServerUserId.Size = new System.Drawing.Size(219, 23);
            this.tbxServerUserId.TabIndex = 3;
            // 
            // tbxServerSid
            // 
            this.tbxServerSid.Location = new System.Drawing.Point(118, 42);
            this.tbxServerSid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerSid.MaxLength = 10;
            this.tbxServerSid.Name = "tbxServerSid";
            this.tbxServerSid.Size = new System.Drawing.Size(219, 23);
            this.tbxServerSid.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label8.Location = new System.Drawing.Point(344, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 17);
            this.label8.TabIndex = 31;
            this.label8.Text = "최대 10자";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label9.Location = new System.Drawing.Point(344, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 17);
            this.label9.TabIndex = 30;
            this.label9.Text = "최대 10자";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label7.Location = new System.Drawing.Point(344, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 29;
            this.label7.Text = "최대 10자";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label6.Location = new System.Drawing.Point(344, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 17);
            this.label6.TabIndex = 28;
            this.label6.Text = "최대 15자";
            // 
            // tbxServerIp
            // 
            this.tbxServerIp.Location = new System.Drawing.Point(118, 7);
            this.tbxServerIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxServerIp.MaxLength = 15;
            this.tbxServerIp.Name = "tbxServerIp";
            this.tbxServerIp.Size = new System.Drawing.Size(219, 23);
            this.tbxServerIp.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(16, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 17);
            this.label5.TabIndex = 27;
            this.label5.Text = "상태";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(16, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 26;
            this.label4.Text = "비밀번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(16, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 25;
            this.label3.Text = "사용자 이름";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(16, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 17);
            this.label2.TabIndex = 24;
            this.label2.Text = "SID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 23;
            this.label1.Text = "호스트 이름";
            // 
            // DbConfigView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "DbConfigView";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private NCASFND.NCasCtrl.NCasButton btnDbConnectTest;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.TextBox tbxServerUserPw;
        private System.Windows.Forms.TextBox tbxServerUserId;
        private System.Windows.Forms.TextBox tbxServerSid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxServerIp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
