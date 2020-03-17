using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NCASFND.NCasDb;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPDbManager
{
    public partial class DbConfigView : ConfigViewBase
    {
        public DbConfigView()
        {
            InitializeComponent();
        }

        protected override void InitControl()
        {
            base.InitControl();
            this.tbxServerIp.Text = ConfigMng.LocalDbServerIp;
            this.tbxServerSid.Text = ConfigMng.LocalDbServerSid;
            this.tbxServerUserId.Text = ConfigMng.LocalDbServerUserId;
            this.tbxServerUserPw.Text = ConfigMng.LocalDbServerPw;
        }

        public override void SaveConfig()
        {
            base.SaveConfig();
            ConfigMng.LocalDbServerIp = this.tbxServerIp.Text;
            ConfigMng.LocalDbServerSid = this.tbxServerSid.Text;
            ConfigMng.LocalDbServerUserId = this.tbxServerUserId.Text;
            ConfigMng.LocalDbServerPw = this.tbxServerUserPw.Text;
        }

        private void btnDbConnectTest_Click(object sender, EventArgs e)
        {
            if(tbxServerIp.Text == string.Empty)
            {
                MessageBox.Show("호스트이름 입력하세요.", "DB설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerIp.Focus();
                return;
            }
            if(tbxServerSid.Text == string.Empty)
            {
                MessageBox.Show("SID를 입력하세요.", "DB설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerSid.Focus();
                return;
            }
            if(tbxServerUserId.Text == string.Empty)
            {
                MessageBox.Show("사용자 이름을 입력하세요.", "DB설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerUserId.Focus();
                return;
            }
            if(tbxServerUserPw.Text == string.Empty)
            {
                MessageBox.Show("비밀번호를 입력하세요.", "DB설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerUserPw.Focus();
                return;
            }
            NCasOracleDb oracleDb = null;
            try
            {
                 oracleDb = new NCasOracleDb();
                oracleDb.Open(tbxServerIp.Text, tbxServerSid.Text, tbxServerUserId.Text, tbxServerUserPw.Text);
                if (oracleDb.IsOpen)
                {
                    lbStatus.Text = "연결성공";
                }
                else
                {
                    lbStatus.Text = "연결실패";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "연결실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), "btnDbConnectTest_Click(object sender, EventArgs e)", err);
            }
            finally
            {
                if (oracleDb != null)
                {
                    oracleDb.Close();
                    oracleDb = null;
                }
            }
        }
    }
}
