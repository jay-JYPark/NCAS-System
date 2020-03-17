using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasType;
using NCASBIZ.NCasProtocol;

namespace NCasPBrdScreen
{
    public class OrderBizData : NCasObject
    {
        private NCasProtocolTc4 brdProtocol = null;
        private bool isLocal = true;

        /// <summary>
        /// 방송발령 TC 프로퍼티
        /// </summary>
        public NCasProtocolTc4 BrdProtocol
        {
            get { return this.brdProtocol; }
            set { this.brdProtocol = value; }
        }

        /// <summary>
        /// 로컬발령인지 리모트발령인지 구분하기 위한 프로퍼티
        /// </summary>
        public bool IsLocal
        {
            get { return this.isLocal; }
            set { this.isLocal = value; }
        }

        public override void CloneFrom(NCasObject obj)
        {
            OrderBizData newObject = obj as OrderBizData;
            this.brdProtocol = newObject.brdProtocol;
            this.isLocal = newObject.isLocal;
        }

        public override NCasObject CloneTo()
        {
            OrderBizData newObject = new OrderBizData();
            newObject.brdProtocol = this.brdProtocol;
            newObject.isLocal = this.isLocal;
            return newObject;
        }
    }
}