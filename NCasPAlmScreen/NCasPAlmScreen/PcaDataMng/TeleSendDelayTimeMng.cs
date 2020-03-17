using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NCASBIZ.NCasEnv;
using NCASBIZ.NCasUtility;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPAlmScreen
{
    public class TeleSendDelayTimeMng
    {
        private static string teleDelayTime = string.Empty;
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasAlmTeleDelayTime.xml";

        /// <summary>
        /// 일제지령 Delay Time 프로퍼티
        /// </summary>
        public static string TeleDelayTime
        {
            get { return teleDelayTime; }
            set { teleDelayTime = value; }
        }

        /// <summary>
        /// 일제지령 Delay Time 로드
        /// </summary>
        public static void LoadTeleDelayTime()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    teleDelayTime = "2000";
                    SaveTeleDelayTime();
                    return;
                }

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(String));
                    teleDelayTime = (String)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("TeleDelayTimeMng", "TeleDelayTimeMng.LoadTeleDelayTime() Method", ex);
            }
        }

        /// <summary>
        /// 일제지령 Delay Time 저장
        /// </summary>
        public static void SaveTeleDelayTime()
        {
            try
            {
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(String));
                    ser.Serialize(stream, teleDelayTime);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("TeleDelayTimeMng", "TeleDelayTimeMng.SaveTeleDelayTime() Method", ex);
            }
        }
    }
}