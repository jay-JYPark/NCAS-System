using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPDbManager
{
    public class ErrTermInfoData
    {
        #region Fields
        private int count;
        private DateTime occurTime = DateTime.MinValue;
        #endregion

        #region Properties
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        public DateTime OccurTime
        {
            get { return occurTime; }
            set { occurTime = value; }
        }
        #endregion
    }
}
