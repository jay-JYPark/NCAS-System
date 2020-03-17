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
using NCASBIZ.NCasData;
using NCASBIZ.NCasDefine;
using NCasAppCommon.Type;

namespace NCasPAlmScreen
{
    public partial class OrderGroupEditForm : Form
    {
        #region element
        private PAlmScreenUIController orderView = null;
        private ProvInfo provInfo = null;
        private List<NCasButton> groupBtnList = new List<NCasButton>();
        private List<GroupContent> lstGroupContent = new List<GroupContent>();
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public OrderGroupEditForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="PAlmScreenUIController"></param>
        public OrderGroupEditForm(PAlmScreenUIController orderView)
            : this()
        {
            this.orderView = orderView;
            this.provInfo = this.orderView.ProvInfo;
            this.Init();
        }
        #endregion

        #region Init
        /// <summary>
        /// 기초데이터 초기화
        /// </summary>
        private void Init()
        {
            this.groupBtnList = this.orderView.GetGroupButton();
            GroupContent content = null;

            foreach (NCasButton btn in this.groupBtnList)
            {
                NCasKeyData keyData = btn.Tag as NCasKeyData;
                GroupInfo groupInfo = new GroupInfo();
                groupInfo.Title = keyData.Name;
                groupInfo.ButtonID = keyData.ID.ToString();
                this.groupSelectComboBox.Items.Insert(0, groupInfo);

                content = new GroupContent();
                content.Title = keyData.ID.ToString();
                this.lstGroupContent.Add(content);
            }

            #region ListView 초기화
            this.groupMemberListView.GridLineStyle = NCasListViewGridLine.GridBoth;
            this.groupMemberListView.GridDashStyle = DashStyle.Dot;
            this.groupMemberListView.ScrollType = NCasListViewScrollType.ScrollBoth;
            this.groupMemberListView.Font = new Font(NCasPAlmScreenRsc.FontName, 11.0f);
            this.groupMemberListView.ColumnHeight = 32;
            this.groupMemberListView.ItemHeight = 29;
            this.groupMemberListView.HideColumnCheckBox = true;

            NCasColumnHeader col = new NCasColumnHeader();
            col.Text = string.Empty;
            col.Width = 30;
            col.SortType = NCasListViewSortType.SortChecked;
            col.TextAlign = HorizontalAlignment.Center;
            col.ColumnLock = true;
            col.ColumnHide = true;
            col.CheckBoxes = true;
            this.groupMemberListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "이름";
            col.Width = 200;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Left;
            this.groupMemberListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "IP";
            col.Width = 160;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.groupMemberListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "소속";
            col.Width = 150;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.groupMemberListView.Columns.Add(col);

            col = new NCasColumnHeader();
            col.Text = "구분";
            col.Width = 120;
            col.SortType = NCasListViewSortType.SortText;
            col.TextAlign = HorizontalAlignment.Center;
            this.groupMemberListView.Columns.Add(col);
            #endregion

            #region ListView Items 셋팅
            foreach (DistInfo distInfo in this.provInfo.LstDists)
            {
                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = distInfo.NetIdToString;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = distInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = distInfo.NetIdToString;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = this.provInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.NCasDefineOrderSource2String(NCasDefineOrderSource.DistCtrlRoom);
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.groupMemberListView.Items.Add(lvi);
            }

            foreach (TermInfo termInfo in this.provInfo.LstTerms)
            {
                if (termInfo.UseFlag != NCasDefineUseStatus.Use)
                    continue;

                NCasListViewItem lvi = new NCasListViewItem();
                lvi.Name = termInfo.IpAddrToSring;

                NCasListViewItem.NCasListViewSubItem sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = termInfo.Name;
                sub.TextAlign = HorizontalAlignment.Left;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = termInfo.IpAddrToSring;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = termInfo.DistInfo.Name;
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                sub = new NCasListViewItem.NCasListViewSubItem();
                sub.Text = "단말";
                sub.TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(sub);

                this.groupMemberListView.Items.Add(lvi);
            }
            #endregion
        }
        #endregion

        #region event method
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.groupSelectComboBox.SelectedItem == null)
            {
                MessageBox.Show("저장할 그룹을 선택하세요.", "그룹정보 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GroupInfo groupInfo = this.groupSelectComboBox.SelectedItem as GroupInfo;
            bool isHave = false;

            foreach (GroupContent groupContent in this.lstGroupContent)
            {
                if (groupContent.Title == groupInfo.ButtonID)
                {
                    foreach (GroupContent grpContent in GroupContentMng.LstGroupContent)
                    {
                        if (grpContent.Title == groupContent.Title)
                        {
                            isHave = true;
                            break;
                        }
                    }

                    if (isHave)
                    {
                        foreach (GroupContent grpContent in GroupContentMng.LstGroupContent)
                        {
                            if (grpContent.Title == groupContent.Title)
                            {
                                grpContent.LstGroupData = groupContent.LstGroupData;
                            }
                        }
                    }
                    else
                    {
                        GroupContentMng.LstGroupContent.Add(groupContent);
                    }
                }
            }

            GroupContentMng.SaveGroupContent();
            MessageBox.Show("그룹정보가 저장되었습니다.", "그룹정보 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// 그룹명 선택 콤보박스 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                NCasColumnHeader nCasColumnHeader = this.groupMemberListView.Columns[0];
                nCasColumnHeader.ColumnHide = false;

                for (int i = 0; i < this.groupMemberListView.Items.Count; i++)
                {
                    this.groupMemberListView.Items[i].Checked = false;
                }

                GroupContent tmpContent = new GroupContent();
                GroupInfo groupInfo = this.groupSelectComboBox.SelectedItem as GroupInfo;

                foreach (GroupContent content in GroupContentMng.LstGroupContent)
                {
                    if (content.Title == groupInfo.ButtonID) //화면에서 선택한 그룹의 ID에 해당되는 정보가 GroupContentMng에 있으면..
                    {
                        tmpContent = content;
                        break;
                    }
                }

                foreach (GroupContent groupContent in this.lstGroupContent)
                {
                    if (groupContent.Title == tmpContent.Title)
                    {
                        groupContent.LstGroupData = tmpContent.LstGroupData;
                        break;
                    }
                }

                if (tmpContent.LstGroupData.Count > 0)
                {
                    foreach (GroupData groupData in tmpContent.LstGroupData)
                    {
                        NCasListViewItem[] lvi = this.groupMemberListView.Items.Find(groupData.IpAddr, false);

                        foreach (NCasListViewItem eachLvi in lvi)
                        {
                            eachLvi.Checked = true;
                        }
                    }
                }

                this.groupMemberListView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("그룹정보를 다시 설정하세요.", "그룹정보 설정", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ListView ItemChecked 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupMemberListView_ItemChecked(object sender, NCasItemCheckedEventArgs e)
        {
            GroupInfo groupInfo = this.groupSelectComboBox.SelectedItem as GroupInfo;

            foreach (GroupContent groupContent in this.lstGroupContent)
            {
                if (groupContent.Title == groupInfo.ButtonID)
                {
                    NCasListViewItem[] lvi = this.groupMemberListView.Items.Find(e.Item.Name, false);
                    GroupData groupData = new GroupData();
                    groupData.IpAddr = e.Item.Name;
                    groupData.IsDist = (lvi[0].SubItems[4].Text == "단말") ? false : true;
                    groupData.Title = lvi[0].SubItems[1].Text;

                    if (e.Item.Checked)
                    {
                        //if (groupContent.LstGroupData.Count == 20)
                        //{
                        //    e.Item.Checked = false;
                        //    MessageBox.Show("시군/단말은 20개까지 추가할 수 있습니다.", "그룹정보 저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}

                        foreach (GroupData grp in groupContent.LstGroupData)
                        {
                            if (grp.IpAddr == groupData.IpAddr)
                            {
                                return;
                            }
                        }

                        groupContent.LstGroupData.Add(groupData);
                    }
                    else
                    {
                        GroupData deleteGroup = new GroupData();

                        foreach (GroupData grp in groupContent.LstGroupData)
                        {
                            if (grp.IpAddr == groupData.IpAddr)
                            {
                                deleteGroup = grp;
                                break;
                            }
                        }

                        if (deleteGroup.Title != string.Empty)
                        {
                            groupContent.LstGroupData.Remove(deleteGroup);
                        }
                    }
                }
            }
        }
        #endregion
    }

    #region 그룹정보 internal class
    /// <summary>
    /// 그룹 정보를 가지는 클래스
    /// </summary>
    internal class GroupInfo
    {
        private string title = string.Empty;
        private string buttonId = string.Empty;

        /// <summary>
        /// 그룹 버튼의 제목
        /// </summary>
        internal string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        /// 그룹 버튼의 ID
        /// </summary>
        internal string ButtonID
        {
            get { return this.buttonId; }
            set { this.buttonId = value; }
        }

        public override string ToString()
        {
            return this.title;
        }
    }
    #endregion
}