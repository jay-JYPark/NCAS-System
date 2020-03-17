using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPDbManager
{
    public partial class UdpConfigView : ConfigViewBase
    {
        public UdpConfigView()
        {
            InitializeComponent();
        }

        protected override void InitControl()
        {
            base.InitControl();
            this.tbxUdpIp.Text = ConfigMng.UdpIp;
            this.tbxUdpPort.Text = ConfigMng.UdpPort.ToString();
        }

        public override void SaveConfig()
        {
            base.SaveConfig();
            try
            {
                ConfigMng.UdpIp = this.tbxUdpIp.Text;
                int port;
                if (int.TryParse(this.tbxUdpPort.Text, out port))
                {
                    ConfigMng.UdpPort = port;
                }
            }
            catch (Exception err)
            {
                string functionName = "SaveConfig()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }
    }
}
