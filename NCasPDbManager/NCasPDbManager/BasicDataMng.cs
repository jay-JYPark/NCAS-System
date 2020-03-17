using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASFND.NCasDb;
using Oracle.DataAccess.Client;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;
using NCASBIZ.NCasDefine;

namespace NCasPDbManager
{
    public class BasicDataMng
    {
        #region Fields
        /// <summary>
        /// 장비정보를 담고 있는 딕셔너리
        /// </summary>
        private static Dictionary<string, MasterInfoData> dicMasterInfoData = new Dictionary<string, MasterInfoData>();
        /// <summary>
        /// 이전시간
        /// </summary>
        private static DateTime oldTime;
        /// <summary>
        /// NCasOracleDb
        /// </summary>
        private static NCasOracleDb oracleDb = null;
        /// <summary>
        /// DBClose동기화 객체
        /// </summary>
        private static object closeObject = new object();
        private static object oldTimeObject = new object();
        #endregion

        #region Properties
        /// <summary>
        /// 장비정보를 담고 있는 딕셔너리
        /// </summary>
        public static Dictionary<string, MasterInfoData> DicMasterInfoData
        {
            get
            { return dicMasterInfoData; }
            set
            { dicMasterInfoData = value; }
        }
        #endregion

        /// <summary>
        /// 초기화 함수
        /// </summary>
        public static void Init()
        {
            LoadMasterInfoData();
        }

        /// <summary>
        /// 데이터베이스 연결을 종료한다.
        /// </summary>
        public static void CloseDataBase()
        {
            try
            {
                lock (closeObject)
                {
                    if (oracleDb != null)
                    {
                        oracleDb.Close();
                        oracleDb = null;
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "DB CLOSE함"));
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "CheckDataBase()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 타이머 이벤트
        /// </summary>
        public static void ProcTimer()
        {
            try
            {
                lock (oldTimeObject)
                {
                    if (NCASBIZ.NCasUtility.NCasUtilityMng.INCasEtcUtility.CheckTimeOut(oldTime, DateTime.Now, 0, 0, 30))
                    {
                        CloseDataBase();
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "ProcTimer()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 데이터베이스를 오픈하고 NcasOracleDb를 반환한다.
        /// </summary>
        public static NCasOracleDb OpenDataBase()
        {
            try
            {
                oldTime = DateTime.Now;
                lock (oldTimeObject)
                {
                    if (oracleDb == null || !oracleDb.IsOpen)
                    {
                        oracleDb = new NCasOracleDb();
                        oracleDb.Open(ConfigMng.LocalDbServerIp, ConfigMng.LocalDbServerSid, ConfigMng.LocalDbServerUserId, ConfigMng.LocalDbServerPw);
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "DB 오픈함"));
                    }
                    else
                    {
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "DB 오픈 되어있음"));
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "OpenDataBase()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return oracleDb;
        }

        /// <summary>
        /// IP를 넘겨받아 해당하는 MasterInfoData를 반환한다.
        /// </summary>
        /// <param name="ip"></param>
        public static MasterInfoData GetMasterInfoDataToIp(string ip)
        {
            MasterInfoData masterInfoData = null;
            try
            {
                if (dicMasterInfoData.ContainsKey(ip))
                {
                    masterInfoData = dicMasterInfoData[ip];
                }
            }
            catch (Exception err)
            {
                string functionName = "GetMasterInfoDataToIp(string ip)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return masterInfoData;
        }

        /// <summary>
        /// 발령원이 시도일 경우 해당 시도의 code값을 반환한다.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="source"></param>
        public static int GetAlarmAreaProvCode(uint ip, NCasDefineOrderSource source)
        {
            int provCode = -1;
            try
            {
                //string[] arrIp = ip.Split('.');
                //if (arrIp.Length < 3)
                //{
                //    return provCode;
                //}

                //string provIp = string.Format("{0}.{1}.0.0", arrIp[0], arrIp[1]);

                //if (source != NCasDefineOrderSource.CentCtrlRoom)
                //{
                //    foreach (MasterInfoData mi in dicMasterInfoData.Values)
                //    {
                //        if (provIp == mi.NetId)
                //        {
                //            provCode = mi.Code;
                //            break;
                //        }
                //    }
                //}

                if (source != NCasDefineOrderSource.CentCtrlRoom)
                {
                    uint beIp;
                    foreach (MasterInfoData mi in dicMasterInfoData.Values)
                    {
                        beIp = NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP(mi.NetId);
                        if ((beIp | 0x0007ffff) == ip)
                        {
                            provCode = mi.ProvCode;
                            break;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                string functionName = "GetAlarmAreaProvCode(uint ip, NCasDefineOrderSource source)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return provCode;
        }

        /// <summary>
        /// 세션과 IP를 넘겨받아 해당 경보단말 MasterInfoData를 반환한다.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static MasterInfoData GetAlarmMasterInfoData(NCASBIZ.NCasDefine.NCasDefineSectionCode section, uint ip)
        {
            MasterInfoData masterInfoData = null;
            try
            {
                uint newIp = ip;
                string masterInfoIp = string.Empty;

                switch (section)
                {
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionGlobal:
                        newIp = 0x0a010100;
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn1:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn2:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn3:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn4:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn5:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn6:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn7:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionProv:
                        newIp &= 0xFFF80000;
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRept:
                        //newIp &= 0xFFFF0000;
                        newIp &= 0xFFFFFF00;
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionDist:
                        if ((newIp & 0xFFFFFF00) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.122.21.0"))
                        {
                            newIp &= 0xFFFFFF00;
                        }
                        else if ((newIp & 0xFFFF0000) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.34.0.0") ||
                                 (newIp & 0xFFFF0000) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.98.0.0") ||
                                 (newIp & 0xFFFF0000) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.50.0.0") ||
                                 (newIp & 0xFFFF0000) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.122.0.0") ||
                                 (newIp & 0xFFFF0000) == NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.StringIP2UintIP("10.82.0.0"))
                        {
                            newIp = (newIp & 0xFFFFFF00) - 0x00020000;
                        }
                        else if (0x0000FDFF == (newIp & 0x0000FDFF))
                        {
                            newIp &= 0xFFFF0000;
                        }
                        else
                        {
                            newIp &= 0xFFFFFF00;
                        }
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionTerm:
                        break;
                }
                masterInfoIp = NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.UintIP2StringIP(newIp);
                if (dicMasterInfoData.ContainsKey(masterInfoIp))
                {
                    masterInfoData = dicMasterInfoData[masterInfoIp];
                }
            }
            catch (Exception err)
            {
                string functionName = "GetAlarmMasterInfoData(NCASBIZ.NCasDefine.NCasDefineSectionCode section, uint ip)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return masterInfoData;
        }

        /// <summary>
        /// 세션과 IP를 넘겨받아 해당 방송단말 MasterInfoData를 반환한다.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static MasterInfoData GetBroadMasterInfoData(NCASBIZ.NCasDefine.NCasDefineSectionCode section, uint ip)
        {
            MasterInfoData masterInfoData = null;
            try
            {
                uint newIp = ip;
                string masterInfoIp = string.Empty;

                switch (section)
                {
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionGlobal:
                        newIp = 0x0a010100;
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn1:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn2:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn3:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn4:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn5:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn6:
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionRegn7:
                        if (newIp == 0x0a01FFFF)
                        {
                            newIp = 0x0a010100;
                        }
                        else if ((newIp & 0xFFFFFF00) == 0x0a01FE00)
                        {

                        }
                        else
                        {
                            newIp &= 0xFFFD0000;
                        }
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionProv:
                        if (newIp == 0x0a01FFFF)
                        {
                            newIp = 0x0a010100;
                        }
                        else if ((newIp & 0xFFFFFF00) == 0x0a01FE00)
                        {

                        }
                        else if ((newIp & 0xFFFFFF00) == 0x0A024300)
                        {

                        }
                        else
                        {
                            newIp &= 0xFFFD0000;
                        }
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionDist:
                        if ((newIp & 0x0000FDFF) == 0x0000FDFF)
                        {
                            newIp &= 0xFFFF0000;
                        }
                        else
                        {
                            newIp &= 0xFFFFFF00;
                        }
                        break;
                    case NCASBIZ.NCasDefine.NCasDefineSectionCode.SectionTerm:
                        break;
                }
                masterInfoIp = NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.UintIP2StringIP(newIp);
                if (dicMasterInfoData.ContainsKey(masterInfoIp))
                {
                    masterInfoData = dicMasterInfoData[masterInfoIp];
                }
            }
            catch (Exception err)
            {
                string functionName = "CheckDataBase()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return masterInfoData;
        }

        /// <summary>
        /// 소멸자
        /// </summary>
        public static void Uninit()
        {
            CloseDataBase();
        }

        /// <summary>
        /// MasterInfo데이터를 불러온다.
        /// </summary>
        private static void LoadMasterInfoData()
        {
            NCasOracleDb oracleDb = null;
            OracleDataReader dataReader = null;
            try
            {
                oracleDb = OpenDataBase();
                oracleDb.QueryData(QueryMng.GetLoadMasterInfoDataQuery(), out dataReader);
                if (dataReader != null)
                {
                    MasterInfoData masterInfoData = null;
                    while (dataReader.Read())
                    {
                        masterInfoData = new MasterInfoData();
                        if (dataReader["CODE"] != DBNull.Value)
                        {
                            masterInfoData.Code = int.Parse(dataReader["CODE"].ToString());
                        }
                        if (dataReader["SERIALNO"] != DBNull.Value)
                        {
                            masterInfoData.SerialNo = int.Parse(dataReader["SERIALNO"].ToString());
                        }
                        if (dataReader["NAME"] != DBNull.Value)
                        {
                            masterInfoData.Name = dataReader["NAME"].ToString();
                        }
                        if (dataReader["NETID"] != DBNull.Value)
                        {
                            masterInfoData.NetId = dataReader["NETID"].ToString();
                        }
                        if (dataReader["SUBMASK"] != DBNull.Value)
                        {
                            masterInfoData.SubMask = dataReader["SUBMASK"].ToString();
                        }
                        if (dataReader["MASTERTYPE"] != DBNull.Value)
                        {
                            masterInfoData.MasterType = int.Parse(dataReader["MASTERTYPE"].ToString());
                        }
                        if (dataReader["PARENTCODE"] != DBNull.Value)
                        {
                            masterInfoData.ParentCode = int.Parse(dataReader["PARENTCODE"].ToString());
                        }
                        if (dataReader["REPTCODE"] != DBNull.Value)
                        {
                            masterInfoData.ReptCode = int.Parse(dataReader["REPTCODE"].ToString());
                        }
                        if (dataReader["PROVCODE"] != DBNull.Value)
                        {
                            masterInfoData.ProvCode = int.Parse(dataReader["PROVCODE"].ToString());
                        }
                        if (dataReader["DISTCODE"] != DBNull.Value)
                        {
                            masterInfoData.DistCode = int.Parse(dataReader["DISTCODE"].ToString());
                        }
                        if (dataReader["DISTNAME"] != DBNull.Value)
                        {
                            masterInfoData.DistName = dataReader["DISTNAME"].ToString();
                        }
                        if (dataReader["TERMCODE"] != DBNull.Value)
                        {
                            masterInfoData.TermCode = int.Parse(dataReader["TERMCODE"].ToString());
                        }
                        if (dataReader["TERMNAME"] != DBNull.Value)
                        {
                            masterInfoData.TermName = dataReader["TERMNAME"].ToString();
                        }
                        if (dataReader["SATEFLAG"] != DBNull.Value)
                        {
                            masterInfoData.SateFlag = int.Parse(dataReader["SATEFLAG"].ToString());
                        }
                        if (dataReader["REPTFLAG"] != DBNull.Value)
                        {
                            masterInfoData.ReptFlag = int.Parse(dataReader["REPTFLAG"].ToString());
                        }
                        if (dataReader["DEVID"] != DBNull.Value)
                        {
                            masterInfoData.DevId = int.Parse(dataReader["DEVID"].ToString());
                        }
                        if (dataReader["BROADID"] != DBNull.Value)
                        {
                            masterInfoData.BroadId = int.Parse(dataReader["BROADID"].ToString());
                        }
                        if (dataReader["BROADNETID"] != DBNull.Value)
                        {
                            masterInfoData.BroadNetId = dataReader["BROADNETID"].ToString();
                        }
                        if (dataReader["BROADSUBMASK"] != DBNull.Value)
                        {
                            masterInfoData.BroadSubMask = dataReader["BROADSUBMASK"].ToString();
                        }
                        if (dataReader["DEPTNETID"] != DBNull.Value)
                        {
                            masterInfoData.DeptNetId = dataReader["DEPTNETID"].ToString();
                        }
                        if (dataReader["DEPTSUBMASK"] != DBNull.Value)
                        {
                            masterInfoData.DeptSubMask = dataReader["DEPTSUBMASK"].ToString();
                        }
                        if (dataReader["REPTNETID"] != DBNull.Value)
                        {
                            masterInfoData.ReptNetId = dataReader["REPTNETID"].ToString();
                        }
                        if (dataReader["REPTSUBMASK"] != DBNull.Value)
                        {
                            masterInfoData.ReptSubMask = dataReader["REPTSUBMASK"].ToString();
                        }
                        if (dataReader["VOICELINENO"] != DBNull.Value)
                        {
                            masterInfoData.VoiceLineNo = dataReader["VOICELINENO"].ToString();
                        }
                        if (dataReader["USEFLAG"] != DBNull.Value)
                        {
                            masterInfoData.UseFlag = int.Parse(dataReader["USEFLAG"].ToString());
                        }
                        if (dataReader["OLDSYSFLAG"] != DBNull.Value)
                        {
                            masterInfoData.OldSysFlag = int.Parse(dataReader["OLDSYSFLAG"].ToString());
                        }
                        if (dataReader["TERMFLAG"] != DBNull.Value)
                        {
                            masterInfoData.TermFlag = int.Parse(dataReader["TERMFLAG"].ToString());
                        }
                        if (dataReader["DSUSHELLNO"] != DBNull.Value)
                        {
                            masterInfoData.DsuShellNo = int.Parse(dataReader["DSUSHELLNO"].ToString());
                        }
                        if (dataReader["DSUCARDNO"] != DBNull.Value)
                        {
                            masterInfoData.DsuCardNo = int.Parse(dataReader["DSUCARDNO"].ToString());
                        }

                        dicMasterInfoData.Add(masterInfoData.NetId, masterInfoData);
                    }
                    dataReader.Dispose();
                }
            }
            catch (Exception err)
            {
                string functionName = "LoadMasterInfoData()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            finally
            {
                if (dataReader != null)
                {
                    dataReader.Dispose();
                }
            }
        }

        /// <summary>
        /// 해당 서버의 CODE를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static int GetLocalCode()
        {
            int localCode = 0;
            try
            {
                string provIp = string.Empty;
                string[] ip = ConfigMng.LocalIp.Split('.');
                if (ip.Length < 3)
                {
                    return localCode;
                }
                provIp = string.Format("{0}.{1}.{2}.{3}", ip[0], ip[1], 0, 0);

                if (dicMasterInfoData.ContainsKey(provIp))
                {
                    localCode = dicMasterInfoData[provIp].Code;
                }
            }
            catch (Exception err)
            {
                string functionName = "GetLocalCode()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return localCode;
        }
    }
}
