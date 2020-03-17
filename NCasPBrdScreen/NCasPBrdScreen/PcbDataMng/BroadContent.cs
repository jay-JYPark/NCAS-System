using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPBrdScreen
{
    public class BroadContent
    {
        private string key = string.Empty;
        private List<BroadText> lstBroadText = new List<BroadText>();
        private string name = string.Empty;

        /// <summary>
        /// 고유 식별자 프로퍼티
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// BroadText의 List 프로퍼티
        /// </summary>
        public List<BroadText> LstBroadText
        {
            get { return this.lstBroadText; }
            set { this.lstBroadText = value; }
        }

        /// <summary>
        /// 문안종류 프로퍼티
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public BroadContent()
        {
            this.key = System.Guid.NewGuid().ToString();
        }
    }
}