using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NCasPDbManager
{
    public partial class ConfigViewBase : UserControl
    {
        #region Fields
        /// <summary>
        /// 컨트롤 탭이 어떤 탭인지 설정한다.
        /// </summary>
        private ConfigViewKind viewKind = ConfigViewKind.None;
        #endregion

        #region Properties
        public ConfigViewKind ViewKind
        {
            get { return viewKind; }
            set { viewKind = value; }
        }
        #endregion

        public void Init()
        {
            InitControl();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public ConfigViewBase()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 환경설정 화면 컨트롤에 정보를 표시한다.
        /// </summary>
        protected virtual void InitControl()
        {
        }

        /// <summary>
        /// 변경된 내용을 저장한다.
        /// </summary>
        public virtual void SaveConfig()
        {
        }
    }
}
