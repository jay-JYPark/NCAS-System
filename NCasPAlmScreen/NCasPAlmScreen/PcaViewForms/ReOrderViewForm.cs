using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    public partial class ReOrderViewForm : Form
    {
        public enum OrderDataSendStatus
        {
            None = 0,
            First = 1,
            End = 2,
            FirstEnd = 3
        }

        private MainForm mainForm = null;
        private ProvInfo provInfo = null;
        private Timer timer = null;
        private AlarmResponseInfo lastResponseInfo = null;
        private AlarmOrderInfo lastAlarmOrderInfo = null;
        private AlarmOrderInfo compareAlarmOrderInfo = null;
        private DateTime lastOrderDateTime = DateTime.Now;

        /// <summary>
        /// 생성자
        /// </summary>
        public ReOrderViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mainForm"></param>
        public ReOrderViewForm(MainForm mainForm)
            : this()
        {
            this.mainForm = mainForm;
            this.provInfo = mainForm.ProvInfo;
            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.SetTermListView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitListView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (this.timer != null)
            {
                this.timer.Tick -= new EventHandler(timer_Tick);
                this.timer.Stop();
                this.timer = null;
            }
        }

        #region ListView 초기화
        /// <summary>
        /// ListView 초기화
        /// </summary>
        public void InitListView()
        {
            this.reOrderListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.reOrderListView.GridDashStyle = DashStyle.Dot;
            this.reOrderListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.reOrderListView.Font = new Font(NCasPAlmScreenRsc.FontName, 11.0f);
            this.reOrderListView.ColumnHeight = 32;
            this.reOrderListView.ItemHeight = 29;

            NCasColumnHeader col = new NCasColumnHeader();
            col.Text = "...";
            col.Width = 33;
            col.SortType = NCasListViewSortType.SortIcon;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "장비명";
            col.Width = 180;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "모드";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령원";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "경보종류";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령매체";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령시각";
            col.Width = 200;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "응답시각";
            col.Width = 0;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "시군구";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.reOrderListView.Columns.Add(col);
        }
        #endregion

        #region ListView 셋팅
        /// <summary>
        /// ListView 셋팅
        /// </summary>
        private void SetTermListView()
        {
            if (this.lastOrderDateTime == this.provInfo.AlarmResponseInfo.OccurTimeToDateTime)
                return;

            this.lastResponseInfo = this.provInfo.AlarmResponseInfo;
            this.compareAlarmOrderInfo = this.provInfo.AlarmOrderInfo;
            this.reOrderListView.Items.Clear();
            NCasListViewItem lvi = null;

            foreach (TermInfo eachTermInfo in this.provInfo.LstTerms)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                if (eachTermInfo.AlarmOrderInfo.OccurTimeToDateTime != this.compareAlarmOrderInfo.OccurTimeToDateTime)
                    continue;

                if (eachTermInfo.AlarmResponseInfo.AlarmResponse != NCasDefineResponse.None)
                    continue;

                if (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast)
                    continue;

                lvi = new NCasListViewItem();
                lvi.Name = eachTermInfo.IpAddrToSring;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachTermInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = NCasUtilityMng.INCasCommUtility.UintIP2StringIPWithPadLeft((uint)eachTermInfo.IpAddr, 3);
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = string.Empty;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                lvi.ImageIndex = (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? 0 : //예비
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? 1 : //경계
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? 2 : //공습
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? 3 : //화생방
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? 4 : //해제
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? 5 : //재난위험(사이렌)
                    (eachTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? 6 : 4; //재난경계(방송)

                lvi.SubItems[3].Text = NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(eachTermInfo.AlarmResponseInfo.Mode);
                lvi.SubItems[4].Text = NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(eachTermInfo.AlarmResponseInfo.Source);
                lvi.SubItems[5].Text = NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(eachTermInfo.AlarmOrderInfo.Kind);
                lvi.SubItems[6].Text = NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(eachTermInfo.AlarmResponseInfo.Media);
                lvi.SubItems[7].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(eachTermInfo.AlarmOrderInfo.OccurTimeToDateTime);

                if (eachTermInfo.AlarmResponseInfo.AlarmResponse == NCasDefineResponse.None)
                {
                    lvi.SubItems[8].Text = string.Empty;
                }
                else
                {
                    lvi.SubItems[8].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(eachTermInfo.AlarmResponseInfo.RespTimeToDateTime);
                }

                lvi.SubItems[9].Text = eachTermInfo.DistInfo.Name;
                this.reOrderListView.Items.Add(lvi);
            }

            this.lastOrderDateTime = this.provInfo.AlarmResponseInfo.OccurTimeToDateTime;
        }
        #endregion

        /// <summary>
        /// 재발령 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReOrder_Click(object sender, EventArgs e)
        {
            if (this.reOrderListView.Items.Count == 0)
                return;

            if (MessageBox.Show(string.Format("현재 보이는 경보단말을 대상으로 '{0}' 발령을 진행하겠습니까?",
                NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String((this.mainForm.MmfMng.GetTermInfoByIp(this.reOrderListView.Items[0].Name)).AlarmOrderInfo.Kind)),
                "재발령", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.lastAlarmOrderInfo = this.provInfo.AlarmOrderInfo;
                this.lastAlarmOrderInfo.AlarmOrderFlag = this.provInfo.AlarmOrderInfo.AlarmOrderFlag;
                this.lastAlarmOrderInfo.CtrlKind = this.provInfo.AlarmOrderInfo.CtrlKind;
                this.lastAlarmOrderInfo.IpAddrAlarmArea = this.provInfo.AlarmOrderInfo.IpAddrAlarmArea;
                this.lastAlarmOrderInfo.IpAddrAlarmAreaToString = this.provInfo.AlarmOrderInfo.IpAddrAlarmAreaToString;
                this.lastAlarmOrderInfo.IpAddrBraodAreaToString = this.provInfo.AlarmOrderInfo.IpAddrBraodAreaToString;
                this.lastAlarmOrderInfo.IpAddrBroadArea = this.provInfo.AlarmOrderInfo.IpAddrBroadArea;
                this.lastAlarmOrderInfo.Kind = this.provInfo.AlarmOrderInfo.Kind;
                this.lastAlarmOrderInfo.Media = this.provInfo.AlarmOrderInfo.Media;
                this.lastAlarmOrderInfo.Mode = this.provInfo.AlarmOrderInfo.Mode;
                this.lastAlarmOrderInfo.OccurTime = this.provInfo.AlarmOrderInfo.OccurTime;
                this.lastAlarmOrderInfo.OccurTimeToDateTime = this.provInfo.AlarmOrderInfo.OccurTimeToDateTime;
                this.lastAlarmOrderInfo.RespReq = this.provInfo.AlarmOrderInfo.RespReq;
                this.lastAlarmOrderInfo.Section = this.provInfo.AlarmOrderInfo.Section;
                this.lastAlarmOrderInfo.Source = this.provInfo.AlarmOrderInfo.Source;

                List<OrderBizData> tmpOrderBuff = new List<OrderBizData>();
                DateTime orderDateTime = DateTime.Now;

                for (int i = 0; i < this.reOrderListView.Items.Count; i++)
                {
                    NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcAlarmOrder);
                    NCasProtocolTc1 protoTc1 = protoBase as NCasProtocolTc1;

                    protoTc1.AlarmNetIdOrIpByString = this.reOrderListView.Items[i].Name;
                    protoTc1.OrderTimeByDateTime = orderDateTime;
                    protoTc1.CtrlKind = NCasDefineControlKind.ControlAlarm;
                    protoTc1.Source = NCasDefineOrderSource.ProvCtrlRoom;
                    protoTc1.AlarmKind = this.lastAlarmOrderInfo.Kind;
                    protoTc1.Mode = this.lastAlarmOrderInfo.Mode;
                    protoTc1.Media = this.lastAlarmOrderInfo.Media;
                    protoTc1.RespReqFlag = NCasDefineRespReq.ResponseReq;
                    protoTc1.AuthenFlag = NCasDefineAuthenticationFlag.EncodeUsed;
                    protoTc1.Sector = NCasDefineSectionCode.SectionTerm;

                    byte[] tmpBuff = NCasProtocolFactory.MakeUdpFrame(protoTc1);
                    OrderBizData orderBizData = new OrderBizData();
                    orderBizData.AllDestinationFlag = false;
                    orderBizData.AlmProtocol = protoTc1;
                    orderBizData.IsLocal = true;
                    orderBizData.LastOrderKind = this.lastAlarmOrderInfo.Kind;
                    orderBizData.OrderDistFlag = false;
                    orderBizData.OrderGroupFlag = false;
                    orderBizData.OrderTermFlag = true;
                    orderBizData.SendBuff = tmpBuff;
                    orderBizData.SelectedDisasterBroadKind = PAlmScreenUIController.DisasterBroadKind.None;
                    orderBizData.SelectedStoredMessage = new StoredMessageText();
                    orderBizData.StoredMessageRepeatCount = 1;
                    orderBizData.SelectedTtsMessage = new TtsMessageText();
                    orderBizData.GroupName = new List<string>();
                    orderBizData.TtsOrderFlag = false;

                    if (i == 0)
                    {
                        orderBizData.IsEnd = PAlmScreenUIController.OrderDataSendStatus.First;
                    }
                    else if (i == (this.reOrderListView.Items.Count - 1))
                    {
                        orderBizData.IsEnd = PAlmScreenUIController.OrderDataSendStatus.End;
                    }
                    else
                    {
                        orderBizData.IsEnd = PAlmScreenUIController.OrderDataSendStatus.None;
                    }

                    if (this.reOrderListView.Items.Count == 1)
                    {
                        orderBizData.IsEnd = PAlmScreenUIController.OrderDataSendStatus.FirstEnd;
                    }

                    tmpOrderBuff.Add(orderBizData);
                }

                foreach (OrderBizData orderBizData in tmpOrderBuff)
                {
                    this.mainForm.SetOrderBizData(orderBizData);
                }
            }
        }

        /// <summary>
        /// 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}