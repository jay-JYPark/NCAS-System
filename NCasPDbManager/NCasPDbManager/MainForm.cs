using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NCASBIZ.NCasType;
using NCASBIZ;
using NCASFND;
using NCASFND.NCasCtrl;
using NCASFND.NCasDb;
using NCASBIZ.NCasDefine;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasUtility;

namespace NCasPDbManager
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// DBManager 비지니스 로직
        /// </summary>
        private DbManagerBiz dbManagerBiz = null;
        /// <summary>
        /// 타이머
        /// </summary>
        private Timer mainTimer = null;
        private Font listViewFont = null;
        private bool closeOk = false;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                if (NCasUtilityMng.INCasEtcUtility.CheckAppOverlapping())
                {
                    MessageBox.Show(" 프로그램이 이미 실행되어 있습니다.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    closeOk = true;
                    this.Close();
                }
                else
                {
                    NCasAnimator.InitAnimator();
                }
            }
            catch (Exception err)
            {
                string functionName = "MainForm()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 타이머를 멈춘다.
        /// </summary>
        private void TimerStop()
        {
            try
            {
                if (mainTimer != null)
                {
                    mainTimer.Tick -= new EventHandler(mainTimer_Tick);
                    mainTimer.Stop();
                }
            }
            catch (Exception err)
            {
                string functionName = "TimerStop()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        void mainTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                SetTime();
                BasicDataMng.ProcTimer();
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "mainTimer_Tick(object sender, EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 타이머를 시작한다.
        /// </summary>
        private void TimerStart()
        {
            try
            {
                mainTimer = new Timer();
                mainTimer.Interval = 1000;
                mainTimer.Tick += new EventHandler(mainTimer_Tick);
                mainTimer.Start();
            }
            catch (Exception err)
            {
                string functionName = "TimerStart()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 프로그램 시간을 설정한다.
        /// </summary>
        private void SetTime()
        {
            try
            {
                string programDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lbDate.Text = programDate;
            }
            catch (Exception err)
            {
                string functionName = "SetTime()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 처리해야하는 카운트를 화면에 표시한다.
        /// </summary>
        private void SetProcessingStandbyCount()
        {
            try
            {
                int processingCount = dbManagerBiz.GetProcessingCount();
                int standbyCount = dbManagerBiz.GetStandbyCount();

                if (processingCount == 1000000)
                {
                    dbManagerBiz.ResetProcessingCount();
                }

                lbProcessingCount.Text = processingCount.ToString();
                lbStandbyCount.Text = standbyCount.ToString();
            }
            catch (Exception err)
            {
                string functionName = "SetProcessingStandbyCount()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 데이터베이스에 연결이 가능한지 확인한다.
        /// </summary>
        private bool CheckDataBase()
        {
            bool isContinue = false;
            try
            {
                NCasOracleDb oracleDb = null;
                ConfigForm configForm = null;

                while (true)
                {
                    try
                    {
                        oracleDb = BasicDataMng.OpenDataBase();
                        if (oracleDb != null && oracleDb.IsOpen)
                        {
                            isContinue = true;
                            break;
                        }
                        configForm = new ConfigForm();
                        if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        {
                            isContinue = false;
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        string functionName = "CheckDataBase()-While";
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                        NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "CheckDataBase()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return isContinue;
        }

        /// <summary>
        /// profile과 연결상태를 입력받아 화면에 표시한다.
        /// </summary>
        /// <param name="nCasProfile"></param>
        /// <param name="isConnect"></param>
        public void SetProfileStatus(NCasProfile nCasProfile, bool isConnect)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    InvokeVoidTwo<NCasProfile, bool> invoke = new InvokeVoidTwo<NCasProfile, bool>(SetProfileStatus);
                    this.Invoke(invoke, nCasProfile, isConnect);
                }
                else
                {
                    for (int i = 0; i < lvClientAdmin.Items.Count; i++)
                    {
                        if (lvClientAdmin.Items[i].Name == nCasProfile.Name)
                        {
                            if (isConnect)
                            {
                                lvClientAdmin.Items[i].ImageIndex = (int)NCasDefineNormalStatus.Noraml;
                            }
                            else
                            {
                                lvClientAdmin.Items[i].ImageIndex = (int)NCasDefineNormalStatus.Abnormal;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "SetProfileStatus(NCasProfile nCasProfile, bool isConnect)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                NCasBizActivator.Active(NCASBIZ.NCasDefine.NCasDefineActivatorCode.ForProv);
                NCASBIZ.NCasEnv.NCasEnvironmentMng.NCasEnvConfig.LoggingContext.UseDebugLogging = true;

                ConfigMng.Init();
                if (!CheckDataBase())
                {
                    this.Close();
                }
                SetVersion();
                //SetLogo();
                InitListViewControl();
                BasicDataMng.Init();
                AddEventListText(string.Format("장비로드 카운트 : {0}", BasicDataMng.DicMasterInfoData.Count.ToString()), NCasDefineNormalStatus.Noraml);
                dbManagerBiz = new DbManagerBiz(this);
                dbManagerBiz.Init();
                TimerStart();
            }
            catch (Exception err)
            {
                string functionName = "OnLoad(EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} - {2} 오류발생 : {3}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), NCasProcName.NCasPDBManager.ToString(), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void SetLogo()
        {
            string[] tmpIp = ConfigMng.LocalIp.Split('.');
            string bClass = string.Empty;
            if (tmpIp.Length > 3)
            {
                bClass = tmpIp[1];
            }
            switch(bClass)
            {
                case "136":
                    break;
                case "96":
                    break;
            }
        }

        private void SetVersion()
        {
            try
            {
                string programName = string.Empty;
                string programVersion = string.Empty;
                programName = NCasAppCommon.Utils.NCasAppUtil.GetProcessStringName(NCasProcName.NCasPDBManager);
                programVersion = string.Format("{0} {1}", "Ver", Application.ProductVersion.ToString());
                this.Text = string.Format("{0} {1}", programName, programVersion);
            }
            catch (Exception err)
            {
                string functionName = "SetVersion()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 리스트뷰를 초기화한다.
        /// </summary>
        private void InitListViewControl()
        {
            try
            {
                #region Base Set
                this.imgList.ColorDepth = ColorDepth.Depth32Bit;
                this.imgList.ImageSize = new Size(27, 22);
                this.imgList.Images.Add(NCasPDbManagerRsc.listIconOk);
                this.imgList.Images.Add(NCasPDbManagerRsc.listIconError);
                this.listViewFont = new Font(NCasPDbManagerRsc.FontName, 12.0f, FontStyle.Bold);

                this.lvClientAdmin.StateImageList.ImageSize = this.imgList.ImageSize;
                this.lvClientAdmin.StateImageList.Images.Add(this.imgList.Images[(int)NCasDefineNormalStatus.Noraml]);
                this.lvClientAdmin.StateImageList.Images.Add(this.imgList.Images[(int)NCasDefineNormalStatus.Abnormal]);

                this.lvClientAdmin.GridLineStyle = NCasListViewGridLine.GridBoth;
                this.lvClientAdmin.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                this.lvClientAdmin.Font = new Font(NCasPDbManagerRsc.FontName, 11.0f);
                this.lvClientAdmin.ColumnHeight = 32;
                this.lvClientAdmin.ItemHeight = 28;

                this.lvEvent.StateImageList.ImageSize = this.imgList.ImageSize;
                this.lvEvent.StateImageList.Images.Add(this.imgList.Images[(int)NCasDefineNormalStatus.Noraml]);
                this.lvEvent.StateImageList.Images.Add(this.imgList.Images[(int)NCasDefineNormalStatus.Abnormal]);

                this.lvEvent.GridLineStyle = NCasListViewGridLine.GridBoth;
                this.lvEvent.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                this.lvEvent.Font = new Font(NCasPDbManagerRsc.FontName, 11.0f);
                this.lvEvent.ColumnHeight = 32;
                this.lvEvent.ItemHeight = 28;
                #endregion

                #region lvClientAdmin ColumnHeather Set
                NCasColumnHeader column = this.lvClientAdmin.Columns.Add("...", 32);
                column.SortType = NCasListViewSortType.SortIcon;
                column.TextAlign = HorizontalAlignment.Center;
                column.ColumnLock = true;

                column = this.lvClientAdmin.Columns.Add("번호", 50);
                column.SortType = NCasListViewSortType.SortInt32;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvClientAdmin.Columns.Add("클라이언트 이름", 200);
                column.SortType = NCasListViewSortType.SortText;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvClientAdmin.Columns.Add("IP", 150);
                column.SortType = NCasListViewSortType.SortText;
                column.TextAlign = HorizontalAlignment.Center;
                #endregion

                #region lvEvent ColumnHeather Set
                column = this.lvEvent.Columns.Add("...", 32);
                column.SortType = NCasListViewSortType.SortIcon;
                column.TextAlign = HorizontalAlignment.Center;
                column.ColumnLock = true;

                column = this.lvEvent.Columns.Add("번호", 50);
                column.SortType = NCasListViewSortType.SortInt32;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvEvent.Columns.Add("시각", 170);
                column.SortType = NCasListViewSortType.SortDateTime;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvEvent.Columns.Add("이벤트 내용", 700);
                column.SortType = NCasListViewSortType.SortText;
                column.TextAlign = HorizontalAlignment.Left;
                #endregion
            }
            catch (Exception err)
            {
                string functionName = "InitListViewControl()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// TCPProfile을 리스트뷰에 등록한다.
        /// </summary>
        public void RegisterTcpProfile(NCasProfile profile)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    InvokeVoidOne<NCasProfile> invoke = new InvokeVoidOne<NCasProfile>(RegisterTcpProfile);
                    this.Invoke(invoke, profile);
                }
                else
                {
                    if (profile == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), "RegisterTcpProfile(NCasProfile profile) : 넘어온 profile이 null값 이므로 return함");
                        return;
                    }

                    NCasListViewItem item = null;
                    NCasListViewItem.NCasListViewSubItem subItem = null;

                    item = new NCasListViewItem();
                    item.ImageIndex = (int)NCasDefineNormalStatus.Abnormal;
                    item.Name = profile.Name;
                    item.Tag = profile;

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (lvClientAdmin.Items.Count + 1).ToString();
                    subItem.TextAlign = HorizontalAlignment.Center;
                    item.SubItems.Add(subItem);

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (profile.Name).ToString();
                    subItem.TextAlign = HorizontalAlignment.Left;
                    item.SubItems.Add(subItem);

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (profile.IpAddr).ToString();
                    subItem.TextAlign = HorizontalAlignment.Left;
                    item.SubItems.Add(subItem);

                    lvClientAdmin.Items.Add(item);
                }
            }
            catch (Exception err)
            {
                string functionName = "RegisterTcpProfile(NCasProfile profile)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 종료 이벤트
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeOk)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                try
                {
                    TimerStop();
                    NCasBizActivator.Inactive();
                    if (dbManagerBiz != null)
                    {
                        dbManagerBiz.UnInit();
                    }
                }
                catch (Exception err)
                {
                    string functionName = "OnClosing(CancelEventArgs e)";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                    NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
                }
            }
            base.OnClosing(e);
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm();
            configForm.StartPosition = FormStartPosition.CenterScreen;
            configForm.Show(this);
        }

        private void lbProcessingCount_Click(object sender, EventArgs e)
        {
            try
            {
                dbManagerBiz.ResetProcessingCount();
            }
            catch (Exception err)
            {
                string functionName = "lbProcessingCount_Click(object sender, EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        public void AddEventListText(string eventText, NCasDefineNormalStatus nomalStatus)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    InvokeVoidTwo<string, NCasDefineNormalStatus> invoke = new InvokeVoidTwo<string, NCasDefineNormalStatus>(AddEventListText);
                    this.Invoke(invoke, eventText, nomalStatus);
                }
                else
                {
                    NCasListViewItem item = null;
                    NCasListViewItem.NCasListViewSubItem subItem = null;

                    item = new NCasListViewItem();
                    item.ImageIndex = (int)nomalStatus;

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (lvEvent.Items.Count + 1).ToString();
                    subItem.TextAlign = HorizontalAlignment.Center;
                    item.SubItems.Add(subItem);

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    subItem.TextAlign = HorizontalAlignment.Center;
                    item.SubItems.Add(subItem);

                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = eventText;
                    subItem.TextAlign = HorizontalAlignment.Left;
                    item.SubItems.Add(subItem);

                    lvEvent.Items.Add(item);
                }
            }
            catch (Exception err)
            {
                string functionName = "AddEventListText(string eventText, NCasDefineNormalStatus nomalStatus)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void btnResetTcRecvCount_Click(object sender, EventArgs e)
        {
            dbManagerBiz.TestRestRecvCount();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            closeOk = true;
        }
    }
}
