using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCasPBrdScreen
{
    public partial class OrderSirenViewForm : Form
    {
        private Timer timer = null;
        private int playTime = 0;
        private double perTime = 0;
        private double percentNumber = 0;

        /// <summary>
        /// 사이렌명 프로퍼티
        /// </summary>
        public string SetText
        {
            set { this.sirenKindLabel.Text = value; }
        }

        /// <summary>
        /// 재생시간 프로퍼티
        /// </summary>
        public int PlayTime
        {
            set { this.playTime = value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (this.timer != null)
            {
                this.timer.Tick -= new EventHandler(timer_Tick);
                this.timer.Stop();
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public OrderSirenViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 사이렌 재생 화면의 타이틀을 셋팅하고, 프로그래스바를 시작한다.
        /// </summary>
        /// <param name="sirenTitle"></param>
        /// <param name="playTime"></param>
        public void StartSirenForm(string sirenTitle, int playTime)
        {
            this.sirenKindLabel.Text = sirenTitle;
            this.playTime = playTime;
            this.timeProgressBar.Maximum = playTime;
            this.perTime = 100.0 / playTime;

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Start();
        }

        /// <summary>
        /// Timer Tick 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            this.timeProgressBar.PerformStep();
            this.perLabel.Text = ((int)this.percentNumber).ToString() + " %";
            this.percentNumber += this.perTime;

            if (this.playTime == 0)
            {
                this.perLabel.Text = "100 %";
            }
            else if (this.playTime == -1)
            {
                this.Close();
            }

            this.playTime--;
        }

        /// <summary>
        /// 사이렌 OFF 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sirenOffBtn_Click(object sender, EventArgs e)
        {
            this.timeProgressBar.Value = this.timeProgressBar.Maximum - 2;
            this.playTime = 2;
            this.percentNumber = 100 - (int)(this.perTime * 2);
            this.sirenOffBtn.Enabled = false;
        }
    }
}