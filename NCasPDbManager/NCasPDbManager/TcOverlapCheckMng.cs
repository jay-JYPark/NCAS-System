using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasDefine;
using NCASFND.NCasLogging;

namespace NCasPDbManager
{
    public class TcOverlapCheckMng
    {
        #region Fields
        private static Dictionary<string, NCasProtocolBase> dicAlarmOrderData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicAlarmRespData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicAlarmResultData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicBroadOrderData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicBroadRespData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicBroadResultData = new Dictionary<string, NCasProtocolBase>();
        private static Dictionary<string, NCasProtocolBase> dicLoginInfoData = new Dictionary<string, NCasProtocolBase>();

        private static List<string> lstAlarmOrderKeyData = new List<string>();
        private static List<string> lstAlarmRespKeyData = new List<string>();
        private static List<string> lstAlarmResultKeyData = new List<string>();
        private static List<string> lstBroadOrderKeyData = new List<string>();
        private static List<string> lstBroadRespKeyData = new List<string>();
        private static List<string> lstBroadResultKeyData = new List<string>();
        private static List<string> lstLoginInfoKeyData = new List<string>();
        #endregion

        public static bool TcOverlapCheck(NCasProtocolBase proto)
        {
            bool isOk = true;
            string overLapDicKey = string.Empty;
            switch (proto.TcCode)
            {
                case NCasDefineTcCode.TcAlarmOrder:
                    NCasProtocolTc1 tc1 = proto as NCasProtocolTc1;
                    overLapDicKey = MakeOverlapDicKey(tc1.OrderTimeByDateTime, tc1.Mode, tc1.Source, tc1.AlarmKind, tc1.AlarmNetIdOrIpByString, NCasDefineOrderResult.None);
                    lock (dicAlarmOrderData)
                    {
                        if (dicAlarmOrderData.ContainsKey(overLapDicKey))
                        {
                            isOk = false;
                            break;
                        }
                    }
                    dicAlarmOrderData.Add(overLapDicKey, tc1);
                    lstAlarmOrderKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcAlarmResponse:
                    NCasProtocolTc2 tc2 = proto as NCasProtocolTc2;
                    overLapDicKey = MakeOverlapDicKey(tc2.OrderTimeByDateTime, tc2.Mode, tc2.Source, tc2.AlarmKind, tc2.RespNetIdOrIpByString, NCasDefineOrderResult.None);
                    lock (dicAlarmRespData)
                    {
                        if (dicAlarmRespData.ContainsKey(overLapDicKey))
                        {
                            isOk = false;
                            break;
                        }
                    }
                    dicAlarmRespData.Add(overLapDicKey, tc2);
                    lstAlarmRespKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcAlarmResult:
                    NCasProtocolTc3 tc3 = proto as NCasProtocolTc3;
                    overLapDicKey = MakeOverlapDicKey(tc3.OrderTimeByDateTime, tc3.Mode, tc3.Source, tc3.AlarmKind, tc3.RespNetIdOrIpByString, tc3.RsltStatus);
                    if (dicAlarmResultData.ContainsKey(overLapDicKey))
                    {
                        isOk = false;
                        break;
                    }
                    dicAlarmResultData.Add(overLapDicKey, tc3);
                    lstAlarmResultKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcBroadOrder:
                    NCasProtocolTc4 tc4 = proto as NCasProtocolTc4;
                    overLapDicKey = MakeOverlapDicKey(tc4.OrderTimeByDateTime, tc4.Mode, tc4.Source, tc4.AlarmKind, tc4.BroadNetIdOrIpByString, NCasDefineOrderResult.None);
                    if (dicBroadOrderData.ContainsKey(overLapDicKey))
                    {
                        isOk = false;
                        break;
                    }
                    dicBroadOrderData.Add(overLapDicKey, tc4);
                    lstBroadOrderKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcBroadResponse:
                    NCasProtocolTc5 tc5 = proto as NCasProtocolTc5;
                    overLapDicKey = MakeOverlapDicKey(tc5.OrderTimeByDateTime, tc5.Mode, tc5.Source, tc5.AlarmKind, tc5.RespNetIdOrIpByString, NCasDefineOrderResult.None);
                    if (dicBroadRespData.ContainsKey(overLapDicKey))
                    {
                        isOk = false;
                        break;
                    }
                    dicBroadRespData.Add(overLapDicKey, tc5);
                    lstBroadRespKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcBroadResult:
                    NCasProtocolTc6 tc6 = proto as NCasProtocolTc6;
                    overLapDicKey = MakeOverlapDicKey(tc6.OrderTimeByDateTime, tc6.Mode, tc6.Source, tc6.AlarmKind, tc6.RespNetIdOrIpByString, tc6.RsltStatus);
                    if (dicBroadResultData.ContainsKey(overLapDicKey))
                    {
                        isOk = false;
                        break;
                    }
                    dicBroadResultData.Add(overLapDicKey, tc6);
                    lstBroadResultKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;

                case NCasDefineTcCode.TcLoginInfo:
                    NCasProtocolTc23 tc23 = proto as NCasProtocolTc23;
                    overLapDicKey = MakeOverlapDicKeyTc23(DateTime.Now.ToString("yyyyMMddHHmmss"), tc23.TermIpByString, tc23.UserId, tc23.Status);
                    if (dicLoginInfoData.ContainsKey(overLapDicKey))
                    {
                        isOk = false;
                        break;
                    }
                    dicLoginInfoData.Add(overLapDicKey, tc23);
                    lstLoginInfoKeyData.Add(overLapDicKey);
                    isOk = true;
                    break;
            }
            CheckDicCount();
            return isOk;
        }

        private static void CheckDicCount()
        {
            int count;
            int maxCount = 10000;//10000;
            if (lstAlarmOrderKeyData.Count > maxCount)
            {
                lock (lstAlarmOrderKeyData)
                {
                    count = lstAlarmOrderKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicAlarmOrderData.Remove(lstAlarmOrderKeyData[i]);
                }
                lock (lstAlarmOrderKeyData)
                {
                    lstAlarmOrderKeyData.RemoveRange(0, count - maxCount);
                }
            }
            if (lstAlarmRespKeyData.Count > maxCount)
            {
                lock (lstAlarmRespKeyData)
                {
                    count = lstAlarmRespKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicAlarmRespData.Remove(lstAlarmRespKeyData[i]);
                }
                lock (lstAlarmRespKeyData)
                {
                    lstAlarmRespKeyData.RemoveRange(0, count - maxCount);
                }
            }
            if (lstAlarmResultKeyData.Count > maxCount)
            {
                lock (lstAlarmResultKeyData)
                {
                    count = lstAlarmResultKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicAlarmResultData.Remove(lstAlarmResultKeyData[i]);
                }
                lock (lstAlarmResultKeyData)
                {
                    lstAlarmResultKeyData.RemoveRange(0, count - maxCount);
                }
            }

            if (lstBroadOrderKeyData.Count > maxCount)
            {
                lock (lstBroadOrderKeyData)
                {
                    count = lstBroadOrderKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicBroadOrderData.Remove(lstBroadOrderKeyData[i]);
                }
                lock (lstBroadOrderKeyData)
                {
                    lstBroadOrderKeyData.RemoveRange(0, count - maxCount);
                }
            }
            if (lstBroadRespKeyData.Count > maxCount)
            {
                lock (lstBroadRespKeyData)
                {
                    count = lstBroadRespKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicBroadRespData.Remove(lstBroadRespKeyData[i]);
                }
                lock (lstBroadRespKeyData)
                {
                    lstBroadRespKeyData.RemoveRange(0, count - maxCount);
                }
            }
            if (lstBroadResultKeyData.Count > maxCount)
            {
                lock (lstBroadResultKeyData)
                {
                    count = lstBroadResultKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicBroadResultData.Remove(lstBroadResultKeyData[i]);
                }
                lock (lstBroadResultKeyData)
                {
                    lstBroadResultKeyData.RemoveRange(0, count - maxCount);
                }
            }
            if(lstLoginInfoKeyData.Count > maxCount)
            {
                lock (lstLoginInfoKeyData)
                {
                    count = lstLoginInfoKeyData.Count;
                }
                for (int i = 0; i < count - maxCount; i++)
                {
                    dicLoginInfoData.Remove(lstLoginInfoKeyData[i]);
                }
                lock (lstLoginInfoKeyData)
                {
                    lstLoginInfoKeyData.RemoveRange(0, count - maxCount);
                }
            }
            NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] dicAlarmOrderData Count : {1}\n dicAlarmRespData Count : {2}\n dicAlarmResultData Count : {3}\n lstAlarmOrderKeyData Count : {4}\n lstAlarmRespKeyData Count : {5}\n lstAlarmResultKeyData Count : {6}\n"
                , DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"),
                dicAlarmOrderData.Count.ToString(), dicAlarmRespData.Count.ToString(), dicAlarmResultData.Count.ToString(), lstAlarmOrderKeyData.Count.ToString(), lstAlarmRespKeyData.Count.ToString(), lstAlarmResultKeyData.Count.ToString()));
        }


        private static string MakeOverlapDicKey(DateTime orderTime, NCasDefineOrderMode orderMode, NCasDefineOrderSource orderSource, NCasDefineOrderKind orderKind, string orderIp, NCasDefineOrderResult orderResult)
        {
            string overlapDicKey = string.Empty;

            overlapDicKey = orderTime.ToString() + ((int)orderMode).ToString() + ((int)orderSource).ToString() + ((int)orderKind).ToString() + orderIp + ((int)orderResult).ToString();

            return overlapDicKey;
        }

        private static string MakeOverlapDicKeyTc23(string occurTime, string ip, string userId, NCasDefineLoginStatus nCasDefineLoginStatus)
        {
            string overlapDicKey = string.Empty;

            overlapDicKey = occurTime + ip + userId + ((int)nCasDefineLoginStatus).ToString();

            return overlapDicKey;
        }
    }
}
