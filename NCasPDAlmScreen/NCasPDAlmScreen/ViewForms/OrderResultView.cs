﻿using System;
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

namespace NCasPDAlmScreen
{
    public partial class OrderResultView : ViewBase
    {
        private ProvInfo provInfo = null;

        /// <summary>
        /// 생성자
        /// </summary>
        public OrderResultView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main">MainForm에서 넘겨주는 참조</param>
        public OrderResultView(MainForm main)
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
            this.orderResultListView.Font = new Font(NCasPDAlmScreenRsc.FontName, 11.0f);
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
        }
        #endregion

        #region ListView 셋팅
        /// <summary>
        /// 발령결과 ListView 셋팅
        /// </summary>
        private void SetTermListView()
        {
            NCasListViewItem lvi = null;

            foreach (TermInfo eachTermInfo in this.provInfo.LstTerms)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                lvi = new NCasListViewItem();
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

                this.orderResultListView.Items.Add(lvi);
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

                TermInfo pTermInfo = this.main.MmfMng.GetTermInfoByIp(listViewItem.Name);

                listViewItem.ImageIndex = (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? 0 : //예비
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? 1 : //경계
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? 2 : //공습
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? 3 : //화생방
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? 4 : //해제
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? 5 : //재난위험(사이렌)
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? 6 : 4; //재난경계(방송)

                listViewItem.SubItems[3].Text = (pTermInfo.AlarmOrderInfo.Mode == NCasDefineOrderMode.RealMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.RealMode) :
                    (pTermInfo.AlarmOrderInfo.Mode == NCasDefineOrderMode.TestMode) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMode2String(NCasDefineOrderMode.TestMode) : "";

                listViewItem.SubItems[4].Text = (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom2) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom2) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.CentCtrlRoom3) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.CentCtrlRoom3) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.DistCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.DistCtrlRoom) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.ProvBroadSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvBroadSelf) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.ProvCtrlRoom) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvCtrlRoom) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.ProvDistribution) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvDistribution) :
                            (pTermInfo.AlarmOrderInfo.Source == NCasDefineOrderSource.ProvTermSelf) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.ProvTermSelf) : "";

                listViewItem.SubItems[5].Text = (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmStandby) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmWatch) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmAttack) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmAttack) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmBiochemist) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmBiochemist) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmCancel) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmCancel) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmClose) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmClose) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterWatch) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterWatch) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.VoiceLineTest) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.VoiceLineTest) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.BroadPublicVoice) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadPublicVoice) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.BroadMessage) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.BroadMessage) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.TermTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.TermTts) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.CenterTts) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.CenterTts) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.AlarmRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.AlarmRecover) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterBoradRecover) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBoradRecover) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterStandby) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterStandby) :
                    (pTermInfo.AlarmOrderInfo.Kind == NCasDefineOrderKind.DisasterBroadcast) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderKind2String(NCasDefineOrderKind.DisasterBroadcast) : "";

                listViewItem.SubItems[6].Text = (pTermInfo.AlarmOrderInfo.Media == NCasDefineOrderMedia.MediaAll) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaAll) :
                    (pTermInfo.AlarmOrderInfo.Media == NCasDefineOrderMedia.MediaLine) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaLine) :
                    (pTermInfo.AlarmOrderInfo.Media == NCasDefineOrderMedia.MediaSate) ? NCasUtilityMng.INCasCommUtility.NCasDefineOrderMedia2String(NCasDefineOrderMedia.MediaSate) : "";

                listViewItem.SubItems[7].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pTermInfo.AlarmOrderInfo.OccurTimeToDateTime);

                if (pTermInfo.AlarmResponseInfo.AlarmResponse == NCasDefineResponse.None)
                {
                    listViewItem.SubItems[8].Text = string.Empty;
                }
                else
                {
                    listViewItem.SubItems[8].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pTermInfo.AlarmResponseInfo.RespTimeToDateTime);
                }
            }
        }
        #endregion
    }
}