using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPAlmScreen
{
    [Serializable]
    public class DeviceStatusDataContainer
    {
        private List<DeviceStatusData> lstDeviceStatusData = new List<DeviceStatusData>();

        public List<DeviceStatusData> LstDeviceStatusData
        {
            get { return lstDeviceStatusData; }
            set { this.lstDeviceStatusData = value; }
        }
    }
}