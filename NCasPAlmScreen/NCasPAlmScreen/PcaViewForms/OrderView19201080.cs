using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NCasAppCommon.Type;
using NCASBIZ.NCasPlcProtocol;
using NCASBIZ.NCasDefine;

namespace NCasPAlmScreen
{
    public partial class OrderView19201080 : ViewBase
    {
        private PAlmScreenUIController controller;

        /// <summary>
        /// 생성자
        /// </summary>
        public OrderView19201080()
        {
            InitializeComponent();
        }

        /// <summary>
        /// override method
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000; //Turn off WS_CLIPCHILDREN
                return parms;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main"></param>
        public OrderView19201080(MainForm main)
            : this()
        {
            this.controller = new PAlmScreenUIController(main);
            this.controller.orderTextBoard = this.orderTextBoard;
            this.controller.labelResponseTermCount = this.labelResponseTermCount;
            this.controller.labelErrorTermCount = this.labelErrorTermCount;
            this.controller.labelTotalTermCount = this.labelTotalTermCount;
            this.controller.labelConErrorTermCount = this.labelConErrorTermCount;
            this.controller.orderViewTableLayout = this.orderViewTableLayout;
            this.controller.orderViewDistTableLayout = this.orderViewDistTableLayout;

            this.controller.btnRC11 = this.btnRC11;
            this.controller.btnRC21 = this.btnRC21;
            this.controller.btnRC31 = this.btnRC31;
            this.controller.btnRC41 = this.btnRC41;
            this.controller.btnRC51 = this.btnRC51;
            this.controller.btnRC61 = this.btnRC61;
            this.controller.btnRC71 = this.btnRC71;
            this.controller.btnRC81 = this.btnRC81;

            this.controller.btnRC12 = this.btnRC12;
            this.controller.btnRC22 = this.btnRC22;
            this.controller.btnRC32 = this.btnRC32;
            this.controller.btnRC42 = this.btnRC42;
            this.controller.btnRC52 = this.btnRC52;
            this.controller.btnRC62 = this.btnRC62;
            this.controller.btnRC72 = this.btnRC72;
            this.controller.btnRC82 = this.btnRC82;

            this.controller.btnRC13 = this.btnRC13;
            this.controller.btnRC23 = this.btnRC23;
            this.controller.btnRC33 = this.btnRC33;
            this.controller.btnRC43 = this.btnRC43;
            this.controller.btnRC53 = this.btnRC53;
            this.controller.btnRC63 = this.btnRC63;
            this.controller.btnRC73 = this.btnRC73;
            this.controller.btnRC83 = this.btnRC83;

            this.controller.btnRC14 = this.btnRC14;
            this.controller.btnRC24 = this.btnRC24;
            this.controller.btnRC34 = this.btnRC34;
            this.controller.btnRC44 = this.btnRC44;
            this.controller.btnRC54 = this.btnRC54;
            this.controller.btnRC64 = this.btnRC64;
            this.controller.btnRC74 = this.btnRC74;
            this.controller.btnRC84 = this.btnRC84;

            this.controller.btnRC15 = this.btnRC15;
            this.controller.btnRC25 = this.btnRC25;
            this.controller.btnRC35 = this.btnRC35;
            this.controller.btnRC45 = this.btnRC45;
            this.controller.btnRC55 = this.btnRC55;
            this.controller.btnRC65 = this.btnRC65;
            this.controller.btnRC75 = this.btnRC75;
            this.controller.btnRC85 = this.btnRC85;

            this.controller.btnRC16 = this.btnRC16;
            this.controller.btnRC26 = this.btnRC26;
            this.controller.btnRC36 = this.btnRC36;
            this.controller.btnRC46 = this.btnRC46;
            this.controller.btnRC56 = this.btnRC56;
            this.controller.btnRC66 = this.btnRC66;
            this.controller.btnRC76 = this.btnRC76;
            this.controller.btnRC86 = this.btnRC86;

            this.controller.btnRC17 = this.btnRC17;
            this.controller.btnRC27 = this.btnRC27;
            this.controller.btnRC37 = this.btnRC37;
            this.controller.btnRC47 = this.btnRC47;
            this.controller.btnRC57 = this.btnRC57;
            this.controller.btnRC67 = this.btnRC67;
            this.controller.btnRC77 = this.btnRC77;
            this.controller.btnRC87 = this.btnRC87;

            this.controller.btnRC18 = this.btnRC18;
            this.controller.btnRC28 = this.btnRC28;
            this.controller.btnRC38 = this.btnRC38;
            this.controller.btnRC48 = this.btnRC48;
            this.controller.btnRC58 = this.btnRC58;
            this.controller.btnRC68 = this.btnRC68;
            this.controller.btnRC78 = this.btnRC78;
            this.controller.btnRC88 = this.btnRC88;

            this.controller.btnRC19 = this.btnRC19;
            this.controller.btnRC29 = this.btnRC29;
            this.controller.btnRC39 = this.btnRC39;
            this.controller.btnRC49 = this.btnRC49;
            this.controller.btnRC59 = this.btnRC59;
            this.controller.btnRC69 = this.btnRC69;
            this.controller.btnRC79 = this.btnRC79;
            this.controller.btnRC89 = this.btnRC89;

            this.controller.btnRC110 = this.btnRC110;
            this.controller.btnRC210 = this.btnRC210;
            this.controller.btnRC310 = this.btnRC310;
            this.controller.btnRC410 = this.btnRC410;
            this.controller.btnRC510 = this.btnRC510;
            this.controller.btnRC610 = this.btnRC610;
            this.controller.btnRC710 = this.btnRC710;
            this.controller.btnRC810 = this.btnRC810;

            this.controller.distSelectPanel = this.distSelectPanel;
            this.controller.distSelectListView = this.distSelectListView;
            this.controller.preStepBtn = this.preStepBtn;
            this.controller.btnAllTermSelect = this.btnAllTermSelect;
            this.controller.btnTermSelectCancel = this.btnTermSelectCancel;

            this.controller.InitButton();
            this.controller.InitEtc();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// 듀얼시스템에서 수신받은 키 데이터를 화면에 반영한다.
        /// </summary>
        /// <param name="keyData"></param>
        public void SetKeyDataFromDual(NCasKeyData keyData)
        {
            this.controller.SetKeyDataFromDual(keyData);
        }

        /// <summary>
        /// PLC로부터 데이터를 수신받아 화면에 반영한다.
        /// </summary>
        /// <param name="plcResponse"></param>
        public void SetKeyPlc(NCasPlcProtocolReqStatusResponse plcResponse)
        {
            this.controller.SetKeyPlc(plcResponse);
        }

        /// <summary>
        /// 중앙/2중앙 발령 승계 기능
        /// </summary>
        /// <param name="ready"></param>
        public void SetCenterAlarmOrder(bool ready)
        {
            if (ready)
            {
                this.controller.OrderStandbyFlag = true;
            }
            else
            {
                this.controller.OrderStandbyFlag = false;
            }
        }
    }
}