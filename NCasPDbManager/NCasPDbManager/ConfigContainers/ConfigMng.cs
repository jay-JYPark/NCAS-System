using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasEnv;
using System.Xml.Serialization;
using NCASFND.NCasLogging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NCASBIZ.NCasType;
using NCasAppCommon.Define;

namespace NCasPDbManager
{
    public class ConfigMng
    {
        #region Fields
        /// <summary>
        /// 로컬 IP를 가지고 담음
        /// </summary>
        private static string localIp = string.Empty;
        /// <summary>
        /// 환경설정 값이 들어 있음
        /// </summary>
        private static NCasDmSerializableConfig config = new NCasDmSerializableConfig();
        /// <summary>
        /// 환경설정 저장경로
        /// </summary>
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasPDbManagerConfig.xml";
        #endregion

        /// <summary>
        /// 초기화 함수
        /// </summary>
        public static void Init()
        {
            GetLocalIp();
            LoadConfig();
        }

        /// <summary>
        /// 로컬아이피를 불러와 설정한다.
        /// </summary>
        private static void GetLocalIp()
        {
            localIp = NCASBIZ.NCasUtility.NCasUtilityMng.INCasEtcUtility.GetIPv4();
        }

        /// <summary>
        /// 로컬 IP를 가지고 담음
        /// </summary>
        public static string LocalIp
        {
            get { return localIp; }
            set { localIp = value; }
        }

        /// <summary>
        /// DB연결 IP
        /// </summary>
        public static string LocalDbServerIp
        {
            get { return config.LocalDbServerIp; }
            set { config.LocalDbServerIp = value; }
        }

        /// <summary>
        /// DB연결 서비스네임
        /// </summary>
        public static string LocalDbServerSid
        {
            get { return config.LocalDbServerSid; }
            set { config.LocalDbServerSid = value; }
        }

        /// <summary>
        /// DB연결 사용자ID
        /// </summary>
        public static string LocalDbServerUserId
        {
            get { return config.LocalDbServerUserId; }
            set { config.LocalDbServerUserId = value; }
        }

        /// <summary>
        /// DB연결 사용자 PW
        /// </summary>
        public static string LocalDbServerPw
        {
            get { return config.LocalDbServerPw; }
            set { config.LocalDbServerPw = value; }
        }

        /// <summary>
        /// Tcp연결에 필요한 Profile정보를 담고있는 리스트
        /// </summary>
        public static List<NCasProfile> LstNCasProfile
        {
            get { return config.LstNCasProfile; }
            set { config.LstNCasProfile = value; }
        }

        /// <summary>
        /// TCP수신 IP
        /// </summary>
        public static string TcpIp
        {
            get { return config.TcpIp; }
            set { config.TcpIp = value; }
        }

        /// <summary>
        /// TCP수신 PORT
        /// </summary>
        public static int TcpPort
        {
            get { return config.TcpPort; }
            set { config.TcpPort = value; }
        }

        /// <summary>
        /// UDP수신 IP
        /// </summary>
        public static string UdpIp
        {
            get { return config.UdpIp; }
            set { config.UdpIp = value; }
        }

        /// <summary>
        /// UDP수신 Port
        /// </summary>
        public static int UdpPort
        {
            get { return config.UdpPort; }
            set { config.UdpPort = value; }
        }

        /// <summary>
        /// Config정보를 불러온다.
        /// </summary>
        private static void LoadConfig()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }
                XmlSerializer reader = new XmlSerializer(config.GetType());
                StreamReader file = new StreamReader(filePath);
                config = (NCasDmSerializableConfig)reader.Deserialize(file);
                file.Close();
            }
            catch (Exception err)
            {
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), "LoadConfig()", err);
            }
        }

        /// <summary>
        /// 환경설정값을 저장한다.
        /// </summary>
        public static bool SaveConfig()
        {
            bool isOk = false;
            try
            {
                XmlSerializer writer = new XmlSerializer(config.GetType());
                StreamWriter file = new StreamWriter(filePath);
                writer.Serialize(file, config);
                file.Close();

                isOk = true;
            }
            catch (Exception err)
            {
                isOk = false;
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), "SaveConfig()", err);
            }
            return isOk;
        }
    }
}
