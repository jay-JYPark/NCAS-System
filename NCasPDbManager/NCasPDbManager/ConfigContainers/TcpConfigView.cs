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
using NCASBIZ.NCasType;
using NCASBIZ.NCasDefine;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPDbManager
{
    public partial class TcpConfigView : ConfigViewBase
    {
        private Font listViewFont = null;
        private bool isChange = false;

        public TcpConfigView()
        {
            InitializeComponent();
        }

        protected override void InitControl()
        {
            base.InitControl();
            InitListViewControl();

            tbxTcpIp.Text = ConfigMng.TcpIp;
            tbxTcpPort.Text = ConfigMng.TcpPort.ToString();
        }

        private void InitListViewControl()
        {
            try
            {
                #region Base Set
                this.imgList.ColorDepth = ColorDepth.Depth32Bit;
                this.imgList.ImageSize = new Size(27, 22);
                this.imgList.Images.Add(NCasPDbManagerRsc.listIconError);
                this.imgList.Images.Add(NCasPDbManagerRsc.listIconOk);
                this.listViewFont = new Font(NCasPDbManagerRsc.FontName, 12.0f, FontStyle.Bold);

                this.lvTcpProfile.StateImageList.ImageSize = this.imgList.ImageSize;
                this.lvTcpProfile.StateImageList.Images.Add(this.imgList.Images[0]);
                this.lvTcpProfile.StateImageList.Images.Add(this.imgList.Images[1]);

                this.lvTcpProfile.GridLineStyle = NCasListViewGridLine.GridBoth;
                this.lvTcpProfile.GridDashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                this.lvTcpProfile.Font = new Font(NCasPDbManagerRsc.FontName, 11.0f);
                this.lvTcpProfile.ColumnHeight = 32;
                this.lvTcpProfile.ItemHeight = 28;
                #endregion

                #region lvClientAdmin ColumnHeather Set
                NCasColumnHeader column = this.lvTcpProfile.Columns.Add("...", 32);
                column.SortType = NCasListViewSortType.SortIcon;
                column.TextAlign = HorizontalAlignment.Center;
                column.ColumnLock = true;

                column = this.lvTcpProfile.Columns.Add("번호", 50);
                column.SortType = NCasListViewSortType.SortInt32;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvTcpProfile.Columns.Add("클라이언트 이름", 200);
                column.SortType = NCasListViewSortType.SortText;
                column.TextAlign = HorizontalAlignment.Center;

                column = this.lvTcpProfile.Columns.Add("IP", 150);
                column.SortType = NCasListViewSortType.SortText;
                column.TextAlign = HorizontalAlignment.Center;
                #endregion

                #region Data Input
                List<NCasProfile> lstNCasProfile = null;
                lstNCasProfile = ConfigMng.LstNCasProfile;
                if (lstNCasProfile == null)
                {
                    return;
                }

                NCasListViewItem item = null;
                NCasListViewItem.NCasListViewSubItem subItem = null;
                for (int i = 0; i < lstNCasProfile.Count; i++)
                {
                    item = new NCasListViewItem();
                    item.ImageIndex = 0;
                    item.Tag = lstNCasProfile[i];

                    //번호
                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (lvTcpProfile.Items.Count + 1).ToString();
                    subItem.TextAlign = HorizontalAlignment.Center;
                    item.SubItems.Add(subItem);
                    //이름
                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (lstNCasProfile[i].Name).ToString();
                    subItem.TextAlign = HorizontalAlignment.Left;
                    item.SubItems.Add(subItem);
                    //IP
                    subItem = new NCasListViewItem.NCasListViewSubItem();
                    subItem.Text = (lstNCasProfile[i].IpAddr).ToString();
                    subItem.TextAlign = HorizontalAlignment.Left;
                    item.SubItems.Add(subItem);

                    lvTcpProfile.Items.Add(item);
                }
                #endregion
            }
            catch (Exception err)
            {
                string functionName = "InitListViewControl()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        public override void SaveConfig()
        {
            base.SaveConfig();
            try
            {
                List<NCasProfile> lstNCasProfile = new List<NCasProfile>();
                NCasProfile profile = null;
                for (int i = 0; i < lvTcpProfile.Items.Count; i++)
                {
                    profile = lvTcpProfile.Items[i].Tag as NCasProfile;
                    if (profile != null)
                    {
                        lstNCasProfile.Add(profile);
                    }
                }
                ConfigMng.LstNCasProfile = lstNCasProfile;

                ConfigMng.TcpIp = tbxTcpIp.Text;
                int tcpPort;
                if (int.TryParse(tbxTcpPort.Text, out tcpPort))
                {
                    ConfigMng.TcpPort = tcpPort;
                }
            }
            catch (Exception err)
            {
                string functionName = "SaveConfig()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void btnProfileNew_Click(object sender, EventArgs e)
        {
            tbxServerName.Text = string.Empty;
            tbxServerIp.Text = string.Empty;
            isChange = false;
        }

        private void btnProfileregister_Click(object sender, EventArgs e)
        {
            if (tbxServerName.Text == string.Empty)
            {
                MessageBox.Show("서버명을 입력하세요.", "TCP설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerName.Focus();
                return;
            }
            if (tbxServerIp.Text == string.Empty)
            {
                MessageBox.Show("서버IP를 입력하세요.", "TCP설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbxServerIp.Focus();
                return;
            }

            NCasListViewItem item = null;
            NCasProfile profile = new NCasProfile(tbxServerIp.Text, tbxServerName.Text);
            if (isChange)
            {
                item = lvTcpProfile.SelectedItems[0];
                if (item == null)
                {
                    MessageBox.Show("TCP접속 리스트에서 한개의 정보를 선택하여 주세요.", "TCP설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                item.Tag = profile;
                item.SubItems[2].Text = tbxServerName.Text;
                item.SubItems[3].Text = tbxServerIp.Text;
            }
            else
            {
                NCasListViewItem.NCasListViewSubItem subItem = null;

                item = new NCasListViewItem();
                item.ImageIndex = 0;
                item.Tag = profile;

                //번호
                subItem = new NCasListViewItem.NCasListViewSubItem();
                subItem.Text = (lvTcpProfile.Items.Count + 1).ToString();
                subItem.TextAlign = HorizontalAlignment.Center;
                item.SubItems.Add(subItem);
                //이름
                subItem = new NCasListViewItem.NCasListViewSubItem();
                subItem.Text = (profile.Name).ToString();
                subItem.TextAlign = HorizontalAlignment.Left;
                item.SubItems.Add(subItem);
                //IP
                subItem = new NCasListViewItem.NCasListViewSubItem();
                subItem.Text = (profile.IpAddr).ToString();
                subItem.TextAlign = HorizontalAlignment.Left;
                item.SubItems.Add(subItem);

                lvTcpProfile.Items.Add(item);
            }
        }

        private void lvTcpProfile_DoubleClick(object sender, EventArgs e)
        {
        }

        private void lvTcpProfile_Click(object sender, EventArgs e)
        {
            if (lvTcpProfile.SelectedItems.Count <= 0)
            {
                return;
            }
            isChange = true;

            NCasProfile profile = lvTcpProfile.SelectedItems[0].Tag as NCasProfile;
            if (profile == null)
            {
                return;
            }

            tbxServerIp.Text = profile.IpAddr;
            tbxServerName.Text = profile.Name;
        }

        private void btnProfileDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvTcpProfile.SelectedItems.Count <= 0)
                {
                    MessageBox.Show("TCP접속 리스트에서 한개의 정보를 선택하여 주세요.", "TCP설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                lvTcpProfile.Items.Remove(lvTcpProfile.SelectedItems[0]);
                for (int i = 0; i < lvTcpProfile.Items.Count; i++)
                {
                    lvTcpProfile.Items[i].SubItems[1].Text = (i + 1).ToString();
                }
            }
            catch (Exception err)
            {
                string functionName = "btnProfileDelete_Click(object sender, EventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }
    }
}
