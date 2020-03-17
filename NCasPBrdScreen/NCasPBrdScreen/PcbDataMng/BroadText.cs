using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPBrdScreen
{
    public class BroadText
    {
        private List<string> lstText = new List<string>();
        private string title = string.Empty;

        /// <summary>
        /// 문안내용 리스트 프로퍼티
        /// </summary>
        public List<string> LstText
        {
            get { return this.lstText; }
            set { this.lstText = value; }
        }

        /// <summary>
        /// 문안제목 프로퍼티
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
    }
}