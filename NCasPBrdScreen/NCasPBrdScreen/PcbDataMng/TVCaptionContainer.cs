using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPBrdScreen
{
    [Serializable]
    public class TVCaptionContainer
    {
        private List<TVCaptionContent> lstTVCaptionContent = new List<TVCaptionContent>();

        /// <summary>
        /// TVCaptionContent 리스트 프로퍼티
        /// </summary>
        public List<TVCaptionContent> LstTVCaptionContent
        {
            get { return this.lstTVCaptionContent; }
        }
    }
}