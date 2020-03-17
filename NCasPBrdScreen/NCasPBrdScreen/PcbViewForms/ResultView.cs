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
using NCASBIZ.NCasData;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasUtility;

namespace NCasPBrdScreen
{
    public partial class ResultView : ViewBase
    {
        private ProvInfo provInfo = null;

        /// <summary>
        /// 생성자
        /// </summary>
        public ResultView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main">MainForm에서 넘겨주는 참조</param>
        public ResultView(MainForm main)
            : this()
        {
            this.main = main;
            this.provInfo = main.ProvInfo;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitListView();
            this.SetTermListView();
            this.main.AddTimerMember(this);
        }

        public override void OnTimer()
        {
            base.OnTimer();
            this.setTermStatusUpdate();
        }

        #region ListView 초기화
        /// <summary>
        /// 발령결과 ListView 초기화
        /// </summary>
        public void InitListView()
        {
            this.orderResultListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.orderResultListView.GridDashStyle = DashStyle.Dot;
            this.orderResultListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.orderResultListView.Font = new Font(NCasPBrdScreenRsc.FontName, 11.0f);
            this.orderResultListView.ColumnHeight = 32;
            this.orderResultListView.ItemHeight = 29;

            NCasColumnHeader col = new NCasColumnHeader();
            col.Text = "...";
            col.Width = 33;
            col.SortType = NCasListViewSortType.SortIcon;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "장비명";
            col.Width = 180;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "모드";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령원";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "경보종류";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령매체";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발령시각";
            col.Width = 200;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "응답시각";
            col.Width = 200;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.orderResultListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "문자방송";
            col.Width = 200;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.orderResultListView.Columns.Add(col);
        }
        #endregion

        #region ListView 셋팅
        /// <summary>
        /// 발령결과 ListView 셋팅
        /// </summary>
        private void SetTermListView()
        {
            NCasListViewItem lvi = null;

            //시도
            foreach (PBroadInfo eachTermInfo in this.provInfo.LstBroads)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                lvi = new NCasListViewItem();
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
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                this.orderResultListView.Items.Add(lvi);
            }

            //주요기관
            foreach (PDeptInfo eachTermInfo in this.provInfo.LstDepts)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                lvi = new NCasListViewItem();
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
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                this.orderResultListView.Items.Add(lvi);
            }

            //광역시
            if (this.provInfo.IsMegaloPolis)
                return;

            foreach (ProvInfo eachProvInfo in this.provInfo.LstBroadRelatedProvInfo)
            {
                if (eachProvInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                foreach (PBroadInfo eachPBroadInfo in eachProvInfo.LstBroads)
                {
                    if (eachPBroadInfo.UseFlag != NCasDefineUseStatus.Use)
                        continue;

                    lvi = new NCasListViewItem();
                    lvi.Name = eachPBroadInfo.IpAddrToString;
                    lvi.Tag = eachPBroadInfo;

                    NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachPBroadInfo.Name;
                    sub.TextAlign = HorizontalAlignment.Left;
                    lvi.SubItems.Add(sub);

                    sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachPBroadInfo.IpAddrToString;
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
                    sub.TextAlign = HorizontalAlignment.Left;
                    lvi.SubItems.Add(sub);

                    this.orderResultListView.Items.Add(lvi);
                }
            }
        }
        #endregion

        #region ListView 업데이트
        /// <summary>
        /// 발령결과 ListView 업데이트
        /// </summary>
        private void setTermStatusUpdate()
        {
            foreach (NCasListViewItem listViewItem in this.orderResultListView.Items)
            {
                if (listViewItem == null)
                    continue;

                if (listViewItem.Name == string.Empty)
                    continue;

                if (listViewItem.Tag is PBroadInfo)
                {
                    PBroadInfo pBroadInfo = this.main.MmfMng.GetProvBroadInfoByIp(listViewItem.Name);

                    listViewItem.ImageIndex = (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? 0 : //예비
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? 1 : //경계
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? 2 : //공습
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? 3 : //화생방
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? 4 : //해제
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? 5 : //재난위험(사이렌)
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? 6 : 4; //재난경계(방송)

                    listViewItem.SubItems[3].Text = (pBroadInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.RealMode) :
                        (pBroadInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.TestMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.TestMode) : string.Empty;

                    listViewItem.SubItems[4].Text = (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom2) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom2) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom3) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom3) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.DistCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.DistCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvBroadSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvBroadSelf) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvDistribution) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvDistribution) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvTermSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvTermSelf) : string.Empty;

                    listViewItem.SubItems[5].Text = (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.None) : string.Empty;

                    listViewItem.SubItems[6].Text = (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaAll) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaAll) :
                        (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaLine) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaLine) :
                        (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaSate) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaSate) : string.Empty;

                    listViewItem.SubItems[7].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.BroadOrderInfo.OccurTimeToDateTime);

                    if (pBroadInfo.BroadResponseInfo.BroadResponse == NCasDefineResponse.None)
                    {
                        listViewItem.SubItems[8].Text = string.Empty;
                    }
                    else
                    {
                        listViewItem.SubItems[8].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.BroadResponseInfo.RespTimeToDateTime);
                    }

                    if (pBroadInfo.BroadOrderInfo.OccurTime == pBroadInfo.BroadCaptionOrderInfo.OccurTime)
                    {
                        listViewItem.SubItems[9].Text = (pBroadInfo.BroadCaptionResultInfo.BroadResult == NCasDefineNormalStatus.Noraml) ?
                            NCasUtilityMng.INCasCommUtility.NCasDefineNormalStatus2String(NCasDefineNormalStatus.Noraml) : NCasUtilityMng.INCasCommUtility.NCasDefineNormalStatus2String(NCasDefineNormalStatus.Abnormal);
                    }
                    else
                    {
                        listViewItem.SubItems[9].Text = string.Empty;
                    }
                }
                else if (listViewItem.Tag is PDeptInfo)
                {
                    PDeptInfo pBroadInfo = this.main.MmfMng.GetProvDeptInfoByIp(listViewItem.Name);

                    listViewItem.ImageIndex = (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? 0 : //예비
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? 1 : //경계
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? 2 : //공습
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? 3 : //화생방
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? 4 : //해제
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? 5 : //재난위험(사이렌)
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? 6 : 4; //재난경계(방송)

                    listViewItem.SubItems[3].Text = (pBroadInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.RealMode) :
                        (pBroadInfo.BroadOrderInfo.Mode == NCasDefineOrderMode.TestMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.TestMode) : string.Empty;

                    listViewItem.SubItems[4].Text = (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom2) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom2) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom3) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom3) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.DistCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.DistCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvBroadSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvBroadSelf) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvCtrlRoom) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvDistribution) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvDistribution) :
                                (pBroadInfo.BroadOrderInfo.Source == NCasDefineOrderSource.ProvTermSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvTermSelf) : string.Empty;

                    listViewItem.SubItems[5].Text = (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) :
                        (pBroadInfo.BroadOrderInfo.Kind == NCasDefineOrderKind.None) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.None) : string.Empty;

                    listViewItem.SubItems[6].Text = (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaAll) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaAll) :
                        (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaLine) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaLine) :
                        (pBroadInfo.BroadOrderInfo.Media == NCasDefineOrderMedia.MediaSate) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaSate) : string.Empty;

                    listViewItem.SubItems[7].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.BroadOrderInfo.OccurTimeToDateTime);

                    if (pBroadInfo.BroadResponseInfo.BroadResponse == NCasDefineResponse.None)
                    {
                        listViewItem.SubItems[8].Text = string.Empty;
                    }
                    else
                    {
                        listViewItem.SubItems[8].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.BroadResponseInfo.RespTimeToDateTime);
                    }

                    listViewItem.SubItems[9].Text = string.Empty;
                }
            }
        }
        #endregion
    }
}