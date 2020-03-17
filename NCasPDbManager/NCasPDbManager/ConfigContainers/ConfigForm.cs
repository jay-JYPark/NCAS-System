using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NCASFND.NCasCtrl;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPDbManager
{
    public partial class ConfigForm : Form
    {
        #region Fields
        /// <summary>
        /// 환경설정 화면을 담는 딕셔너리
        /// </summary>
        private Dictionary<ConfigViewKind, ConfigViewBase> dicConfigViewMng = new Dictionary<ConfigViewKind, ConfigViewBase>();
        /// <summary>
        /// 환경설정 버튼을 관리하는 딕셔너리
        /// </summary>
        private Dictionary<ConfigViewKind, NCasButton> dicConfigButtonMng = new Dictionary<ConfigViewKind, NCasButton>();
        #endregion

        public ConfigForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 저장버튼을 누르면 환경설정을 저장한다.
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                foreach (ConfigViewBase view in dicConfigViewMng.Values)
                {
                    view.SaveConfig();
                }
            }
            catch (Exception err)
            {
                string functionName = "SaveConfig()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 넘어온 ConfigViewKind로 해당하는 Kind의 화면을 띄워준다.
        /// </summary>
        /// <param name="configViewKind"></param>
        private void OpenConfigView(ConfigViewKind configViewKind)
        {
            try
            {
                foreach (ConfigViewBase viewBase in dicConfigViewMng.Values)
                {
                    if (configViewKind == (ConfigViewKind)viewBase.ViewKind)
                    {
                        viewBase.Visible = true;
                    }
                    else
                    {
                        viewBase.Visible = false;
                    }
                }
                foreach (NCasButton btn in dicConfigButtonMng.Values)
                {
                    if (configViewKind == (ConfigViewKind)btn.Tag)
                    {
                        btn.CheckedValue = true;
                    }
                    else
                    {
                        btn.CheckedValue = false;
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "OpenConfigView(ConfigViewKind configViewKind)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 환경설정창을 생성하여 딕셔너리에 저장한다.
        /// 초기에 DBConfig 화면이 보여지도록 한다.
        /// </summary>
        private void LoadCofnigView()
        {
            try
            {
                ConfigViewBase view = new UdpConfigView();
                view.Init();
                view.ViewKind = ConfigViewKind.udpConfig;
                dicConfigViewMng.Add(view.ViewKind, view);
                view.Dock = DockStyle.Fill;
                view.Parent = plViewMain;
                view.Show();

                view = new TcpConfigView();
                view.Init();
                view.ViewKind = ConfigViewKind.tcpConfig;
                dicConfigViewMng.Add(view.ViewKind, view);
                view.Dock = DockStyle.Fill;
                view.Parent = plViewMain;
                view.Show();

                view = new DbConfigView();
                view.Init();
                view.ViewKind = ConfigViewKind.dbConfig;
                dicConfigViewMng.Add(view.ViewKind, view);
                view.Dock = DockStyle.Fill;
                view.Parent = plViewMain;
                view.Show();
            }
            catch (Exception err)
            {
                string functionName = "LoadCofnigView()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                InitMenuControl();
                LoadCofnigView();
                OpenConfigView(ConfigViewKind.dbConfig);
            }
            catch (Exception err)
            {
                string functionName = "OnLoad(EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 메뉴버튼을 정의한다.
        /// </summary>
        private void InitMenuControl()
        {
            try
            {
                btnUdpTab.Tag = ConfigViewKind.udpConfig;
                dicConfigButtonMng.Add(ConfigViewKind.udpConfig, btnUdpTab);

                btnTcpTab.Tag = ConfigViewKind.tcpConfig;
                dicConfigButtonMng.Add(ConfigViewKind.tcpConfig, btnTcpTab);

                btnDbTab.Tag = ConfigViewKind.dbConfig;
                dicConfigButtonMng.Add(ConfigViewKind.dbConfig, btnDbTab);
            }
            catch (Exception err)
            {
                string functionName = "InitMenuControl()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void TabButton_Click(object sender, EventArgs e)
        {
            try
            {
                NCasButton configButton = sender as NCasButton;
                if (configButton == null)
                {
                    return;
                }
                OpenConfigView((ConfigViewKind)configButton.Tag);
            }
            catch (Exception err)
            {
                string functionName = "TabButton_Click(object sender, EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
            if (ConfigMng.SaveConfig())
            {
                MessageBox.Show("저장을 성공하였습니다.\n저장된 설정을 적용하려면 프로그램을 재시작해주세요.", "환경설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
