using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
using NCasPBrdScreen;

namespace NCasPBrdScreenOld
{
    public partial class OrderView : ViewBase
    {
        #region element
        private NCasDefineOrderMode selectedOrderMode = NCasDefineOrderMode.None;
        private NCasDefineOrderMedia selectedOrderMedia = NCasDefineOrderMedia.None;
        private List<string> lstSelectedOrderIP = new List<string>();
        private List<string> lstSelectedOrderIpMegaloPolis = new List<string>();
        private NCasDefineOrderKind selectedOrderKind = NCasDefineOrderKind.None;
        private NCasDefineOrderKind lastOrderKind = NCasDefineOrderKind.None;
        private List<NCasButton> lstSelectedButtons = new List<NCasButton>();
        private NCasButton curButton = null;
        private NCasButton wrongOperationBtn = null;
        private NCasKeyAction orderSirenAction = NCasKeyAction.None;
        private NCasDefineCaption selectedTVCaptionMode = NCasDefineCaption.None;
        private NCasProtocolTc4 lastTc4 = null;
        private NCasProtocolTc20 lastTc20 = null;
        private List<string> lastLstSelectedOrderIP = null;
        private bool wrongOperationFlag = false;
        private bool tvCaptionSelectedFlag = false;
        private bool allDestinationFlag = false;
        private bool allMegaloPolisDestinationFlag = false;
        private bool broadShareFlag = false;
        private bool orderStandbyFlag = false;
        private bool lampTestFlag = false;
        private ProvInfo provInfo = null;
        private readonly string watchSiren = "경계 사이렌";
        private readonly string attackSiren = "공습 사이렌";
        private readonly string disasterSiren = "재난경계 사이렌";
        private readonly int watchSirenTime = 30;
        private readonly int attackSirenTime = 35;
        private readonly int disasterSirenTime = 50;
        private ImageList orderKindImageList = null;
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
            this.OrderBtnArrange();
            this.labelTotalTermCount.Text = (this.provInfo.GetUsableBroadTermCnt() + this.provInfo.GetUsableDeptTermCnt()).ToString();
            this.InitImageList();
            this.main.AddTimerMember(this);
        }

        public override void OnTimer()
        {
            this.SetOrderText();
            this.SetOrderResponseCount();
        }
        #endregion

        #region 버튼 배치
        /// <summary>
        /// 화면에 방송발령 버튼 초기화 및 배치
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
                    this.wrongOperationBtn.LstAnimation.ImageSize = new Size(NCasPBrdScreenRsc.btnAlertNormal.Width, NCasPBrdScreenRsc.btnAlertNormal.Height);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPBrdScreenRsc.btnAlertNormal);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPBrdScreenRsc.btnAlertError);
                    this.wrongOperationBtn.AnimationInterval = 500;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Cancel || keyData.KeyActioin == NCasKeyAction.Confirm //선택취소와 확인 버튼
                    || keyData.KeyActioin == NCasKeyAction.WatchSiren || keyData.KeyActioin == NCasKeyAction.AttackSiren //경계와 공습사이렌
                    || keyData.KeyActioin == NCasKeyAction.DisasterSiren || keyData.KeyActioin == NCasKeyAction.SirenOff //재해사이렌과 사이렌OFF 
                    || keyData.KeyActioin == NCasKeyAction.BroadFinish) //방송종료
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.UseCheck = false;
                }
                else
                {
                    this.curButton.ForeColor = Color.FromArgb(1, 2, 56);
                    this.curButton.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.curButton.LstAnimation.ImageSize = new Size(NCasPBrdScreenRsc.btnAlertNormal.Width, NCasPBrdScreenRsc.btnAlertNormal.Height);
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

            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertError);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertGray);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertGreen);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertNormal);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertOrange);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertPink);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertPupple);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertRed);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertSelected);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertWhite);
            this.orderKindImageList.Images.Add(NCasPBrdScreenRsc.btnAlertYellow);
        }
        #endregion

        #region 버튼 이벤트
        private void btnRC11_MouseDown(object sender, MouseEventArgs e)
        {
            NCasButton selectBtn = null;
            NCasKeyData keyData = null;
            bool isLocal = true;

            if (sender is KeyBizData)
            {
                isLocal = (sender as KeyBizData).IsLocal;
                selectBtn = this.GetNCasButton((sender as KeyBizData).KeyData.KeyActioin, (sender as KeyBizData).KeyData.Info);
                keyData = (sender as KeyBizData).KeyData;
                selectBtn.CheckedValue = ((keyData.KeyStatus == NCasKeyState.Check) ? true : false);
            }
            else if (sender is NCasButton)
            {
                selectBtn = sender as NCasButton;
                keyData = (NCasKeyData)selectBtn.Tag;

                if (selectBtn.CheckedValue == true)
                {
                    keyData.KeyStatus = NCasKeyState.Check;
                }
                else
                {
                    keyData.KeyStatus = NCasKeyState.UnCheck;
                }
            }

            if (selectBtn == null)
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
                            this.CheckKeyReal(selectBtn);
                        }
                        else
                        {
                            this.CheckKeyRealFromDual(selectBtn);
                        }
                    }
                    break;

                case NCasKeyAction.Test: //시험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyTest(selectBtn);
                    }
                    break;

                case NCasKeyAction.WrongOperation: //오조작
                    this.CheckKeyWrongOperation();
                    break;

                case NCasKeyAction.Line: //유선
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyLine(selectBtn);
                    }
                    break;

                case NCasKeyAction.Sate: //위성
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeySate(selectBtn);
                    }
                    break;

                case NCasKeyAction.ProveAllDestination: //시도 방송국 전체 (발령대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyProveAllDestination(selectBtn);
                    }
                    break;

                case NCasKeyAction.BroadOneDestination: //개별 방송국 (발령대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyBroadOneDestination(selectBtn);
                    }
                    break;

                case NCasKeyAction.BroadShare: //방송공유
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyBroadShare(selectBtn);
                    }
                    break;

                case NCasKeyAction.RealTvCaption: //실제 TV자막
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyRealTvCaption(selectBtn);
                    }
                    break;

                case NCasKeyAction.TestTvCaption: //시험 TV자막
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyTestTvCaption(selectBtn);
                    }
                    break;

                case NCasKeyAction.MegaloPolisAllDestination: //광역시 전체 (발령 대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyMegaloPolisAllDestination(selectBtn);
                    }
                    break;

                case NCasKeyAction.Ready: //예비
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyReady(selectBtn);
                    }
                    break;

                case NCasKeyAction.Watch: //경계
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyWatch(selectBtn);
                    }
                    break;

                case NCasKeyAction.Attack: //공습
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyAttack(selectBtn);
                    }
                    break;

                case NCasKeyAction.Biological: //화생방
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyBiological(selectBtn);
                    }
                    break;

                case NCasKeyAction.DisasterBroad: //재난경계(재해방송)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyDisasterBroad(selectBtn);
                    }
                    break;

                case NCasKeyAction.DisasterWatch: //재난위험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyDisasterWatch(selectBtn);
                    }
                    break;

                case NCasKeyAction.Clear: //해제
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyClear(selectBtn);
                    }
                    break;

                case NCasKeyAction.Cancel: //선택 취소
                    this.CheckKeyCancel();
                    break;

                case NCasKeyAction.LampTest: //램프 시험
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.CheckKeyLampTest(selectBtn);
                    }
                    break;

                case NCasKeyAction.Confirm: //확인
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyConfirm(selectBtn);
                        }
                        else
                        {
                            this.CheckKeyConfirmFromDual(selectBtn);
                        }
                    }
                    break;

                case NCasKeyAction.WatchSiren: //경계 사이렌
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyWatchSiren();
                        }
                        else
                        {
                            this.CheckKeyWatchSirenFromDual();
                        }
                    }
                    break;

                case NCasKeyAction.AttackSiren: //공습 사이렌
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyAttackSiren();
                        }
                        else
                        {
                            this.CheckKeyAttackSirenFromDual();
                        }
                    }
                    break;

                case NCasKeyAction.DisasterSiren: //재해 사이렌
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (isLocal == true)
                        {
                            this.CheckKeyDisasterSiren();
                        }
                        else
                        {
                            this.CheckKeyDisasterSirenFromDual();
                        }
                    }
                    break;

                case NCasKeyAction.SirenOff: //사이렌 OFF
                    this.CheckKeySirenOff();
                    break;

                case NCasKeyAction.BroadFinish: //방송종료
                    if (isLocal == true)
                    {
                        this.CheckKeyBroadFinish();
                    }
                    else
                    {
                        this.CheckKeyBroadFinishFromDual();
                    }
                    break;

                default:
                    break;
            }

            if (isLocal == true)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    return;

                //듀얼로 버튼 데이터 전송
                if (keyData.KeyActioin == NCasKeyAction.Real)
                {
                    keyData.KeyStatus = (selectBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
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

                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    allStatus.SetBtnStatus((btn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Select);
                }

                if (this.wrongOperationFlag)
                {
                    allStatus.SetBtnStatus((this.wrongOperationBtn.Tag as NCasKeyData).ID, NCasDefinePlcBtnStatus.Blink);
                }

                string sirenTitle = string.Empty;
                int playTime = 0;

                if (this.orderSirenAction == NCasKeyAction.WatchSiren)
                {
                    allStatus.SetBtnStatus(NCasDefineButtonCode.BtnRC26, NCasDefinePlcBtnStatus.Select);
                    sirenTitle = this.watchSiren;
                    playTime = this.watchSirenTime;
                }
                else if (this.orderSirenAction == NCasKeyAction.AttackSiren)
                {
                    allStatus.SetBtnStatus(NCasDefineButtonCode.BtnRC36, NCasDefinePlcBtnStatus.Select);
                    sirenTitle = this.attackSiren;
                    playTime = this.attackSirenTime;
                }
                else if (this.orderSirenAction == NCasKeyAction.DisasterSiren)
                {
                    allStatus.SetBtnStatus(NCasDefineButtonCode.BtnRC56, NCasDefinePlcBtnStatus.Select);
                    sirenTitle = this.disasterSiren;
                    playTime = this.disasterSirenTime;
                }

                NCasPlcProtocolFactory.MakeFrame(allStatus);
                this.main.SetPlcKeyData(allStatus);

                if (this.orderSirenAction != NCasKeyAction.None)
                {
                    using (OrderSirenViewForm orderSirenViewForm = new OrderSirenViewForm())
                    {
                        orderSirenViewForm.StartSirenForm(sirenTitle, playTime);
                        orderSirenViewForm.ShowDialog();
                        this.orderSirenAction = NCasKeyAction.None;
                        //사이렌 버튼의 normal상태를 전송하기 위해서 btnRC11_MouseDown 호출
                        this.btnRC11_MouseDown(this.wrongOperationBtn, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                    }
                }
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

            Console.WriteLine("선택한 광역시 발령대상 수 - " + lstSelectedOrderIpMegaloPolis.Count.ToString());
            foreach (string str in this.lstSelectedOrderIpMegaloPolis)
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

            Console.WriteLine("현재 상태 - " + ((this.wrongOperationFlag == true) ? "오조작 상태" : "정상 상태"));

            Console.WriteLine("시도 전체 선택 - " + ((this.allDestinationFlag == true) ? "시도 전체" : "시도 전체 아님"));

            Console.WriteLine("광역시 전체 선택 - " + ((this.allMegaloPolisDestinationFlag == true) ? "광역시 전체" : "광역시 전체 아님"));

            Console.WriteLine("방송공유 선택 - " + ((this.broadShareFlag == true) ? "방송공유" : "방송공유 아님"));

            Console.WriteLine("예비 발령 - " + ((this.orderStandbyFlag == true) ? "예비 상태" : "예비 아님"));

            Console.WriteLine("TV캡션 선택 - " + ((this.tvCaptionSelectedFlag == true) ? "TV자막 선택" : "TV자막 미선택"));

            Console.WriteLine("TV자막 모드 - " + ((this.selectedTVCaptionMode == NCasDefineCaption.None) ? "선택 안함" :
                (this.selectedTVCaptionMode == NCasDefineCaption.RealTvCaption) ? "실제" :
                (this.selectedTVCaptionMode == NCasDefineCaption.ExerciseTvCaption) ? "훈련" :
                (this.selectedTVCaptionMode == NCasDefineCaption.TestTvCaption) ? "시험" : "실패"));
            #endregion
        }
        #endregion

        #region 실제 버튼
        /// <summary>
        /// 실제 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyReal(NCasButton selectBtn)
        {
            if (!selectBtn.CheckedValue == true)
            {
                if (MessageBox.Show("실제 발령을 해제하겠습니까?", "실제 발령 해제", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.selectedOrderMode = NCasDefineOrderMode.None;
                }
                else
                {
                    if (selectBtn.CheckedValue == false)
                    {
                        selectBtn.CheckedValue = true;
                    }
                    return;
                }
            }
            else
            {
                using (OrderConfirmForm confirm = new OrderConfirmForm())
                {
                    confirm.PasswordConfirmEvent += new EventHandler(confirm_PasswordConfirmEvent);
                    confirm.ShowDialog();
                    confirm.PasswordConfirmEvent -= new EventHandler(confirm_PasswordConfirmEvent);
                }
            }

            if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if (btn.CheckedValue == true)
                    {
                        btn.CheckedValue = false;
                    }
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedButtons.Clear();
                this.lstSelectedOrderIP.Clear();
                this.lstSelectedOrderIpMegaloPolis.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.allMegaloPolisDestinationFlag = false;
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;
                this.broadShareFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);

                if (selectBtn.CheckedValue == false)
                {
                    selectBtn.CheckedValue = true;
                }

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

                    NCasPlcProtocolFactory.MakeFrame(allStatus);
                    this.main.SetPlcKeyData(allStatus);
                    this.lampTestFlag = false;
                }
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.TestMode)
            {
                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if (btn.CheckedValue == true)
                    {
                        btn.CheckedValue = false;
                    }
                }

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }

                if (this.wrongOperationFlag == true)
                {
                    this.CheckKeyWrongOperation();
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedButtons.Clear();
                this.lstSelectedOrderIP.Clear();
                this.lstSelectedOrderIpMegaloPolis.Clear();
                this.selectedOrderMode = NCasDefineOrderMode.None;
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.allMegaloPolisDestinationFlag = false;
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;
                this.broadShareFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);
            }
        }

        /// <summary>
        /// 비밀번호 확인 일치 이벤트
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
                    if (btn.CheckedValue == true)
                    {
                        btn.CheckedValue = false;
                    }
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedButtons.Clear();
                this.lstSelectedOrderIP.Clear();
                this.lstSelectedOrderIpMegaloPolis.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.allMegaloPolisDestinationFlag = false;
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;
                this.broadShareFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);

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
                    if (btn.CheckedValue == true)
                    {
                        btn.CheckedValue = false;
                    }
                }

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }

                if (this.wrongOperationFlag == true)
                {
                    this.CheckKeyWrongOperation();
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedButtons.Clear();
                this.lstSelectedOrderIP.Clear();
                this.lstSelectedOrderIpMegaloPolis.Clear();
                this.selectedOrderMode = NCasDefineOrderMode.None;
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.allMegaloPolisDestinationFlag = false;
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;
                this.broadShareFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);
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
            if (!selectBtn.CheckedValue == true)
            {
                this.selectedOrderMode = NCasDefineOrderMode.None;

                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if (btn.CheckedValue == true)
                    {
                        btn.CheckedValue = false;
                    }
                }

                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedButtons.Clear();
                this.selectedOrderMedia = NCasDefineOrderMedia.None;
                this.lstSelectedOrderIP.Clear();
                this.lstSelectedOrderIpMegaloPolis.Clear();
                this.selectedOrderMode = NCasDefineOrderMode.None;
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.allDestinationFlag = false;
                this.allMegaloPolisDestinationFlag = false;
                this.wrongOperationFlag = false;
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;
                this.broadShareFlag = false;
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);
            }
            else
            {
                if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
                {
                    NCasAnimator.AddItem(this.wrongOperationBtn);
                    this.wrongOperationFlag = true;

                    if (selectBtn.CheckedValue == true)
                    {
                        selectBtn.CheckedValue = false;
                    }
                }
                else
                {
                    this.selectedOrderMode = NCasDefineOrderMode.TestMode;

                    if (selectBtn.CheckedValue == false)
                    {
                        selectBtn.CheckedValue = true;
                    }

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

                        NCasPlcProtocolFactory.MakeFrame(allStatus);
                        this.main.SetPlcKeyData(allStatus);
                        this.lampTestFlag = false;
                    }
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

        #region 시도 방송국 전체 (발령대상) 버튼
        /// <summary>
        /// 시도 방송국 전체 (발령대상) 버튼 동작
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

                if (this.selectedOrderKind != NCasDefineOrderKind.None) //대상지역을 처음부터 다시 셋팅
                {
                    this.SetOnButton(NCasKeyAction.MegaloPolisAllDestination);
                    this.lstSelectedOrderIpMegaloPolis.Clear();
                    this.SetOffButton(NCasKeyAction.MegaloPolisAllDestination);
                    this.allMegaloPolisDestinationFlag = false;
                }

                this.allDestinationFlag = true;

                if (!this.lstSelectedOrderIP.Contains(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4())))
                {
                    this.lstSelectedOrderIP.Clear();
                    this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4()));
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.BroadOneDestination);
                this.ClearOrderKind();
            }
            else
            {
                this.lstSelectedOrderIP.Clear();
                this.allDestinationFlag = false;

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.SetOffButton(NCasKeyAction.BroadOneDestination);
                this.ClearOrderKind();
                this.SetOrderDestinationButtonColorOff();
            }
        }
        #endregion

        #region 개별 방송국 (발령대상) 버튼
        /// <summary>
        /// 개별 방송국 (발령대상) 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyBroadOneDestination(NCasButton selectBtn)
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

            PBroadInfo pBroadInfo = this.main.MmfMng.GetProvInfoBroadInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info));

            if (selectBtn.CheckedValue == true)
            {
                this.SetOrderDestinationButtonColorOff();

                if (this.selectedOrderKind != NCasDefineOrderKind.None) //대상지역을 처음부터 다시 셋팅
                {
                    this.SetOnButton(NCasKeyAction.BroadOneDestination);
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButton(NCasKeyAction.BroadOneDestination);
                }

                if (!this.lstSelectedOrderIP.Contains(pBroadInfo.IpAddrToString))
                {
                    this.lstSelectedOrderIP.Add(pBroadInfo.IpAddrToString);
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.ClearOrderKind();

                if (selectBtn.CheckedValue == false)
                {
                    selectBtn.CheckedValue = true;
                }
            }
            else
            {
                if (this.lstSelectedOrderIP.Contains(pBroadInfo.IpAddrToString))
                {
                    this.lstSelectedOrderIP.Remove(pBroadInfo.IpAddrToString);
                }

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.ClearOrderKind();
                this.SetOrderDestinationButtonColorOff();
            }
        }
        #endregion

        #region 방송공유 버튼
        /// <summary>
        /// 방송공유 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyBroadShare(NCasButton selectBtn)
        {
            if (this.provInfo.IsBroadShareRequest == true) //시도
            {
                this.wrongOperationFlag = true;
            }
            else //광역시
            {
                NCasKeyData keyData = new NCasKeyData();
                keyData.KeyActioin = NCasKeyAction.BroadShare;

                if (selectBtn.CheckedValue)
                {
                    keyData.KeyStatus = NCasKeyState.Check;
                    this.broadShareFlag = true;

                    if (!this.lstSelectedButtons.Contains(selectBtn))
                    {
                        this.lstSelectedButtons.Add(selectBtn);
                    }
                }
                else
                {
                    keyData.KeyStatus = NCasKeyState.UnCheck;
                    this.broadShareFlag = false;

                    if (this.lstSelectedButtons.Contains(selectBtn))
                    {
                        this.lstSelectedButtons.Remove(selectBtn);
                    }
                }

                this.main.SendBroadShareKeyData(keyData);
            }

            if (this.wrongOperationFlag)
            {
                if (selectBtn.CheckedValue == true)
                {
                    selectBtn.CheckedValue = false;
                }
                else
                {
                    selectBtn.CheckedValue = true;
                }

                NCasAnimator.AddItem(this.wrongOperationBtn);
            }
        }
        #endregion

        #region 실제 TV자막 버튼
        /// <summary>
        /// 실제 TV자막 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyRealTvCaption(NCasButton selectBtn)
        {
            if (this.selectedOrderMode != NCasDefineOrderMode.RealMode || this.selectedOrderMedia == NCasDefineOrderMedia.None)
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

            if (selectBtn.CheckedValue == true)
            {
                this.tvCaptionSelectedFlag = true;
                this.selectedTVCaptionMode = NCasDefineCaption.RealTvCaption;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOffButton(NCasKeyAction.TestTvCaption);
            }
            else
            {
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }
            }
        }
        #endregion

        #region 시험 TV자막 버튼
        /// <summary>
        /// 시험 TV자막 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyTestTvCaption(NCasButton selectBtn)
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

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.tvCaptionSelectedFlag = true;
                this.selectedTVCaptionMode = NCasDefineCaption.ExerciseTvCaption;

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOffButton(NCasKeyAction.RealTvCaption);
            }
            else
            {
                this.tvCaptionSelectedFlag = false;
                this.selectedTVCaptionMode = NCasDefineCaption.None;

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }
            }
        }
        #endregion

        #region 광역시 전체 (발령 대상) 버튼
        /// <summary>
        /// 광역시 전체 (발령 대상) 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyMegaloPolisAllDestination(NCasButton selectBtn)
        {
            if (this.broadShareFlag == false)
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

                if (this.selectedOrderKind != NCasDefineOrderKind.None) //대상지역을 처음부터 다시 셋팅
                {
                    this.SetOnButton(NCasKeyAction.ProveAllDestination);
                    this.SetOnButton(NCasKeyAction.BroadOneDestination);
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButton(NCasKeyAction.ProveAllDestination);
                    this.SetOffButton(NCasKeyAction.BroadOneDestination);
                    this.allDestinationFlag = false;
                }

                this.allMegaloPolisDestinationFlag = true;

                if (!this.lstSelectedOrderIpMegaloPolis.Contains(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(this.provInfo.LstBroadRelatedProvInfo[0].NetIdToString)))
                {
                    this.lstSelectedOrderIpMegaloPolis.Add(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(this.provInfo.LstBroadRelatedProvInfo[0].NetIdToString));
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.MegaloPolisOneDestination);
                this.ClearOrderKind();
            }
            else
            {
                this.allMegaloPolisDestinationFlag = false;

                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                if (this.lstSelectedOrderIpMegaloPolis.Contains(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(this.provInfo.LstBroadRelatedProvInfo[0].NetIdToString)))
                {
                    this.lstSelectedOrderIpMegaloPolis.Remove(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(this.provInfo.LstBroadRelatedProvInfo[0].NetIdToString));
                }

                this.SetOffButton(NCasKeyAction.MegaloPolisOneDestination);
                this.ClearOrderKind();
                this.SetOrderDestinationButtonColorOff();
            }
        }
        #endregion

        #region 개별 광역시 (발령 대상) 버튼 - 버튼 없애기로 함 (2015/04/20)
        /// <summary>
        /// 개별 광역시 (발령 대상) 버튼 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyMegaloPolisOneDestination(NCasButton selectBtn)
        {
        }
        #endregion

        #region 예비 버튼
        /// <summary>
        /// 예비 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyReady(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0))
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
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
                this.SetOffButton(NCasKeyAction.Clear);
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.Clear);
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
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None || selectBtn.CheckedValue == false ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.orderStandbyFlag == false)
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
                this.SetOffButton(NCasKeyAction.Ready);
                this.SetOffButton(NCasKeyAction.Attack);
                this.SetOffButton(NCasKeyAction.Watch);
                this.SetOffButton(NCasKeyAction.Biological);
                this.SetOffButton(NCasKeyAction.DisasterBroad);
                this.SetOffButton(NCasKeyAction.DisasterWatch);
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
                    btnKeyData.KeyActioin == NCasKeyAction.Line || btnKeyData.KeyActioin == NCasKeyAction.Sate ||
                    btnKeyData.KeyActioin == NCasKeyAction.RealTvCaption || btnKeyData.KeyActioin == NCasKeyAction.TestTvCaption ||
                    btnKeyData.KeyActioin == NCasKeyAction.BroadShare))
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
            this.lstSelectedOrderIpMegaloPolis.Clear();
            this.selectedOrderKind = NCasDefineOrderKind.None;
            this.allDestinationFlag = false;
            this.allMegaloPolisDestinationFlag = false;
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

                NCasPlcProtocolFactory.MakeFrame(allStatus);
                this.main.SetPlcKeyData(allStatus);
                this.lampTestFlag = false;
            }
        }
        #endregion

        #region 확인 버튼
        /// <summary>
        /// 확인 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyConfirm(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.selectedOrderKind == NCasDefineOrderKind.None)
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
            this.lastOrderKind = this.selectedOrderKind;

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }

            this.SetOrderDestinationButtonColorOff();

            if (this.allDestinationFlag)
            {
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
            }
            else
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if ((btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.BroadOneDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                    }
                }
            }

            if (this.allMegaloPolisDestinationFlag)
            {
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);
            }

            this.lstSelectedOrderIP.Clear();
            this.lstSelectedOrderIpMegaloPolis.Clear();
            this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
            this.SetOffButtonAny(NCasKeyAction.BroadOneDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisAllDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisOneDestination);
            this.allDestinationFlag = false;
            this.allMegaloPolisDestinationFlag = false;
            this.ClearOrderKind();
        }

        /// <summary>
        /// 듀얼시스템에서 수신받는 확인 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyConfirmFromDual(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None ||
                (this.lstSelectedOrderIP.Count == 0 && this.lstSelectedOrderIpMegaloPolis.Count == 0) || this.selectedOrderKind == NCasDefineOrderKind.None)
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

            this.ConfirmFromDual();
            this.lastOrderKind = this.selectedOrderKind;

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }

            this.SetOrderDestinationButtonColorOff();

            if (this.allDestinationFlag)
            {
                this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
            }
            else
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    if ((btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.BroadOneDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                    }
                }
            }

            if (this.allMegaloPolisDestinationFlag)
            {
                this.SetOrderKindButtonColorMegaAll(this.selectedOrderKind);
            }

            this.lstSelectedOrderIP.Clear();
            this.lstSelectedOrderIpMegaloPolis.Clear();
            this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
            this.SetOffButtonAny(NCasKeyAction.BroadOneDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisAllDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisOneDestination);
            this.allDestinationFlag = false;
            this.allMegaloPolisDestinationFlag = false;
            this.ClearOrderKind();
        }
        #endregion

        #region 경계 사이렌 버튼
        /// <summary>
        /// 경계 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyWatchSiren()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.AlarmWatch)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }

            this.orderSirenAction = NCasKeyAction.WatchSiren;
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 경계 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyWatchSirenFromDual()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.AlarmWatch)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }
        }
        #endregion

        #region 공습 사이렌 버튼
        /// <summary>
        /// 공습 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyAttackSiren()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.AlarmAttack)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }

            this.orderSirenAction = NCasKeyAction.AttackSiren;
        }

        /// <summary>
        /// 듀얼시스템에서 받은 공습 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyAttackSirenFromDual()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.AlarmAttack)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }
        }
        #endregion

        #region 재해 사이렌 버튼
        /// <summary>
        /// 재해 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyDisasterSiren()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }

            this.orderSirenAction = NCasKeyAction.DisasterSiren;
        }

        /// <summary>
        /// 듀얼시스템에서 받은 재해 사이렌 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyDisasterSirenFromDual()
        {
            if (this.lastOrderKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);
                return;
            }
        }
        #endregion

        #region 사이렌 OFF 버튼 - 사용되지 않음
        /// <summary>
        /// 사이렌 OFF 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeySirenOff()
        {
        }
        #endregion

        #region 방송종료 버튼
        /// <summary>
        /// 방송종료 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyBroadFinish()
        {
            if (this.lastTc4 != null)
            {
                DateTime orderTime = DateTime.Now;

                if (this.lastLstSelectedOrderIP != null)
                {
                    foreach (string ipAddr in this.lastLstSelectedOrderIP)
                    {
                        NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(this.lastTc4.GetDatas());
                        NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                        protoTc4.BroadNetIdOrIpByString = ipAddr;
                        protoTc4.OrderTimeByDateTime = orderTime;
                        protoTc4.AlarmKind = NCasDefineOrderKind.AlarmClose;
                        NCasProtocolFactory.MakeUdpFrame(protoTc4);
                        this.main.SetOrderBizData(protoTc4);

                        //TV자막 발령했었다면..
                        if (this.lastTc20 != null)
                        {
                            TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.Code, this.lastTc20.Mode, NCasDefineOrderKind.AlarmClose);
                            NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.ParseFrame(this.lastTc20.GetDatas());
                            NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                            proto20.DisplayIpByString = ipAddr;
                            proto20.DisplayText = captionData.TvText;
                            proto20.Kind = NCasDefineOrderKind.AlarmClose;
                            proto20.OrderTimeByDateTime = orderTime;
                            proto20.RepeatNum = (byte)captionData.RepeatCount;
                            NCasProtocolFactory.MakeUdpFrame(proto20);
                            this.main.SetTVCaptionData(proto20);
                        }
                    }

                    System.Threading.Thread.Sleep(500);

                    foreach (string ipAddr in this.lastLstSelectedOrderIP)
                    {
                        NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(this.lastTc4.GetDatas());
                        NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                        protoTc4.BroadNetIdOrIpByString = ipAddr;
                        protoTc4.OrderTimeByDateTime = orderTime;
                        protoTc4.AlarmKind = NCasDefineOrderKind.AlarmClose;
                        NCasProtocolFactory.MakeUdpFrame(protoTc4);
                        this.main.SetOrderBizData(protoTc4);

                        //TV자막 발령했었다면..
                        if (this.lastTc20 != null)
                        {
                            TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.Code, this.lastTc20.Mode, NCasDefineOrderKind.AlarmClose);
                            NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.ParseFrame(this.lastTc20.GetDatas());
                            NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                            proto20.DisplayIpByString = ipAddr;
                            proto20.DisplayText = captionData.TvText;
                            proto20.Kind = NCasDefineOrderKind.AlarmClose;
                            proto20.OrderTimeByDateTime = orderTime;
                            proto20.RepeatNum = (byte)captionData.RepeatCount;
                            NCasProtocolFactory.MakeUdpFrame(proto20);
                            this.main.SetTVCaptionData(proto20);
                        }
                    }
                }
            }

            this.lastTc4 = null;
            this.lastTc20 = null;
            this.lastLstSelectedOrderIP = null;
            this.orderStandbyFlag = false;
            this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
            this.SetOffButtonAny(NCasKeyAction.BroadOneDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisAllDestination);
            this.lstSelectedOrderIP.Clear();
            this.lstSelectedOrderIpMegaloPolis.Clear();
            this.allDestinationFlag = false;
            this.allMegaloPolisDestinationFlag = false;
            this.ClearOrderKind();
            this.SetOrderKindButtonColorProvAll(NCasDefineOrderKind.None);
            this.SetOrderKindButtonColorMegaAll(NCasDefineOrderKind.None);
        }

        /// <summary>
        /// 듀얼시스템에서 수신받는 방송종료 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyBroadFinishFromDual()
        {
            this.lastTc4 = null;
            this.lastTc20 = null;
            this.lastLstSelectedOrderIP = null;
            this.orderStandbyFlag = false;
            this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
            this.SetOffButtonAny(NCasKeyAction.BroadOneDestination);
            this.SetOffButtonAny(NCasKeyAction.MegaloPolisAllDestination);
            this.lstSelectedOrderIP.Clear();
            this.lstSelectedOrderIpMegaloPolis.Clear();
            this.allDestinationFlag = false;
            this.allMegaloPolisDestinationFlag = false;
            this.ClearOrderKind();
            this.SetOrderKindButtonColorProvAll(NCasDefineOrderKind.None);
            this.SetOrderKindButtonColorMegaAll(NCasDefineOrderKind.None);
        }
        #endregion

        #region private method
        /// <summary>
        /// 확인 버튼에 대한 처리 메소드
        /// </summary>
        private void Confirm()
        {
            DateTime orderTime = DateTime.Now;

            foreach (string ipAddr in this.lstSelectedOrderIP) //시도 발령대상으로 전송
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadOrder);
                NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                protoTc4.AlarmNetIdOrIpByString = this.provInfo.Code.ToString();
                protoTc4.BroadNetIdOrIpByString = ipAddr;
                protoTc4.OrderTimeByDateTime = orderTime;
                protoTc4.CtrlKind = NCasDefineControlKind.ControlBraod;
                protoTc4.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc4.Mode = this.selectedOrderMode;
                protoTc4.Media = this.selectedOrderMedia;
                protoTc4.AlarmKind = this.selectedOrderKind;
                protoTc4.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc4.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                protoTc4.Sector = (this.allDestinationFlag == true) ? NCasDefineSectionCode.SectionProv : NCasDefineSectionCode.SectionBroad;
                NCasProtocolFactory.MakeUdpFrame(protoTc4);
                this.main.SetOrderBizData(protoTc4);

                //마지막 발령을 저장하기 위한 작업. 반복 저장되어도 필요한 정보는 변경되지 않으므로 관계없음.
                this.lastTc4 = protoTc4;
                this.lastLstSelectedOrderIP = new List<string>();

                foreach (string ip in this.lstSelectedOrderIP)
                {
                    this.lastLstSelectedOrderIP.Add(ip);
                }

                //TV자막 선택한 경우
                if (this.tvCaptionSelectedFlag &&
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch || this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist || this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmClose))
                {
                    TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.Code, this.selectedTVCaptionMode, this.selectedOrderKind);
                    NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadCaption);
                    NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                    proto20.DisplayIpByString = ipAddr;
                    proto20.DisplayText = captionData.TvText;
                    proto20.Kind = this.selectedOrderKind;
                    proto20.Media = this.selectedOrderMedia;
                    proto20.Mode = this.selectedTVCaptionMode;
                    proto20.OrderTimeByDateTime = orderTime;
                    proto20.RepeatNum = (byte)captionData.RepeatCount;
                    proto20.Sector = (this.allDestinationFlag == true) ? NCasDefineSectionCode.SectionProv : NCasDefineSectionCode.SectionBroad;
                    proto20.Source = NCasDefineOrderSource.ProvCtrlRoom;
                    NCasProtocolFactory.MakeUdpFrame(proto20);
                    this.main.SetTVCaptionData(proto20);

                    //마지막 TV자막을 저장하기 위한 작업. 반복 저장되어도 필요한 정보는 변경되지 않으므로 관계 없음.
                    this.lastTc20 = proto20;
                }
            }

            foreach (string ipAddr in this.lstSelectedOrderIpMegaloPolis) //광역시 발령대상으로 전송
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadOrder);
                NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                protoTc4.AlarmNetIdOrIpByString = this.provInfo.Code.ToString();
                protoTc4.BroadNetIdOrIpByString = ipAddr;
                protoTc4.OrderTimeByDateTime = orderTime;
                protoTc4.CtrlKind = NCasDefineControlKind.ControlBraod;
                protoTc4.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc4.Mode = this.selectedOrderMode;
                protoTc4.Media = this.selectedOrderMedia;
                protoTc4.AlarmKind = this.selectedOrderKind;
                protoTc4.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc4.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                protoTc4.Sector = NCasDefineSectionCode.SectionBroadShare;
                NCasProtocolFactory.MakeUdpFrame(protoTc4);
                this.main.SetOrderBizData(protoTc4);

                //TV자막 선택한 경우
                if (this.tvCaptionSelectedFlag &&
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch || this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist || this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel))
                {
                    TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.LstBroadRelatedProvInfo[0].Code, this.selectedTVCaptionMode, this.selectedOrderKind);
                    NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadCaption);
                    NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                    proto20.DisplayIpByString = ipAddr;
                    proto20.DisplayText = captionData.TvText;
                    proto20.Kind = this.selectedOrderKind;
                    proto20.Media = this.selectedOrderMedia;
                    proto20.Mode = this.selectedTVCaptionMode;
                    proto20.OrderTimeByDateTime = orderTime;
                    proto20.RepeatNum = (byte)captionData.RepeatCount;
                    proto20.Sector = NCasDefineSectionCode.SectionBroadShare;
                    proto20.Source = NCasDefineOrderSource.ProvCtrlRoom;
                    NCasProtocolFactory.MakeUdpFrame(proto20);
                    this.main.SetTVCaptionData(proto20);
                }
            }
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 확인 버튼에 대한 처리 메소드
        /// </summary>
        private void ConfirmFromDual()
        {
            DateTime orderTime = DateTime.Now;

            foreach (string ipAddr in this.lstSelectedOrderIP) //시도 발령대상으로 전송
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadOrder);
                NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                protoTc4.AlarmNetIdOrIpByString = this.provInfo.Code.ToString();
                protoTc4.BroadNetIdOrIpByString = ipAddr;
                protoTc4.OrderTimeByDateTime = orderTime;
                protoTc4.CtrlKind = NCasDefineControlKind.ControlBraod;
                protoTc4.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc4.Mode = this.selectedOrderMode;
                protoTc4.Media = this.selectedOrderMedia;
                protoTc4.AlarmKind = this.selectedOrderKind;
                protoTc4.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc4.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                protoTc4.Sector = (this.allDestinationFlag == true) ? NCasDefineSectionCode.SectionProv : NCasDefineSectionCode.SectionBroad;
                NCasProtocolFactory.MakeUdpFrame(protoTc4);

                //마지막 발령을 저장하기 위한 작업. 반복 저장되어도 필요한 정보는 변경되지 않으므로 관계없음.
                this.lastTc4 = protoTc4;
                this.lastLstSelectedOrderIP = new List<string>();

                foreach (string ip in this.lstSelectedOrderIP)
                {
                    this.lastLstSelectedOrderIP.Add(ip);
                }

                //TV자막 선택한 경우
                if (this.tvCaptionSelectedFlag &&
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch || this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist || this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmClose))
                {
                    TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.Code, this.selectedTVCaptionMode, this.selectedOrderKind);
                    NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadCaption);
                    NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                    proto20.DisplayIpByString = ipAddr;
                    proto20.DisplayText = captionData.TvText;
                    proto20.Kind = this.selectedOrderKind;
                    proto20.Media = this.selectedOrderMedia;
                    proto20.Mode = this.selectedTVCaptionMode;
                    proto20.OrderTimeByDateTime = orderTime;
                    proto20.RepeatNum = (byte)captionData.RepeatCount;
                    proto20.Sector = (this.allDestinationFlag == true) ? NCasDefineSectionCode.SectionProv : NCasDefineSectionCode.SectionBroad;
                    proto20.Source = NCasDefineOrderSource.ProvCtrlRoom;
                    NCasProtocolFactory.MakeUdpFrame(proto20);

                    //마지막 TV자막을 저장하기 위한 작업. 반복 저장되어도 필요한 정보는 변경되지 않으므로 관계 없음.
                    this.lastTc20 = proto20;
                }
            }

            foreach (string ipAddr in this.lstSelectedOrderIpMegaloPolis) //광역시 발령대상으로 전송
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadOrder);
                NCasProtocolTc4 protoTc4 = protoBase as NCasProtocolTc4;
                protoTc4.AlarmNetIdOrIpByString = this.provInfo.Code.ToString();
                protoTc4.BroadNetIdOrIpByString = ipAddr;
                protoTc4.OrderTimeByDateTime = orderTime;
                protoTc4.CtrlKind = NCasDefineControlKind.ControlBraod;
                protoTc4.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc4.Mode = this.selectedOrderMode;
                protoTc4.Media = this.selectedOrderMedia;
                protoTc4.AlarmKind = this.selectedOrderKind;
                protoTc4.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc4.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                protoTc4.Sector = NCasDefineSectionCode.SectionBroadShare;
                NCasProtocolFactory.MakeUdpFrame(protoTc4);

                //TV자막 선택한 경우
                if (this.tvCaptionSelectedFlag &&
                    (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch || this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack ||
                    this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist || this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel))
                {
                    TVCaptionData captionData = TVCaptionContentMng.GetTvCaptionData(this.provInfo.LstBroadRelatedProvInfo[0].Code, this.selectedTVCaptionMode, this.selectedOrderKind);
                    NCasProtocolBase tvCaptionProtocol = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadCaption);
                    NCasProtocolTc20 proto20 = tvCaptionProtocol as NCasProtocolTc20;
                    proto20.DisplayIpByString = ipAddr;
                    proto20.DisplayText = captionData.TvText;
                    proto20.Kind = this.selectedOrderKind;
                    proto20.Media = this.selectedOrderMedia;
                    proto20.Mode = this.selectedTVCaptionMode;
                    proto20.OrderTimeByDateTime = orderTime;
                    proto20.RepeatNum = (byte)captionData.RepeatCount;
                    proto20.Sector = NCasDefineSectionCode.SectionBroadShare;
                    proto20.Source = NCasDefineOrderSource.ProvCtrlRoom;
                    NCasProtocolFactory.MakeUdpFrame(proto20);
                }
            }
        }

        /// <summary>
        /// NCasKeyAction을 받아 해당하는 NCasButton을 반환한다.
        /// </summary>
        /// <param name="keyAction"></param>
        /// <returns></returns>
        private NCasButton GetNCasButton(NCasKeyAction keyAction, string info)
        {
            NCasButton btn = null;

            if (keyAction == NCasKeyAction.BroadOneDestination)
            {
                foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
                {
                    NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                    if (btnKeyData.KeyActioin == keyAction)
                    {
                        if (btnKeyData.Info == info.Replace("\0", ""))
                        {
                            btn = ncasBtn;
                            break;
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
                        btn = ncasBtn;
                        break;
                    }
                }
            }

            return btn;
        }

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
        }

        /// <summary>
        /// 방송발령에 대한 정보를 화면 상단에 표시한다.
        /// </summary>
        private void SetOrderText()
        {
            this.orderTextBoard.ClearTextBlock();

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem3(this.provInfo.BroadOrderInfo.OccurTimeToDateTime) +
                " [" + this.provInfo.Name + "]에서 [" +
                NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(this.provInfo.BroadOrderInfo.Media) + "]으로 ", Color.FromArgb(3, 41, 87)));

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(this.provInfo.BroadOrderInfo.Mode) + "] ",
                (this.provInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? Color.FromArgb(232, 82, 53) : Color.FromArgb(6, 147, 6)));

            this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(this.provInfo.BroadOrderInfo.Kind) + "] 경보를 발령했습니다.", Color.FromArgb(3, 41, 87)));
        }

        /// <summary>
        /// 방송발령에 대한 응답 카운트를 화면에 표시한다.
        /// </summary>
        private void SetOrderResponseCount()
        {
            this.labelResponseTermCount.Text = (this.provInfo.BroadRespCnt + this.provInfo.DeptRespCnt).ToString();
            this.labelErrorTermCount.Text = this.provInfo.FaultBroadResponseCnt.ToString();
        }

        /// <summary>
        /// 발령종류 버튼을 모두 해제한다.
        /// </summary>
        private void ClearOrderKind()
        {
            this.selectedOrderKind = NCasDefineOrderKind.None;
            this.SetOffButton(NCasKeyAction.Ready);
            this.SetOffButton(NCasKeyAction.Attack);
            this.SetOffButton(NCasKeyAction.Watch);
            this.SetOffButton(NCasKeyAction.Biological);
            this.SetOffButton(NCasKeyAction.DisasterBroad);
            this.SetOffButton(NCasKeyAction.DisasterWatch);
            this.SetOffButton(NCasKeyAction.Clear);
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

                if (keyData.KeyActioin == NCasKeyAction.ProveAllDestination || keyData.KeyActioin == NCasKeyAction.BroadOneDestination ||
                    keyData.KeyActioin == NCasKeyAction.MegaloPolisAllDestination || keyData.KeyActioin == NCasKeyAction.MegaloPolisOneDestination)
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

        /// <summary>
        /// 발령대상의 버튼의 깜박임을 해제한다.
        /// </summary>
        private void SetOrderDestinationButtonColorOff()
        {
            foreach (NCasButton btn in this.lstSelectedButtons)
            {
                NCasKeyData keyData = (NCasKeyData)btn.Tag;

                if (keyData.KeyActioin == NCasKeyAction.ProveAllDestination || keyData.KeyActioin == NCasKeyAction.BroadOneDestination ||
                    keyData.KeyActioin == NCasKeyAction.MegaloPolisAllDestination || keyData.KeyActioin == NCasKeyAction.MegaloPolisOneDestination)
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

        /// <summary>
        /// 시도의 모든 발령대상 버튼 Normal 색을 셋팅한다.
        /// </summary>
        /// <param name="orderKind"></param>
        private void SetOrderKindButtonColorProvAll(NCasDefineOrderKind orderKind)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.ProveAllDestination || btnKeyData.KeyActioin == NCasKeyAction.BroadOneDestination)
                {
                    ncasBtn.CheckedValue = false;
                    this.SetOrderKindButtonColor(ncasBtn, orderKind);
                }
            }
        }

        /// <summary>
        /// 광역시의 모든 발령대상 버튼 Normal 색을 셋팅한다.
        /// </summary>
        /// <param name="orderKind"></param>
        private void SetOrderKindButtonColorMegaAll(NCasDefineOrderKind orderKind)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.MegaloPolisAllDestination || btnKeyData.KeyActioin == NCasKeyAction.MegaloPolisOneDestination)
                {
                    ncasBtn.CheckedValue = false;
                    this.SetOrderKindButtonColor(ncasBtn, orderKind);
                }
            }
        }
        #endregion

        #region public method
        /// <summary>
        /// PLC로부터 데이터를 수신받아 화면에 반영한다.
        /// </summary>
        /// <param name="plcResponse"></param>
        public void SetKeyPlc(NCasPlcProtocolReqStatusResponse plcResponse)
        {
        }

        /// <summary>
        /// 방송공유 버튼을 화면에 반영한다.
        /// </summary>
        /// <param name="check"></param>
        public void SetKeyBroadShare(bool check)
        {
            if (check)
            {
                this.SetOnButton(NCasKeyAction.BroadShare);
                this.broadShareFlag = true;
            }
            else
            {
                this.SetOffButtonAny(NCasKeyAction.BroadShare);
                this.broadShareFlag = false;
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
        #endregion
    }
}