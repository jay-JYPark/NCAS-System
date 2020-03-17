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
using NCasContentsModule.StoMsg;
using NCasContentsModule.TTS;

namespace NCasPAlmScreen
{
    public partial class PAlmScreenUIController : ViewBase
    {
        #region enum
        public enum DisasterBroadKind
        {
            None = 0,
            Mic = 1,
            Tts = 2,
            StroredMessage = 3
        }

        public enum OrderDataSendStatus
        {
            None = 0,
            First = 1,
            End = 2,
            FirstEnd = 3
        }
        #endregion

        #region element
        private NCasDefineOrderMode selectedOrderMode = NCasDefineOrderMode.None;
        private NCasDefineOrderMedia selectedOrderMedia = NCasDefineOrderMedia.None;
        private List<string> lstSelectedOrderIP = new List<string>();
        private NCasDefineOrderKind selectedOrderKind = NCasDefineOrderKind.None;
        private NCasDefineOrderKind lastOrderKind = NCasDefineOrderKind.None;
        private List<NCasButton> lstSelectedButtons = new List<NCasButton>();
        private DisasterBroadKind selectedDisasterBroadKind = DisasterBroadKind.None;
        private DisasterBroadKind lastDisasterBroadKind = DisasterBroadKind.None;
        private List<GroupContent> selectedGroupContents = new List<GroupContent>();
        private StoredMessageText selectedStoredMessage = new StoredMessageText();
        private int storedMessageRepeatCount = 1;
        private TtsMessageText selectedTtsMessage = new TtsMessageText();
        private List<string> groupNameLst = new List<string>();
        private int selectedDistCode = 0;
        private bool orderStandbyFlag = false;
        private bool allDestinationFlag = false;
        private bool orderGroupFlag = false;
        private bool orderDistFlag = false;
        private bool orderTermFlag = false;
        private bool orderAllDeptFlag = false;
        private bool orderDeptFlag = false;
        private bool lampTestFlag = false;
        private bool wrongOperationFlag = false;
        private bool realButtonFromPlc = false;
        private ProvInfo provInfo = null;
        private ImageList orderKindImageList = null;
        private ImageList orderKindDistImageList = null;
        private NCasButton curButton = null;
        private NCasButton wrongOperationBtn = null;
        private OrderStoredViewForm orderStoredViewForm = null;
        private readonly string WrongOperationIP = "0.0.0.0";
        private List<OrderBizData> deptOrderBizData = new List<OrderBizData>();
        private Timer termErrorTimer = null;

        public NCASFND.NCasCtrl.NCasTextBoard orderTextBoard;
        public System.Windows.Forms.Label labelErrorTermCount;
        public System.Windows.Forms.Label labelResponseTermCount;
        public System.Windows.Forms.Label labelTotalTermCount;
        public System.Windows.Forms.Label labelConErrorTermCount;
        public System.Windows.Forms.TableLayoutPanel orderViewTableLayout;
        public System.Windows.Forms.TableLayoutPanel orderViewDistTableLayout;
        public NCASFND.NCasCtrl.NCasButton btnRC45;
        public NCASFND.NCasCtrl.NCasButton btnRC35;
        public NCASFND.NCasCtrl.NCasButton btnRC25;
        public NCASFND.NCasCtrl.NCasButton btnRC13;
        public NCASFND.NCasCtrl.NCasButton btnRC14;
        public NCASFND.NCasCtrl.NCasButton btnRC24;
        public NCASFND.NCasCtrl.NCasButton btnRC15;
        public NCASFND.NCasCtrl.NCasButton btnRC55;
        public NCASFND.NCasCtrl.NCasButton btnRC54;
        public NCASFND.NCasCtrl.NCasButton btnRC64;
        public NCASFND.NCasCtrl.NCasButton btnRC23;
        public NCASFND.NCasCtrl.NCasButton btnRC34;
        public NCASFND.NCasCtrl.NCasButton btnRC44;
        public NCASFND.NCasCtrl.NCasButton btnRC85;
        public NCASFND.NCasCtrl.NCasButton btnRC75;
        public NCASFND.NCasCtrl.NCasButton btnRC65;
        public NCASFND.NCasCtrl.NCasButton btnRC74;
        public NCASFND.NCasCtrl.NCasButton btnRC84;
        public NCASFND.NCasCtrl.NCasButton btnRC33;
        public NCASFND.NCasCtrl.NCasButton btnRC43;
        public NCASFND.NCasCtrl.NCasButton btnRC53;
        public NCASFND.NCasCtrl.NCasButton btnRC83;
        public NCASFND.NCasCtrl.NCasButton btnRC73;
        public NCASFND.NCasCtrl.NCasButton btnRC63;
        public NCASFND.NCasCtrl.NCasButton btnRC82;
        public NCASFND.NCasCtrl.NCasButton btnRC81;
        public NCASFND.NCasCtrl.NCasButton btnRC72;
        public NCASFND.NCasCtrl.NCasButton btnRC62;
        public NCASFND.NCasCtrl.NCasButton btnRC61;
        public NCASFND.NCasCtrl.NCasButton btnRC52;
        public NCASFND.NCasCtrl.NCasButton btnRC51;
        public NCASFND.NCasCtrl.NCasButton btnRC41;
        public NCASFND.NCasCtrl.NCasButton btnRC42;
        public NCASFND.NCasCtrl.NCasButton btnRC32;
        public NCASFND.NCasCtrl.NCasButton btnRC31;
        public NCASFND.NCasCtrl.NCasButton btnRC22;
        public NCASFND.NCasCtrl.NCasButton btnRC21;
        public NCASFND.NCasCtrl.NCasButton btnRC11;
        public NCASFND.NCasCtrl.NCasButton btnRC12;
        public NCASFND.NCasCtrl.NCasButton btnRC89;
        public NCASFND.NCasCtrl.NCasButton btnRC88;
        public NCASFND.NCasCtrl.NCasButton btnRC810;
        public NCASFND.NCasCtrl.NCasButton btnRC87;
        public NCASFND.NCasCtrl.NCasButton btnRC86;
        public NCASFND.NCasCtrl.NCasButton btnRC76;
        public NCASFND.NCasCtrl.NCasButton btnRC77;
        public NCASFND.NCasCtrl.NCasButton btnRC78;
        public NCASFND.NCasCtrl.NCasButton btnRC79;
        public NCASFND.NCasCtrl.NCasButton btnRC710;
        public NCASFND.NCasCtrl.NCasButton btnRC66;
        public NCASFND.NCasCtrl.NCasButton btnRC67;
        public NCASFND.NCasCtrl.NCasButton btnRC68;
        public NCASFND.NCasCtrl.NCasButton btnRC69;
        public NCASFND.NCasCtrl.NCasButton btnRC610;
        public NCASFND.NCasCtrl.NCasButton btnRC56;
        public NCASFND.NCasCtrl.NCasButton btnRC57;
        public NCASFND.NCasCtrl.NCasButton btnRC58;
        public NCASFND.NCasCtrl.NCasButton btnRC510;
        public NCASFND.NCasCtrl.NCasButton btnRC59;
        public NCASFND.NCasCtrl.NCasButton btnRC46;
        public NCASFND.NCasCtrl.NCasButton btnRC47;
        public NCASFND.NCasCtrl.NCasButton btnRC48;
        public NCASFND.NCasCtrl.NCasButton btnRC49;
        public NCASFND.NCasCtrl.NCasButton btnRC410;
        public NCASFND.NCasCtrl.NCasButton btnRC310;
        public NCASFND.NCasCtrl.NCasButton btnRC39;
        public NCASFND.NCasCtrl.NCasButton btnRC38;
        public NCASFND.NCasCtrl.NCasButton btnRC37;
        public NCASFND.NCasCtrl.NCasButton btnRC36;
        public NCASFND.NCasCtrl.NCasButton btnRC26;
        public NCASFND.NCasCtrl.NCasButton btnRC27;
        public NCASFND.NCasCtrl.NCasButton btnRC28;
        public NCASFND.NCasCtrl.NCasButton btnRC29;
        public NCASFND.NCasCtrl.NCasButton btnRC210;
        public NCASFND.NCasCtrl.NCasButton btnRC16;
        public NCASFND.NCasCtrl.NCasButton btnRC17;
        public NCASFND.NCasCtrl.NCasButton btnRC18;
        public NCASFND.NCasCtrl.NCasButton btnRC19;
        public NCASFND.NCasCtrl.NCasButton btnRC110;
        public NCASFND.NCasCtrl.NCasButton btnRC71;
        public NCASFND.NCasCtrl.NCasPanel distSelectPanel;
        public NCASFND.NCasCtrl.NCasButton btnAllTermSelect;
        public NCASFND.NCasCtrl.NCasButton btnTermSelectCancel;
        public NCASFND.NCasCtrl.NCasListView distSelectListView;
        public NCASFND.NCasCtrl.NCasButton preStepBtn;
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public PAlmScreenUIController()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main"></param>
        public PAlmScreenUIController(MainForm main)
        {
            this.main = main;
            this.provInfo = main.ProvInfo;
            this.InitImageList();
            this.main.AddTimerMember(this);
            this.termErrorTimer = new Timer();
            this.termErrorTimer.Interval = 10000;
            this.termErrorTimer.Tick += new EventHandler(termErrorTimer_Tick);
            this.termErrorTimer.Start();
        }
        #endregion

        #region virtual method
        public override void OnTimer()
        {
            if (this.provInfo.AlarmOrderInfo.OccurTimeToDateTime > this.provInfo.BroadOrderInfo.OccurTimeToDateTime)
            {
                this.labelTotalTermCount.Text = this.provInfo.GetUsableAlarmTermCnt().ToString();
                this.SetOrderText();
                this.SetOrderResponseCount();
            }
            else
            {
                this.labelTotalTermCount.Text = this.provInfo.GetUsableDeptTermCnt().ToString();
                this.SetOrderTextBroad();
                this.SetOrderResponseCountBroad();
            }
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

        /// <summary>
        /// 예비발령 프로퍼티
        /// </summary>
        public bool OrderStandbyFlag
        {
            set { this.orderStandbyFlag = value; }
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

                //203, 203, 203 검정
                //255, 0, 0 빨강
                if (keyData.KeyActioin == NCasKeyAction.Real) //실제 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    //this.curButton.ForeColor = Color.FromArgb(255, 0, 0);
                }
                else if (keyData.KeyActioin == NCasKeyAction.Test) //시험 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                }
                else if (keyData.KeyActioin == NCasKeyAction.WrongOperation) //오조작 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.wrongOperationBtn = this.curButton;
                    this.wrongOperationBtn.UseCheck = false;
                    this.wrongOperationBtn.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.wrongOperationBtn.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertNormal);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertError);
                    this.wrongOperationBtn.AnimationInterval = 500;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Cancel || keyData.KeyActioin == NCasKeyAction.Confirm
                    || keyData.KeyActioin == NCasKeyAction.BroadFinish) //선택취소, 확인 버튼, 방송종료
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.curButton.UseCheck = false;
                }
                else
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.curButton.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.curButton.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.curButton.AnimationInterval = 500;
                }

                this.curButton.ForeColor = keyData.ClrText;
                this.curButton.Font = new Font("맑은 고딕", keyData.TxtSize);
                this.curButton.Padding = new Padding(keyData.PadLeft, keyData.PadTop, keyData.PadRight, keyData.PadBottom);
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
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                }
                else if (keyData.KeyActioin == NCasKeyAction.Test) //시험 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                }
                else if (keyData.KeyActioin == NCasKeyAction.WrongOperation) //오조작 버튼
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.wrongOperationBtn = this.curButton;
                    this.wrongOperationBtn.UseCheck = false;
                    this.wrongOperationBtn.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.wrongOperationBtn.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertNormal);
                    this.wrongOperationBtn.LstAnimation.Images.Add(NCasPAlmScreenRsc.btnAlertError);
                    this.wrongOperationBtn.AnimationInterval = 500;
                }
                else if (keyData.KeyActioin == NCasKeyAction.Cancel || keyData.KeyActioin == NCasKeyAction.Confirm
                    || keyData.KeyActioin == NCasKeyAction.BroadFinish) //선택취소와 확인 버튼, 방송종료
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.curButton.UseCheck = false;
                }
                else
                {
                    this.curButton.ForeColor = Color.FromArgb(203, 203, 203);
                    this.curButton.LstAnimation.ColorDepth = ColorDepth.Depth32Bit;
                    this.curButton.LstAnimation.ImageSize = new Size(NCasPAlmScreenRsc.btnAlertNormal.Width, NCasPAlmScreenRsc.btnAlertNormal.Height);
                    this.curButton.AnimationInterval = 500;
                }

                this.curButton.ForeColor = keyData.ClrText;
                this.curButton.Font = new Font("맑은 고딕", keyData.TxtSize);
                this.curButton.Padding = new Padding(keyData.PadLeft, keyData.PadTop, keyData.PadRight, keyData.PadBottom);
            }
        }
        #endregion

        #region 초기화
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        public void InitEtc()
        {
            //this.labelTotalTermCount.Text = this.provInfo.GetUsableAlarmTermCnt().ToString();
        }

        /// <summary>
        /// 버튼 컨트롤 초기화
        /// </summary>
        public void InitButton()
        {
            this.btnRC11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC21.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC31.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC41.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC51.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC61.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC71.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC81.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC12.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC22.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC32.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC42.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC52.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC62.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC72.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC82.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC13.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC23.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC33.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC43.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC53.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC63.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC73.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC83.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC14.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC24.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC34.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC44.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC54.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC64.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC74.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC84.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC15.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC25.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC35.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC45.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC55.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC65.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC75.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC85.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC16.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC26.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC36.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC46.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC56.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC66.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC76.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC86.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC17.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC27.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC37.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC47.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC57.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC67.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC77.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC87.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC18.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC28.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC38.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC48.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC58.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC68.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC78.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC88.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC19.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC29.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC39.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC49.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC59.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC69.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC79.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC89.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.btnRC110.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC210.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC310.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC410.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC510.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC610.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC710.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);
            this.btnRC810.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRC11_MouseDown);

            this.OrderBtnArrange();

            this.distSelectListView.ItemChecked += new NCASFND.NCasItemCheckedEventHandler(this.distSelectListView_ItemChecked);
            this.distSelectListView.MouseDoubleClick += new MouseEventHandler(distSelectListView_MouseDoubleClick);
            this.preStepBtn.Click += new System.EventHandler(this.preStepBtn_Click);
            this.btnAllTermSelect.Click += new System.EventHandler(this.btnAllTermSelect_Click);
            this.btnTermSelectCancel.Click += new System.EventHandler(this.btnTermSelectCancel_Click);

            this.InitDistDetailListView();
            this.ShowDistDetailSelectForm(true);
            this.btnAllTermSelect_Click(this, new EventArgs());
            this.preStepBtn.Visible = false;
            this.ShowDistDetailSelectForm(false);
        }
        #endregion

        #region ImageList 초기화
        /// <summary>
        /// 발령종류에 따른 ImageList 초기화
        /// </summary>
        private void InitImageList()
        {
            this.orderKindImageList = new ImageList();
            this.orderKindImageList.ImageSize = new Size(214, 68);
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
            this.orderKindDistImageList.ImageSize = new Size(110, 68);
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
            this.distSelectListView.Font = new Font(NCasPAlmScreenRsc.FontName, 15.0f);
            this.distSelectListView.ColumnHeight = 40;
            this.distSelectListView.ItemHeight = 38;
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
            col.Width = 250;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.distSelectListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 140;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.distSelectListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "소속";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.distSelectListView.Columns.Add(col);
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
                this.SetBtnChecked(ncasBtn, ((keyData.KeyStatus == NCasKeyState.Check) ? true : false));
            }
            else if (sender is NCasButton)
            {
                ncasBtn = sender as NCasButton;
                keyData = (NCasKeyData)ncasBtn.Tag;

                if (ncasBtn.CheckedValue == true)
                {
                    this.SetBtnChecked(ncasBtn, true);
                    keyData.KeyStatus = NCasKeyState.Check;
                }
                else
                {
                    this.SetBtnChecked(ncasBtn, false);
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
                            if (this.realButtonFromPlc == true)
                            {
                                this.CheckKeyRealFromDual(ncasBtn);
                            }
                            else
                            {
                                this.CheckKeyReal(ncasBtn);
                            }
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
                        using (OrderGroupEditForm orderGroupEditForm = new OrderGroupEditForm(this))
                        {
                            orderGroupEditForm.ShowDialog();
                        }
                    }
                    break;

                case NCasKeyAction.DistOneDestination: //시군 또는 개별단말 (발령 대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (keyData.KeyStatus == NCasKeyState.RightButton)
                        {
                            this.lstSelectedOrderIP.Add(this.WrongOperationIP);
                        }
                        else
                        {
                            this.CheckKeyDistOneDestination(ncasBtn);
                        }
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        this.CheckKeyDistOneDestinationRight(ncasBtn);
                        this.selectedDistCode = int.Parse((ncasBtn.Tag as NCasKeyData).Info);
                    }
                    break;

                case NCasKeyAction.DeptAllDestination: //주요기관 전체 (발령 대상)
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (keyData.KeyStatus == NCasKeyState.RightButton)
                        {
                            this.lstSelectedOrderIP.Add(this.WrongOperationIP);
                        }
                        else
                        {
                            this.CheckKeyDeptDestination(ncasBtn);
                        }
                    }
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        this.CheckKeyDeptDestinationRight(ncasBtn);
                    }
                    break;

                case NCasKeyAction.BroadFinish: //주요기관 방송종료
                    if (isLocal == true)
                    {
                        this.CheckKeyBroadFinish(ncasBtn);
                    }
                    else
                    {
                        this.CheckKeyBroadFinishFromDual(ncasBtn);
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
                //개별 단말 선택 시 듀얼시스템 오조작을 방지하기 위한 처리
                if ((keyData.KeyActioin == NCasKeyAction.DistOneDestination && e.Button == System.Windows.Forms.MouseButtons.Right)
                    || (keyData.KeyActioin == NCasKeyAction.DeptAllDestination && e.Button == System.Windows.Forms.MouseButtons.Right))
                {
                    keyData.KeyStatus = NCasKeyState.RightButton;
                    this.main.SetKeyBizData(keyData);
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    return;

                if (keyData.KeyActioin != NCasKeyAction.Confirm)
                {
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
                }

                if (keyData.KeyActioin != NCasKeyAction.LampTest)
                {
                    //단일 버튼 데이터 전송
                    NCasPlcProtocolBase plcProtoBase = NCasPlcProtocolFactory.CreatePlcProtocol(NCasDefinePlcCommand.SetUnitStatus);
                    NCasPlcProtocolSetUnitStatus unitStatus = plcProtoBase as NCasPlcProtocolSetUnitStatus;
                    unitStatus.BtnCode = keyData.ID;
                    NCasPlcProtocolFactory.MakeFrame(unitStatus);

                    if (keyData.KeyActioin != NCasKeyAction.Real)
                    {
                        this.main.SetPlcKeyData(unitStatus);
                    }

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
            }

            if (keyData.KeyActioin == NCasKeyAction.Confirm)
            {
                if (isLocal == true)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            #region 상태 표출 테스트
            //Console.WriteLine("\n선택한 버튼리스트에 있는 버튼 수 - " + this.lstSelectedButtons.Count.ToString());

            //Console.WriteLine("선택한 발령 모드 - " + ((this.selectedOrderMode == NCasDefineOrderMode.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.None) :
            //    (this.selectedOrderMode == NCasDefineOrderMode.RealMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.RealMode) :
            //    (this.selectedOrderMode == NCasDefineOrderMode.TestMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.TestMode) : "실패"));

            //Console.WriteLine("선택한 미디어 모드 - " + ((this.selectedOrderMedia == NCasDefineOrderMedia.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.None) :
            //    (this.selectedOrderMedia == NCasDefineOrderMedia.MediaAll) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaAll) :
            //    (this.selectedOrderMedia == NCasDefineOrderMedia.MediaLine) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaLine) :
            //    (this.selectedOrderMedia == NCasDefineOrderMedia.MediaSate) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaSate) : "실패"));

            //Console.WriteLine("선택한 시도 발령대상 수 - " + lstSelectedOrderIP.Count.ToString());
            //foreach (string str in this.lstSelectedOrderIP)
            //{
            //    Console.WriteLine("      대상은 - " + str);
            //}

            //Console.WriteLine("선택한 발령종류 - " + ((this.selectedOrderKind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
            //        (this.selectedOrderKind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) : ""));

            //Console.WriteLine("가장 마지막 발령종류 - " + ((this.lastOrderKind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
            //        (this.lastOrderKind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
            //        (this.lastOrderKind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
            //        (this.lastOrderKind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
            //        (this.lastOrderKind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
            //        (this.lastOrderKind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
            //        (this.lastOrderKind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
            //        (this.lastOrderKind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
            //        (this.lastOrderKind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
            //        (this.lastOrderKind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
            //        (this.lastOrderKind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) : ""));

            //Console.WriteLine("오조작 상태 - " + ((this.wrongOperationFlag == true) ? "O" : "X"));

            //Console.WriteLine("시도 전체 선택 - " + ((this.allDestinationFlag == true) ? "O" : "X"));

            //Console.WriteLine("그룹 발령 선택 - " + ((this.orderGroupFlag == true) ? "O" : "X"));

            //Console.WriteLine("시군 발령 선택 - " + ((this.orderDistFlag == true) ? "O" : "X"));

            //Console.WriteLine("개별단말 발령 선택 - " + ((this.orderTermFlag == true) ? "O" : "X"));

            //Console.WriteLine("주요기관 전체 발령 선택 - " + ((this.orderAllDeptFlag == true) ? "O" : "X"));

            //Console.WriteLine("주요기관 개별 발령 선택 - " + ((this.orderDeptFlag == true) ? "O" : "X"));

            //Console.WriteLine("예비 발령 - " + ((this.orderStandbyFlag == true) ? "O" : "X"));

            //Console.WriteLine("램프시험 - " + ((this.lampTestFlag == true) ? "O" : "X"));

            //Console.WriteLine("재난경계 종류 선택한 거 - " + ((this.selectedDisasterBroadKind == DisasterBroadKind.None) ? "선택안함" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.Mic) ? "마이크" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.Tts) ? "TTS" :
            //    (this.selectedDisasterBroadKind == DisasterBroadKind.StroredMessage) ? "저장메시지" : "선택안함"));

            //Console.WriteLine("마지막 재난경계 발령한 거 - " + ((this.lastDisasterBroadKind == DisasterBroadKind.None) ? "선택안함" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.Mic) ? "마이크" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.Tts) ? "TTS" :
            //    (this.lastDisasterBroadKind == DisasterBroadKind.StroredMessage) ? "저장메시지" : "선택안함"));

            //Console.WriteLine("선택한 그룹 버튼 수 - " + this.selectedGroupContents.Count.ToString());
            //foreach (GroupContent groupContent in this.selectedGroupContents)
            //{
            //    Console.WriteLine("      버튼은 - " + groupContent.Title);
            //}

            //Console.WriteLine("저장메시지 - ");
            //Console.WriteLine("      제목 - " + this.selectedStoredMessage.Title);
            //Console.WriteLine("      번호 - " + this.selectedStoredMessage.MsgNum);
            //Console.WriteLine("      시간 - " + this.selectedStoredMessage.PlayTime.ToString());
            //Console.WriteLine("      반복횟수 - " + this.storedMessageRepeatCount.ToString());

            //Console.WriteLine("TTS메시지 - ");
            //Console.WriteLine("      제목 - " + this.selectedTtsMessage.Title);
            //Console.WriteLine("      내용 - " + this.selectedTtsMessage.Text);
            #endregion
        }
        #endregion

        #region Confirm method
        /// <summary>
        /// 확인 버튼에 대한 처리 메소드
        /// </summary>
        private bool Confirm()
        {
            DateTime orderDateTime = DateTime.Now;
            List<OrderBizData> tmpOrderBuff = new List<OrderBizData>();

            if (this.selectedDisasterBroadKind != DisasterBroadKind.StroredMessage) //저장메시지 발령이 아니므로 TC 1을 만들어 biz로 보낸다.
            {
                tmpOrderBuff = this.getProtoTc1List(orderDateTime);
            }
            else //저장메시지 발령이므로 TC 151을 만들어 biz로 보낸다.
            {
                tmpOrderBuff = this.getProtoTc151List(orderDateTime);
            }

            foreach (OrderBizData orderBizData in tmpOrderBuff)
            {
                this.main.SetOrderBizData(orderBizData);
            }

            if (this.orderAllDeptFlag == true || this.orderDeptFlag == true)
            {
                this.deptOrderBizData = tmpOrderBuff;
            }

            return true;
        }
        #endregion

        #region private method
        #region 발령 TC 1 만드는 메소드
        /// <summary>
        /// TC 1을 만들어 리턴하는 메소드
        /// </summary>
        /// <returns></returns>
        private List<OrderBizData> getProtoTc1List(DateTime orderTime)
        {
            List<OrderBizData> tmpOrderBuff = new List<OrderBizData>();

            for (int i = 0; i < this.lstSelectedOrderIP.Count; i++)
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcAlarmOrder);
                NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;

                protoTc1.AlarmNetIdOrIpByString = this.lstSelectedOrderIP[i];
                protoTc1.OrderTimeByDateTime = orderTime;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    protoTc1.BroadNetIdOrIpByString = this.lstSelectedOrderIP[i];
                }

                protoTc1.CtrlKind = NCasDefineControlKind.ControlAlarm;
                protoTc1.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc1.AlarmKind = this.selectedOrderKind;
                protoTc1.Mode = this.selectedOrderMode;
                protoTc1.Media = this.selectedOrderMedia;
                protoTc1.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeNotUsed;
                }

                protoTc1.Sector = (this.allDestinationFlag == true || this.orderAllDeptFlag == true) ? NCasDefineSectionCode.SectionProv :
                    (this.orderDistFlag == true) ? NCasDefineSectionCode.SectionDist :
                    (this.orderDeptFlag == true) ? NCasDefineSectionCode.SectionBroad :
                    (this.orderTermFlag == true) ? NCasDefineSectionCode.SectionTerm : NCasDefineSectionCode.None;

                if (this.orderGroupFlag)
                {
                    protoTc1.Sector = (this.GetIsDist(this.lstSelectedOrderIP[i]) == true) ? NCasDefineSectionCode.SectionDist : NCasDefineSectionCode.SectionTerm;
                }

                if (protoTc1.Sector == NCasDefineSectionCode.SectionDist)
                {
                    for (int j = 0; j < this.provInfo.LstRepts.Count; j++)
                    {
                        if (this.provInfo.NetIdToString == "10.24.0.0") //경기
                        {
                            if (this.lstSelectedOrderIP[i] == "10.25.3.255")
                            {
                                protoTc1.Sector = NCasDefineSectionCode.SectionRept;
                                break;
                            }
                        }
                        else
                        {
                            if (NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.lstSelectedOrderIP[i]) == this.provInfo.LstRepts[j].NetIdToString)
                            {
                                protoTc1.Sector = NCasDefineSectionCode.SectionRept;
                                break;
                            }
                        }
                    }
                }

                byte[] tmpBuff = NCasProtocolFactory.MakeUdpFrame(protoTc1);
                OrderBizData orderBizData = new OrderBizData();
                orderBizData.AllDestinationFlag = this.allDestinationFlag;
                orderBizData.AlmProtocol = protoTc1;
                orderBizData.IsLocal = true;
                orderBizData.LastOrderKind = this.lastOrderKind;
                orderBizData.OrderDistFlag = this.orderDistFlag;
                orderBizData.OrderGroupFlag = this.orderGroupFlag;
                orderBizData.OrderTermFlag = this.orderTermFlag;
                orderBizData.SendBuff = tmpBuff;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    orderBizData.OrderTermFlag = true;
                }

                orderBizData.SelectedDisasterBroadKind = this.selectedDisasterBroadKind;
                orderBizData.SelectedStoredMessage = this.selectedStoredMessage;
                orderBizData.StoredMessageRepeatCount = this.storedMessageRepeatCount;
                orderBizData.SelectedTtsMessage = this.selectedTtsMessage;

                foreach (string eachStr in this.groupNameLst)
                {
                    orderBizData.GroupName.Add(eachStr);
                }

                if (i == 0)
                {
                    orderBizData.IsEnd = OrderDataSendStatus.First;
                }
                else if (i == (this.lstSelectedOrderIP.Count - 1))
                {
                    orderBizData.IsEnd = OrderDataSendStatus.End;
                }
                else
                {
                    orderBizData.IsEnd = OrderDataSendStatus.None;
                }

                if (this.lstSelectedOrderIP.Count == 1)
                {
                    orderBizData.IsEnd = OrderDataSendStatus.FirstEnd;
                }

                if (this.lastDisasterBroadKind == DisasterBroadKind.Tts) //마지막 발령이 TTS발령인지..
                {
                    orderBizData.TtsOrderFlag = true;
                }
                else
                {
                    orderBizData.TtsOrderFlag = false;
                }

                tmpOrderBuff.Add(orderBizData);
            }

            return tmpOrderBuff;
        }
        #endregion

        #region 발령 TC 151 만드는 메소드
        /// <summary>
        /// TC 151을 만들어 리턴하는 메소드
        /// </summary>
        /// <returns></returns>
        private List<OrderBizData> getProtoTc151List(DateTime orderTime)
        {
            List<OrderBizData> tmpOrderBuff = new List<OrderBizData>();

            for (int i = 0; i < this.lstSelectedOrderIP.Count; i++)
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcPublicAlarmOrder);
                NCasProtocolTc151 protoTc1 = protoBase as NCasProtocolTc151;

                protoTc1.AlarmNetIdOrIpByString = this.lstSelectedOrderIP[i];
                protoTc1.OrderTimeByDateTime = orderTime;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    protoTc1.BroadNetIdOrIpByString = this.lstSelectedOrderIP[i];
                }

                protoTc1.CtrlKind = NCasDefineControlKind.ControlAlarm;
                protoTc1.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc1.AlarmKind = NCasDefineOrderKind.BroadMessage;
                protoTc1.Mode = this.selectedOrderMode;
                protoTc1.Media = this.selectedOrderMedia;
                protoTc1.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeNotUsed;
                }

                protoTc1.Sector = (this.allDestinationFlag == true || this.orderAllDeptFlag == true) ? NCasDefineSectionCode.SectionProv :
                    (this.orderDistFlag == true) ? NCasDefineSectionCode.SectionDist :
                    (this.orderDeptFlag == true) ? NCasDefineSectionCode.SectionBroad :
                    (this.orderTermFlag == true) ? NCasDefineSectionCode.SectionTerm : NCasDefineSectionCode.None;

                if (this.orderGroupFlag)
                {
                    protoTc1.Sector = (this.GetIsDist(this.lstSelectedOrderIP[i]) == true) ? NCasDefineSectionCode.SectionDist : NCasDefineSectionCode.SectionTerm;
                }

                if (protoTc1.Sector == NCasDefineSectionCode.SectionDist)
                {
                    for (int j = 0; j < this.provInfo.LstRepts.Count; j++)
                    {
                        if (this.provInfo.NetIdToString == "10.24.0.0") //경기
                        {
                            if (this.lstSelectedOrderIP[i] == "10.25.3.255")
                            {
                                protoTc1.Sector = NCasDefineSectionCode.SectionRept;
                                break;
                            }
                        }
                        else
                        {
                            if (NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.lstSelectedOrderIP[i]) == this.provInfo.LstRepts[j].NetIdToString)
                            {
                                protoTc1.Sector = NCasDefineSectionCode.SectionRept;
                                break;
                            }
                        }
                    }
                }

                protoTc1.MsgNum1 = GetStoredMsgHeaderNumber(int.Parse(this.selectedStoredMessage.MsgNum));
                protoTc1.MsgNum2 = int.Parse(this.selectedStoredMessage.MsgNum);
                protoTc1.MsgNum3 = GetStoredMsgTailNumber(int.Parse(this.selectedStoredMessage.MsgNum));
                protoTc1.RepeatNum = (byte)this.storedMessageRepeatCount;

                NCasProtocolFactory.MakeUdpFrame(protoTc1);
                OrderBizData orderBizData = new OrderBizData();
                orderBizData.AllDestinationFlag = this.allDestinationFlag;
                orderBizData.AlmProtocol = protoTc1;
                orderBizData.IsLocal = true;
                orderBizData.LastOrderKind = this.lastOrderKind;
                orderBizData.OrderDistFlag = this.orderDistFlag;
                orderBizData.OrderGroupFlag = this.orderGroupFlag;
                orderBizData.OrderTermFlag = this.orderTermFlag;

                if (this.orderAllDeptFlag || this.orderDeptFlag)
                {
                    orderBizData.OrderTermFlag = true;
                }

                orderBizData.SelectedDisasterBroadKind = this.selectedDisasterBroadKind;
                orderBizData.SelectedStoredMessage = this.selectedStoredMessage;
                orderBizData.StoredMessageRepeatCount = this.storedMessageRepeatCount;
                orderBizData.SelectedTtsMessage = this.selectedTtsMessage;

                foreach (string eachStr in this.groupNameLst)
                {
                    orderBizData.GroupName.Add(eachStr);
                }

                if (i == 0)
                {
                    orderBizData.IsEnd = OrderDataSendStatus.First;
                }
                else if (i == (this.lstSelectedOrderIP.Count - 1))
                {
                    orderBizData.IsEnd = OrderDataSendStatus.End;
                }
                else
                {
                    orderBizData.IsEnd = OrderDataSendStatus.None;
                }

                if (this.lstSelectedOrderIP.Count == 1)
                {
                    orderBizData.IsEnd = OrderDataSendStatus.FirstEnd;
                }

                if (this.lastDisasterBroadKind == DisasterBroadKind.Tts) //마지막 발령이 TTS발령인지..
                {
                    orderBizData.TtsOrderFlag = true;
                }
                else
                {
                    orderBizData.TtsOrderFlag = false;
                }

                tmpOrderBuff.Add(orderBizData);
            }

            return tmpOrderBuff;
        }
        #endregion

        #region 원하는 버튼 제어
        /// <summary>
        /// 버튼의 체크상태를 셋팅한다.
        /// </summary>
        /// <param name="check"></param>
        private void SetBtnChecked(NCasButton btn, bool check)
        {
            if (check)
            {
                btn.CheckedValue = check;
                btn.ForeColor = Color.FromArgb(86, 169, 255);
            }
            else
            {
                btn.CheckedValue = check;

                if ((btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.Real
                    || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.WrongOperation)
                {
                    //btn.ForeColor = Color.FromArgb(203, 203, 203);
                    btn.ForeColor = ((btn.Tag) as NCasKeyData).ClrText;
                }
                else
                {
                    //btn.ForeColor = Color.FromArgb(203, 203, 203);
                    btn.ForeColor = ((btn.Tag) as NCasKeyData).ClrText;
                }
            }
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
                        this.SetBtnChecked(ncasBtn, true);
                        ncasBtn.ForeColor = Color.FromArgb(86, 169, 255);

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
                        this.SetBtnChecked(ncasBtn, true);
                        ncasBtn.ForeColor = Color.FromArgb(86, 169, 255);

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
                        this.SetBtnChecked(ncasBtn, false);
                        ncasBtn.ForeColor = ((ncasBtn.Tag) as NCasKeyData).ClrText;

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
                        this.SetBtnChecked(ncasBtn, false);
                        ncasBtn.ForeColor = ((ncasBtn.Tag) as NCasKeyData).ClrText;

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
                    this.SetBtnChecked(ncasBtn, false);
                    ncasBtn.ForeColor = ((ncasBtn.Tag) as NCasKeyData).ClrText;

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
                    this.SetBtnChecked(ncasBtn, false);
                    ncasBtn.ForeColor = ((ncasBtn.Tag) as NCasKeyData).ClrText;

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
                    this.SetBtnChecked(ncasBtn, false);

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
                else
                {
                    this.SetBtnChecked(ncasBtn, true);

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
                    this.SetBtnChecked(ncasBtn, false);

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
                else
                {
                    this.SetBtnChecked(ncasBtn, true);

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
                    this.SetBtnChecked(ncasBtn, false);
                    this.SetOrderKindButtonColor(ncasBtn, orderKind);
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.ProveAllDestination || btnKeyData.KeyActioin == NCasKeyAction.GroupDestination ||
                    btnKeyData.KeyActioin == NCasKeyAction.DistOneDestination)
                {
                    this.SetBtnChecked(ncasBtn, false);
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
                    keyData.KeyActioin == NCasKeyAction.DistOneDestination || keyData.KeyActioin == NCasKeyAction.TermDestination)
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
                    keyData.KeyActioin == NCasKeyAction.DistOneDestination || keyData.KeyActioin == NCasKeyAction.TermDestination)
                {
                    if (btn.CheckedValue == true)
                    {
                        this.SetBtnChecked(btn, false);
                    }

                    btn.LstAnimation.Images.Clear();
                    btn.LstAnimation.Images.Add(this.orderKindImageList.Images[3]);

                    switch (orderKind)
                    {
                        case NCasDefineOrderKind.AlarmStandby:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[2]);
                            btn.ForeColor = Color.FromArgb(144, 233, 84);
                            break;

                        case NCasDefineOrderKind.AlarmAttack:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[7]);
                            btn.ForeColor = Color.FromArgb(255, 12, 14);
                            break;

                        case NCasDefineOrderKind.AlarmWatch:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[4]);
                            btn.ForeColor = Color.FromArgb(249, 164, 82);
                            break;

                        case NCasDefineOrderKind.AlarmBiochemist:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[10]);
                            btn.ForeColor = Color.FromArgb(255, 249, 64);
                            break;

                        case NCasDefineOrderKind.DisasterBroadcast:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[6]);
                            btn.ForeColor = Color.FromArgb(210, 56, 190);
                            break;

                        case NCasDefineOrderKind.DisasterWatch:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[5]);
                            btn.ForeColor = Color.FromArgb(245, 117, 102);
                            break;

                        case NCasDefineOrderKind.AlarmCancel:
                            btn.LstAnimation.Images.Add(this.orderKindImageList.Images[9]);
                            btn.ForeColor = Color.FromArgb(255, 255, 255);
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
            this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                this.distSelectPanel.Dock = DockStyle.Fill;
                this.orderViewDistTableLayout.SendToBack();
                this.distSelectPanel.BringToFront();
            }
            else
            {
                this.distSelectPanel.Dock = DockStyle.None;
                this.orderViewDistTableLayout.Dock = DockStyle.Fill;
                this.distSelectPanel.SendToBack();
                this.orderViewDistTableLayout.BringToFront();
            }
        }
        #endregion

        #region timer method
        private void termErrorTimer_Tick(object sender, EventArgs e)
        {
            int termErrorCnt =0;

            foreach (TermInfo eachTerminfo in this.provInfo.LstTerms)
            {
                if (eachTerminfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal)
                {
                    termErrorCnt++;
                }
            }

            this.labelConErrorTermCount.Text = termErrorCnt.ToString();
        }

        /// <summary>
        /// 경보발령에 대한 응답 카운트를 화면에 표시한다.
        /// </summary>
        private void SetOrderResponseCount()
        {
            this.labelResponseTermCount.Text = this.provInfo.AlarmRespCnt.ToString();
            this.labelErrorTermCount.Text = this.provInfo.FaultAlmResponseCnt.ToString();
            //this.labelConErrorTermCount.Text = this.provInfo.FaultConnectionTermCnt.ToString();
        }

        /// <summary>
        /// 방송발령에 대한 응답 카운트를 화면에 표시한다.
        /// </summary>
        private void SetOrderResponseCountBroad()
        {
            this.labelResponseTermCount.Text = this.provInfo.DeptRespCnt.ToString();
            this.labelErrorTermCount.Text = this.provInfo.FaultOnlyDeptResponseCnt.ToString();
            //this.labelConErrorTermCount.Text = this.provInfo.FaultConnectionTermCnt.ToString();
        }

        /// <summary>
        /// 경보발령에 대한 정보를 화면 상단에 표시한다.
        /// </summary>
        private void SetOrderText()
        {
            this.orderTextBoard.ClearTextBlock();
            this.orderTextBoard.FontSize = 30;

            if (this.provInfo.AlarmOrderInfo.Kind != NCasDefineOrderKind.None)
            {
                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem3(this.provInfo.AlarmOrderInfo.OccurTimeToDateTime) +
                    " [" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(this.provInfo.AlarmOrderInfo.Source) + "]에서 [" +
                    NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(this.provInfo.AlarmOrderInfo.Media) + "]으로 ", Color.FromArgb(255, 255, 255)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(this.provInfo.AlarmOrderInfo.Mode) + "] ",
                    (this.provInfo.AlarmOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? Color.FromArgb(232, 82, 53) : Color.FromArgb(6, 147, 6)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(this.provInfo.AlarmOrderInfo.Kind) + "] 경보를 발령했습니다.", Color.FromArgb(255, 255, 255)));
            }
        }

        /// <summary>
        /// 방송발령에 대한 정보를 화면 상단에 표시한다.
        /// </summary>
        private void SetOrderTextBroad()
        {
            this.orderTextBoard.ClearTextBlock();
            this.orderTextBoard.FontSize = 30;

            if (this.provInfo.BroadOrderInfo.Kind != NCasDefineOrderKind.None)
            {
                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem3(this.provInfo.BroadOrderInfo.OccurTimeToDateTime) +
                    " [" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(this.provInfo.BroadOrderInfo.Source) + "]에서 [" +
                    NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(this.provInfo.BroadOrderInfo.Media) + "]으로 ", Color.FromArgb(255, 255, 255)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(this.provInfo.BroadOrderInfo.Mode) + "] ",
                    (this.provInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? Color.FromArgb(232, 82, 53) : Color.FromArgb(6, 147, 6)));

                this.orderTextBoard.AddTextBlock(new NCasTextBoard.NCasTextBoardBlock(
                    "[" + NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(this.provInfo.BroadOrderInfo.Kind) + "] 방송을 발령했습니다.", Color.FromArgb(255, 255, 255)));
            }
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
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

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
            this.orderDeptFlag = false;
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
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;
                lvi.Tag = eachTermInfo;

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
                NCasListViewItem lvi = null;

                foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                {
                    if (e.Item.Name == eachItem.Name)
                    {
                        lvi = eachItem;
                        break;
                    }
                }

                if (lvi.Tag is TermInfo)
                {
                    this.orderTermFlag = true;
                }
                else if (lvi.Tag is PDeptInfo)
                {
                    this.orderDeptFlag = true;
                }

                this.lstSelectedOrderIP.Add(e.Item.Name);
            }
            else
            {
                this.lstSelectedOrderIP.Remove(e.Item.Name);

                if (this.lstSelectedOrderIP.Count == 0)
                {
                    this.orderTermFlag = false;
                    this.orderDeptFlag = false;
                }
            }
        }

        /// <summary>
        /// 개별단말 화면에서 리스트뷰 MouseDoubleClick 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void distSelectListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.distSelectListView.SelectedItems.Count == 0)
                return;

            if (!this.distSelectListView.SelectedItems[0].Checked)
            {
                if (this.distSelectListView.SelectedItems[0].Tag is TermInfo)
                {
                    this.orderTermFlag = true;
                }
                else if (this.distSelectListView.SelectedItems[0].Tag is PDeptInfo)
                {
                    this.orderDeptFlag = true;
                }

                this.lstSelectedOrderIP.Add(this.distSelectListView.SelectedItems[0].Name);
                this.distSelectListView.SelectedItems[0].Checked = true;
            }
            else
            {
                this.lstSelectedOrderIP.Remove(this.distSelectListView.SelectedItems[0].Name);
                this.distSelectListView.SelectedItems[0].Checked = false;

                if (this.lstSelectedOrderIP.Count == 0)
                {
                    this.orderTermFlag = false;
                    this.orderDeptFlag = false;
                }
            }

            this.distSelectListView.Refresh();
        }
        #endregion

        #region etc method
        /// <summary>
        /// 방송할 저장메시지에 해당되는 Header 메시지 번호를 반환한다.
        /// </summary>
        /// <param name="storedMsgNumber">방송할 저장메시지 번호</param>
        /// <returns>방송할 저장메시지에 해당되는 Header 메시지 번호</returns>
        private int GetStoredMsgHeaderNumber(int storedMsgNumber)
        {
            int resultNum = 0;

            if ((storedMsgNumber % 2 == 0) && (storedMsgNumber > 201 && storedMsgNumber < 219))
            {
                resultNum = 954;
            }
            else if (storedMsgNumber == 157 || storedMsgNumber == 158)
            {
                resultNum = 954;
            }
            else
            {
                resultNum = 951;
            }

            return resultNum;
        }

        /// <summary>
        /// 방송할 저장메시지에 해당되는 Tail 메시지 번호를 반환한다.
        /// </summary>
        /// <param name="storedMsgNumber">방송할 저장메시지 번호</param>
        /// <returns>방송할 저장메시지에 해당되는 Tail 메시지 번호</returns>
        private int GetStoredMsgTailNumber(int storedMsgNumber)
        {
            int resultNum = 0;

            if (storedMsgNumber > 155 && storedMsgNumber < 170)
            {
                resultNum = 509;
            }
            else
            {
                resultNum = 502;
            }

            return resultNum;
        }

        /// <summary>
        /// 버튼Code를 받아 해당하는 NCasButton을 반환한다.
        /// </summary>
        /// <param name="btnCode"></param>
        /// <returns></returns>
        private NCasButton GetNCasButton(string btnCode)
        {
            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.ID.ToString() == btnCode)
                {
                    return ncasBtn;
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.ID.ToString() == btnCode)
                {
                    return ncasBtn;
                }
            }

            return null;
        }

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
                    if (NCasUtilityMng.INCasCommUtility.AddIpAddr(groupData.IpAddr, 0, 0, 0, 255) == ipAddr)
                    {
                        return groupData.IsDist;
                    }
                    else if (groupData.IpAddr == ipAddr)
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
            if (plcResponse.Mode == NCasDefinePlcMode.Real && plcResponse.BtnCode == NCasDefineButtonCode.None) //실제 키를 돌린 경우..
            {
                NCasButton plcPushBtn = this.GetNCasButton(NCasKeyAction.Real, "", "");
                this.realButtonFromPlc = true;
                this.SetBtnChecked(plcPushBtn, true);
                this.btnRC11_MouseDown(plcPushBtn, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                this.realButtonFromPlc = false;
            }
            else
            {
                if (plcResponse.BtnCode == NCasDefineButtonCode.None) //실제 키를 시험으로 돌린 경우..
                {
                    NCasButton plcPushBtn = this.GetNCasButton(NCasKeyAction.Real, "", "");
                    this.realButtonFromPlc = true;
                    this.SetBtnChecked(plcPushBtn, false);
                    this.btnRC11_MouseDown(plcPushBtn, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                    this.realButtonFromPlc = false;
                }
                else //그 외 일반 버튼 눌린 경우..
                {
                    NCasButton plcPushBtn = this.GetNCasButton(plcResponse.BtnCode.ToString());

                    if ((plcPushBtn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.Real)
                        return;

                    if (!((plcPushBtn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.WrongOperation
                        || (plcPushBtn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.Cancel
                        || (plcPushBtn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.Confirm))
                    {
                        if (plcPushBtn.CheckedValue == true)
                        {
                            this.SetBtnChecked(plcPushBtn, false);
                        }
                        else
                        {
                            this.SetBtnChecked(plcPushBtn, true);
                        }
                    }

                    this.btnRC11_MouseDown(plcPushBtn, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
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
                    this.SetBtnChecked(selectBtn, true);
                    return;
                }
            }

            if (this.selectedOrderMode == NCasDefineOrderMode.RealMode)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    this.SetBtnChecked(btn, false);
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                this.SetBtnChecked(selectBtn, false);
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    this.SetBtnChecked(btn, false);
                }

                this.SetBtnChecked(selectBtn, false);
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(btn, false);
                }

                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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

                this.SetBtnChecked(selectBtn, true);
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
                this.SetBtnChecked(selectBtn, false);
            }
            else if (this.selectedOrderMode == NCasDefineOrderMode.None)
            {
                foreach (NCasButton btn in this.lstSelectedButtons)
                {
                    this.SetBtnChecked(btn, false);
                }

                this.SetBtnChecked(selectBtn, false);
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
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
                    this.SetBtnChecked(btn, false);
                }

                this.SetBtnChecked(selectBtn, false);
                this.SetOrderDestinationButtonColorOff();
                this.lstSelectedOrderIP.Clear();
                this.selectedOrderKind = NCasDefineOrderKind.None;
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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

            if (this.lampTestFlag)
            {
                this.SetOffButton(NCasKeyAction.LampTest);
                NCasButton tmpLampBtn = this.GetNCasButton(NCasKeyAction.LampTest, "", "");
                this.btnRC11_MouseDown(tmpLampBtn, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
            }
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.SetOrderDestinationButtonColorOff();
                this.SetOffButtonAny(NCasKeyAction.TermDestination);
                this.ShowDistDetailSelectForm(false);
                this.allDestinationFlag = true;
                this.orderGroupFlag = false;
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.orderAllDeptFlag = false;
                this.orderDeptFlag = false;

                if (!this.lstSelectedOrderIP.Contains(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4())))
                {
                    this.lstSelectedOrderIP.Clear();
                    this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdToProvBroadIp(NCasUtilityMng.INCasEtcUtility.GetIPv4()), 0, 7, 0, 0));
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.SetOnButton(NCasKeyAction.ProveAllDestination);
                this.SetOnButton(NCasKeyAction.GroupDestination);
                this.SetOnButton(NCasKeyAction.DistOneDestination);
                this.SetOffButton(NCasKeyAction.TermDestination);
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
                this.SetOffButton(NCasKeyAction.TermDestination);
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            this.ShowDistDetailSelectForm(false);

            if (this.orderDistFlag || this.orderTermFlag || this.orderAllDeptFlag || this.orderDeptFlag) //시군 또는 단말이 선택되어 있으면 선택해제, 주요기관 포함
            {
                this.SetOffButton(NCasKeyAction.DistOneDestination);
                this.SetOffButton(NCasKeyAction.TermDestination);
                this.lstSelectedOrderIP.Clear();
                this.orderDistFlag = false;
                this.orderTermFlag = false;
                this.orderAllDeptFlag = false;
                this.orderDeptFlag = false;
            }

            if (selectBtn.CheckedValue == true)
            {
                this.SetOrderDestinationButtonColorOff();

                if (this.selectedOrderKind != NCasDefineOrderKind.None)
                {
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                    this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
                    this.SetOffButtonAny(NCasKeyAction.TermDestination);
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                    this.SetBtnChecked(selectBtn, true);
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
                        if (!this.lstSelectedOrderIP.Contains(NCasUtilityMng.INCasCommUtility.UintIP2StringIP((NCasUtilityMng.INCasCommUtility.StringIP2UintIP(groupData.IpAddr)) | 255)))
                        {
                            this.lstSelectedOrderIP.Add(groupData.IpAddr);
                        }
                    }
                }

                this.ClearOrderKind();

                if (!this.groupNameLst.Contains(selectBtn.Text))
                {
                    this.groupNameLst.Add(selectBtn.Text);
                }
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

                if (this.groupNameLst.Contains(selectBtn.Text))
                {
                    this.groupNameLst.Remove(selectBtn.Text);
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
                || this.allDestinationFlag == true || this.orderGroupFlag == true || this.orderAllDeptFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue)
            {
                this.SetOrderDestinationButtonColorOff();

                if (this.selectedOrderKind != NCasDefineOrderKind.None)
                {
                    this.SetOnButton(NCasKeyAction.GroupDestination);
                    this.SetOnButton(NCasKeyAction.DistOneDestination);
                    this.lstSelectedOrderIP.Clear();
                    this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                    this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
                }

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.orderDistFlag = true;
                DistInfo distInfo = this.main.MmfMng.GetDistInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info));
                this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(distInfo.NetIdToString, 0, 0, 0, 255));
                this.ClearOrderKind();
                this.SetBtnChecked(selectBtn, true);
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
                || this.allDestinationFlag == true || this.orderGroupFlag == true || this.orderDistFlag == true || this.orderAllDeptFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    this.SetBtnChecked(selectBtn, true);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, false);
                }

                return;
            }

            this.distSelectListView.Items.Clear();

            foreach (TermInfo eachTermInfo in this.main.MmfMng.GetDistInfoByCode(int.Parse((selectBtn.Tag as NCasKeyData).Info)).LstTerms)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;
                lvi.Tag = eachTermInfo;

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
            this.btnAllTermSelect.Visible = true;
            this.ShowDistDetailSelectForm(true);
        }
        #endregion

        #region 주요기관 (발령 대상) 버튼
        /// <summary>
        /// 주요기관 (발령 대상) 버튼 마우스왼쪽 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyDeptDestination(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.allDestinationFlag == true || this.orderGroupFlag == true || this.orderDistFlag)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue)
            {
                this.SetOrderDestinationButtonColorOff();

                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.orderAllDeptFlag = true;
                this.lstSelectedOrderIP.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.NetIdToString, 0, 0, 253, 255));
                this.ClearOrderKind();
                this.SetBtnChecked(selectBtn, true);
            }
            else
            {
                if (this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Remove(selectBtn);
                }

                this.orderAllDeptFlag = false;
                this.lstSelectedOrderIP.Remove(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.NetIdToString, 0, 0, 253, 255));
            }
        }

        /// <summary>
        /// 시군 또는 개별단말 (발령 대상) 버튼 마우스오른쪽 동작
        /// </summary>
        /// <param name="selectBtn"></param>
        private void CheckKeyDeptDestinationRight(NCasButton selectBtn)
        {
            if (this.selectedOrderMode == NCasDefineOrderMode.None || this.selectedOrderMedia == NCasDefineOrderMedia.None
                || this.allDestinationFlag == true || this.orderGroupFlag == true || this.orderDistFlag == true || this.orderAllDeptFlag == true)
            {
                this.wrongOperationFlag = true;
            }

            if (this.wrongOperationFlag)
            {
                NCasAnimator.AddItem(this.wrongOperationBtn);

                if (selectBtn.CheckedValue == true)
                {
                    this.SetBtnChecked(selectBtn, true);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, false);
                }

                return;
            }

            this.distSelectListView.Items.Clear();

            foreach (PDeptInfo eachTermInfo in this.provInfo.LstDepts)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToString;
                lvi.Tag = eachTermInfo;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.IpAddrToString;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.ProvInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.distSelectListView.Items.Add(lvi);
            }

            this.preStepBtn.Visible = false;
            this.btnAllTermSelect.Visible = false;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }

                    this.SetBtnChecked(ncasBtn, false);

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (!(btnKeyData.KeyActioin == NCasKeyAction.Real || btnKeyData.KeyActioin == NCasKeyAction.Test ||
                    btnKeyData.KeyActioin == NCasKeyAction.Line || btnKeyData.KeyActioin == NCasKeyAction.Sate))
                {
                    if (NCasAnimator.ContainsItem(ncasBtn))
                    {
                        NCasAnimator.RemoveItem(ncasBtn);
                    }

                    this.SetBtnChecked(ncasBtn, false);

                    if (this.lstSelectedButtons.Contains(ncasBtn))
                    {
                        this.lstSelectedButtons.Remove(ncasBtn);
                    }
                }
            }

            this.lstSelectedOrderIP.Clear();
            this.selectedOrderKind = NCasDefineOrderKind.None;
            this.selectedDisasterBroadKind = DisasterBroadKind.None;
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
                    this.SetBtnChecked(selectBtn, false);
                }

                return;
            }

            NCasKeyData keyData = (NCasKeyData)selectBtn.Tag;

            if (keyData.KeyActioin == NCasKeyAction.Confirm)
            {
                //듀얼로 버튼 데이터 전송..다른 작업을 할 수도 있으니 일단 나눠놓자..
                if (keyData.KeyActioin == NCasKeyAction.Real)
                {
                    keyData.KeyStatus = (selectBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }
                else if (keyData.KeyActioin == NCasKeyAction.TtsOrder)
                {
                    keyData.KeyStatus = (selectBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }
                else if (keyData.KeyActioin == NCasKeyAction.MsgOrder)
                {
                    keyData.KeyStatus = (selectBtn.CheckedValue == true) ? NCasKeyState.Check : NCasKeyState.UnCheck;
                }

                this.main.SetKeyBizData(keyData);
            }

            bool rst = this.Confirm();
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
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.DistOneDestination
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.TermDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                        this.SetBtnChecked(btn, true);
                    }
                }
            }

            this.lastOrderKind = this.selectedOrderKind;
            this.lastDisasterBroadKind = this.selectedDisasterBroadKind;

            if (this.selectedDisasterBroadKind == DisasterBroadKind.StroredMessage)
            {
                if (this.orderStoredViewForm != null)
                {
                    this.orderStoredViewForm.StoredMsgFinishEvent -= new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
                    this.orderStoredViewForm.Close();
                    this.orderStoredViewForm = null;
                }

                this.orderStoredViewForm = new OrderStoredViewForm();
                this.orderStoredViewForm.StoredMsgFinishEvent += new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
                this.orderStoredViewForm.StartSirenForm(this.selectedStoredMessage.PlayTime);
                this.orderStoredViewForm.Show(this);
            }
            else
            {
                if (this.orderStoredViewForm != null)
                {
                    this.orderStoredViewForm.StoredMsgFinishEvent -= new EventHandler(orderStoredViewForm_StoredMsgFinishEvent);
                    this.orderStoredViewForm.Close();
                    this.orderStoredViewForm = null;
                }
            }

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }
            else if (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel)
            {
                if (this.orderAllDeptFlag == false && this.orderDeptFlag == false)
                {
                    if (this.orderTermFlag)
                    {
                        foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                        {
                            eachItem.Checked = false;
                        }

                        this.distSelectListView.Refresh();
                    }

                    if (orderGroupFlag)
                    {
                        this.orderStandbyFlag = false;
                        this.SetOrderDestinationButtonColorOff();
                        this.lstSelectedOrderIP.Clear();
                        this.selectedOrderKind = NCasDefineOrderKind.None;
                        this.allDestinationFlag = false;
                        this.orderGroupFlag = false;
                        this.orderDistFlag = false;
                        this.orderTermFlag = false;
                        this.ShowDistDetailSelectForm(false);
                        this.groupNameLst.Clear();
                        List<NCasButton> deleteBtn = new List<NCasButton>();

                        foreach (NCasButton ncasBtn in this.lstSelectedButtons)
                        {
                            NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                            if (btnKeyData.KeyActioin == NCasKeyAction.GroupDestination || btnKeyData.KeyActioin == NCasKeyAction.DistOneDestination)
                            {
                                this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                                this.SetBtnChecked(ncasBtn, false);
                                deleteBtn.Add(ncasBtn);
                            }
                        }

                        foreach (NCasButton eachBtn in deleteBtn)
                        {
                            if (this.lstSelectedButtons.Contains(eachBtn))
                            {
                                this.lstSelectedButtons.Remove(eachBtn);
                            }
                        }
                    }
                    else
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
                        this.ShowDistDetailSelectForm(false);
                        this.groupNameLst.Clear();
                    }
                }
                else
                {
                    this.SetOrderDestinationButtonColorOff();

                    if (this.orderDeptFlag)
                    {
                        foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                        {
                            eachItem.Checked = false;
                        }

                        this.distSelectListView.Refresh();
                    }

                    this.ShowDistDetailSelectForm(false);
                    this.lstSelectedOrderIP.Clear();
                }
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
                    this.SetBtnChecked(selectBtn, false);
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
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.DistOneDestination
                        || (btn.Tag as NCasKeyData).KeyActioin == NCasKeyAction.TermDestination)
                    {
                        this.SetOrderKindButtonColor(btn, this.selectedOrderKind);
                        this.SetBtnChecked(btn, true);
                    }
                }
            }

            this.lastOrderKind = this.selectedOrderKind;
            this.lastDisasterBroadKind = this.selectedDisasterBroadKind;

            if (this.lastOrderKind == NCasDefineOrderKind.AlarmStandby)
            {
                this.orderStandbyFlag = true;
            }
            else if (this.lastOrderKind == NCasDefineOrderKind.AlarmCancel)
            {
                if (this.orderAllDeptFlag == false && this.orderDeptFlag == false)
                {
                    if (this.orderTermFlag)
                    {
                        foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                        {
                            eachItem.Checked = false;
                        }

                        this.distSelectListView.Refresh();
                    }

                    if (orderGroupFlag)
                    {
                        this.orderStandbyFlag = false;
                        this.SetOrderDestinationButtonColorOff();
                        this.lstSelectedOrderIP.Clear();
                        this.selectedOrderKind = NCasDefineOrderKind.None;
                        this.allDestinationFlag = false;
                        this.orderGroupFlag = false;
                        this.orderDistFlag = false;
                        this.orderTermFlag = false;
                        this.ShowDistDetailSelectForm(false);
                        this.groupNameLst.Clear();
                        List<NCasButton> deleteBtn = new List<NCasButton>();

                        foreach (NCasButton ncasBtn in this.lstSelectedButtons)
                        {
                            NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                            if (btnKeyData.KeyActioin == NCasKeyAction.GroupDestination || btnKeyData.KeyActioin == NCasKeyAction.DistOneDestination)
                            {
                                this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                                this.SetBtnChecked(ncasBtn, false);
                                deleteBtn.Add(ncasBtn);
                            }
                        }

                        foreach (NCasButton eachBtn in deleteBtn)
                        {
                            if (this.lstSelectedButtons.Contains(eachBtn))
                            {
                                this.lstSelectedButtons.Remove(eachBtn);
                            }
                        }
                    }
                    else
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
                        this.ShowDistDetailSelectForm(false);
                        this.groupNameLst.Clear();
                    }
                }
                else
                {
                    this.SetOrderDestinationButtonColorOff();

                    if (this.orderDeptFlag)
                    {
                        foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                        {
                            eachItem.Checked = false;
                        }

                        this.distSelectListView.Refresh();
                    }

                    this.ShowDistDetailSelectForm(false);
                    this.lstSelectedOrderIP.Clear();
                }


                //기존꺼
                //if (this.orderAllDeptFlag == false && this.orderDeptFlag == false)
                //{
                //    this.orderStandbyFlag = false;
                //    this.SetOrderDestinationButtonColorOff();
                //    this.lstSelectedOrderIP.Clear();
                //    this.selectedOrderKind = NCasDefineOrderKind.None;
                //    this.allDestinationFlag = false;
                //    this.orderGroupFlag = false;
                //    this.orderDistFlag = false;
                //    this.orderTermFlag = false;
                //    this.SetOrderKindButtonColorProvAll(this.selectedOrderKind);
                //    this.SetOffButtonAny(NCasKeyAction.ProveAllDestination);
                //    this.SetOffButtonAny(NCasKeyAction.GroupDestination);
                //    this.SetOffButtonAny(NCasKeyAction.DistOneDestination);
                //}
                //else
                //{
                //    this.SetOrderDestinationButtonColorOff();
                //}
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
            else
            {
                this.SetBtnChecked(selectBtn, true);
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.selectedDisasterBroadKind = DisasterBroadKind.Tts;
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
            else
            {
                this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
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
                this.SetBtnChecked(selectBtn, false);
                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.selectedDisasterBroadKind = DisasterBroadKind.Tts;
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

                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
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
                this.SetBtnChecked(selectBtn, false);
                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.selectedDisasterBroadKind = DisasterBroadKind.StroredMessage;
                this.SetOffButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
            }
            else
            {
                this.SetBtnChecked(selectBtn, true);
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
                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
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
                    this.SetBtnChecked(selectBtn, false);
                }
                else
                {
                    this.SetBtnChecked(selectBtn, true);
                }

                return;
            }

            if (selectBtn.CheckedValue == true)
            {
                if (!this.lstSelectedButtons.Contains(selectBtn))
                {
                    this.lstSelectedButtons.Add(selectBtn);
                }

                this.selectedDisasterBroadKind = DisasterBroadKind.StroredMessage;
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

                this.selectedDisasterBroadKind = DisasterBroadKind.Mic;
                this.SetOnButton(NCasKeyAction.MicOrder);
                this.SetOffButton(NCasKeyAction.TtsOrder);
                this.SetOffButton(NCasKeyAction.MsgOrder);
            }
        }
        #endregion

        #region 방송종료 버튼
        /// <summary>
        /// 방송종료 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyBroadFinish(NCasButton selectBtn)
        {
            if (this.deptOrderBizData.Count == 0)
                return;

            DateTime orderTime = DateTime.Now;
            List<OrderBizData> tmpOrderBuff = new List<OrderBizData>();

            for (int i = 0; i < this.deptOrderBizData.Count; i++)
            {
                NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcAlarmOrder);
                NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;

                protoTc1.AlarmNetIdOrIpByString = this.deptOrderBizData[i].AlmProtocol.AlarmNetIdOrIpByString;
                protoTc1.BroadNetIdOrIpByString = this.deptOrderBizData[i].AlmProtocol.AlarmNetIdOrIpByString;
                protoTc1.OrderTimeByDateTime = orderTime;
                protoTc1.CtrlKind = NCasDefineControlKind.ControlAlarm;
                protoTc1.Source = NCasDefineOrderSource.ProvCtrlRoom;
                protoTc1.AlarmKind = NCasDefineOrderKind.AlarmClose;
                protoTc1.Mode = this.selectedOrderMode;
                protoTc1.Media = this.selectedOrderMedia;
                protoTc1.RespReqFlag = NCasDefineRespReq.ResponseReq;
                protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeNotUsed;
                protoTc1.Sector = this.deptOrderBizData[i].AlmProtocol.Sector;

                if (protoTc1.Mode == NCasDefineOrderMode.None)
                {
                    protoTc1.Mode = NCasDefineOrderMode.TestMode;
                }

                if (protoTc1.Media == NCasDefineOrderMedia.None)
                {
                    protoTc1.Media = NCasDefineOrderMedia.MediaAll;
                }

                if (protoTc1.Sector == NCasDefineSectionCode.None)
                {
                    protoTc1.Sector = NCasDefineSectionCode.SectionProv;
                }

                byte[] tmpBuff = NCasProtocolFactory.MakeUdpFrame(protoTc1);
                OrderBizData orderBizData = new OrderBizData();
                orderBizData.AllDestinationFlag = this.allDestinationFlag;
                orderBizData.AlmProtocol = protoTc1;
                orderBizData.IsLocal = true;
                orderBizData.LastOrderKind = this.lastOrderKind;
                orderBizData.OrderDistFlag = false;
                orderBizData.OrderGroupFlag = false;
                orderBizData.OrderTermFlag = true;
                orderBizData.SelectedDisasterBroadKind = DisasterBroadKind.None;
                orderBizData.SelectedStoredMessage = this.selectedStoredMessage;
                orderBizData.StoredMessageRepeatCount = this.storedMessageRepeatCount;
                orderBizData.SelectedTtsMessage = this.selectedTtsMessage;
                orderBizData.IsEnd = OrderDataSendStatus.FirstEnd;
                orderBizData.TtsOrderFlag = false;
                orderBizData.SendBuff = tmpBuff;
                tmpOrderBuff.Add(orderBizData);
            }

            foreach (OrderBizData eachOrderBizData in tmpOrderBuff)
            {
                this.main.SetOrderBizData(eachOrderBizData);
            }

            if (this.orderDeptFlag)
            {
                foreach (NCasListViewItem eachItem in this.distSelectListView.Items)
                {
                    eachItem.Checked = false;
                }

                this.distSelectListView.Refresh();
            }

            this.orderStandbyFlag = false;
            this.orderAllDeptFlag = false;
            this.orderDeptFlag = false;
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
            this.SetOffButtonAny(NCasKeyAction.TermDestination);
            this.ClearOrderKind();
            this.deptOrderBizData.Clear();

            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.TermDestination)
                {
                    this.SetBtnChecked(ncasBtn, false);
                    this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.TermDestination)
                {
                    this.SetBtnChecked(ncasBtn, false);
                    this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                }
            }

            this.ShowDistDetailSelectForm(false);
        }

        /// <summary>
        /// 듀얼시스템에서 수신받는 방송종료 버튼 동작
        /// </summary>
        /// <param name="?"></param>
        private void CheckKeyBroadFinishFromDual(NCasButton selectBtn)
        {
            this.orderStandbyFlag = false;
            this.orderAllDeptFlag = false;
            this.orderDeptFlag = false;
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
            this.SetOffButtonAny(NCasKeyAction.TermDestination);
            this.ClearOrderKind();

            foreach (NCasButton ncasBtn in this.orderViewTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.TermDestination)
                {
                    this.SetBtnChecked(ncasBtn, false);
                    this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                }
            }

            foreach (NCasButton ncasBtn in this.orderViewDistTableLayout.Controls)
            {
                NCasKeyData btnKeyData = (NCasKeyData)ncasBtn.Tag;

                if (btnKeyData.KeyActioin == NCasKeyAction.TermDestination)
                {
                    this.SetBtnChecked(ncasBtn, false);
                    this.SetOrderKindButtonColor(ncasBtn, NCasDefineOrderKind.None);
                }
            }
        }
        #endregion
    }
}