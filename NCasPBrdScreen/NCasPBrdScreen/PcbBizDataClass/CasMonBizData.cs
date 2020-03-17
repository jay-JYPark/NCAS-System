using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasType;

namespace NCasPBrdScreen
{
    public class CasMonBizData : NCasObject
    {
        private bool useSate = false;
        private bool isLocal = true;

        /// <summary>
        /// 위성 사용 유무 프로퍼티
        /// </summary>
        public bool UseSate
        {
            get { return this.useSate; }
            set { this.useSate = value; }
        }

        /// <summary>
        /// 자시도, 타시도 프로퍼티
        /// </summary>
        public bool IsLocal
        {
            get { return this.isLocal; }
            set { this.isLocal = value; }
        }

        /// <summary>
        /// byte[]를 CasMonBizData 데이터로 변환
        /// </summary>
        /// <param name="data"></param>
        public void CasMonFromByteArray(byte[] data)
        {

        }

        /// <summary>
        /// CasMonBizData 데이터를 byte[]로 변환
        /// </summary>
        /// <returns></returns>
        public byte[] CasMonToByteArray()
        {
            return new byte[0] { };
        }

        public override void CloneFrom(NCasObject obj)
        {
            CasMonBizData newObject = obj as CasMonBizData;
            this.useSate = newObject.useSate;
            this.isLocal = newObject.isLocal;
        }

        public override NCasObject CloneTo()
        {
            CasMonBizData newObject = new CasMonBizData();
            newObject.useSate = this.useSate;
            newObject.isLocal = this.isLocal;
            return newObject;
        }
    }
}