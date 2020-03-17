using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasType;

namespace NCasPBrdScreen
{
    public class TtsStopBizData : NCasObject
    {
        private byte[] data = new byte[5] { 2, 2, 9, 0, 0 };

        /// <summary>
        /// TTS 데이터 프로퍼티
        /// </summary>
        public byte[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public override void CloneFrom(NCasObject obj)
        {
            TtsStopBizData newObject = obj as TtsStopBizData;
            this.data = newObject.data;
        }

        public override NCasObject CloneTo()
        {
            TtsStopBizData newObject = new TtsStopBizData();
            newObject.data = this.data;
            return newObject;
        }
    }
}