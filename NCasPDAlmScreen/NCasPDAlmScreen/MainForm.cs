//#define debug
#define release

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

using NCASFND;
using NCASFND.NCasIo;
using NCASFND.NCasNet;
using NCASFND.NCasCtrl;
using NCASFND.NCasLogging;
using NCASBIZ;
using NCASBIZ.NCasEnv;
using NCASBIZ.NCasType;
using NCASBIZ.NCasBiz;
using NCASBIZ.NCasData;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasUtility;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasPlcProtocol;
using NCasAppCommon.Define;
using NCasAppCommon.Type;
using NCasMsgCommon.Tts;
using NCasMsgCommon.Std;
using NCasContentsModule;
using NCasContentsModule.StoMsg;
using NCasContentsModule.TTS;

namespace NCasPDAlmScreen
{
    public partial class MainForm : Form
    {
        #region Dll Import
        [DllImport("user32")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern int PostMessage(int hwnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hwnd, int Msg, int wParam, int lParam);
        #endregion

        #region enum
        public enum ViewKind
        {
            None = 0,
            OrderView19201080 = 1,
            ResultView = 2,
            DevMonView = 3
        }
        #endregion

        #region element
        private readonly string IP_LOOPBACK = "127.0.0.1";
        private readonly int PDMainSessionPort = 19999;
        private readonly string TtsEditorFilePath = "C:\\NCAS\\PROV\\BIN\\NCasTtsEditor.exe";
        private NCasMMFMng mmfMng = null;
        private PDAlmScreenBiz pDAlmScreenBiz = null;
        private Dictionary<ViewKind, ViewBase> dicViews = new Dictionary<ViewKind, ViewBase>();
        private Timer commonTimer = null;
        private List<ViewBase> lstTimerMembers = new List<ViewBase>();
        private NCasNetSessionServerMng pDMainTcpServer = null;
        private NCasUdpSocket udpCasMon = null;
        private NCasUdpSocket recvUdpKey = null;
        private NCasUdpSocket recvUdpLauncher = null;
        private ProvInfo provInfo = null;
        private ViewKind currentViewKind = ViewKind.None;
        private int ttsDelayTime = 5000;
        #endregion

        #region property
        /// <summary>
        /// 시도 데이터파일 프로퍼티
        /// </summary>
        public ProvInfo ProvInfo
        {
            get { return this.provInfo; }
        }

        /// <summary>
        /// NCasMMFMng 프로퍼티
        /// </summary>
        public NCasMMFMng MmfMng
        {
            get { return this.mmfMng; }
        }

        /// <summary>
        /// TTS Delay Time 프로퍼티
        /// </summary>
        public int TtsDelayTime
        {
            get { return this.ttsDelayTime; }
        }
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        #region override method
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                NCasBizActivator.Active(NCASBIZ.NCasDefine.NCasDefineActivatorCode.ForProv);
                this.InitMmfInfo(NCasUtilityMng.INCasEtcUtility.GetIPv4());
                NCasEnvironmentMng.NCasEnvConfig.NetSessionContext.UseConnectionChecking = true;
                NCasEnvironmentMng.NCasEnvConfig.NetSessionContext.UsePolling = true;
                NCasEnvironmentMng.NCasEnvConfig.LoggingContext.UseDebugLogging = true;

                NCasProfile profile = new NCasProfile();
                
#if release
                profile.IpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasEtcUtility.GetIPv4(), 0, 0, 0, 1);
#endif

#if debug
                profile.IpAddr = "158.181.17.226";
#endif

                profile.Port = this.PDMainSessionPort;
                profile.Name = "PDMainScreen";

                this.pDMainTcpServer = new NCasNetSessionServerMng();
                this.pDMainTcpServer.PollingDatas = new byte[] { 0x01 };
                this.pDMainTcpServer.RecvNetSessionClient += new NCasNetSessionRecvEventHandler(pDMainTcpServer_RecvNetSessionClient);
                this.pDMainTcpServer.AddProfile(profile);
                this.pDMainTcpServer.StartSessionServerMng(NCasUtilityMng.INCasEtcUtility.GetIPv4(), this.PDMainSessionPort);

                this.udpCasMon = new NCasUdpSocket();
                this.udpCasMon.Listen(IP_LOOPBACK, (int)NCasPortID.PortIdRecvCasMonData);
                this.udpCasMon.ReceivedData += new NCasUdpRecvEventHandler(udpCasMon_ReceivedData);

                this.recvUdpKey = new NCasUdpSocket();
                this.recvUdpKey.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePdaDevAlmKey);
                this.recvUdpKey.ReceivedData += new NCasUdpRecvEventHandler(recvUdpKey_ReceivedData);

                this.recvUdpLauncher = new NCasUdpSocket();
                this.recvUdpLauncher.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePdaScreenLauncher);
                this.recvUdpLauncher.ReceivedData += new NCasUdpRecvEventHandler(recvUdpLauncher_ReceivedData);

                DeviceStatusMng.LoadDeviceStatusDatas();
                PasswordMng.LoadPassword();
                NCasContentsMng.LoadTtsOptionFromFile();
                TtsDelayTimeMng.LoadTtsDelayTime();
                GroupContentMng.LoadGroupContent();
                DistIconMng.LoadDistIconDatas();
                DisasterBroadFlagMng.LoadDisasterBroadFlag();

                this.ttsDelayTime = int.Parse(TtsDelayTimeMng.TtsDelayTime);
                this.pDAlmScreenBiz = new PDAlmScreenBiz(this);
                this.InitView();
                NCasAnimator.InitAnimator();
                this.OpenView(ViewKind.None);
                this.InitLogoImage(this.provInfo.Code);
                this.StartTimer(1000);
                this.Text = "민방위 시도 지진해일 경보시스템 " + NCasUtilityMng.INCasEtcUtility.GetVersionInfo();
                this.labelTotalTermCount.Text = this.provInfo.GetUsableAlarmTermCnt().ToString();

#if release
                PDevInfo pDevInfo = this.mmfMng.GetPDevInfoByIp(NCasUtilityMng.INCasEtcUtility.GetIPv4());
#endif

#if debug
                PDevInfo pDevInfo = this.mmfMng.GetPDevInfoByIp("10.96.1.231");
#endif

                if (pDevInfo == null)
                {
                    MessageBox.Show("IP가 정상적이지 않습니다.\n네트워크를 확인하세요.", "시도 지진해일 경보시스템", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NCasLoggingMng.ILogging.WriteLog("MainForm", "IP가 정상적이지 않습니다.");
                }

                if (!(pDevInfo.DevId == NCasDefineDeviceKind.JijinAlarmCtrlSys1))
                {
                    MessageBox.Show("IP가 정상적이지 않습니다.\n네트워크를 확인하세요.", "시도 지진해일 경보시스템", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NCasLoggingMng.ILogging.WriteLog("MainForm", "IP가 정상적이지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("MainForm", "MainForm.OnLoad(EventArgs e) Method", ex);
                return;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.StopTimer();

            if (this.pDMainTcpServer != null)
            {
                this.pDMainTcpServer.RecvNetSessionClient -= new NCasNetSessionRecvEventHandler(pDMainTcpServer_RecvNetSessionClient);
                this.pDMainTcpServer.StopSessionServerMng();
            }

            if (this.udpCasMon != null)
            {
                this.udpCasMon.Close();
            }

            if (this.recvUdpKey != null)
            {
                this.recvUdpKey.Close();
            }

            if (this.recvUdpLauncher != null)
            {
                this.recvUdpLauncher.Close();
            }

            if (this.pDAlmScreenBiz != null)
            {
                this.pDAlmScreenBiz.UnInit();
            }

            this.mmfMng.CloseAllMMF();
            NCasAnimator.UninitAnimator();
            NCasBizActivator.Inactive();

            Process[] findProcess = Process.GetProcessesByName("NCasTtsEditor");

            foreach (Process process in findProcess)
            {
                process.Kill();
            }
        }
        #endregion

        #region UI Event
        /// <summary>
        /// cover 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topLeftNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.currentViewKind == ViewKind.None)
                return;

            btnOrderMenu.CheckedValue = false;
            btnOrderResultMenu.CheckedValue = false;
            btnDevMonMenu.CheckedValue = false;
            this.OpenView(ViewKind.None);
            this.currentViewKind = ViewKind.None;
        }

        /// <summary>
        /// Top 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrderMenu_MouseDown(object sender, MouseEventArgs e)
        {
            NCasButton btnSelect = sender as NCasButton;
            ViewKind selectViewKind = (ViewKind)btnSelect.Tag;
            btnOrderMenu.CheckedValue = false;
            btnOrderResultMenu.CheckedValue = false;
            btnDevMonMenu.CheckedValue = false;

            if (selectViewKind == this.currentViewKind)
            {
                if (btnSelect.CheckedValue == false)
                {
                    btnSelect.CheckedValue = true;
                }

                return;
            }

            this.OpenView(selectViewKind);
            this.currentViewKind = selectViewKind;
        }

        /// <summary>
        /// TTS편집 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBroadTextMenu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!System.IO.File.Exists(this.TtsEditorFilePath))
                {
                    MessageBox.Show("TTS편집기가 아래 경로에 없습니다.\n" + this.TtsEditorFilePath + "를 확인하세요.", "TTS편집", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.btnBroadTextMenu.CheckedValue = false;
                    return;
                }

                Process process = new Process();
                process.StartInfo.FileName = this.TtsEditorFilePath;
                process.Start();

                Process[] findProcess = Process.GetProcessesByName("NCasTtsEditor");
                IntPtr windows = IntPtr.Zero;

                foreach (Process eachProcess in findProcess)
                {
                    if (eachProcess.ProcessName == "NCasTtsEditor")
                    {
                        windows = eachProcess.MainWindowHandle;
                        break;
                    }
                }

                if (windows != IntPtr.Zero && (int)windows > 0)
                {
                    Screen scr = Screen.PrimaryScreen;
                    Rectangle rec = scr.Bounds;

                    if (rec.Width < 1002 || rec.Height < 785)
                    {
                        SetWindowPos(windows.ToInt32(), -1, 0, 0, 1002, 785, 0x10);
                    }
                    else
                    {
                        SetWindowPos(windows.ToInt32(), -1, (rec.Width - 1002) / 2, (rec.Height - 785) / 2, 1002, 785, 0x10);
                    }
                }

                process.WaitForExit();
                this.btnBroadTextMenu.CheckedValue = false;
                findProcess = Process.GetProcessesByName("NCasTtsEditor");

                foreach (Process eachProcess in findProcess)
                {
                    eachProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TTS편집기가 정상적이지 않습니다.\nTTS편집기를 확인하세요.", "TTS편집", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NCasLoggingMng.ILogging.WriteLog("MainForm.btnBroadTextMenu_Click", "TTS편집기 실행 - " + ex.Message);
            }
        }
        #endregion

        #region event method
        /// <summary>
        /// 재난운영대에서 수신되는 데이터 리시브 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pDMainTcpServer_RecvNetSessionClient(object sender, NCasNetSessionRecvEventArgs e)
        {
            if (e.Len == 1)
                return;

            byte[] tmpBuff = new byte[e.Len];
            System.Buffer.BlockCopy(e.Buff, 0, tmpBuff, 0, e.Len);
            XmlSerializer serializer = new XmlSerializer(typeof(DistIconDataContainer), new Type[] { typeof(DistIconData) });
            string tmpStr = Encoding.UTF8.GetString(tmpBuff, 0, tmpBuff.Length);
            StringReader sr = new StringReader(tmpStr);
            DistIconDataContainer distIconDataContainer = (DistIconDataContainer)serializer.Deserialize(sr);

            DistIconMng.LstDistIconData = distIconDataContainer.LstDistIconData;
            DistIconMng.SaveDistIconDatas();
            OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];

            MethodInvoker invoker = delegate()
            {
                orderView.SetDistIconReArrange();
            };

            if (this.InvokeRequired)
            {
                Invoke(invoker);
            }
            else
            {
                invoker();
            }
        }

        /// <summary>
        /// commonTimer Tick 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commonTimer_Tick(object sender, EventArgs e)
        {
            this.labelMainTime.Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(DateTime.Now);
            this.SetOrderText();
            this.SetOrderResponseCount();

            foreach (ViewBase viewBase in this.lstTimerMembers)
            {
                viewBase.OnTimer();
            }
        }

        /// <summary>
        /// NCasLauncher에서 수신받는 프로그램 종료 데이터
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void recvUdpLauncher_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
            try
            {
                byte[] tmpBuff = new byte[e.Len];
                System.Buffer.BlockCopy(e.Buff, 0, tmpBuff, 0, e.Len);
                NCasLauncherCmdData launcherCmdData = new NCasLauncherCmdData(tmpBuff);

                if (launcherCmdData.Command == NCasLauncherCmd.CloseProcess)
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm", "NCasLauncher에서 수신받는 Method Exception - " + ex.Message);
            }
        }

        /// <summary>
        /// 지진해일 조작반 UDP 데이터 수신 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void recvUdpKey_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
            byte[] tmpBuff = new byte[e.Len];
            System.Buffer.BlockCopy(e.Buff, 0, tmpBuff, 0, e.Len);

            if (tmpBuff[5] != 13)
                return;

            NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(tmpBuff);
            NCasProtocolTc148Sub13 protoSub13 = protoBase as NCasProtocolTc148Sub13;
            OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];

            MethodInvoker invoker = delegate()
            {
                orderView.SetKeyPlc(protoSub13);
            };

            if (this.InvokeRequired)
            {
                this.Invoke(invoker);
            }
            else
            {
                invoker();
            }
        }

        /// <summary>
        /// CasMon UDP 데이터 수신 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void udpCasMon_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
        }
        #endregion

        #region public method
        /// <summary>
        /// 그룹정보 리스트 클리어 메소드
        /// </summary>
        public void SetGroupListClear()
        {
            OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];
            orderView.GroupNameLst.Clear();
        }

        /// <summary>
        /// 조작반 데이터를 pDAlmScreenBiz로 넘겨주는 메소드
        /// </summary>
        /// <param name="plcProtocol"></param>
        public void SetPlcKeyData(NCasProtocolBase plcProtocol)
        {
            this.pDAlmScreenBiz.AddBizData(plcProtocol);
        }

        /// <summary>
        /// 발령 데이터 처리 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        public void SetOrderBizData(OrderBizData orderBizData)
        {
            this.pDAlmScreenBiz.AddBizData(orderBizData);
        }

        /// <summary>
        /// Timer 멤버로 등록하는 메소드
        /// </summary>
        /// <param name="viewBase"></param>
        public void AddTimerMember(ViewBase viewBase)
        {
            if (!this.lstTimerMembers.Contains(viewBase))
            {
                this.lstTimerMembers.Add(viewBase);
            }
        }

        /// <summary>
        /// Timer 멤버에서 제거하는 메소드
        /// </summary>
        /// <param name="viewBase"></param>
        public void RemoveTimerMember(ViewBase viewBase)
        {
            if (this.lstTimerMembers.Contains(viewBase))
            {
                this.lstTimerMembers.Remove(viewBase);
            }
        }
        #endregion

        #region private method
        /// <summary>
        /// 경보발령에 대한 응답 카운트를 화면에 표시한다.
        /// </summary>
        private void SetOrderResponseCount()
        {
            this.labelResponseTermCount.Text = this.provInfo.AlarmRespCnt.ToString();
            this.labelErrorTermCount.Text = this.provInfo.FaultAlmResponseCnt.ToString();
        }

        /// <summary>
        /// 경보발령에 대한 정보를 화면 상단에 표시한다. (발령 현황)
        /// </summary>
        private void SetOrderText()
        {
            this.orderTextBoard.ClearTextBlock();
            this.orderTextBoard.FontSize = 24;

            if (this.provInfo.AlarmOrderInfo.Kind != NCasDefineOrderKind.None)
            {
                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem3(this.provInfo.AlarmOrderInfo.OccurTimeToDateTime) +
                    " [" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(this.provInfo.AlarmOrderInfo.Source) + "]               [" +
                    NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(this.provInfo.AlarmOrderInfo.Media) + "]", Color.FromArgb(255, 255, 255)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    " [" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(this.provInfo.AlarmOrderInfo.Mode) + "]",
                    (this.provInfo.AlarmOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? Color.FromArgb(232, 82, 53) : Color.FromArgb(6, 147, 6)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    " [" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(this.provInfo.AlarmOrderInfo.Kind) + "]", Color.FromArgb(255, 255, 255)));
            }
        }

        /// <summary>
        /// 타이머 초기화 및 실행 메소드
        /// </summary>
        /// <param name="interval"></param>
        private void StartTimer(int interval)
        {
            this.commonTimer = new Timer();
            this.commonTimer.Interval = interval;
            this.commonTimer.Tick += new EventHandler(commonTimer_Tick);
            this.commonTimer.Start();
        }

        /// <summary>
        /// 타이머 종료 메소드
        /// </summary>
        private void StopTimer()
        {
            if (this.commonTimer != null)
            {
                this.commonTimer.Tick -= new EventHandler(commonTimer_Tick);
                this.commonTimer.Stop();
            }
        }

        /// <summary>
        /// 각 ViewKind와 툴바를 매핑하는 작업
        /// </summary>
        private void InitView()
        {
            //메인 화면
            ViewBase coverViewBase = CoverView.CreateView(ViewKind.None, this);
            coverViewBase.ViewKind = ViewKind.None;
            coverViewBase.Dock = System.Windows.Forms.DockStyle.Fill;
            coverViewBase.Location = new System.Drawing.Point(0, 0);
            coverViewBase.Name = "coverView";
            coverViewBase.Size = new System.Drawing.Size(1904, 937);
            this.middlePanel.Controls.Add(coverViewBase);
            this.dicViews.Add(ViewKind.None, coverViewBase);

            //경보발령 화면
            ViewBase orderViewBase = OrderView19201080.CreateView(ViewKind.OrderView19201080, this);
            orderViewBase.Interval = 1000;
            orderViewBase.ViewKind = ViewKind.OrderView19201080;
            orderViewBase.Dock = System.Windows.Forms.DockStyle.Fill;
            orderViewBase.Location = new System.Drawing.Point(0, 0);
            orderViewBase.Name = "orderView";
            orderViewBase.Size = new System.Drawing.Size(1904, 937);
            this.middlePanel.Controls.Add(orderViewBase);
            this.dicViews.Add(ViewKind.OrderView19201080, orderViewBase);

            //경보결과 화면
            ViewBase resultViewBase = OrderResultView.CreateView(ViewKind.ResultView, this);
            resultViewBase.Interval = 1000;
            resultViewBase.ViewKind = ViewKind.ResultView;
            resultViewBase.Dock = System.Windows.Forms.DockStyle.Fill;
            resultViewBase.Location = new System.Drawing.Point(0, 0);
            resultViewBase.Name = "resultView";
            resultViewBase.Size = new System.Drawing.Size(1904, 937);
            this.middlePanel.Controls.Add(resultViewBase);
            this.dicViews.Add(ViewKind.ResultView, resultViewBase);

            //장비감시 화면
            ViewBase deviceMonViewBase = DeviceMonitorView.CreateView(ViewKind.DevMonView, this);
            deviceMonViewBase.Interval = 1000;
            deviceMonViewBase.ViewKind = ViewKind.DevMonView;
            deviceMonViewBase.Dock = System.Windows.Forms.DockStyle.Fill;
            deviceMonViewBase.Location = new System.Drawing.Point(0, 0);
            deviceMonViewBase.Name = "deviceMonitorView";
            deviceMonViewBase.Size = new System.Drawing.Size(1904, 937);
            this.middlePanel.Controls.Add(deviceMonViewBase);
            this.dicViews.Add(ViewKind.DevMonView, deviceMonViewBase);

            this.btnOrderMenu.Tag = ViewKind.OrderView19201080;
            this.btnOrderResultMenu.Tag = ViewKind.ResultView;
            this.btnDevMonMenu.Tag = ViewKind.DevMonView;
        }

        /// <summary>
        /// 시도 로고이미지를 화면에 셋팅한다.
        /// </summary>
        /// <param name="proveCode">셋팅할 시도의 Code</param>
        private void InitLogoImage(int proveCode)
        {
            Image image = null;

            switch (proveCode)
            {
                case 1670:
                    image = NCasPDAlmScreenRsc._1670;
                    break;

                case 1671:
                    image = NCasPDAlmScreenRsc._1671;
                    break;

                case 1672:
                    image = NCasPDAlmScreenRsc._1672;
                    break;

                case 1673:
                    image = NCasPDAlmScreenRsc._1673;
                    break;

                case 1674:
                    image = NCasPDAlmScreenRsc._1674;
                    break;

                case 1675:
                    image = NCasPDAlmScreenRsc._1675;
                    break;

                case 1676:
                    image = NCasPDAlmScreenRsc._1676;
                    break;

                case 1677:
                    image = NCasPDAlmScreenRsc._1677;
                    break;

                case 1678:
                    image = NCasPDAlmScreenRsc._1678;
                    break;

                case 1679:
                    image = NCasPDAlmScreenRsc._1679;
                    break;

                case 1680:
                    image = NCasPDAlmScreenRsc._1680;
                    break;

                case 1681:
                    image = NCasPDAlmScreenRsc._1681;
                    break;

                case 1682:
                    image = NCasPDAlmScreenRsc._1682;
                    break;

                case 1683:
                    image = NCasPDAlmScreenRsc._1683;
                    break;

                case 1684:
                    image = NCasPDAlmScreenRsc._1684;
                    break;

                case 1685:
                    image = NCasPDAlmScreenRsc._1685;
                    break;

                case 2481:
                    image = NCasPDAlmScreenRsc._1686;
                    break;

                default:
                    image = NCasPDAlmScreenRsc._1670;
                    break;
            }

            this.topRightLogoPictureBox.BackgroundImage = image;
        }

        /// <summary>
        /// 선택된 ViewKind를 화면에 표시
        /// </summary>
        /// <param name="viewKind"></param>
        private void OpenView(ViewKind viewKind)
        {
            switch (viewKind)
            {
                case ViewKind.None:
                    this.dicViews[ViewKind.None].BringToFront();
                    break;

                case ViewKind.OrderView19201080:
                    this.dicViews[ViewKind.OrderView19201080].BringToFront();
                    this.btnOrderMenu.CheckedValue = true;
                    break;

                case ViewKind.ResultView:
                    this.dicViews[ViewKind.ResultView].BringToFront();
                    this.btnOrderResultMenu.CheckedValue = true;
                    break;

                case ViewKind.DevMonView:
                    this.dicViews[ViewKind.DevMonView].BringToFront();
                    this.btnDevMonMenu.CheckedValue = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// ViewKind 화면을 숨긴다
        /// </summary>
        /// <param name="viewKind"></param>
        private void CloseView(ViewKind viewKind)
        {
            switch (viewKind)
            {
                case ViewKind.None:
                    this.dicViews[ViewKind.None].SendToBack();
                    break;

                case ViewKind.OrderView19201080:
                    this.dicViews[ViewKind.OrderView19201080].SendToBack();
                    this.btnOrderMenu.CheckedValue = false;
                    break;

                case ViewKind.ResultView:
                    this.dicViews[ViewKind.ResultView].SendToBack();
                    this.btnOrderResultMenu.CheckedValue = false;
                    break;

                case ViewKind.DevMonView:
                    this.dicViews[ViewKind.DevMonView].SendToBack();
                    this.btnDevMonMenu.CheckedValue = false;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 모든 ViewKind 화면을 숨긴다
        /// </summary>
        private void HideAllViewForm()
        {
            foreach (KeyValuePair<ViewKind, ViewBase> eachDic in this.dicViews)
            {
                eachDic.Value.SendToBack();
            }

            this.btnOrderMenu.CheckedValue = false;
            this.btnOrderResultMenu.CheckedValue = false;
            this.btnDevMonMenu.CheckedValue = false;
        }

        /// <summary>
        /// 서버 로컬IP를 받아 해당하는 MMF파일을 로드한다.
        /// </summary>
        /// <param name="localIpAddr">서버 로컬IP</param>
        private void InitMmfInfo(string localIpAddr)
        {
            this.mmfMng = new NCasMMFMng();
            this.mmfMng.LoadAllMMF();

#if release
            this.provInfo = this.mmfMng.GetProvInfoByNetId(localIpAddr);
#endif

#if debug
            this.provInfo = this.mmfMng.GetProvInfoByNetId("10.112.1.231");
#endif

            if (this.provInfo == null)
            {
                MessageBox.Show("데이터파일을 정상적으로 로드하지 못했습니다.", "데이터파일 로드", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("MainForm.InitMmfInfo(string localIpAddr) Method Error!");
            }
        }
        #endregion
    }
}