using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasType;

namespace NCasPDbManager
{
    [Serializable]
    public class NCasDmSerializableConfig
    {
        #region Fields
        /// <summary>
        /// DB서버IP
        /// </summary>
        private string localDbServerIp = string.Empty;
        /// <summary>
        /// DB 서비스명(SID)
        /// </summary>
        private string localDbServerSid = string.Empty;
        /// <summary>
        /// DB접속 사용자 ID
        /// </summary>
        private string localDbServerUserId = string.Empty;
        /// <summary>
        /// DB서버 접속 사용자 PW
        /// </summary>
        private string localDbServerPw = string.Empty;
        /// <summary>
        /// UDP 수신 IP
        /// </summary>
        private string udpIp = string.Empty;
        /// <summary>
        /// UDP수신 포트
        /// </summary>
        private int udpPort;
        /// <summary>
        /// TCP수신 IP
        /// </summary>
        private string tcpIp = string.Empty;
        /// <summary>
        /// TCP수신 포트
        /// </summary>
        private int tcpPort;
        /// <summary>
        /// TCP설정에 필요한 TcpProfileData데이터
        /// </summary>
        private List<NCasProfile> lstNCasProfile = new List<NCasProfile>();
        #endregion

        #region Properites
        /// <summary>
        /// DB서버IP
        /// </summary>
        public string LocalDbServerIp
        {
            get
            {
                return localDbServerIp;
            }
            set
            {
                localDbServerIp = value;
            }
        }

        /// <summary>
        /// DB 서비스명(SID)
        /// </summary>
        public string LocalDbServerSid
        {
            get
            {
                return localDbServerSid;
            }
            set
            {
                localDbServerSid = value;
            }
        }

        /// <summary>
        /// DB접속 사용자 ID
        /// </summary>
        public string LocalDbServerUserId
        {
            get
            {
                return localDbServerUserId;
            }
            set
            {
                localDbServerUserId = value;
            }
        }

        /// <summary>
        /// DB서버 접속 사용자 PW
        /// </summary>
        public string LocalDbServerPw
        {
            get
            {
                return localDbServerPw;
            }
            set
            {
                localDbServerPw = value;
            }
        }
        /// <summary>
        /// TCP 수신 IP
        /// </summary>
        public string TcpIp
        {
            get { return tcpIp; }
            set { tcpIp = value; }
        }

        /// <summary>
        /// TCP 수신 포트
        /// </summary>
        public int TcpPort
        {
            get { return tcpPort; }
            set { tcpPort = value; }
        }

        /// <summary>
        /// UDP 수신 IP
        /// </summary>
        public string UdpIp
        {
            get
            {
                return udpIp;
            }
            set
            {
                udpIp = value;
            }
        }

        /// <summary>
        /// UDP수신 포트
        /// </summary>
        public int UdpPort
        {
            get
            {
                return udpPort;
            }
            set
            {
                udpPort = value;
            }
        }

        /// <summary>
        /// TCP설정에 필요한 TcpProfileData데이터
        /// </summary>
        public List<NCasProfile> LstNCasProfile
        {
            get
            {
                return lstNCasProfile;
            }
            set
            {
                lstNCasProfile = value;
            }
        }
        #endregion
    }
}
