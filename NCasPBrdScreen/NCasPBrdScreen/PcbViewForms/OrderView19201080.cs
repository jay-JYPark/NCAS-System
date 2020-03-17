using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NCASBIZ.NCasDefine;
using NCASBIZ.NCasPlcProtocol;
using NCasAppCommon.Type;
using NCasAppCommon.Define;

namespace NCasPBrdScreen
{
    public partial class OrderView19201080 : ViewBase
    {
        private PBrdScreenUIController controller;

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
            this.controller = new PBrdScreenUIController(main);
            this.controller.orderViewTableLayout = this.orderViewTableLayout;

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

            this.controller.labelTotalTermCount = this.label1;
            this.controller.labelResponseTermCount = this.label2;
            this.controller.labelErrorTermCount = this.label3;
            this.controller.labelTotalBroadTime = this.label4;
            this.controller.orderTextBoard = this.nCasTextBoard1;
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
        /// 방송공유 버튼을 화면에 반영한다.
        /// </summary>
        /// <param name="check"></param>
        public void SetKeyBroadShare(bool check)
        {
            if (check)
            {
                this.controller.SetOnButtonPublic(NCasKeyAction.BroadShare);
                this.controller.BroadShareFlag = true;
            }
            else
            {
                this.controller.SetOffButtonAnyPublic(NCasKeyAction.BroadShare);
                this.controller.BroadShareFlag = false;
            }

            this.controller.SetBroadShareFromMega(check);
        }

        /// <summary>
        /// PLC로부터 데이터를 수신받아 화면에 반영한다.
        /// </summary>
        /// <param name="plcResponse"></param>
        public void SetKeyPlc(NCasPlcProtocolReqStatusResponse plcResponse)
        {
            this.controller.SetKeyPlc(plcResponse);
        }
    }
}