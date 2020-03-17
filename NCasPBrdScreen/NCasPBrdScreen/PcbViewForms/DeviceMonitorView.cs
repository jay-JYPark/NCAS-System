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
using NCASBIZ.NCasUtility;
using NCASBIZ.NCasDefine;

namespace NCasPBrdScreen
{
    public partial class DeviceMonitorView : ViewBase
    {
        private CentInfo centInfo = null;
        private CentInfo centInfo2 = null;
        private ProvInfo provInfo = null;

        /// <summary>
        /// 서버's 종류
        /// </summary>
        private enum DevKind
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,

            /// <summary>
            /// 중앙, 2중앙 서버
            /// </summary>
            CentDev = 1,

            /// <summary>
            /// 시도 서버
            /// </summary>
            ProvDev = 2,

            /// <summary>
            /// 방송 단말
            /// </summary>
            BroadDev = 3,

            /// <summary>
            /// 주요기관 단말
            /// </summary>
            DeptDev = 4
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public DeviceMonitorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main">MainForm에서 넘겨주는 참조</param>
        public DeviceMonitorView(MainForm main)
            : this()
        {
            this.main = main;
            this.centInfo = main.CentInfo;
            this.centInfo2 = main.CentInfo2;
            this.provInfo = main.ProvInfo;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitListView();
            this.SetServerListView();
            this.SetTermListView();
            this.main.AddTimerMember(this);
        }

        public override void OnTimer()
        {
            base.OnTimer();
            this.setServerStatusUpdate();
            this.setTermStatusUpdate();
        }

        #region ListView 초기화 및 셋팅
        /// <summary>
        /// 장비감시 ListView 초기화
        /// </summary>
        public void InitListView()
        {
            this.serverStatusListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.serverStatusListView.GridDashStyle = DashStyle.Dot;
            this.serverStatusListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.serverStatusListView.Font = new Font(NCasPBrdScreenRsc.FontName, 11.0f);
            this.serverStatusListView.ColumnHeight = 35;
            this.serverStatusListView.ItemHeight = 33;

            NCasColumnHeader col = new NCasColumnHeader();
            col.Text = "...";
            col.Width = 33;
            col.SortType = NCasListViewSortType.SortIcon;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            this.serverStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "장비명";
            col.Width = 250;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.serverStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.serverStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "상태";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.serverStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발생시각";
            col.Width = 230;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.serverStatusListView.Columns.Add(col);

            this.termStatusListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.termStatusListView.GridDashStyle = DashStyle.Dot;
            this.termStatusListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.termStatusListView.Font = new Font(NCasPBrdScreenRsc.FontName, 11.0f);
            this.termStatusListView.ColumnHeight = 35;
            this.termStatusListView.ItemHeight = 33;

            col = new NCasColumnHeader();
            col.Text = "...";
            col.Width = 33;
            col.SortType = NCasListViewSortType.SortIcon;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            this.termStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "장비명";
            col.Width = 250;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.termStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.termStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "상태";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.termStatusListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "발생시각";
            col.Width = 230;
            col.SortType = NCasListViewSortType.SortDateTime;
            col.TextAlign = HorizontalAlignment.Center;
            this.termStatusListView.Columns.Add(col);
        }

        /// <summary>
        /// 통제소 장비 ListView 셋팅
        /// </summary>
        private void SetServerListView()
        {
            NCasListViewItem lvi = null;

            //중앙
            foreach (CDevInfo eachCDevInfo in this.centInfo.LstDevs)
            {
                if (eachCDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys1 ||
                    eachCDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys2)
                {
                    if (eachCDevInfo.UseFlag != NCasDefineUseStatus.Use)
                        continue;

                    lvi = new NCasListViewItem();
                    lvi.Name = eachCDevInfo.IpAddrToString;
                    lvi.Tag = DevKind.CentDev;
                    lvi.ImageIndex = 0;

                    NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachCDevInfo.Name;
                    sub.TextAlign = HorizontalAlignment.Left;
                    lvi.SubItems.Add(sub);

                    sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachCDevInfo.IpAddrToString;
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

                    this.serverStatusListView.Items.Add(lvi);
                }
            }

            //2중앙
            foreach (CDevInfo eachCDevInfo in this.centInfo2.LstDevs)
            {
                if (eachCDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys1 ||
                    eachCDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys2)
                {
                    if (eachCDevInfo.UseFlag != NCasDefineUseStatus.Use)
                        continue;

                    lvi = new NCasListViewItem();
                    lvi.Name = eachCDevInfo.IpAddrToString;
                    lvi.Tag = DevKind.CentDev;
                    lvi.ImageIndex = 0;

                    NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachCDevInfo.Name;
                    sub.TextAlign = HorizontalAlignment.Left;
                    lvi.SubItems.Add(sub);

                    sub = new NCasListViewItem.NCasListViewSubItem();
                    sub.Text = eachCDevInfo.IpAddrToString;
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

                    this.serverStatusListView.Items.Add(lvi);
                }
            }

            //시도
            foreach (PDevInfo eachPDevInfo in this.provInfo.LstDevs)
            {
                if (eachPDevInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                if (eachPDevInfo.IpAddrToString == NCasUtilityMng.INCasEtcUtility.GetIPv4())
                    continue;

                lvi = new NCasListViewItem();
                lvi.Name = eachPDevInfo.IpAddrToString;
                lvi.Tag = DevKind.ProvDev;
                lvi.ImageIndex = 0;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachPDevInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = eachPDevInfo.IpAddrToString;
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

                this.serverStatusListView.Items.Add(lvi);
            }

            //광역시
            if (this.provInfo.IsMegaloPolis)
                return;

            foreach (ProvInfo eachProvInfo in this.provInfo.LstBroadRelatedProvInfo)
            {
                if (eachProvInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                foreach (PDevInfo eachPDevInfo in eachProvInfo.LstDevs)
                {
                    if (eachPDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys1 ||
                    eachPDevInfo.DevId == NCASBIZ.NCasDefine.NCasDefineDeviceKind.BroadCtrlSys2)
                    {
                        if (eachPDevInfo.UseFlag != NCasDefineUseStatus.Use)
                            continue;

                        lvi = new NCasListViewItem();
                        lvi.Name = eachPDevInfo.IpAddrToString;
                        lvi.Tag = DevKind.ProvDev;
                        lvi.ImageIndex = 0;

                        NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                        sub.Text = eachPDevInfo.Name;
                        sub.TextAlign = HorizontalAlignment.Left;
                        lvi.SubItems.Add(sub);

                        sub = new NCasListViewItem.NCasListViewSubItem();
                        sub.Text = eachPDevInfo.IpAddrToString;
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

                        this.serverStatusListView.Items.Add(lvi);
                    }
                }
            }
        }

        /// <summary>
        /// 방송단말 ListView 셋팅
        /// </summary>
        private void SetTermListView()
        {
            NCasListViewItem lvi = null;

            foreach (PBroadInfo eachTermInfo in this.provInfo.LstBroads)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                lvi = new NCasListViewItem();
                lvi.Tag = DevKind.BroadDev;
                lvi.Name = eachTermInfo.IpAddrToString;
                lvi.ImageIndex = 0;

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

                this.termStatusListView.Items.Add(lvi);
            }

            foreach (PDeptInfo eachTermInfo in this.provInfo.LstDepts)
            {
                if (eachTermInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                lvi = new NCasListViewItem();
                lvi.Tag = DevKind.DeptDev;
                lvi.Name = eachTermInfo.IpAddrToString;
                lvi.ImageIndex = 0;

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

                this.termStatusListView.Items.Add(lvi);
            }
        }
        #endregion

        #region ListView 업데이트
        /// <summary>
        /// 통제소 장비 ListView 업데이트
        /// </summary>
        private void setServerStatusUpdate()
        {
            foreach (NCasListViewItem listViewItem in this.serverStatusListView.Items)
            {
                if (listViewItem == null)
                    continue;

                if (listViewItem.Name == string.Empty)
                    continue;

                if ((DevKind)listViewItem.Tag == DevKind.CentDev)
                {
                    CDevInfo cDevInfo = this.main.MmfMng.GetCDevInfoByIp(listViewItem.Name);

                    if (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml)
                    {
                        listViewItem.ImageIndex = 1;
                    }
                    else
                    {
                        listViewItem.ImageIndex = 0;
                    }

                    listViewItem.SubItems[3].Text = (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.None) ? "없음" :
                        (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer) ? "무응답" :
                        (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) ? "이상" : "정상";

                    if ((cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) || (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer))
                    {
                        listViewItem.SubItems[4].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(cDevInfo.DevStsInfo.OccurTimeToDateTime);
                    }
                    else if ((cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml) || (cDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.None))
                    {
                        listViewItem.SubItems[4].Text = string.Empty;
                    }
                }
                else if ((DevKind)listViewItem.Tag == DevKind.ProvDev)
                {
                    PDevInfo pDevInfo = this.main.MmfMng.GetPDevInfoByIp(listViewItem.Name);

                    if (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml)
                    {
                        listViewItem.ImageIndex = 1;
                    }
                    else
                    {
                        listViewItem.ImageIndex = 0;
                    }

                    listViewItem.SubItems[3].Text = (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.None) ? "없음" :
                        (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer) ? "무응답" :
                        (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) ? "이상" : "정상";

                    if ((pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) || (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer))
                    {
                        listViewItem.SubItems[4].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pDevInfo.DevStsInfo.OccurTimeToDateTime);
                    }
                    else if ((pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml) || (pDevInfo.DevStsInfo.Status == NCasDefineNormalStatus.None))
                    {
                        listViewItem.SubItems[4].Text = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 방송단말 ListView 업데이트
        /// </summary>
        private void setTermStatusUpdate()
        {
            foreach (NCasListViewItem listViewItem in this.termStatusListView.Items)
            {
                if (listViewItem == null)
                    continue;

                if (listViewItem.Name == string.Empty)
                    continue;

                if ((DevKind)listViewItem.Tag == DevKind.BroadDev)
                {
                    PBroadInfo pBroadInfo = this.main.MmfMng.GetProvBroadInfoByIp(listViewItem.Name);

                    if (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml)
                    {
                        listViewItem.ImageIndex = 1;
                    }
                    else
                    {
                        listViewItem.ImageIndex = 0;
                    }

                    listViewItem.SubItems[3].Text = (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.None) ? "없음" :
                        (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer) ? "무응답" :
                        (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) ? "이상" : "정상";

                    if ((pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) || (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer))
                    {
                        listViewItem.SubItems[4].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.DevStsInfo.OccurTimeToDateTime);
                    }
                    else if ((pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml) || (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.None))
                    {
                        listViewItem.SubItems[4].Text = string.Empty;
                    }
                }
                else if ((DevKind)listViewItem.Tag == DevKind.DeptDev)
                {
                    PDeptInfo pBroadInfo = this.main.MmfMng.GetProvDeptInfoByIp(listViewItem.Name);

                    if (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml)
                    {
                        listViewItem.ImageIndex = 1;
                    }
                    else
                    {
                        listViewItem.ImageIndex = 0;
                    }

                    listViewItem.SubItems[3].Text = (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.None) ? "없음" :
                        (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer) ? "무응답" :
                        (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) ? "이상" : "정상";

                    if ((pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Abnormal) || (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.NoAnswer))
                    {
                        listViewItem.SubItems[4].Text = NCasUtilityMng.INCasCommUtility.MakeDateTimeFormatForCasSystem(pBroadInfo.DevStsInfo.OccurTimeToDateTime);
                    }
                    else if ((pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.Noraml) || (pBroadInfo.DevStsInfo.Status == NCasDefineNormalStatus.None))
                    {
                        listViewItem.SubItems[4].Text = string.Empty;
                    }
                }
            }
        }
        #endregion
    }
}