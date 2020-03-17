using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasType;
using NCASBIZ.NCasProtocol;

namespace NCasPDbManager
{
    public class TcData : NCasObject
    {
        #region Fields
        private DateTime recvTime;
        private NCasProtocolBase protoBase;
        #endregion

        #region Properties
        public DateTime RecvTime
        {
            get { return recvTime; }
            set { recvTime = value; }
        }
        public NCasProtocolBase ProtoBase
        {
            get { return protoBase; }
            set { protoBase = value; }
        }
        #endregion

        public override void CloneFrom(NCasObject obj)
        {
            TcData newObject = obj as TcData;
            this.recvTime = newObject.recvTime;
            this.protoBase = newObject.protoBase;
        }

        public override NCasObject CloneTo()
        {
            TcData newObject = new TcData();
            newObject.recvTime = this.recvTime;
            newObject.protoBase = this.protoBase;
            return newObject;
        }
    }
}
