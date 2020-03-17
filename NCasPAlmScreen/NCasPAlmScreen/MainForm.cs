//#define debug
#define release

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Sockets;

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

namespace NCasPAlmScreen
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
        private readonly string TtsEditorFilePath = "C:\\NCAS\\PROV\\BIN\\NCasTtsEditor.exe";
        private NCasMMFMng mmfMng = null;
        private PAlmScreenBiz pAlmScreenBiz = null;
        private Dictionary<ViewKind, ViewBase> dicViews = new Dictionary<ViewKind, ViewBase>();
        private Timer commonTimer = null;
        private List<ViewBase> lstTimerMembers = new List<ViewBase>();
        private NCasUdpSocket udpCasMon = null;
        private NCasUdpSocket recvUdpKey = null;
        private NCasUdpSocket recvUdpGis = null;
        private NCasUdpSocket recvUdpLauncher = null;
        private NCasUdpSocket recvCenterAlarm = null;
        private CentInfo centInfo = null;
        private CentInfo centInfo2 = null;
        private ProvInfo provInfo = null;
        private ViewKind currentViewKind = ViewKind.None;
        private List<Control> lstDeviceStatusLabel = new List<Control>();
        private NCasNetSessionMngBase dualSessionMng = null;
        private NCasProfile profile = null;
        private int ttsDelayTime = 5000;
        private int teleDelayTime = 2000;
        #endregion

        #region property
        /// <summary>
        /// 중앙 데이터파일 프로퍼티
        /// </summary>
        public CentInfo CentInfo
        {
            get { return this.centInfo; }
        }

        /// <summary>
        /// 2중앙 데이터파일 프로퍼티
        /// </summary>
        public CentInfo CentInfo2
        {
            get { return this.centInfo2; }
        }

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

        /// <summary>
        /// 일제지령 Delay Time 프로퍼티
        /// </summary>
        public int TeleDelayTime
        {
            get { return this.teleDelayTime; }
        }
        #endregion

        #region override Method
        /// <summary>
        /// OnLoad override method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                NCasBizActivator.Active(NCASBIZ.NCasDefine.NCasDefineActivatorCode.ForProv);
                this.InitMmfInfo(NCasUtilityMng.INCasEtcUtility.GetIPv4());
                NCasEnvironmentMng.NCasEnvConfig.NetSessionContext.UseConnectionChecking = true;
                NCasEnvironmentMng.NCasEnvConfig.NetSessionContext.UsePolling = true;
                this.InitDualSession(NCasUtilityMng.INCasEtcUtility.GetIPv4());

                this.udpCasMon = new NCasUdpSocket();
                this.udpCasMon.Listen(IP_LOOPBACK, (int)NCasPortID.PortIdRecvCasMonData);
                this.udpCasMon.ReceivedData += new NCasUdpRecvEventHandler(udpCasMon_ReceivedData);

                this.recvUdpKey = new NCasUdpSocket();
                this.recvUdpKey.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePcaScreenPlc);
                this.recvUdpKey.ReceivedData += new NCasUdpRecvEventHandler(recvUdpKey_ReceivedData);

                this.recvUdpGis = new NCasUdpSocket();
                this.recvUdpGis.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePcaScreen);
                this.recvUdpGis.ReceivedData += new NCasUdpRecvEventHandler(recvUdpGis_ReceivedData);

                this.recvUdpLauncher = new NCasUdpSocket();
                this.recvUdpLauncher.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePcaScreenLauncher);
                this.recvUdpLauncher.ReceivedData += new NCasUdpRecvEventHandler(recvUdpLauncher_ReceivedData);

                this.recvCenterAlarm = new NCasUdpSocket();
                this.recvCenterAlarm.Listen(this.IP_LOOPBACK, (int)NCasPipes.PipePcaScreenAlm);
                this.recvCenterAlarm.ReceivedData += new NCasUdpRecvEventHandler(recvCenterAlarm_ReceivedData);

                KeyDataMng.LoadKeyDatas();
                DeviceStatusMng.LoadDeviceStatusDatas();
                PasswordMng.LoadPassword();
                GroupContentMng.LoadGroupContent();
                NCasContentsMng.LoadTtsOptionFromFile();
                TtsDelayTimeMng.LoadTtsDelayTime();
                TeleSendDelayTimeMng.LoadTeleDelayTime();

                this.ttsDelayTime = int.Parse(TtsDelayTimeMng.TtsDelayTime);
                this.teleDelayTime = int.Parse(TeleSendDelayTimeMng.TeleDelayTime);
                this.pAlmScreenBiz = new PAlmScreenBiz(this);
                this.InitDeviceStatus();
                this.InitView();
                NCasAnimator.InitAnimator();
                this.OpenView(ViewKind.None);
                this.InitLogoImage(this.provInfo.Code);
                this.StartTimer(1000);
                this.Text = "민방위 시도 경보대시스템 " + NCasUtilityMng.INCasEtcUtility.GetVersionInfo();
                PDevInfo pDevInfo = this.mmfMng.GetPDevInfoByIp(NCasUtilityMng.INCasEtcUtility.GetIPv4());

                if (pDevInfo == null)
                {
                    MessageBox.Show("IP가 정상적이지 않습니다.\n네트워크를 확인하세요.", "시도 경보대시스템", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NCasLoggingMng.ILogging.WriteLog("MainForm", "IP가 정상적이지 않습니다.");
                    return;
                }

                if (!(pDevInfo.DevId == NCasDefineDeviceKind.AlarmCtrlSys1 || pDevInfo.DevId == NCasDefineDeviceKind.AlarmCtrlSys2))
                {
                    MessageBox.Show("IP가 정상적이지 않습니다.\n네트워크를 확인하세요.", "시도 경보대시스템", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NCasLoggingMng.ILogging.WriteLog("MainForm", "IP가 정상적이지 않습니다.");
                }

                NCasEnvironmentMng.NCasEnvConfig.LoggingContext.UseDebugLogging = true;
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("MainForm", "MainForm.OnLoad(EventArgs e) Method", ex);
                return;
            }
        }

        /// <summary>
        /// OnClosing override method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.StopTimer();
            this.UninitDualSession();

            if (this.udpCasMon != null)
            {
                this.udpCasMon.Close();
            }

            if (this.recvUdpKey != null)
            {
                this.recvUdpKey.Close();
            }

            if (this.recvUdpGis != null)
            {
                this.recvUdpGis.Close();
            }

            if (this.recvUdpLauncher != null)
            {
                this.recvUdpLauncher.Close();
            }

            if (this.pAlmScreenBiz != null)
            {
                this.pAlmScreenBiz.UnInit();
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

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            if (NCasUtilityMng.INCasEtcUtility.CheckAppOverlapping())
            {
                MessageBox.Show("프로그램이 이미 실행되어 있습니다.", "시도 경보대시스템", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Dispose();
                Application.Exit();
                return;
            }
        }
        #endregion

        #region event method
        /// <summary>
        /// 중앙, 2중앙 발령 승계를 위한 데이터 수신
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvCenterAlarm_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
            try
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(e.Buff);

                if (protoBase.TcCode == NCasDefineTcCode.TcAlarmOrder)
                {
                    NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;

                    if (protoTc1.Source == NCasDefineOrderSource.CentCtrlRoom
                        || protoTc1.Source == NCasDefineOrderSource.CentCtrlRoom2
                        || protoTc1.Source == NCasDefineOrderSource.CentCtrlRoom3)
                    {
                        if (protoTc1.AlarmKind == NCasDefineOrderKind.AlarmStandby)
                        {
                            OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];

                            MethodInvoker invoker = delegate()
                            {
                                orderView.SetCenterAlarmOrder(true);
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
                        else if (protoTc1.AlarmKind == NCasDefineOrderKind.AlarmCancel)
                        {
                            OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];

                            MethodInvoker invoker = delegate()
                            {
                                orderView.SetCenterAlarmOrder(false);
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
                    }
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm", "중앙, 2중앙 발령 승계를 위한 데이터 수신 Method Exception - " + ex.Message);
            }
        }

        /// <summary>
        /// NCasLauncher에서 수신받는 프로그램 종료 데이터
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvUdpLauncher_ReceivedData(object sender, NCasUdpRecvEventArgs e)
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
        /// GIS서버에서 수신받는 발령 데이터(TC 1)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvUdpGis_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
            try
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(e.Buff);
                NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;
                this.mmfMng.WriteOrder(protoTc1);
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm", "GIS서버에서 수신받는 Method Exception - " + ex.Message);
            }
        }

        /// <summary>
        /// 조작반 UDP 데이터 수신 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvUdpKey_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
            byte[] tmpBuff = new byte[e.Len];
            System.Buffer.BlockCopy(e.Buff, 0, tmpBuff, 0, e.Len);
            NCasPlcProtocolBase protoBase = NCasPlcProtocolFactory.ParseFrame(tmpBuff);

            if (protoBase.Command == NCasDefinePlcCommand.ReqStatusResponse)
            {
                NCasPlcProtocolReqStatusResponse protoStsResp = protoBase as NCasPlcProtocolReqStatusResponse;
                OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];

                MethodInvoker invoker = delegate()
                {
                    orderView.SetKeyPlc(protoStsResp);
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
        }

        /// <summary>
        /// CasMon UDP 데이터 수신 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void udpCasMon_ReceivedData(object sender, NCasUdpRecvEventArgs e)
        {
        }

        /// <summary>
        /// commonTimer Tick 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commonTimer_Tick(object sender, EventArgs e)
        {
            this.labelMainTime.Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(DateTime.Now);

            foreach (ViewBase viewBase in this.lstTimerMembers)
            {
                viewBase.OnTimer();
            }

            this.UpdateDeviceStatus();
        }

        /// <summary>
        /// cover 메뉴 다운 이벤트
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
        /// Top Menu 다운 이벤트
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
        /// 재발령 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reOrderMenu_Click(object sender, EventArgs e)
        {
            this.reOrderMenu.CheckedValue = true;

            using (ReOrderViewForm reOrderViewForm = new ReOrderViewForm(this))
            {
                reOrderViewForm.ShowDialog();
            }

            this.reOrderMenu.CheckedValue = false;
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

                Process[] findKillProcess = Process.GetProcessesByName("NCasTtsEditor");
                findKillProcess = Process.GetProcessesByName("NCasTtsEditor");

                foreach (Process eachProcess in findKillProcess)
                {
                    eachProcess.Kill();
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

                /*
                process.WaitForExit();
                this.btnBroadTextMenu.CheckedValue = false;
                findProcess = Process.GetProcessesByName("NCasTtsEditor");

                foreach (Process eachProcess in findProcess)
                {
                    eachProcess.Kill();
                }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show("TTS편집기가 정상적이지 않습니다.\nTTS편집기를 확인하세요.", "TTS편집", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NCasLoggingMng.ILogging.WriteLog("MainForm.btnBroadTextMenu_Click", "TTS편집기 실행 - " + ex.Message);
            }
        }
        #endregion

        #region public method
        /// <summary>
        /// 듀얼시스템으로부터 수신받은 키 데이터를 화면에 반영한다.
        /// </summary>
        /// <param name="keyData"></param>
        public void SetKeyDataFromDual(NCasKeyData keyData)
        {
            MethodInvoker invoker = delegate()
            {
                OrderView19201080 orderView = (OrderView19201080)this.dicViews[ViewKind.OrderView19201080];
                orderView.SetKeyDataFromDual(keyData);
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
        /// 조작반 데이터를 pAlmScreenBiz로 넘겨주는 메소드
        /// </summary>
        /// <param name="plcProtocol"></param>
        public void SetPlcKeyData(NCasPlcProtocolBase plcProtocol)
        {
            this.pAlmScreenBiz.AddBizData(plcProtocol);
        }

        /// <summary>
        /// 듀얼시스템으로 키 데이터를 전송한다.
        /// </summary>
        /// <param name="keyData"></param>
        public void SendKeyDataToDual(NCasKeyData keyData)
        {
            byte[] tmpBuff = keyData.KeyDataToByteArry();

            if (tmpBuff == null)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm.SendKeyDataToDual(NCasKeyData keyData)", "NCasKeyData.KeyDataToByteArry()가 null");
                return;
            }

            if (this.dualSessionMng == null)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm.SendKeyDataToDual(NCasKeyData keyData)", "DUAL 시스템이 null");
                return;
            }

            if (this.dualSessionMng is NCasNetSessionServerMng)
            {
                (this.dualSessionMng as NCasNetSessionServerMng).SendData(this.profile, tmpBuff, tmpBuff.Length);
            }
            else if (this.dualSessionMng is NCasNetSessionClientMng)
            {
                (this.dualSessionMng as NCasNetSessionClientMng).SendData(this.profile, tmpBuff, tmpBuff.Length);
            }
        }

        /// <summary>
        /// 듀얼로 버튼키 데이터를 전달하기 위한 메소드
        /// </summary>
        /// <param name="keyData"></param>
        public void SetKeyBizData(NCasKeyData keyData)
        {
            KeyBizData keyBizData = new KeyBizData();
            keyBizData.IsLocal = true;
            keyBizData.KeyData = keyData;
            this.pAlmScreenBiz.AddBizData(keyBizData);
        }

        /// <summary>
        /// 발령 데이터 처리 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        public void SetOrderBizData(OrderBizData orderBizData)
        {
            this.pAlmScreenBiz.AddBizData(orderBizData);
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
        /// 서버 로컬IP를 받아 Dual Session을 관리 및 초기화한다.
        /// </summary>
        /// <param name="localIpAddr">서버 로컬IP</param>
        private void InitDualSession(string localIpAddr)
        {
            try
            {
                PDevInfo devInfo = this.mmfMng.GetPDevInfoByIp(localIpAddr);

                if (devInfo == null)
                {
                    MessageBox.Show("IP가 정상적이지 않습니다.", "듀얼시스템 관리", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (devInfo.DevId == NCasDefineDeviceKind.AlarmCtrlSys1)
                {
                    this.profile = new NCasProfile();
                    this.profile.IpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(localIpAddr, 0, 0, 0, 1);
                    this.profile.Port = (int)NCasPortID.PortIdIntDualAll;
                    this.profile.Name = "AlarmCtrlSys2";

                    this.dualSessionMng = new NCasNetSessionServerMng();
                    this.dualSessionMng.PollingDatas = new byte[] { 0x01 };
                    this.dualSessionMng.RecvNetSessionClient += new NCasNetSessionRecvEventHandler(dualSessionMng_RecvNetSessionClient);
                    (this.dualSessionMng as NCasNetSessionServerMng).AddProfile(profile);
                    (this.dualSessionMng as NCasNetSessionServerMng).StartSessionServerMng(localIpAddr, (int)NCasPortID.PortIdIntDualAll);
                }
                else if (devInfo.DevId == NCasDefineDeviceKind.AlarmCtrlSys2)
                {
                    this.profile = new NCasProfile();
                    this.profile.IpAddr = NCasUtilityMng.INCasCommUtility.SubtractIpAddr(localIpAddr, 0, 0, 0, 1);
                    this.profile.Port = (int)NCasPortID.PortIdIntDualAll;
                    this.profile.Name = "AlarmCtrlSys1";

                    this.dualSessionMng = new NCasNetSessionClientMng();
                    this.dualSessionMng.PollingDatas = new byte[] { 0x01 };
                    this.dualSessionMng.RecvNetSessionClient += new NCasNetSessionRecvEventHandler(dualSessionMng_RecvNetSessionClient);
                    (this.dualSessionMng as NCasNetSessionClientMng).AddProfile(profile);
                    (this.dualSessionMng as NCasNetSessionClientMng).StartSessionClientMng();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILogging.WriteLog("MainForm", "DUAL시스템 세션 초기화 시 Exception - " + ex.Message);
                MessageBox.Show("IP가 정상적이지 않습니다.", "듀얼시스템 관리", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 듀얼 시스템에서 수신받는 데이터 리시브 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dualSessionMng_RecvNetSessionClient(object sender, NCasNetSessionRecvEventArgs e)
        {
            if (e.Len == 1)
                return;

            byte[] tmpBuff = new byte[e.Len];
            System.Buffer.BlockCopy(e.Buff, 0, tmpBuff, 0, e.Len);
            NCasKeyData dualKeyData = new NCasKeyData(tmpBuff);
            KeyBizData keyBizData = new KeyBizData();
            keyBizData.IsLocal = false;
            keyBizData.KeyData = dualKeyData;
            this.pAlmScreenBiz.AddBizData(keyBizData);
        }

        /// <summary>
        /// Dual Session을 해제한다.
        /// </summary>
        private void UninitDualSession()
        {
            if (this.dualSessionMng == null)
                return;

            this.dualSessionMng.RecvNetSessionClient -= new NCasNetSessionRecvEventHandler(dualSessionMng_RecvNetSessionClient);

            if (this.dualSessionMng is NCasNetSessionServerMng)
            {
                (this.dualSessionMng as NCasNetSessionServerMng).StopSessionServerMng();
            }
            else if (this.dualSessionMng is NCasNetSessionClientMng)
            {
                (this.dualSessionMng as NCasNetSessionClientMng).StopSessionClientMng();
            }
        }

        /// <summary>
        /// 메인 화면 하단의 장비 상태를 업데이트한다.
        /// </summary>
        private void UpdateDeviceStatus()
        {
            foreach (Label label in this.lstDeviceStatusLabel)
            {
                DevStsInfo devInfo = this.mmfMng.GetDevStsInfoByIp(label.Tag.ToString());

                if (devInfo.Status == NCasDefineNormalStatus.Noraml)
                {
                    label.ImageIndex = 0;
                    label.ForeColor = Color.FromArgb(17, 231, 255);
                }
                else //NCasDefineNormalStatus.Noraml이 아니면 모두 '이상'으로 처리하기로 함. 2015/04/20
                {
                    label.ImageIndex = 1;
                    label.ForeColor = Color.FromArgb(255, 0, 0);
                }
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
        /// 시도 로고이미지를 화면에 셋팅한다.
        /// </summary>
        /// <param name="proveCode">셋팅할 시도의 Code</param>
        private void InitLogoImage(int proveCode)
        {
            Image image = null;

            switch (proveCode)
            {
                case 1670:
                    image = NCasPAlmScreenRsc._1670;
                    break;

                case 1671:
                    image = NCasPAlmScreenRsc._1671;
                    break;

                case 1672:
                    image = NCasPAlmScreenRsc._1672;
                    break;

                case 1673:
                    image = NCasPAlmScreenRsc._1673;
                    break;

                case 1674:
                    image = NCasPAlmScreenRsc._1674;
                    break;

                case 1675:
                    image = NCasPAlmScreenRsc._1675;
                    break;

                case 1676:
                    image = NCasPAlmScreenRsc._1676;
                    break;

                case 1677:
                    image = NCasPAlmScreenRsc._1677;
                    break;

                case 1678:
                    image = NCasPAlmScreenRsc._1678;
                    break;

                case 1679:
                    image = NCasPAlmScreenRsc._1679;
                    break;

                case 1680:
                    image = NCasPAlmScreenRsc._1680;
                    break;

                case 1681:
                    image = NCasPAlmScreenRsc._1681;
                    break;

                case 1682:
                    image = NCasPAlmScreenRsc._1682;
                    break;

                case 1683:
                    image = NCasPAlmScreenRsc._1683;
                    break;

                case 1684:
                    image = NCasPAlmScreenRsc._1684;
                    break;

                case 1685:
                    image = NCasPAlmScreenRsc._1685;
                    break;

                case 2481:
                    image = NCasPAlmScreenRsc._1686;
                    break;

                default:
                    image = NCasPAlmScreenRsc._1670;
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
            ViewBase resultViewBase = ResultView.CreateView(ViewKind.ResultView, this);
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

            this.btnOrderMenu.Tag = MainForm.ViewKind.OrderView19201080;
            this.btnOrderResultMenu.Tag = ViewKind.ResultView;
            this.btnDevMonMenu.Tag = ViewKind.DevMonView;
        }

        /// <summary>
        /// 메인 화면 하단의 장비 상태바를 초기화한다.
        /// </summary>
        private void InitDeviceStatus()
        {
            lstDeviceStatusLabel.Add(this.labelDevStatus1);
            lstDeviceStatusLabel.Add(this.labelDevStatus2);
            lstDeviceStatusLabel.Add(this.labelDevStatus3);
            lstDeviceStatusLabel.Add(this.labelDevStatus4);
            lstDeviceStatusLabel.Add(this.labelDevStatus5);
            lstDeviceStatusLabel.Add(this.labelDevStatus6);
            lstDeviceStatusLabel.Add(this.labelDevStatus7);
            lstDeviceStatusLabel.Add(this.labelDevStatus8);
            lstDeviceStatusLabel.Add(this.labelDevStatus9);

            for (int i = 0; i < lstDeviceStatusLabel.Count; i++)
            {
                try
                {
                    lstDeviceStatusLabel[i].Text = "        " + DeviceStatusMng.LstDeviceStatusData[i].Name;
                    lstDeviceStatusLabel[i].Tag = DeviceStatusMng.LstDeviceStatusData[i].IpAddr;
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("MainForm", "MainForm.InitDeviceStatus() Method", ex);
                }
            }
        }

        /// <summary>
        /// 서버 로컬IP를 받아 해당하는 MMF파일을 로드한다.
        /// </summary>
        /// <param name="localIpAddr">서버 로컬IP</param>
        private void InitMmfInfo(string localIpAddr)
        {
            this.mmfMng = new NCasMMFMng();
            this.mmfMng.LoadAllMMF();
            this.centInfo = this.mmfMng.GetCentInfoByNetId("10.1.0.0");
            this.centInfo2 = this.mmfMng.GetCentInfoByNetId("10.2.0.0");

#if release
            this.provInfo = this.mmfMng.GetProvInfoByNetId(localIpAddr);
#endif

#if debug
            this.provInfo = this.mmfMng.GetProvInfoByNetId("10.40.1.1"); //대전
            //this.provInfo = this.mmfMng.GetProvInfoByNetId("10.88.1.1"); //대구
            //this.provInfo = this.mmfMng.GetProvInfoByNetId("10.24.1.1"); //경기
#endif

            if (this.centInfo == null || this.centInfo2 == null || this.provInfo == null)
            {
                MessageBox.Show("데이터파일을 정상적으로 로드하지 못했습니다.", "데이터파일 로드", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("MainForm.InitMmfInfo(string localIpAddr) Method Error!");
            }
        }
        #endregion
    }
}