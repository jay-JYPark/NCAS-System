using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPBrdScreen
{
    public class TVCaptionContent
    {
        private int provCode = 0;
        private List<TVCaptionData> tvCaptionData = new List<TVCaptionData>();

        /// <summary>
        /// 시도 Code 프로퍼티
        /// </summary>
        public int ProvCode
        {
            get { return this.provCode; }
            set { this.provCode = value; }
        }

        /// <summary>
        /// TV자막 리스트 프로퍼티
        /// </summary>
        public List<TVCaptionData> TVCaptionData
        {
            get { return this.tvCaptionData; }
            set { this.tvCaptionData = value; }
        }
    }
}