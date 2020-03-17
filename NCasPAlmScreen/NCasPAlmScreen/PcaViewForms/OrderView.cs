using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NCASFND;
using NCASFND.NCasCtrl;
using NCASFND.NCasLogging;
using NCASBIZ.NCasData;
using NCASBIZ.NCasEnv;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasPlcProtocol;
using NCASBIZ.NCasUtility;
using NCasAppCommon.Define;
using NCasAppCommon.Type;
using NCasMsgCommon.Std;
using NCasMsgCommon.Tts;
using NCasContentsModule.TTS;
using NCasContentsModule.StoMsg;
using NCasPAlmScreen;

namespace NCasPAlmScreenOld
{
    public partial class OrderView : ViewBase
    {
        #region enum
        //public enum DisasterBroadKind 구버전
        //{
        //    None = 0,
        //    Mic = 1,
        //    Tts = 2,
        //    StroredMessage = 3
        //}

        //public enum OrderDataSendStatus
        //{
        //    None = 0,
        //    First = 1,
        //    End = 2
        //}
        #endregion

        #region element
        private NCasDefineOrderMode selectedOrderMode = NCasDefineOrderMode.None;
        private NCasDefineOrderMedia selectedOrderMedia = NCasDefineOrderMedia.None;
        private List<string> lstSelectedOrderIP = new List<string>();
        private NCasDefineOrderKind selectedOrderKind = NCasDefineOrderKind.None;
        private NCasDefineOrderKind lastOrderKind = NCasDefineOrderKind.None;
        private List<NCasButton> lstSelectedButtons = new List<NCasButton>();
        //private DisasterBroadKind selectedDisasterBroadKind = DisasterBroadKind.None; 구버전
        //private DisasterBroadKind lastDisasterBroadKind = DisasterBroadKind.None;
        private List<GroupContent> selectedGroupContents = new List<GroupContent>();
        private StoredMessageText selectedStoredMessage = new StoredMessageText();
        private int storedMessageRepeatCount = 1;
        private TtsMessageText selectedTtsMessage = new TtsMessageText();
        private int selectedDistCode = 0;
        private bool orderStandbyFlag = false;
        private bool allDestinationFlag = false;
        private bool orderGroupFlag = false;
        private bool orderDistFlag = false;
        private bool orderTermFlag = false;
        private bool lampTestFlag = false;
        private bool wrongOperationFlag = false;
        private ProvInfo provInfo = null;
        private ImageList orderKindImageList = null;
        private ImageList orderKindDistImageList = null;
        private NCasButton curButton = null;
        private NCasButton wrongOperationBtn = null;
        private OrderStoredViewForm orderStoredViewForm = null;
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public OrderView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main">MainForm에서 넘겨주는 참조</param>
        public OrderView(MainForm main)
            : this()
        {
            this.main = main;
            this.provInfo = main.ProvInfo;
        }
        #endregion

        #region override method
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitImageList();
            this.OrderBtnArrange();
            this.labelTotalTermCount.Text = this.provInfo.GetUsableAlarmTermCnt().ToString();
            this.InitDistDetailListView();
            this.ShowDistDetailSelectForm(false);
            this.main.AddTimerMember(this);
        }

        public override void OnTimer()
        {
            this.SetOrderText();
            this.SetOrderResponseCount();
        }
        #endregion

        #region 프로퍼티
        /// <summary>
        /// ProvInfo 프로퍼티
        /// </summary>
        public ProvInfo ProvInfo
        {
            get { return this.provInfo; }
        }
        #endregion

        #region 버튼 배치
        /// <summary>
        /// 화면에 경보발령 버튼 초기화 및 배치
        /// </summary>
        private void OrderBtnArrange()
        {
            foreach (NCasKeyData keyData in KeyDataMng.LstKeyData)
            {
                Control[] controls = this.orderViewTableLayout.Controls.Find(keyData.ID.ToString(), false);

                if (controls == null)
                    continue;

                if (controls.Length == 0)
                    continue;

                this.curButton = controls[0] as NCasButton;
                this.curButton.Tag = keyData;

                if (keyData.KeyActioin == NCasKeyAction.None)
                {
                    this.curButton.Text = String.Empty;
                    this.curButton.Enabled = false;
                    continue;
                }
                else
                {
                    this.curButton.Text = keyData.Name;
                }

                if (keyData.KeyActioin == NCasKeyAction.Real) //실제 버튼
                {
                    this.curButton.ForeColor = Color.Red;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Test) //시험 버튼
                {
                    this.curButton.ForeColor = Color.Blue;
                }
                else if (keyData.KeyActioin == NCasKeyAction.WrongOperation) //오조작 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.wrongOperationBtn = this.curButton;
                    this.wrongOperationBtn.UseCheck = false;
                    this.wrongOperationBtn.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.wrongOperationBtn.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertNormal);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertError);
                    this.wrongOperationBtn.AnimationInterval = 500;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Cancel || keyData.KeyActioin == NCasKeyAction.Confirm) //선택취소와 확인 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.UseCheck = false;
                }
                else
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.curButton.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.curButton.AnimationInterval = 500;
                }
            }

            foreach (NCasKeyData keyData in KeyDataMng.LstKeyData)
            {
                Control[] controls = this.orderViewDistTableLayout.Controls.Find(keyData.ID.ToString(), false);

                if (controls == null)
                    continue;

                if (controls.Length == 0)
                    continue;

                this.curButton = controls[0] as NCasButton;
                this.curButton.Tag = keyData;

                if (keyData.KeyActioin == NCasKeyAction.None)
                {
                    this.curButton.Text = String.Empty;
                    this.curButton.Enabled = false;
                    continue;
                }
                else
                {
                    this.curButton.Text = keyData.Name;
                }

                if (keyData.KeyActioin == NCasKeyAction.Real) //실제 버튼
                {
                    this.curButton.ForeColor = Color.Red;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Test) //시험 버튼
                {
                    this.curButton.ForeColor = Color.Blue;
                }
                else if (keyData.KeyActioin == NCasKeyAction.WrongOperation) //오조작 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.wrongOperationBtn = this.curButton;
                    this.wrongOperationBtn.UseCheck = false;
                    this.wrongOperationBtn.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.wrongOperationBtn.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertNormal);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertError);
                    this.wrongOperationBtn.AnimationInterval = 500;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Cancel || keyData.KeyActioin == NCasKeyAction.Confirm) //선택취소와 확인 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.UseCheck = false;
                }
                else
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.curButton.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.curButton.AnimationInterval = 500;
                }
            }
        }
        #endregion

        #region ImageList 초기화
        /// <summary>
        /// 발령종류에 따른 ImageList 초기화
        /// </summary>
        private void InitImageList()
        {
            this.orderKindImageList = new ImageList();
            this.orderKindImageList.ImageSize = new Size(251, 69);
            this.orderKindImageList.ColorDepth = ColorDepth.Depth32Bit;
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertError);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertGray);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertGreen);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertNormal);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertOrange);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertPink);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertPupple);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertRed);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertSelected);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertWhite);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnAlertYellow);

            this.orderKindDistImageList = new ImageList();
            this.orderKindDistImageList.ImageSize = new Size(90, 69);
            this.orderKindDistImageList.ColorDepth = ColorDepth.Depth32Bit;
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertError);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertGray);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertGreen);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertNormal);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertOrange);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertPink);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertPupple);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertRed);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertSelected);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertWhite);
            this.orderKindImageList.Images.Add(NCasPAlmScreenRsc.btnRgAlertYellow);
        }
        #endregion

        #region ListView 초기화
        /// <summary>
        /// 단말개별 리스트뷰 초기화
        /// </summary>
        private void InitDistDetailListView()
        {
            this.distSelectListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.distSelectListView.GridDashStyle = DashStyle.Dot;
            this.distSelectListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.distSelectListView.Font = new Font(NCasPAlmScreenRsc.FontName, 11.0f);
            this.distSelectListView.ColumnHeight = 32;
            this.distSelectListView.ItemHeight = 29;
            this.distSelectListView.HideColumnCheckBox = true;

            NCasColumnHeader col = new NCasColumnHeader();
            col.Text = string.Empty;
            col.Width = 30;
            col.SortType = NCasListViewSortType.SortChecked;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            col.CheckBoxes = true;
            this.distSelectListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "이름";
            col.Width = 210;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.distSelectListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 130;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.distSelectListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "소속";
            col.Width = 110;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.distSelectListView.Columns.Add(col);
        }
        #endregion

        #region Confirm method
        /// <summary>
        /// 확인 버튼에 대한 처리 메소드
        /// </summary>
        private void Confirm()
        {
            DateTime orderTime = DateTime.Now;

            for (int i = 0; i < this.lstSelectedOrderIP.Count; i++)
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcAlarmOrder);
                NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;

                protoTc1.AlarmNetIdOrIpByString = this.lstSelectedOrderIP[i];
                protoTc1.OrderTimeByDateTime = orderTime;
                protoTc1.CtrlKind = NCasDefineControlKind.ControlAlarm;
                protoTc1.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc1.AlarmKind = this.selectedOrderKind;
                protoTc1.Mode = this.selectedOrderMode;
                protoTc1.Media = this.selectedOrderMedia;
                protoTc1.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                protoTc1.Sector = (this.allDestinationFlag == true) ? NCasDefineSectionCode.SectionProv :
                    (this.orderDistFlag == true) ? NCasDefineSectionCode.SectionDist :
                    (this.orderTermFlag == true) ? NCasDefineSectionCode.SectionTerm : NCasDefineSectionCode.None;

                if (this.orderGroupFlag)
                {
                    protoTc1.Sector = (this.GetIsDist(this.lstSelectedOrderIP[i]) == true) ? NCasDefineSectionCode.SectionDist : NCasDefineSectionCode.SectionTerm;
                }

                NCasProtocolFactory.MakeUdpFrame(protoTc1);

                OrderBizData orderBizData = new OrderBizData();
                orderBizData.AllDestinationFlag = this.allDestinationFlag;
                orderBizData.AlmProtocol = protoTc1;
                orderBizData.IsLocal = true;
                orderBizData.LastOrderKind = this.lastOrderKind;
                orderBizData.OrderDistFlag = this.orderDistFlag;
                orderBizData.OrderGroupFlag = this.orderGroupFlag;
                orderBizData.OrderTermFlag = this.orderTermFlag;
                //orderBizData.SelectedDisasterBroadKind = this.selectedDisasterBroadKind; 구버전
                orderBizData.SelectedStoredMessage = this.selectedStoredMessage;
                orderBizData.StoredMessageRepeatCount = this.storedMessageRepeatCount;
                orderBizData.SelectedTtsMessage = this.selectedTtsMessage;

                //if (i == 0) 구버전
                //{
                //    orderBizData.IsEnd = OrderDataSendStatus.First;
                //}
                //else if (i == (this.lstSelectedOrderIP.Count - 1))
                //{
                //    orderBizData.IsEnd = OrderDataSendStatus.End;
                //}
                //else
                //{
                //    orderBizData.IsEnd = OrderDataSendStatus.None;
                //}

                //구버전
                //if (this.lastDisasterBroadKind == DisasterBroadKind.Tts) //마지막 발령이 TTS발령인지..
                //{
                //    orderBizData.TtsOrderFlag = true;
                //}
                //else
                //{
                //    orderBizData.TtsOrderFlag = false;
                //}

                this.main.SetOrderBizData(orderBizData);
            }
        }
        #endregion

        #region 버튼 이벤트
        /// <summary>
        /// 경보발령 버튼 MouseDown 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRC11_MouseDown(object sender, MouseEventArgs e)
        {
            NCasButton ncasBtn = null;
            NCasKeyData keyData = null;
            bool isLocal = true;

            if (sender is KeyBizData)
            {
                isLocal = (sender as KeyBizData).IsLocal;
                ncasBtn = this.GetNCasButton((sender as KeyBizData).KeyData.KeyActioin, (sender as KeyBizData).KeyData.ID.ToString(), (sender as KeyBizData).KeyData.Info);
                keyData = (sender as KeyBizData).KeyData;
                ncasBtn.CheckedValue = ((keyData.KeyStatus == NCasKeyState.Check) ? true : false);
            }
            else if (sender is NCasButton)
            {
                ncasBtn = sender as NCasButton;
                keyData = (NCasKeyData)ncasBtn.Tag;

                if (ncasBtn.CheckedValue == true)
                {
                    keyData.KeyStatus = NCasKeyState.Check;
                }
                else
                {
                    keyData.KeyStatus = NCasKeyState.UnCheck;
                }
            }

            if (ncasBtn == null)
            {
                NCasLoggingMng.ILogging.WriteLog("OrderView", "btnRC11_MouseDown.selectBtn is null");
                return;
            }

            switch (keyData.KeyActioin)
            {
                case NCasKeyAction.Real: //실제
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyReal(ncasBtn);
                        }
                        else
                        {
                            this.CheckKeyRealFromDual(ncasBtn);
                        }
                    }
                    break;

                case NCasKeyAction.Test: //시험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyTest(ncasBtn);
                    }
                    break;

                case NCasKeyAction.WrongOperation: //오조작
                    this.CheckKeyWrongOperation();
                    break;

                case NCasKeyAction.LampTest: //램프시험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyLampTest(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Line: //유선
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyLine(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Sate: //위성
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeySate(ncasBtn);
                    }
                    break;

                case NCasKeyAction.ProveAllDestination: //시도 전체
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyProveAllDestination(ncasBtn);
                    }
                    break;

                case NCasKeyAction.GroupDestination: //그룹 (발령 대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyGroupDestination(ncasBtn);
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        //using (OrderGroupEditForm orderGroupEditForm = new OrderGroupEditForm(this)) 구버전
                        //{
                        //    orderGroupEditForm.ShowDialog();
                        //}
                    }
                    break;

                case NCasKeyAction.DistOneDestination: //시군 또는 개별단말 (발령 대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyDistOneDestination(ncasBtn);
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        this.CheckKeyDistOneDestinationRight(ncasBtn);
                        this.selectedDistCode = int.Parse((ncasBtn.Tag as NCasKeyData).Info);
                    }
                    break;

                case NCasKeyAction.Ready: //예비
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyReady(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Watch: //경계
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyWatch(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Attack: //공습
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyAttack(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Biological: //화생방
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyBiological(ncasBtn);
                    }
                    break;

                case NCasKeyAction.DisasterWatch: //재난위험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyDisasterWatch(ncasBtn);
                    }
                    break;

                case NCasKeyAction.DisasterBroad: //재난경계(재해방송)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyDisasterBroad(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Clear: //해제
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyClear(ncasBtn);
                    }
                    break;

                case NCasKeyAction.Cancel: //선택 취소
                    this.CheckKeyCancel();
                    break;

                case NCasKeyAction.Confirm: //확인
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyConfirm(ncasBtn);
                        }
                        else
                        {
                            this.CheckKeyConfirmFromDual(ncasBtn);
                        }
                    }
                    break;

                case NCasKeyAction.MicOrder: //재난경계 MIC
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyMicOrder(ncasBtn);
                    }
                    break;

                case NCasKeyAction.TtsOrder: //재난경계 TTS
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyTtsOrder(ncasBtn);
                        }
                        else
                        {
                            this.CheckKeyTtsOrderFromDual(ncasBtn);
                        }
                    }
                    break;

                case NCasKeyAction.MsgOrder: //재난경계 저장메시지
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyMsgOrder(ncasBtn);
                        }
                        else
                        {
                            this.CheckKeyMsgOrderFromDual(ncasBtn);
                        }
                    }
                    break;

                default:
                    break;
            }

            if (isLocal == true)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    return;

                //듀얼로 버튼 데이터 전송..다른 작업을 할 수도 있으니 일단 나눠놓자..
                if (keyData.KeyActioin == NCasKeyAction.Real)
                {
                    keyData.KeyStatus = (ncasBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }
                else if (keyData.KeyActioin == NCasKeyAction.TtsOrder)
                {
                    keyData.KeyStatus = (ncasBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }
                else if (keyData.KeyActioin == NCasKeyAction.MsgOrder)
                {
                    keyData.KeyStatus = (ncasBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }

                this.main.SetKeyBizData(keyData);

                //단일 버튼 데이터 전송
                NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetUnitStatus);
                NCasPlcProtocolSetUnitStatus unitStatus = plcProtoBase as NCasPlcProtocolSetUnitStatus;
                unitStatus.BtnCode = keyData.ID;
                NCasPlcProtocolFactory.MakeFrame(unitStatus);
                this.main.SetPlcKeyData(unitStatus);

                //전체 버튼 데이터 전송
                plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetAllStatus);
                NCasPlcProtocolSetAllStatus allStatus = plcProtoBase as NCasPlcProtocolSetAllStatus;

                foreach (NCasButton Btn in this.orderViewTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                }

                foreach (NCasButton Btn in this.orderViewDistTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                }

                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    allStatus.SetBtnStatus((btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Select);
                }

                if (this.wrongOperationFlag)
                {
                    allStatus.SetBtnStatus((this.wrongOperationBtn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Blink);
                }

                NCasPlcProtocolFactory.MakeFrame(allStatus);
                this.main.SetPlcKeyData(allStatus);
            }

            #region 상태 표출 테스트
            Console.WriteLine("\n선택한 버튼리스트에 있는 버튼 수 - " + this.lstSelectedButtons.Count.ToString());

            Console.WriteLine("선택한 발령 모드 - " + ((this.selectedOrderMode == NCasDefineOrderMode.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.None) :
                (this.selectedOrderMode == NCasDefineOrderMode.RealMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.RealMode) :
                (this.selectedOrderMode == NCasDefineOrderMode.TestMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.TestMode) : "실패"));

            Console.WriteLine("선택한 미디어 모드 - " + ((this.selectedOrderMedia == NCasDefineOrderMedia.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.None) :
                (this.selectedOrderMedia == NCasDefineOrderMedia.MediaAll) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaAll) :
                (this.selectedOrderMedia == NCasDefineOrderMedia.MediaLine) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaLine) :
                (this.selectedOrderMedia == NCasDefineOrderMedia.MediaSate) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaSate) : "실패"));

            Console.WriteLine("선택한 시도 발령대상 수 - " + lstSelectedOrderIP.Count.ToString());
            foreach (string str in this.lstSelectedOrderIP)
            {
                Console.WriteLine("      대상은 - " + str);
            }

            Console.WriteLine("선택한 발령종류 - " + ((this.selectedOrderKind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
                    (this.selectedOrderKind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
                    (this.selectedOrderKind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
                    (this.selectedOrderKind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
                    (this.selectedOrderKind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
                    (this.selectedOrderKind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
                    (this.selectedOrderKind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
                    (this.selectedOrderKind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
                    (this.selectedOrderKind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
                    (this.selectedOrderKind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) : ""));

            Console.WriteLine("가장 마지막 발령종류 - " + ((this.lastOrderKind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
                    (this.lastOrderKind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
                    (this.lastOrderKind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
                    (this.lastOrderKind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
                    (this.lastOrderKind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
                    (this.lastOrderKind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
                    (this.lastOrderKind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
                    (this.lastOrderKind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
                    (this.lastOrderKind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
                    (this.lastOrderKind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
                    (this.lastOrderKind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) : ""));

            Console.WriteLine("오조작 상태 - " + ((this.wrongOperationFlag == true) ? "O" : "X"));

            Console.WriteLine("시도 전체 선택 - " + ((this.allDestinationFlag == true) ? "O" : "X"));

            Console.WriteLine("그룹 발령 선택 - " + ((this.orderGroupFlag == true) ? "O" : "X"));

            Console.WriteLine("시군 발령 선택 - " + ((this.orderDistFlag == true) ? "O" : "X"));

            Console.WriteLine("개별단말 발령 선택 - " + ((this.orderTermFlag == true) ? "O" : "X"));

            Console.WriteLine("예비 발령 - " + ((this.orderStandbyFlag == true) ? "O" : "X"));

            Console.WriteLine("램프시험 - " + ((this.lampTestFlag == true) ? "O" : "X"));

            //구버전
            //Console.WriteLine("재난경계 종류 선택한 거 - " + ((this.selectedDisasterBroadKind == DisasterBroadKind.None) ? "선택안함" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.Mic) ? "마이크" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.Tts) ? "TTS" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.StroredMessage) ? "저장메시지" : "선택안함"));

            //Console.WriteLine("마지막 재난경계 발령한 거 - " + ((this.lastDisasterBroadKind == DisasterBroadKind.None) ? "선택안함" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.Mic) ? "마이크" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.Tts) ? "TTS" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.StroredMessage) ? "저장메시지" : "선택안함"));

            Console.WriteLine("선택한 그룹 버튼 수 - " + this.selectedGroupContents.Count.ToString());
            foreach (GroupContent groupContent in this.selectedGroupContents)
            {
                Console.WriteLine("      버튼은 - " + groupContent.Title);
            }

            Console.WriteLine("저장메시지 - ");
            Console.WriteLine("      제목 - " + this.selectedStoredMessage.Title);
            Console.WriteLine("      번호 - " + this.selectedStoredMessage.MsgNum);
            Console.WriteLine("      시간 - " + this.selectedStoredMessage.PlayTime.ToString());
            Console.WriteLine("      반복횟수 - " + this.storedMessageRepeatCount.ToString());

            Console.WriteLine("TTS메시지 - ");
            Console.WriteLine("      제목 - " + this.selectedTtsMessage.Title);
            Console.WriteLine("      내용 - " + this.selectedTtsMessage.Text);
            #endregion
        }
        #endregion

        #region private method
        #region 원하는 버튼 제어
        /// <summary>
        /// 해제되어 있는 버튼을 선택한다.
        /// </summary>
        /// <param name="keyActionDefine">선택할 버튼</param>
        private void SetOnButton(NCasKeyAction keyActionDefine)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    if (ncasBtn.CheckedValue == false)
                    {
                        ncasBtn.CheckedValue = true;

                        if (!this.lstSelectedButtons.Contains(ncasBtn))
                        {
                            this.lstSelectedButtons.Add(ncasBtn);
                        }
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    if (ncasBtn.CheckedValue == false)
                    {
                        ncasBtn.CheckedValue = true;

                        if (!this.lstSelectedButtons.Contains(ncasBtn))
                        {
                            this.lstSelectedButtons.Add(ncasBtn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 선택되어 있는 버튼 선택을 해제한다.
        /// </summary>
        /// <param name="keyActionDefine">해제할 버튼</param>
        private void SetOffButton(NCasKeyAction keyActionDefine)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    if (ncasBtn.CheckedValue == true)
                    {
                        ncasBtn.CheckedValue = false;

                        if (this.lstSelectedButtons.Contains(ncasBtn))
                        {
                            this.lstSelectedButtons.Remove(ncasBtn);
                        }

                        if (NCasAnimator.ContainsItem(ncasBtn))
                        {
                            NCasAnimator.RemoveItem(ncasBtn);
                        }
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    if (ncasBtn.CheckedValue == true)
                    {
                        ncasBtn.CheckedValue = false;

                        if (this.lstSelectedButtons.Contains(ncasBtn))
                        {
                            this.lstSelectedButtons.Remove(ncasBtn);
                        }

                        if (NCasAnimator.ContainsItem(ncasBtn))
                        {
                            NCasAnimator.RemoveItem(ncasBtn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 버튼 상태와 관계없이 버튼 선택을 해제한다.
        /// </summary>
        /// <param name="keyActionDefine">해제할 버튼</param>
        private void SetOffButtonAny(NCasKeyAction keyActionDefine)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    ncasBtn.CheckedValue = false;

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }

                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == keyActionDefine)
                {
                    ncasBtn.CheckedValue = false;

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }

                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }
                }
            }
        }
        #endregion

        #region 원하는 버튼 Color 셋팅 - 발령종류에 따라..
        /// <summary>
        /// 발령종류에 따라 버튼 Normal 색을 셋팅한다.
        /// </summary>
        /// <param name="ncasBtn"></param>
        /// <param name="orderKind"></param>
        private void SetOrderKindButtonColor(NCasButton ncasBtn, NCasDefineOrderKind orderKind)
        {
            switch (orderKind)
            {
                case NCasDefineOrderKind.AlarmStandby:
                    ncasBtn.Image = this.orderKindImageList.Images[2];
                    break;

                case NCasDefineOrderKind.AlarmWatch:
                    ncasBtn.Image = this.orderKindImageList.Images[4];
                    break;

                case NCasDefineOrderKind.AlarmAttack:
                    ncasBtn.Image = this.orderKindImageList.Images[7];
                    break;

                case NCasDefineOrderKind.AlarmBiochemist:
                    ncasBtn.Image = this.orderKindImageList.Images[10];
                    break;

                case NCasDefineOrderKind.DisasterBroadcast:
                    ncasBtn.Image = this.orderKindImageList.Images[6];
                    break;

                case NCasDefineOrderKind.DisasterWatch:
                    ncasBtn.Image = this.orderKindImageList.Images[5];
                    break;

                case NCasDefineOrderKind.AlarmCancel:
                    ncasBtn.Image = this.orderKindImageList.Images[9];
                    break;

                default:
                    ncasBtn.Image = this.orderKindImageList.Images[3];
                    break;
            }
        }
        #endregion

        #region 발령대상 관련
        /// <summary>
        /// IP에 해당하는 시군 버튼을 반대의 상태로 바꾼다.
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="isOn"></param>
        private void SetOrderKindOneDistDestinationSelect(string ipAddr, bool isOn)
        {
            DistInfo distinfo = this.main.MmfMng.GetDistInfoByNetId(ipAddr);

            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin != NCasKeyAction.DistOneDestination)
                    continue;

                if (btnKeyData.Info != distinfo.Code.ToString())
                    continue;

                if (isOn == false)
                {
                    ncasBtn.CheckedValue = false;

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
                else
                {
                    ncasBtn.CheckedValue = true;

                    if (!this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Add(ncasBtn);
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin != NCasKeyAction.DistOneDestination)
                    continue;

                if (btnKeyData.Info != distinfo.Code.ToString())
                    continue;

                if (isOn == false)
                {
                    ncasBtn.CheckedValue = false;

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
                else
                {
                    ncasBtn.CheckedValue = true;

                    if (!this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Add(ncasBtn);
                    }
                }
            }
        }

        /// <summary>
        /// 발령대상의 모든 버튼을 발령종류에 따라 Color를 셋팅한다.
        /// </summary>
        /// <param name="orderKind"></param>
        private void SetOrderKindButtonColorProvAll(NCasDefineOrderKind orderKind)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.ProveAllDestination || btnKeyData.KeyActioin == NCasKeyAction.GroupDestination ||
                    btnKeyData.KeyActioin == NCasKeyAction.DistOneDestination)
                {
                    this.SetOrderKindButtonColor(ncasBtn, orderKind);
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.ProveAllDestination || btnKeyData.KeyActioin == NCasKeyAction.GroupDestination ||
                    btnKeyData.KeyActioin == NCasKeyAction.DistOneDestination)
                {
                    this.SetOrderKindButtonColor(ncasBtn, orderKind);
                }
            }
        }

        /// <summary>
        /// 발령대상의 모든 버튼의 깜박임을 해제한다.
        /// </summary>
        private void SetOrderDestinationButtonColorOff()
        {
            foreach (NCasButton btn in this.lstSelectedButtons)
            {
                NCasKeyData keyData = (NCasKeyData)btn.Tag;

                if (keyData.KeyActioin == NCasKeyAction.ProveAllDestination || keyData.KeyActioin == NCasKeyAction.GroupDestination ||
                    keyData.KeyActioin == NCasKeyAction.DistOneDestination)
                {
                    btn.LstAnimation.Images.Clear();

                    if (NCasAnimator.ContainsItem(btn))
                    {
                        NCasAnimator.RemoveItem(btn);
                    }
                }
            }
        }

        /// <summary>
        /// 발령종류에 따라 버튼을 셋팅하고 깜빡인다.
        /// </summary>
        /// <param name="orderKind"></param>
        private void SetOrderKindButtonColorOn(NCasDefineOrderKind orderKind)
        {
            foreach (NCasButton btn in this.lstSelectedButtons)
            {
                NCasKeyData keyData = (NCasKeyData)btn.Tag;

                if (keyData.KeyActioin == NCasKeyAction.ProveAllDestination || keyData.KeyActioin == NCasKeyAction.GroupDestination ||
                    keyData.KeyActioin == NCasKeyAction.DistOneDestination)
                {
                    btn.LstAnimation.Images.Clear();
                    btn.LstAnimation.Images.Add(this.orderKindImageList.Images[3]);

                    switch (orderKind)
                    {
                        case NCasDefineOrderKind.AlarmStandby:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[2]);
                            break;

                        case NCasDefineOrderKind.AlarmAttack:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[7]);
                            break;

                        case NCasDefineOrderKind.AlarmWatch:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[4]);
                            break;

                        case NCasDefineOrderKind.AlarmBiochemist:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[10]);
                            break;

                        case NCasDefineOrderKind.DisasterBroadcast:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[6]);
                            break;

                        case NCasDefineOrderKind.DisasterWatch:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[5]);
                            break;

                        case NCasDefineOrderKind.AlarmCancel:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[9]);
                            break;

                        default:
                            break;
                    }

                    if (!NCasAnimator.ContainsItem(btn))
                    {
                        NCasAnimator.AddItem(btn);
                    }
                }
            }
        }
        #endregion

        #region 발령종류 관련
        /// <summary>
        /// 발령종류 버튼을 모두 해제한다.
        /// </summary>
        private void ClearOrderKind()
        {
            this.selectedOrderKind = NCasDefineOrderKind.None;
            //this.selectedDisasterBroadKind = DisasterBroadKind.None; 구버전
            this.SetOffButton(NCasKeyAction.Ready);
            this.SetOffButton(NCasKeyAction.Attack);
            this.SetOffButton(NCasKeyAction.Watch);
            this.SetOffButton(NCasKeyAction.Biological);
            this.SetOffButton(NCasKeyAction.DisasterBroad);
            this.SetOffButton(NCasKeyAction.DisasterWatch);
            this.SetOffButton(NCasKeyAction.Clear);
            this.SetOffButton(NCasKeyAction.MicOrder);
            this.SetOffButton(NCasKeyAction.TtsOrder);
            this.SetOffButton(NCasKeyAction.MsgOrder);
        }
        #endregion

        #region 화면 관련
        /// <summary>
        /// 시군 단말선택 화면을 보이거나 숨긴다.
        /// </summary>
        /// <param name="isDistShow">true - 시군 단말선택 화면, false - 시도에 속한 전체 시군선택 화면</param>
        private void ShowDistDetailSelectForm(bool isDistShow)
        {
            if (isDistShow)
            {
                this.orderViewDistTableLayout.Dock = DockStyle.None;
                this.orderViewDistTableLayout.Visible = false;
                this.distSelectPanel.Dock = DockStyle.Fill;
                this.distSelectPanel.Visible = true;
            }
            else
            {
                this.orderViewDistTableLayout.Dock = DockStyle.Fill;
                this.orderViewDistTableLayout.Visible = true;
                this.distSelectPanel.Dock = DockStyle.None;
                this.distSelectPanel.Visible = false;
            }
        }
        #endregion

        #region timer method
        /// <summary>
        /// 경보발령에 대한 응답 카운트를 화면에 표시한다.
        /// </summary>
        private void SetOrderResponseCount()
        {
            this.labelResponseTermCount.Text = this.provInfo.AlarmRespCnt.ToString();
            this.labelErrorTermCount.Text = this.provInfo.FaultAlmResponseCnt.ToString();
        }

        /// <summary>
        /// 경보발령에 대한 정보를 화면 상단에 표시한다.
        /// </summary>
        private void SetOrderText()
        {
            this.orderTextBoard.ClearTextBlock();

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem3(this.provInfo.AlarmOrderInfo.OccurTimeToDateTime) +
                " [" + this.provInfo.Name + "]에서 [" +
                NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(this.provInfo.AlarmOrderInfo.Media) + "]으로 ", Color.FromArgb(3, 41, 87)));

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(this.provInfo.AlarmOrderInfo.Mode) + "] ",
                (this.provInfo.AlarmOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? Color.FromArgb(232, 82, 53) : Color.FromArgb(6, 147, 6)));

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(this.provInfo.AlarmOrderInfo.Kind) + "] 경보를 발령했습니다.", Color.FromArgb(3, 41, 87)));
        }
        #endregion

        #region etc event
        /// <summary>
        /// 저장메시지 진행화면에서 방송종료 시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orderStoredViewForm_StoredMsgFinishEvent(object sender, EventArgs e)
        {
            if (this.orderStoredViewForm != null)
            {
                this.orderStoredViewForm.StoredMsgFinishEvent -= new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
                this.orderStoredViewForm.Close();
                this.orderStoredViewForm = null;
            }
        }

        /// <summary>
        /// 이전 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void preStepBtn_Click(object sender, EventArgs e)
        {
            if (this.selectedDistCode == 0)
                return;

            this.lstSelectedOrderIP.Clear();
            this.ClearOrderKind();
            this.distSelectListView.Items.Clear();

            foreach (TermInfo eachTermInfo in this.main.MmfMng.GetDistInfoByCode(this.selectedDistCode).LstTerms)
            {
                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.IpAddrToSring;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.DistInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.distSelectListView.Items.Add(lvi);
            }

            this.preStepBtn.Visible = false;
        }

        /// <summary>
        /// 개별단말 화면에서 취소버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTermSelectCancel_Click(object sender, EventArgs e)
        {
            this.orderTermFlag = false;
            this.lstSelectedOrderIP.Clear();
            this.ShowDistDetailSelectForm(false);
            this.ClearOrderKind();
            this.selectedDistCode = 0;
        }

        /// <summary>
        /// 개별단말 화면에서 전체단말버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllTermSelect_Click(object sender, EventArgs e)
        {
            this.lstSelectedOrderIP.Clear();
            this.distSelectListView.Items.Clear();
            this.ClearOrderKind();
            this.preStepBtn.Visible = true;

            foreach (TermInfo eachTermInfo in this.provInfo.LstTerms)
            {
                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.IpAddrToSring;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.DistInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.distSelectListView.Items.Add(lvi);
            }
        }

        /// <summary>
        /// 개별단말 화면에서 리스트뷰 Item 체크 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void distSelectListView_ItemChecked(object sender, NCasItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                this.orderTermFlag = true;
                this.lstSelectedOrderIP.Add(e.Item.Name);
            }
            else
            {
                this.lstSelectedOrderIP.Remove(e.Item.Name);

                if (this.lstSelectedOrderIP.Count == 0)
                {
                    this.orderTermFlag = false;
                }
            }
        }
        #endregion

        #region etc method
        /// <summary>
        /// NCasKeyAction을 받아 해당하는 NCasButton을 반환한다.
        /// </summary>
        /// <param name="keyAction"></param>
        /// <returns></returns>
        private NCasButton GetNCasButton(NCasKeyAction keyAction, string btnCode, string info)
        {
            if (keyAction == NCasKeyAction.GroupDestination)
            {
                foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        if (btnKeyData.ID.ToString() == btnCode)
                        {
                            return ncasBtn;
                        }
                    }
                }

                foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        if (btnKeyData.ID.ToString() == btnCode)
                        {
                            return ncasBtn;
                        }
                    }
                }
            }
            else if (keyAction == NCasKeyAction.DistOneDestination)
            {
                foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        if (btnKeyData.Info == info.Replace("\0", ""))
                        {
                            return ncasBtn;
                        }
                    }
                }

                foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        if (btnKeyData.Info == info.Replace("\0", ""))
                        {
                            return ncasBtn;
                        }
                    }
                }
            }
            else
            {
                foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        return ncasBtn;
                    }
                }

                foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        return ncasBtn;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// IP가 시군인지 단말인지 확인
        /// </summary>
        /// <param name="ipAddr">IP</param>
        /// <returns>true - 시도, false - 시군</returns>
        private bool GetIsDist(string ipAddr)
        {
            foreach (GroupContent groupContent in this.selectedGroupContents)
            {
                foreach (GroupData groupData in groupContent.LstGroupData)
                {
                    if (groupData.IpAddr == ipAddr)
                    {
                        return groupData.IsDist;
                    }
                }
            }

            return false;
        }
        #endregion
        #endregion

        #region public method
        /// <summary>
        /// PLC로부터 데이터를 수신받아 화면에 반영한다.
        /// </summary>
        /// <param name="plcResponse"></param>
        public void SetKeyPlc(NCasPlcProtocolReqStatusResponse plcResponse)
        {
            if (plcResponse.Mode == NCasDefinePlcMode.Real && plcResponse.BtnCode == NCasDefineButtonCode.None)
            {
                //실제키를 돌린 경우
                MessageBox.Show("조작반 실제 키 돌렸어~");
            }
            else
            {
                if (plcResponse.BtnCode == NCasDefineButtonCode.None)
                {
                    //실제키를 시험으로 돌린 경우
                    MessageBox.Show("조작반 시험으로 돌렸어~");
                }
                else
                {
                    //그 외 일반 버튼을 누른 경우
                    MessageBox.Show("버튼 눌렀네~");
                }
            }
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 키 데이터를 화면에 반영한다.
        /// </summary>
        /// <param name="keyData"></param>
        public void SetKeyDataFromDual(NCasKeyData keyData)
        {
            KeyBizData keyBizData = new KeyBizData();
            keyBizData.IsLocal = false;
            keyBizData.KeyData = keyData;

            this.btnRC11_MouseDown(keyBizData, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        /// <summary>
        /// 화면의 버튼 중에서 그룹 버튼을 리스트로 반환한다.
        /// </summary>
        /// <returns></returns>
        public List<NCasButton> GetGroupButton()
        {
            List<NCasButton> rstBtnList = new List<NCasButton>();

            foreach (NCasButton btn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)btn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.GroupDestination)
                {
                    if (!rstBtnList.Contains(btn))
                    {
                        rstBtnList.Add(btn);
                    }
                }
            }

            foreach (NCasButton btn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)btn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.GroupDestination)
                {
                    if (!rstBtnList.Contains(btn))
                    {
                        rstBtnList.Add(btn);
                    }
                }
            }

            return rstBtnList;
        }
        #endregion

        #region 실제 버튼
        /// <summary>
        /// 실제 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyReal(NCasButton selectBtn)
        {
            if (selectBtn.CheckedValue == true)
            {
                using (OrderConfirmForm confirm = new OrderConfirmForm())
                {
                    confirm.PasswordConfirmEvent += new EventHandler(confirm_PasswordConfirmEvent);
                    confirm.ShowDialog();
                    confirm.PasswordConfirmEvent -= new EventHandler(confirm_PasswordConfirmEvent);
                }
            }
            else
            {
                if (MessageBox.Show("실제 발령을 해제하겠습니까?", "실제 발령 해제", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.selectedOrderMode = NCasDefineOrderMode.None;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                    return;
                }
            }

            if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    btn.CheckedValue = false;
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.lstSelectedButtons.Clear();
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;

                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.Line);
                this.SetOnButton(NCasKeyAction.Sate);
                this.selectedOrderMedia = NCasDefineOrderMedia.MediaAll;

                if (this.wrongOperationFlag)
                {
                    this.CheckKeyWrongOperation();
                }

                if (this.lampTestFlag)
                {
                    this.SetOffButton(NCasKeyAction.LampTest);

                    //램프시험 중지
                    NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetAllStatus);
                    NCasPlcProtocolSetAllStatus allStatus = plcProtoBase as NCasPlcProtocolSetAllStatus;

                    foreach (NCasButton Btn in this.orderViewTableLayout.Controls)
                    {
                        allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                    }

                    foreach (NCasButton Btn in this.orderViewDistTableLayout.Controls)
                    {
                        allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                    }

                    NCasPlcProtocolFactory.MakeFrame(allStatus);
                    this.main.SetPlcKeyData(allStatus);
                    this.lampTestFlag = false;
                }
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.TestMode)
            {
                selectBtn.CheckedValue = false;
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    btn.CheckedValue = false;
                }

                selectBtn.CheckedValue = false;
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.lstSelectedButtons.Clear();
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.ShowDistDetailSelectForm(false);

                if (this.wrongOperationFlag)
                {
                    this.CheckKeyWrongOperation();
                }
            }
        }

        /// <summary>
        /// 실제발령 비밀번호 확인 컨펌 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirm_PasswordConfirmEvent(object sender, EventArgs e)
        {
            this.selectedOrderMode = NCasDefineOrderMode.RealMode;
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 실제 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyRealFromDual(NCasButton selectBtn)
        {
            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderMode = NCasDefineOrderMode.RealMode;
            }
            else
            {
                if (this.selectedOrderMode == NCasDefineOrderMode.TestMode)
                {
                    this.selectedOrderMode = NCasDefineOrderMode.TestMode;
                }
                else
                {
                    this.selectedOrderMode = NCasDefineOrderMode.None;
                }
            }

            if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    btn.CheckedValue = false;
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.lstSelectedButtons.Clear();
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;

                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                selectBtn.CheckedValue = true;
                this.SetOnButton(NCasKeyAction.Line);
                this.SetOnButton(NCasKeyAction.Sate);
                this.selectedOrderMedia = NCasDefineOrderMedia.MediaAll;

                if (this.wrongOperationFlag)
                {
                    this.CheckKeyWrongOperation();
                }

                if (this.lampTestFlag)
                {
                    this.SetOffButton(NCasKeyAction.LampTest);
                    this.lampTestFlag = false;
                }
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.TestMode)
            {
                selectBtn.CheckedValue = false;
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    btn.CheckedValue = false;
                }

                selectBtn.CheckedValue = false;
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.lstSelectedButtons.Clear();
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.ShowDistDetailSelectForm(false);

                if (this.wrongOperationFlag)
                {
                    this.CheckKeyWrongOperation();
                }
            }
        }
        #endregion

        #region 시험 버튼
        /// <summary>
        /// 시험 버튼 동작
        /// </summary>
        /// <param name="keyData"></param>
        private void CheckKeyTest(NCasButton selectBtn)
        {
            if (selectBtn.CheckedValue == true)
            {
                if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
                {
                    NCasAnimator.AddItem(this.wrongOperationBtn);
                    this.wrongOperationFlag = true;
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    this.selectedOrderMode = NCasDefineOrderMode.TestMode;

                    if (!this.lstSelectedButtons.Contains(selectBtn))
                    {
                        this.lstSelectedButtons.Add(selectBtn);
                    }

                    this.SetOnButton(NCasKeyAction.Line);
                    this.SetOnButton(NCasKeyAction.Sate);
                    this.selectedOrderMedia = NCasDefineOrderMedia.MediaAll;

                    if (this.wrongOperationFlag)
                    {
                        this.CheckKeyWrongOperation();
                    }

                    if (this.lampTestFlag)
                    {
                        this.SetOffButton(NCasKeyAction.LampTest);

                        //램프시험 중지
                        NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetAllStatus);
                        NCasPlcProtocolSetAllStatus allStatus = plcProtoBase as NCasPlcProtocolSetAllStatus;

                        foreach (NCasButton Btn in this.orderViewTableLayout.Controls)
                        {
                            allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                        }

                        foreach (NCasButton Btn in this.orderViewDistTableLayout.Controls)
                        {
                            allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                        }

                        NCasPlcProtocolFactory.MakeFrame(allStatus);
                        this.main.SetPlcKeyData(allStatus);
                        this.lampTestFlag = false;
                    }
                }
            }
            else
            {
                this.selectedOrderMode = NCasDefineOrderMode.None;

                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    btn.CheckedValue = false;
                }

                selectBtn.CheckedValue = false;
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.lstSelectedButtons.Clear();
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.ShowDistDetailSelectForm(false);

                if (this.wrongOperationFlag)
                {
                    this.CheckKeyWrongOperation();
                }
            }
        }
        #endregion

        #region 오조작 버튼
        /// <summary>
        /// 오조작 버튼 동작
        /// </summary>
        private void CheckKeyWrongOperation()
        {
            if (NCasAnimator.ContainsItem(this.wrongOperationBtn))
            {
                NCasAnimator.RemoveItem(this.wrongOperationBtn);
            }

            this.wrongOperationFlag = false;
        }
        #endregion

        #region 램프시험 버튼
        /// <summary>
        /// 램프시험 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyLampTest(NCasButton selectBtn)
        {
            if (this.selectedOrderMode != NCasDefineOrderMode.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                //램프시험 요청
                NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetAllStatus);
                NCasPlcProtocolSetAllStatus allStatus = plcProtoBase as NCasPlcProtocolSetAllStatus;

                foreach (NCasButton Btn in this.orderViewTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Blink);
                }

                foreach (NCasButton Btn in this.orderViewDistTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Blink);
                }

                NCasPlcProtocolFactory.MakeFrame(allStatus);
                this.main.SetPlcKeyData(allStatus);
                this.lampTestFlag = true;
            }
            else
            {
                //램프시험 중지
                NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetAllStatus);
                NCasPlcProtocolSetAllStatus allStatus = plcProtoBase as NCasPlcProtocolSetAllStatus;

                foreach (NCasButton Btn in this.orderViewTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                }

                foreach (NCasButton Btn in this.orderViewDistTableLayout.Controls)
                {
                    allStatus.SetBtnStatus((Btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Normal);
                }

                NCasPlcProtocolFactory.MakeFrame(allStatus);
                this.main.SetPlcKeyData(allStatus);
                this.lampTestFlag = false;
            }
        }
        #endregion

        #region 유선 버튼
        /// <summary>
        /// 유선 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyLine(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (this.selectedOrderMedia == NCasDefineOrderMedia.MediaSate)
                {
                    this.selectedOrderMedia = NCasDefineOrderMedia.MediaAll;
                }
                else
                {
                    this.selectedOrderMedia = NCasDefineOrderMedia.MediaLine;
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.Sate);
                this.selectedOrderMedia = NCasDefineOrderMedia.MediaSate;
            }
        }
        #endregion

        #region 위성 버튼
        /// <summary>
        /// 위성 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeySate(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (this.selectedOrderMedia == NCasDefineOrderMedia.MediaLine)
                {
                    this.selectedOrderMedia = NCasDefineOrderMedia.MediaAll;
                }
                else
                {
                    this.selectedOrderMedia = NCasDefineOrderMedia.MediaSate;
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.Line);
                this.selectedOrderMedia = NCasDefineOrderMedia.MediaLine;
            }
        }
        #endregion

        #region 시도 전체(발령대상) 버튼
        /// <summary>
        /// 시도 전체(발령대상) 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyProveAllDestination(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.SetOrderDestinationButtonColorOff();
                this.ShowDistDetailSelectForm(false);
                this.allDestinationFlag = true;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;

                if (!this.lstSelectedOrderIP.Contains(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4())))
                {
                    this.lstSelectedOrderIP.Clear();
                    this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4()));
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.ProveAllDestination);
                this.SetOnButton(NCasKeyAction.GroupDestination);
                this.SetOnButton(NCasKeyAction.DistOneDestination);
                this.ClearOrderKind();
            }
            else
            {
                this.SetOrderDestinationButtonColorOff();
                this.allDestinationFlag = false;
                this.lstSelectedOrderIP.Clear();

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.SetOffButton(NCasKeyAction.ProveAllDestination);
                this.SetOffButton(NCasKeyAction.GroupDestination);
                this.SetOffButton(NCasKeyAction.DistOneDestination);
                this.ClearOrderKind();
            }
        }
        #endregion

        #region 그룹(발령 대상) 버튼
        /// <summary>
        /// 그룹(발령 대상) 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyGroupDestination(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || this.allDestinationFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            this.ShowDistDetailSelectForm(false);

            if (this.orderDistFlag || this.orderTermFlag) //시군 또는 단말이 선택되어 있으면 선택해제
            {
                this.SetOffButton(NCasKeyAction.DistOneDestination);
                this.lstSelectedOrderIP.Clear();
                this.orderDistFlag = false;
                this.orderTermFlag = false;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.SetOrderDestinationButtonColorOff();

                if (this.selectedOrderKind != NCasDefineOrderKind.None)
                {
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                    this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                    selectBtn.CheckedValue = true;
                }

                this.orderGroupFlag = true;
                GroupContent groupContent = GroupContentMng.GetGroupContent((selectBtn.Tag as NCasKeyData).ID.ToString());

                if (!this.selectedGroupContents.Contains(groupContent))
                {
                    this.selectedGroupContents.Add(groupContent);
                }

                if (groupContent.LstGroupData.Count <= 0)
                    return;

                foreach (GroupData groupData in groupContent.LstGroupData)
                {
                    if (groupData.IsDist)
                    {
                        this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(groupData.IpAddr, 0, 0, 0, 255));
                        this.SetOrderKindOneDistDestinationSelect(groupData.IpAddr, true);
                    }
                    else
                    {
                        this.lstSelectedOrderIP.Add(groupData.IpAddr);
                    }
                }

                this.ClearOrderKind();
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                GroupContent groupContent = GroupContentMng.GetGroupContent((selectBtn.Tag as NCasKeyData).ID.ToString());

                if (this.selectedGroupContents.Contains(groupContent))
                {
                    this.selectedGroupContents.Remove(groupContent);
                }

                if (groupContent.LstGroupData.Count <= 0)
                    return;

                foreach (GroupData groupData in groupContent.LstGroupData)
                {
                    if (groupData.IsDist)
                    {
                        this.lstSelectedOrderIP.Remove(NCasUtilityMng.INCasCommUtility.AddIpAddr(groupData.IpAddr, 0, 0, 0, 255));
                        this.SetOrderKindOneDistDestinationSelect(groupData.IpAddr, false);

                        if (this.lstSelectedOrderIP.Contains(NCasUtilityMng.INCasCommUtility.AddIpAddr(groupData.IpAddr, 0, 0, 0, 255)))
                        {
                            this.SetOrderKindOneDistDestinationSelect(groupData.IpAddr, true);
                        }
                    }
                    else
                    {
                        this.lstSelectedOrderIP.Remove(groupData.IpAddr);
                    }
                }

                if (this.lstSelectedOrderIP.Count == 0)
                {
                    this.orderGroupFlag = false;
                }
            }
        }
        #endregion

        #region 시군 또는 개별단말 (발령 대상) 버튼
        /// <summary>
        /// 시군 또는 개별단말 (발령 대상) 버튼 마우스왼쪽 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyDistOneDestination(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.allDestinationFlag == true || this.orderGroupFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue)
            {
                this.SetOrderDestinationButtonColorOff();

                if (this.selectedOrderKind != NCasDefineOrderKind.None)
                {
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                    this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                    selectBtn.CheckedValue = true;
                }

                this.orderDistFlag = true;
                DistInfo distInfo = this.main.MmfMng.GetDistInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info));
                this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(distInfo.NetIdToString, 0, 0, 0, 255));
                this.ClearOrderKind();
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                DistInfo distInfo = this.main.MmfMng.GetDistInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info));
                this.lstSelectedOrderIP.Remove(NCasUtilityMng.INCasCommUtility.AddIpAddr(distInfo.NetIdToString, 0, 0, 0, 255));

                if (this.lstSelectedOrderIP.Count == 0)
                {
                    this.orderDistFlag = false;
                }
            }
        }

        /// <summary>
        /// 시군 또는 개별단말 (발령 대상) 버튼 마우스오른쪽 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyDistOneDestinationRight(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.allDestinationFlag == true || this.orderGroupFlag == true || this.orderDistFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = true;
                }
                else
                {
                    selectBtn.CheckedValue = false;
                }

                return;
            }

            this.distSelectListView.Items.Clear();

            foreach (TermInfo eachTermInfo in this.main.MmfMng.GetDistInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info)).LstTerms)
            {
                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.IpAddrToSring;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.DistInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.distSelectListView.Items.Add(lvi);
            }

            this.ShowDistDetailSelectForm(true);
        }
        #endregion

        #region 예비 버튼
        /// <summary>
        /// 예비 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyReady(NCasButton selectBtn)
        {

            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.AlarmStandby;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.AlarmStandby);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 경계 버튼
        /// <summary>
        /// 경계 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyWatch(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0 || this.orderStandbyFlag == false)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.AlarmWatch;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.AlarmWatch);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 공습 버튼
        /// <summary>
        /// 공습 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyAttack(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0 || this.orderStandbyFlag == false)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.AlarmAttack;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.AlarmAttack);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 화생방 버튼
        /// <summary>
        /// 화생방 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyBiological(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0 || this.orderStandbyFlag == false)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.AlarmBiochemist;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.AlarmBiochemist);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 재난위험 버튼
        /// <summary>
        /// 재난위험 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyDisasterWatch(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0 || this.orderStandbyFlag == false)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.DisasterWatch;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.DisasterWatch);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 재난경계(재해방송) 버튼
        /// <summary>
        /// 재난경계(재해방송) 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyDisasterBroad(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0 || this.orderStandbyFlag == false)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.DisasterBroadcast;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.DisasterBroadcast);
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
                this.SetOnButton(NCasKeyAction.MicOrder);
                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
            }
        }
        #endregion

        #region 해제 버튼
        /// <summary>
        /// 해제 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyClear(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || selectBtn.CheckedValue == false || this.lstSelectedOrderIP.Count == 0)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.selectedOrderKind = NCasDefineOrderKind.AlarmCancel;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOrderKindButtonColorOn(NCasDefineOrderKind.AlarmCancel);
                //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 선택취소 버튼
        /// <summary>
        /// 선택취소 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyCancel()
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (!(btnKeyData.KeyActioin == NCasKeyAction.Real || btnKeyData.KeyActioin == NCasKeyAction.Test ||
                    btnKeyData.KeyActioin == NCasKeyAction.Line || btnKeyData.KeyActioin == NCasKeyAction.Sate))
                {
                    if (ncasBtn.CheckedValue == true)
                    {
                        ncasBtn.CheckedValue = false;
                    }

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }

                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (!(btnKeyData.KeyActioin == NCasKeyAction.Real || btnKeyData.KeyActioin == NCasKeyAction.Test ||
                    btnKeyData.KeyActioin == NCasKeyAction.Line || btnKeyData.KeyActioin == NCasKeyAction.Sate))
                {
                    if (ncasBtn.CheckedValue == true)
                    {
                        ncasBtn.CheckedValue = false;
                    }

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }

                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }
                }
            }

            this.lstSelectedOrderIP.Clear();
            this.selectedOrderKind = NCasDefineOrderKind.None;
            //this.selectedDisasterBroadKind = DisasterBroadKind.None;구버전
            this.allDestinationFlag = false;
            this.orderGroupFlag = false;
            this.orderDistFlag = false;
            this.orderTermFlag = false;
        }
        #endregion

        #region 확인 버튼
        /// <summary>
        /// 확인 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyConfirm(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind == NCasDefineOrderKind.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }

                return;
            }

            this.Confirm();
            this.SetOrderDestinationButtonColorOff();

            if (this.allDestinationFlag)
            {
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOnButton(NCasKeyAction.ProveAllDestination);
                this.SetOnButton(NCasKeyAction.GroupDestination);
                this.SetOnButton(NCasKeyAction.DistOneDestination);
            }
            else
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if ((btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.GroupDestination
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.DistOneDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                        btn.CheckedValue = true;
                    }
                }
            }

            this.lastOrderKind = this.selectedOrderKind;
            //this.lastDisasterBroadKind = this.selectedDisasterBroadKind;구버전

            //if (this.selectedDisasterBroadKind == DisasterBroadKind.StroredMessage)구버전
            //{
            //    if (this.orderStoredViewForm != null)
            //    {
            //        this.orderStoredViewForm.StoredMsgFinishEvent -= new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
            //        this.orderStoredViewForm.Close();
            //        this.orderStoredViewForm = null;
            //    }

            //    this.orderStoredViewForm = new OrderStoredViewForm();
            //    this.orderStoredViewForm.StoredMsgFinishEvent += new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
            //    this.orderStoredViewForm.StartSirenForm(this.selectedStoredMessage.PlayTime);
            //    this.orderStoredViewForm.Show(this);
            //}
            //else
            //{
            //    if (this.orderStoredViewForm != null)
            //    {
            //        this.orderStoredViewForm.StoredMsgFinishEvent -= new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
            //        this.orderStoredViewForm.Close();
            //        this.orderStoredViewForm = null;
            //    }
            //}

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }
            else if (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel)
            {
                this.orderStandbyFlag = false;
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
                this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
            }

            this.ClearOrderKind();
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 확인 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyConfirmFromDual(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind == NCasDefineOrderKind.None)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }

                return;
            }

            this.SetOrderDestinationButtonColorOff();

            if (this.allDestinationFlag)
            {
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOnButton(NCasKeyAction.ProveAllDestination);
                this.SetOnButton(NCasKeyAction.GroupDestination);
                this.SetOnButton(NCasKeyAction.DistOneDestination);
            }
            else
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if ((btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.GroupDestination
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.DistOneDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                        btn.CheckedValue = true;
                    }
                }
            }

            this.lastOrderKind = this.selectedOrderKind;
            //this.lastDisasterBroadKind = this.selectedDisasterBroadKind;구버전

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }
            else if (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel)
            {
                this.orderStandbyFlag = false;
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
                this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
            }

            this.ClearOrderKind();
        }
        #endregion

        #region 재난경계 MIC 버튼
        /// <summary>
        /// 재난경계 MIC 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyMicOrder(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
            else
            {
                selectBtn.CheckedValue = true;
            }
        }
        #endregion

        #region 재난경계 TTS 버튼
        /// <summary>
        /// 재난경계 TTS 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyTtsOrder(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.Tts;구버전
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
            else
            {
                selectBtn.CheckedValue = true;
            }

            using (NCasTtsMsgForm form = new NCasTtsMsgForm())
            {
                form.SelectTtsMsgEvent += new EventHandler<SelectTtsMsgEventArgs>(form_SelectTtsMsgEvent);
                form.ShowDialog();
                form.SelectTtsMsgEvent -= new EventHandler<SelectTtsMsgEventArgs>(form_SelectTtsMsgEvent);
            }
        }

        /// <summary>
        /// TTS메시지 화면에서 발생하는 메시지 선택 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_SelectTtsMsgEvent(object sender, SelectTtsMsgEventArgs e)
        {
            if (e.SelectFlag) //TTS메시지를 선택했으면..
            {
                this.selectedTtsMessage = e.TtsMsg;
            }
            else
            {
                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
                this.SetOnButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }

        /// <summary>
        /// 듀얼시스템에서 수신받는 재난경계 TTS 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyTtsOrderFromDual(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.Tts;구버전
                this.SetOnButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
                this.SetOnButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 재난경계 저장메시지 버튼
        /// <summary>
        /// 재난경계 저장메시지 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyMsgOrder(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.StroredMessage;구버전
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
            }
            else
            {
                selectBtn.CheckedValue = true;
            }

            using (NCasStoredMsgForm form = new NCasStoredMsgForm())
            {
                form.SelectStoredMsgEvent += new EventHandler<SelectStoredMsgEventArgs>(form_SelectStoredMsgEvent);
                form.ShowDialog();
                form.SelectStoredMsgEvent -= new EventHandler<SelectStoredMsgEventArgs>(form_SelectStoredMsgEvent);
            }
        }

        /// <summary>
        /// 저장메시지 화면에서 발생하는 메시지 선택 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_SelectStoredMsgEvent(object sender, SelectStoredMsgEventArgs e)
        {
            if (e.SelectFlag) //저장메시지를 선택했으면..
            {
                this.selectedStoredMessage = e.StoredMsg;
                this.storedMessageRepeatCount = e.RepeatCount;
            }
            else
            {
                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
                this.SetOnButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }

        /// <summary>
        /// 듀얼시스템에서 수신받는 재난경계 저장메시지 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyMsgOrderFromDual(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.lstSelectedOrderIP.Count == 0 || this.selectedOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.StroredMessage;구버전
                this.SetOnButton(NCasKeyAction.MsgOrder);
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                //this.selectedDisasterBroadKind = DisasterBroadKind.Mic;구버전
                this.SetOnButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion
    }
}