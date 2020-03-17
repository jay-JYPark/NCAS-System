using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NCASFND.NCasCtrl;

namespace NCasPBrdScreen
{
    public partial class BroadTextView : Form
    {
        private NCasButton selectBtn = new NCasButton();

        /// <summary>
        /// 생성자
        /// </summary>
        public BroadTextView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.InitButton();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        /// <summary>
        /// 버튼 초기화
        /// </summary>
        private void InitButton()
        {
            for (int i = 0; i < BroadContentMng.LstBroadContent.Count; i++)
            {
                Control[] control = this.btnTableLayoutPanel.Controls.Find("btnBroadText" + (i + 1).ToString(), false);

                if (control == null)
                    continue;

                if (control.Length == 0)
                    continue;

                (control[0] as NCasButton).Enabled = true;
                (control[0] as NCasButton).Text = BroadContentMng.LstBroadContent[i].Name;
                (control[0] as NCasButton).Tag = BroadContentMng.LstBroadContent[i].LstBroadText;
            }
        }

        /// <summary>
        /// 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBroadText1_MouseDown(object sender, MouseEventArgs e)
        {
            NCasButton ncasBtn = sender as NCasButton;
            List<BroadText> broadText = (List<BroadText>)ncasBtn.Tag;

            if (ncasBtn.CheckedValue)
            {
                this.selectBtn = ncasBtn;
                this.selectDisasterLabel.Text = ncasBtn.Text;
                this.selectDetailDisasterComboBox.Items.Clear();
                this.broadTextBox1.Text = string.Empty;
                this.broadTextBox2.Text = string.Empty;
                this.ClearCheckAllButton();
                ncasBtn.CheckedValue = true;

                foreach (BroadText text in broadText)
                {
                    this.selectDetailDisasterComboBox.Items.Add(text.Title);
                }
            }
            else
            {
                this.selectBtn = new NCasButton();
                this.selectDisasterLabel.Text = string.Empty;
                this.selectDetailDisasterComboBox.Items.Clear();
                this.broadTextBox1.Text = string.Empty;
                this.broadTextBox2.Text = string.Empty;
            }
        }

        /// <summary>
        /// 상세 상황 콤보박스 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectDetailDisasterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.broadTextBox1.Text = string.Empty;
            this.broadTextBox2.Text = string.Empty;

            foreach (BroadText text in (this.selectBtn.Tag as List<BroadText>))
            {
                if (text.Title != this.selectDetailDisasterComboBox.SelectedItem.ToString())
                    continue;

                if (text.LstText.Count == 0)
                {
                    this.broadTextBox1.Text = "문안이 없습니다.";
                    this.broadTextBox2.Text = "문안이 없습니다.";
                }
                else if (text.LstText.Count == 1)
                {
                    this.broadTextBox1.Text = text.LstText[0];
                }
                else if (text.LstText.Count == 2)
                {
                    this.broadTextBox1.Text = text.LstText[0];
                    this.broadTextBox2.Text = text.LstText[1];
                }
            }
        }

        /// <summary>
        /// 모든 버튼의 체크상태를 해제한다.
        /// </summary>
        private void ClearCheckAllButton()
        {
            for (int i = 0; i < BroadContentMng.LstBroadContent.Count; i++)
            {
                Control[] control = this.btnTableLayoutPanel.Controls.Find("btnBroadText" + (i + 1).ToString(), false);

                if (control == null)
                    continue;

                if (control.Length == 0)
                    continue;

                (control[0] as NCasButton).CheckedValue = false;
            }
        }
    }
}