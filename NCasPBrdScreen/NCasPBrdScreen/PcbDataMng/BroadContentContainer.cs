using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPBrdScreen
{
    [Serializable]
    public class BroadContentContainer
    {
        private List<BroadContent> lstBroadContents = new List<BroadContent>();

        /// <summary>
        /// 방송문안 리스트 프로퍼티
        /// </summary>
        public List<BroadContent> LstBroadContents
        {
            get { return this.lstBroadContents; }
            set { this.lstBroadContents = value; }
        }
    }
}