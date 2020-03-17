using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasDefine;

namespace NCasPBrdScreen
{
    public class TVCaptionData
    {
        private NCasDefineCaption tvCaptionMode = NCasDefineCaption.None;
        private NCasDefineOrderKind orderKind = NCasDefineOrderKind.None;
        private string tvText = string.Empty;
        private int repeatCount = 10;

        /// <summary>
        /// TV자막 모드 프로퍼티
        /// </summary>
        public NCasDefineCaption TvCaptionMode
        {
            get { return this.tvCaptionMode; }
            set { this.tvCaptionMode = value; }
        }

        /// <summary>
        /// 경보종류 프로퍼티
        /// </summary>
        public NCasDefineOrderKind OrderKind
        {
            get { return this.orderKind; }
            set { this.orderKind = value; }
        }

        /// <summary>
        /// TV자막 문안 프로퍼티
        /// </summary>
        public string TvText
        {
            get { return this.tvText; }
            set { this.tvText = value; }
        }

        /// <summary>
        /// 반복횟수 프러퍼티
        /// </summary>
        public int RepeatCount
        {
            get { return this.repeatCount; }
            set { this.repeatCount = value; }
        }
    }
}