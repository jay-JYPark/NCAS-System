using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasType;

namespace NCasPBrdScreen
{
    public class RecordBizData : NCasObject
    {
        private byte[] data = new byte[16];

        /// <summary>
        /// 녹음방송 데이터 프로퍼티
        /// </summary>
        public byte[] Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// 녹음 시작 데이터
        /// </summary>
        public void StartRecording()
        {

        }

        /// <summary>
        /// 녹음 방송 종료 데이터
        /// </summary>
        public void StopRecording()
        {

        }

        public override void CloneFrom(NCasObject obj)
        {
            RecordBizData newObject = obj as RecordBizData;
            this.data = newObject.data;
        }

        public override NCasObject CloneTo()
        {
            RecordBizData newObject = new RecordBizData();
            newObject.data = this.data;
            return newObject;
        }
    }
}