//#define debug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasBiz;
using NCASFND.NCasDb;
using NCASFND.NCasNet;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasType;
using System.Windows.Forms;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasEnv;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;
using Oracle.DataAccess.Client;

namespace NCasPDbManager
{
    public class DbManagerBiz : NCasBizProcess
    {
        #region TEST
        //int tcpRecvCount = 0;
        //int tcpInsertAsyncDataCount = 0;
        //int tcpInsertAsyncArchiveCount = 0;
        //int asyncExternCount = 0;
        //int asyncDispCount = 0;
        //int tc1Count = 0;
        //int tc2Count = 0;
        //int tc3Count = 0;
        #endregion

        #region Fields
        /// <summary>
        /// MainForm
        /// </summary>
        private MainForm mainForm = null;
        /// <summary>
        /// TCP서버 매니저
        /// </summary>
        private NCasNetSessionServerMng tcpServerMng = null;
        /// <summary>
        /// UDP수신을 위한 소켓
        /// 기존 시도시스템에서 들어오는 데이터를 수용하기 위한 UDP
        /// </summary>
        private NCasUdpSocket udpSocket = null;
        /// <summary>
        /// 처리 카운트
        /// </summary>
        private int processingCount = 0;
        /// <summary>
        /// 대기카운트
        /// </summary>
        private int standbyCount = 0;

        private bool isFirstConnect = false;
        #endregion

        public DbManagerBiz(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        #region Init Uninit Code
        /// <summary>
        /// 데이터베이스 연결을 종료한다.
        /// </summary>
        private void CloseDataBase()
        {
            BasicDataMng.CloseDataBase();
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public void Init()
        {
            InitNet();
        }

        /// <summary>
        /// 소멸자
        /// </summary>
        public void UnInit()
        {
            UnInitNet();
            CloseDataBase();
        }

        /// <summary>
        /// TCP를 소멸한다.
        /// </summary>
        private void UnInitTcp()
        {
            try
            {
                if (tcpServerMng != null)
                {
                    tcpServerMng.AcceptNetSessionClient -= new NCASBIZ.NCasNetSessionAcceptEventHandler(tcpServerMng_AcceptNetSessionClient);
                    tcpServerMng.CloseNetSessionClient -= new NCASBIZ.NCasNetSessionCloseEventHandler(tcpServerMng_CloseNetSessionClient);
                    tcpServerMng.RecvNetSessionClient -= new NCASBIZ.NCasNetSessionRecvEventHandler(tcpServerMng_RecvNetSessionClient);
                    tcpServerMng.StopSessionServerMng();
                }
            }
            catch (Exception err)
            {
                string functionName = "UnInitTcp()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// UDP를 소멸한다.
        /// </summary>
        private void UnInitUdpNet()
        {
            try
            {
                if (udpSocket != null)
                {
                    udpSocket.ReceivedData -= new NCASFND.NCasUdpRecvEventHandler(udpSocket_ReceivedData);
                    udpSocket.Close();
                }
            }
            catch (Exception err)
            {
                string functionName = "UnInitUdpNet()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 네트워크를 소멸한다.
        /// </summary>
        private void UnInitNet()
        {
            UnInitUdpNet();
            UnInitTcp();
        }

        /// <summary>
        /// TP통신을 초기화 한다.
        /// </summary>
        private void InitTcpNet()
        {
            try
            {
                tcpServerMng = new NCasNetSessionServerMng();
                foreach (NCasProfile profile in ConfigMng.LstNCasProfile)
                {
                    mainForm.RegisterTcpProfile(profile);
                    tcpServerMng.AddProfile(profile);
                }

                //NCasEnvironmentMng.NCasEnvConfig.NetSessionContext.UseConnectionChecking = false;

                tcpServerMng.AcceptNetSessionClient += new NCASBIZ.NCasNetSessionAcceptEventHandler(tcpServerMng_AcceptNetSessionClient);
                tcpServerMng.CloseNetSessionClient += new NCASBIZ.NCasNetSessionCloseEventHandler(tcpServerMng_CloseNetSessionClient);
                tcpServerMng.RecvNetSessionClient += new NCASBIZ.NCasNetSessionRecvEventHandler(tcpServerMng_RecvNetSessionClient);

                byte[] pollingData = GetPollingData();
                if (pollingData == null)
                {
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), "InitTcpNet() : 폴링데이터 생성 실패(null)");
                }
                else
                {
                    tcpServerMng.PollingDatas = GetPollingData();
                }

                if (ConfigMng.TcpIp == string.Empty)
                {
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), "InitTcpNet() : 폴링데이터 생성 실패(null)");
                }
                tcpServerMng.StartSessionServerMng(ConfigMng.TcpIp, ConfigMng.TcpPort);
            }
            catch (Exception err)
            {
                string functionName = "InitTcpNet()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private byte[] GetPollingData()
        {
            NCasProtocolBase protoBase = null;
            byte[] sendData = null;
            try
            {
                protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcDevStsReq);
                NCasProtocolTc7 protocolTc7 = protoBase as NCasProtocolTc7;
                sendData = NCasProtocolFactory.MakeTcpFrame(protoBase);
            }
            catch (Exception err)
            {
                string functionName = "GetPollingData()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return sendData;
        }

        /// <summary>
        /// UDP통신을 초기화 한다.
        /// </summary>
        private void InitUdpNet()
        {
            try
            {
                udpSocket = new NCasUdpSocket();
                udpSocket.Listen(ConfigMng.UdpIp, ConfigMng.UdpPort);
                udpSocket.ReceivedData += new NCASFND.NCasUdpRecvEventHandler(udpSocket_ReceivedData);
            }
            catch (Exception err)
            {
                string functionName = "InitUdpNet()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 네트워크 설정을 초기화한다.
        /// </summary>
        private void InitNet()
        {
            InitUdpNet();
            InitTcpNet();
        }
        #endregion

        /// <summary>
        /// 경보발령 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc1(TcData tcData)
        {
            try
            {
                NCasProtocolTc1 tc1 = null;

                tc1 = tcData.ProtoBase as NCasProtocolTc1;
                string eventText = string.Empty;

                if (tc1 == null)
                {
                    eventText = "ProcTc1(NCasProtocolBase protoBase) : TC1을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                //if (tc1.CtrlKind == NCasDefineControlKind.ControlBraod)
                //{
                //    eventText = "ProcTc1(NCasProtocolBase protoBase) : TC1을 수신하였으나 방송발령 정보이므로 처리하지 못하고 ProcTc4()를 호출함";
                //    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                //    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                //    ProcTc4(tcData);
                //    return;
                //}

                //if (CheckBroadIp(tc1.AlarmNetIdOrIpByString))
                //{
                //    eventText = "ProcTc1(NCasProtocolBase protoBase) : TC1을 수신하였으나 방송발령 정보이므로 처리하지 못하고 ProcTc4()를 호출함";
                //    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                //    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                //    ProcTc4(tcData);
                //    return;
                //}

                if (tc1.Source == NCasDefineOrderSource.ProvCtrlRoom && NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.IsDeptOrderByTc1Tc2Tc3(tc1.BroadNetIdOrIpByString))
                {
                    eventText = "ProcTc1(NCasProtocolBase protoBase) : TC1을 수신하였으나 방송발령 정보이므로 처리하지 못하고 ProcTc4()를 호출함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    NCasProtocolTc4 tc4 = GetTc4ToTc1(tc1);
                    tcData.ProtoBase = tc4;
                    ProcTc4(tcData);
                    return;
                }

                MasterInfoData masterinfoData = null;

                masterinfoData = BasicDataMng.GetAlarmMasterInfoData(tc1.Sector, tc1.AlarmNetIdOrIp);
                if (masterinfoData == null)
                {
                    eventText = string.Format("ProcTc1(NCasProtocolBase protoBase) : TC1을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc1.AlarmNetIdOrIp.ToString(), tc1.AlarmNetIdOrIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                #region 발령정보 데이터 입력
                //이미 들어있는 데이인지 확인
                string query = string.Empty;
                query = QueryMng.GetCheckHaveAlarmOrderHistQuery(tc1.Mode, tc1.OrderTimeByDateTime, masterinfoData.Code, (int)tc1.Source);

                int sameOrderCount = CountSelectQueryData(query);
                if (sameOrderCount > 0)
                {
                    return;
                }

                //OrderHist테이블에 저장
                query = QueryMng.GetInsertAlarmOrderHistQuery(tc1.Mode, tc1.OrderTimeByDateTime, masterinfoData.Code, (int)tc1.CtrlKind, (int)tc1.Source, (int)tc1.AlarmKind, (int)tc1.Sector, (int)tc1.Media, (int)tc1.RespReqFlag, 1);
                InsertUpdateDeleteQueryData(query);
                #endregion

                #region 발령에 대한 응답/결과 데이터 임시로 입력
                //응답
                query = QueryMng.GetInsertAlarmRespResultHistQuery(tc1, NCasDefineTcCode.TcAlarmResponse, masterinfoData);
                InsertUpdateDeleteQueryData(query);

                //결과
                query = QueryMng.GetInsertAlarmRespResultHistQuery(tc1, NCasDefineTcCode.TcAlarmResult, masterinfoData);
                InsertUpdateDeleteQueryData(query);
                #endregion

                #region 발령정보가 없어 미 처리된 응답/결과처리
                //응답처리
                query = QueryMng.GetCheckHavePreAlarmRespResultHistQuery(tc1, NCasDefineTcCode.TcAlarmResponse);
                List<AlarmRespResultData> lstAlarmRespResultData = null;
                lstAlarmRespResultData = GetAlarmRespResultData(query);

                if (lstAlarmRespResultData != null)
                {
                    for (int i = 0; i < lstAlarmRespResultData.Count; i++)
                    {
                        query = QueryMng.GetUpdateAlarmRespResultHistQuery(tc1.Mode, NCasDefineTcCode.TcAlarmResponse, tc1.OrderTimeByDateTime, lstAlarmRespResultData[i].RespResultTime, lstAlarmRespResultData[i].AreaCode, lstAlarmRespResultData[i].BroadCtrlFlag, lstAlarmRespResultData[i].Section, lstAlarmRespResultData[i].AlarmKind, lstAlarmRespResultData[i].Media, lstAlarmRespResultData[i].RespResultFlag, lstAlarmRespResultData[i].Source, lstAlarmRespResultData[i].DevCode, lstAlarmRespResultData[i].DevKind, string.Empty);
                        InsertUpdateDeleteQueryData(query);
                        query = QueryMng.GetDeletePreAlarmRespResultHistQuery(NCasDefineTcCode.TcAlarmResponse, tc1, lstAlarmRespResultData[i]);
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                //결과처리
                query = QueryMng.GetCheckHavePreAlarmRespResultHistQuery(tc1, NCasDefineTcCode.TcAlarmResult);
                lstAlarmRespResultData = GetAlarmRespResultData(query);

                if (lstAlarmRespResultData != null)
                {
                    for (int i = 0; i < lstAlarmRespResultData.Count; i++)
                    {
                        query = QueryMng.GetUpdateAlarmRespResultHistQuery(tc1.Mode, NCasDefineTcCode.TcAlarmResult, tc1.OrderTimeByDateTime, lstAlarmRespResultData[i].RespResultTime, lstAlarmRespResultData[i].AreaCode, lstAlarmRespResultData[i].BroadCtrlFlag, lstAlarmRespResultData[i].Section, lstAlarmRespResultData[i].AlarmKind, lstAlarmRespResultData[i].Media, lstAlarmRespResultData[i].RespResultFlag, lstAlarmRespResultData[i].Source, lstAlarmRespResultData[i].DevCode, lstAlarmRespResultData[i].DevKind, string.Empty);
                        InsertUpdateDeleteQueryData(query);
                        query = QueryMng.GetDeletePreAlarmRespResultHistQuery(NCasDefineTcCode.TcAlarmResult, tc1, lstAlarmRespResultData[i]);
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                SetProcessingStandbyCount();
                #endregion
            }
            catch (Exception err)
            {
                string functionName = "ProcTc1(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// TC1이지만 IP가 방송 또는 주요기관인지 확인하는 함수
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CheckBroadIp(string p)
        {
            bool isBroad = false;
            try
            {
                string broadIp = string.Empty;
                string[] ip = p.Split('.');
                if (ip.Length < 3)
                {
                    return false;
                }
                broadIp = ip[2].ToString();
                if(broadIp == "253" || broadIp == "254")
                {
                    isBroad = true;
                }
            }
            catch (Exception err)
            {
                string functionName = "CheckBroadIp()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return isBroad;
        }

        /// <summary>
        /// 경보응답 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc2(TcData tcData)
        {
            try
            {
                NCasProtocolTc2 tc2 = null;
                string eventText = string.Empty;

                tc2 = tcData.ProtoBase as NCasProtocolTc2;

                if (tc2 == null)
                {
                    eventText = "ProcTc2(NCasProtocolBase protoBase) : TC2을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (tc2.Source == NCasDefineOrderSource.ProvCtrlRoom && NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.IsDeptOrderByTc1Tc2Tc3(tc2.BroadNetIdOrIpByString))
                {
                    eventText = "ProcTc1(NCasProtocolBase protoBase) : TC2을 수신하였으나 방송발령 정보이므로 처리하지 못하고 ProcTc5()를 호출함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    NCasProtocolTc5 tc5 = GetTc5ToTc2(tc2);
                    tcData.ProtoBase = tc5;
                    ProcTc5(tcData);
                    return;
                }

                MasterInfoData masterinfoData = null;
                masterinfoData = BasicDataMng.GetMasterInfoDataToIp(tc2.RespNetIdOrIpByString);
                if (masterinfoData == null)
                {
                    eventText = string.Format("ProcTc2(NCasProtocolBase protoBase) : TC2을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc2.RespNetIdOrIp.ToString(), tc2.RespNetIdOrIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData areaMasterInfoData = null;
                areaMasterInfoData = BasicDataMng.GetAlarmMasterInfoData(tc2.Sector, tc2.RespNetIdOrIp);
                if (areaMasterInfoData == null)
                {
                    eventText = string.Format("ProcTc2(NCasProtocolBase protoBase) : TC2을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc2.RespNetIdOrIp.ToString(), tc2.RespNetIdOrIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;

                query = QueryMng.GetCheckHaveAlarmRespResultHistQuery(tc2.Mode, NCasDefineTcCode.TcAlarmResponse, masterinfoData, tc2.OrderTimeByDateTime, (int)tc2.Source);

                int sameOrderCount = CountSelectQueryData(query);
                if (sameOrderCount > 0)//데이터가 들어있으면 업데이트하고 없으면 Pre테이블에 저장한다.
                {
                    query = QueryMng.GetUpdateAlarmRespResultHistQuery(tc2.Mode, NCasDefineTcCode.TcAlarmResponse, tc2.OrderTimeByDateTime, tcData.RecvTime, areaMasterInfoData.Code, (int)tc2.CtrlKind, (int)tc2.Sector, (int)tc2.AlarmKind, (int)tc2.Media, (int)tc2.RespStatus, (int)tc2.Source, masterinfoData.Code, masterinfoData.DevId, string.Empty);
                    InsertUpdateDeleteQueryData(query);
                }
                else
                {
                    query = QueryMng.GetInsertPreAlarmRespResultHistQuery(tc2.Mode, NCasDefineTcCode.TcAlarmResponse, masterinfoData, tc2.OrderTimeByDateTime, tcData.RecvTime, (int)tc2.CtrlKind, (int)tc2.Sector, (int)tc2.Source, (int)tc2.AlarmKind, (int)tc2.Media, string.Empty);
                    InsertUpdateDeleteQueryData(query);
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc2(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 경보결과 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc3(TcData tcData)
        {
            try
            {
                NCasProtocolTc3 tc3 = null;
                tc3 = tcData.ProtoBase as NCasProtocolTc3;
                string eventText = string.Empty;

                if (tc3 == null)
                {
                    eventText = "ProcTc3(NCasProtocolBase protoBase) : TC3을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (tc3.Source == NCasDefineOrderSource.ProvCtrlRoom && NCASBIZ.NCasUtility.NCasUtilityMng.INCasCommUtility.IsDeptOrderByTc1Tc2Tc3(tc3.BroadNetIdOrIpByString))
                {
                    eventText = "ProcTc3(NCasProtocolBase protoBase) : TC3을 수신하였으나 방송발령 정보이므로 처리하지 못하고 ProcTc6()를 호출함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    NCasProtocolTc6 tc6 = GetTc6ToTc3(tc3);
                    tcData.ProtoBase = tc6;
                    ProcTc6(tcData);
                    return;
                }

                MasterInfoData masterinfoData = null;
                masterinfoData = BasicDataMng.GetMasterInfoDataToIp(tc3.RespNetIdOrIpByString);
                if (masterinfoData == null)
                {
                    eventText = string.Format("ProcTc3(NCasProtocolBase protoBase) : TC3을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc3.RespNetIdOrIp, tc3.RespNetIdOrIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData areaMasterInfoData = null;
                areaMasterInfoData = BasicDataMng.GetAlarmMasterInfoData(tc3.Sector, tc3.RespNetIdOrIp);
                if (areaMasterInfoData == null)
                {
                    eventText = string.Format("ProcTc3(NCasProtocolBase protoBase) : TC3을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc3.RespNetIdOrIp, tc3.RespNetIdOrIpByString, ((int)tc3.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                query = QueryMng.GetCheckHaveAlarmRespResultHistQuery(tc3.Mode, NCasDefineTcCode.TcAlarmResult, masterinfoData, tc3.OrderTimeByDateTime, (int)tc3.Source);

                int sameOrderCount = CountSelectQueryData(query);
                if (sameOrderCount > 0)//데이터가 들어있으면 업데이트하고 없으면 Pre테이블에 저장한다.
                {
                    query = QueryMng.GetUpdateAlarmRespResultHistQuery(tc3.Mode, NCasDefineTcCode.TcAlarmResult, tc3.OrderTimeByDateTime, tcData.RecvTime, areaMasterInfoData.Code, (int)tc3.CtrlKind, (int)tc3.Sector, (int)tc3.AlarmKind, (int)tc3.Media, (int)tc3.RsltStatus, (int)tc3.Source, masterinfoData.Code, masterinfoData.DevId, MakeHornSpeakerStatusString(tc3.SpkStatus));
                    InsertUpdateDeleteQueryData(query);
                }
                else
                {
                    if (tc3.Source == NCasDefineOrderSource.ProvTermSelf)
                    {
#if debug
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "----------단말자체발령 들어옴----------"));
#endif
                        query = QueryMng.GetInsertAlarmOrderHistQuery(tc3.Mode, tc3.OrderTimeByDateTime, masterinfoData.Code, (int)tc3.CtrlKind, (int)tc3.Source, (int)tc3.AlarmKind, (int)tc3.Sector, (int)tc3.Media, (int)tc3.RsltStatus, 1);
                        InsertUpdateDeleteQueryData(query);
#if debug
                        NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "단말자체발령 ORDER쿼리 : " + query));
#endif
                        query = QueryMng.GetInsertAlarmRespResultHistQuery(tc3.Mode, NCasDefineTcCode.TcAlarmResponse, masterinfoData, tc3.OrderTimeByDateTime, tcData.RecvTime, masterinfoData.Code, (int)tc3.CtrlKind, (int)tc3.Sector, (int)tc3.Source, (int)tc3.AlarmKind, (int)tc3.Media, (int)tc3.RsltStatus, string.Empty);
                        InsertUpdateDeleteQueryData(query);

                        query = QueryMng.GetInsertAlarmRespResultHistQuery(tc3.Mode, NCasDefineTcCode.TcAlarmResult, masterinfoData, tc3.OrderTimeByDateTime, tcData.RecvTime, masterinfoData.Code, (int)tc3.CtrlKind, (int)tc3.Sector, (int)tc3.Source, (int)tc3.AlarmKind, (int)tc3.Media, (int)tc3.RsltStatus, MakeHornSpeakerStatusString(tc3.SpkStatus));
                        InsertUpdateDeleteQueryData(query);
                    }
                    else
                    {
                        query = QueryMng.GetInsertPreAlarmRespResultHistQuery(tc3.Mode, NCasDefineTcCode.TcAlarmResult, masterinfoData, tc3.OrderTimeByDateTime, tcData.RecvTime, (int)tc3.CtrlKind, (int)tc3.Sector, (int)tc3.Source, (int)tc3.AlarmKind, (int)tc3.Media, MakeHornSpeakerStatusString(tc3.SpkStatus));
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc3(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송발령 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc4(TcData tcData)
        {
            try
            {
                NCasProtocolTc4 tc4 = null;
                tc4 = tcData.ProtoBase as NCasProtocolTc4;
                string eventText = string.Empty;

                if (tc4 == null)
                {
                    eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData areaMasterInfoData = null;
                MasterInfoData devMasterInfoData = null;
                areaMasterInfoData = BasicDataMng.GetBroadMasterInfoData(tc4.Sector, tc4.BroadNetIdOrIp);

                if (areaMasterInfoData == null)
                {
                    eventText = string.Format("ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 등록되어 있는 지역정보가 없어 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc4.BroadNetIdOrIp.ToString(), tc4.BroadNetIdOrIpByString, ((int)tc4.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                devMasterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc4.BroadNetIdOrIpByString);

                if (devMasterInfoData == null)
                {
                    eventText = string.Format("ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc4.BroadNetIdOrIp.ToString(), tc4.BroadNetIdOrIpByString, ((int)tc4.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                //StringBuilder strBuilder = new StringBuilder();
                NCasDefineSectionCode sectionVal = NCasDefineSectionCode.None;
                NCasDefineDeviceKind devKind = NCasDefineDeviceKind.None;
                string query = string.Empty;
                string orderTableName = string.Empty;
                string respTableName = string.Empty;
                string resultTableName = string.Empty;
                int queryCount = 0;
                if (tc4.Mode == NCasDefineOrderMode.RealMode)
                {
                    orderTableName = "REALBROADORDERHIST";
                    respTableName = "REALBROADRESPHIST";
                    resultTableName = "REALBROADRESULTHIST";
                }
                else
                {
                    orderTableName = "TESTBROADORDERHIST";
                    respTableName = "TESTBROADRESPHIST";
                    resultTableName = "TESTBROADRESULTHIST";
                }

                if ((tc4.BroadNetIdOrIp & 0xffff00ff) == 0x0a0100ff)
                {
                    sectionVal = NCasDefineSectionCode.SectionGlobal;
                }
                else
                {
                    if ((tc4.BroadNetIdOrIp & 0xffffff00) == 0x0a01fe00)
                    {
                        sectionVal = NCasDefineSectionCode.SectionGlobal;
                    }
                    else if ((tc4.BroadNetIdOrIp & 0xffffff00) == 0x0A024300)
                    {
                        sectionVal = NCasDefineSectionCode.SectionGlobal;
                    }
                    else
                    {
                        sectionVal = tc4.Sector;
                    }
                }

                switch (sectionVal)
                {
                    case NCasDefineSectionCode.SectionGlobal:
                        if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FEFF)//전국방송
                        {
                            devKind = NCasDefineDeviceKind.CentBroadSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                            {
                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9}, '{10}' FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                    orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                    respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                    resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);
                            }
                            else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                            {
                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9}, '{10}' FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                    orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                        respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);

                                string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);

                            }
                        }
                        if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)//전국기관
                        {
                            devKind = NCasDefineDeviceKind.DeptSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND DevKind = {3}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                            {
                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9}, '{10}' FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                    orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                        respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);
                            }
                            else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                            {
                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9}, '{10}' FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                    orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                        respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);

                                query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                                InsertUpdateDeleteQueryData(query);
                            }
                        }

                        if ((((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) || (tc4.BroadNetIdOrIp & 0x0000FF00) == 0x00004300) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))//시도특정방송
                        {
                            int areaCode = 1670;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Code = {2}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), devMasterInfoData.Code);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10}, '{11}' FROM DUAL",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, devMasterInfoData.Code, devMasterInfoData.DevId, tc4.AlarmNetIdOrIp.ToString());
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                            InsertUpdateDeleteQueryData(query);
                        }

                        if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FD00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                        {
                            devKind = NCasDefineDeviceKind.PdeptSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Code = {2}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), devMasterInfoData.Code);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10}, '{11}' FROM DUAL",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, devMasterInfoData.Code, devMasterInfoData.DevId, tc4.AlarmNetIdOrIp.ToString());
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                            InsertUpdateDeleteQueryData(query);
                        }
                        break;

                    case NCasDefineSectionCode.SectionRegn1:
                    case NCasDefineSectionCode.SectionRegn2:
                    case NCasDefineSectionCode.SectionRegn3:
                    case NCasDefineSectionCode.SectionRegn4:
                    case NCasDefineSectionCode.SectionRegn5:
                    case NCasDefineSectionCode.SectionRegn6:
                    case NCasDefineSectionCode.SectionRegn7:
                    case NCasDefineSectionCode.SectionProv:
                        if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FEFF)
                        {
                            devKind = NCasDefineDeviceKind.ProvBroadSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} A, ProvBroadInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9}, '{10}' FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {11} AND UseFlag = 1",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString(), areaMasterInfoData.Code);//Check
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {8} AND UseFlag = 1",
                                    respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {8} AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);
                        }

                        if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)
                        {
                            devKind = NCasDefineDeviceKind.PdeptSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} A, ProvDeptInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9}, '{10}' FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {11} AND UseFlag = 1",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString(), areaMasterInfoData.Code);//Check
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                   respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);
                        }

                        if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                        {
                            devKind = NCasDefineDeviceKind.ProvBroadSys;

                            query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND CODE = {3}",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, areaMasterInfoData.Code);
                            queryCount = CountSelectQueryData(query);

                            if (queryCount > 0)
                            {
                                eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                                return;
                            }

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10}, '{11}' FROM DUAL",
                                orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, areaMasterInfoData.Code, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, code, {8} FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {9} AND UseFlag = 1",
                                    respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)devKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);

                            query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, code, {8} FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {9} AND UseFlag = 1",
                                        resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)devKind, areaMasterInfoData.Code);
                            InsertUpdateDeleteQueryData(query);
                        }
                        break;

                    case NCasDefineSectionCode.SectionBroad:
                        devKind = NCasDefineDeviceKind.ProvBroadSys;

                        query = string.Format("SELECT COUNT(*) COUNT FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND CODE = {3}",
                            orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, areaMasterInfoData.Code);
                        queryCount = CountSelectQueryData(query);

                        if (queryCount > 0)
                        {
                            eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                            NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                            NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                            return;
                        }

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10}, '{11}' FROM DUAL",
                            orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, areaMasterInfoData.Code, (int)devKind, tc4.AlarmNetIdOrIp.ToString());
                        InsertUpdateDeleteQueryData(query);

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code, (int)devKind);
                        InsertUpdateDeleteQueryData(query);

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code, (int)devKind);
                        InsertUpdateDeleteQueryData(query);
                        break;

                    case NCasDefineSectionCode.SectionDist:
                        devKind = NCasDefineDeviceKind.PdeptSys;

                        query = string.Format("SELECT COUNT(*) COUNT FROM {0} A, ProvDeptInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                            orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                        queryCount = CountSelectQueryData(query);

                        if (queryCount > 0)
                        {
                            eventText = "ProcTc4(NCasProtocolBase protoBase) : TC4을 수신하였으나 이미 들어있는 데이터 이므로 넘어간다.";
                            NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                            NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                            return;
                        }

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind, SHARENETID) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9}, '{10}' FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {11} AND UseFlag = 1",//Check
                            orderTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, tc4.AlarmNetIdOrIp.ToString(), areaMasterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                respTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);

                        query = string.Format("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                               resultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                        break;
                }

                SetProcessingStandbyCount();

                #region 발령정보가 없어 미 처리된 응답/결과처리
                //응답처리
                query = QueryMng.GetCheckHavePreBroadRespResultHistQuery(tc4, NCasDefineTcCode.TcBroadResponse);
                List<BroadRespResultData> lstBroadRespResultData = null;
                lstBroadRespResultData = GetBroadRespResultData(query);

                if (lstBroadRespResultData != null)
                {
                    for (int i = 0; i < lstBroadRespResultData.Count; i++)
                    {
                        query = QueryMng.GetUpdateBroadRespResultHistQuery(tc4.Mode, NCasDefineTcCode.TcBroadResponse, tc4.OrderTimeByDateTime, lstBroadRespResultData[i].RespResultTime, lstBroadRespResultData[i].AreaCode, lstBroadRespResultData[i].BroadCtrlFlag, lstBroadRespResultData[i].Section, lstBroadRespResultData[i].AlarmKind, lstBroadRespResultData[i].Media, lstBroadRespResultData[i].RespResultFlag, lstBroadRespResultData[i].Source, lstBroadRespResultData[i].DevCode, lstBroadRespResultData[i].DevKind, lstBroadRespResultData[i].CaptionCode, lstBroadRespResultData[i].CloseProcFlag);
                        InsertUpdateDeleteQueryData(query);
                        query = QueryMng.GetDeletePreBroadRespResultHistQuery(NCasDefineTcCode.TcBroadResponse, tc4, lstBroadRespResultData[i]);
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                //결과처리
                query = QueryMng.GetCheckHavePreBroadRespResultHistQuery(tc4, NCasDefineTcCode.TcBroadResult);
                lstBroadRespResultData = GetBroadRespResultData(query);

                if (lstBroadRespResultData != null)
                {
                    for (int i = 0; i < lstBroadRespResultData.Count; i++)
                    {
                        query = QueryMng.GetUpdateBroadRespResultHistQuery(tc4.Mode, NCasDefineTcCode.TcBroadResult, tc4.OrderTimeByDateTime, lstBroadRespResultData[i].RespResultTime, lstBroadRespResultData[i].AreaCode, lstBroadRespResultData[i].BroadCtrlFlag, lstBroadRespResultData[i].Section, lstBroadRespResultData[i].AlarmKind, lstBroadRespResultData[i].Media, lstBroadRespResultData[i].RespResultFlag, lstBroadRespResultData[i].Source, lstBroadRespResultData[i].DevCode, lstBroadRespResultData[i].DevKind, lstBroadRespResultData[i].CaptionCode, lstBroadRespResultData[i].CloseProcFlag);
                        InsertUpdateDeleteQueryData(query);
                        query = QueryMng.GetDeletePreBroadRespResultHistQuery(NCasDefineTcCode.TcBroadResult, tc4, lstBroadRespResultData[i]);
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                SetProcessingStandbyCount();
                #endregion
            }
            catch (Exception err)
            {
                string functionName = "ProcTc4(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송발령응답 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc5(TcData tcData)
        {
            try
            {
                NCasProtocolTc5 tc5 = null;
                tc5 = tcData.ProtoBase as NCasProtocolTc5;

                string eventText = string.Empty;

                if (tc5 == null)
                {
                    eventText = "ProcTc5(NCasProtocolBase protoBase) : TC5을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterinfoData = null;
                masterinfoData = BasicDataMng.GetAlarmMasterInfoData(tc5.Sector, tc5.BroadNetIdOrIp);
                if (masterinfoData == null)
                {
                    eventText = string.Format("ProcTc5(NCasProtocolBase protoBase) : TC5을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc5.BroadNetIdOrIp.ToString(), tc5.BroadNetIdOrIpByString, ((int)tc5.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                int areaCode = masterinfoData.Code;

                MasterInfoData masterInfoData2 = null;
                masterInfoData2 = BasicDataMng.GetMasterInfoDataToIp(tc5.RespNetIdOrIpByString);

                string query = string.Empty;
                query = QueryMng.GetCheckHaveBroadRespResultHistQuery(tc5.Mode, NCasDefineTcCode.TcBroadResponse, tc5.OrderTimeByDateTime, (int)tc5.Source, masterInfoData2.Code);
                int queryCount = CountSelectQueryData(query);

                if (queryCount != 0)
                {
                    query = QueryMng.GetUpdateBroadRespResultHist(tc5.Mode, NCasDefineTcCode.TcBroadResponse, tc5.OrderTimeByDateTime, areaCode, (int)tc5.CtrlKind, (int)tc5.Sector, (int)tc5.AlarmKind, (int)tc5.Media, DateTime.Now, masterInfoData2.Code, (int)tc5.Source, (int)tc5.RespStatus);
                    InsertUpdateDeleteQueryData(query);
                }
                else
                {
                    query = QueryMng.GetInsertPreBroadRespResultHistQuery((int)tc5.Mode, NCasDefineTcCode.TcBroadResponse, tc5.OrderTimeByDateTime, tcData.RecvTime, areaCode, (int)tc5.CtrlKind, (int)tc5.Sector, (int)tc5.Source, (int)tc5.AlarmKind, (int)tc5.Media, 1, (int)tc5.RespStatus, masterInfoData2.Code, masterInfoData2.DevId);
                    InsertUpdateDeleteQueryData(query);
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc5(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송발령결과 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc6(TcData tcData)
        {
            try
            {
                NCasProtocolTc6 tc6 = null;
                tc6 = tcData.ProtoBase as NCasProtocolTc6;

                string eventText = string.Empty;

                if (tc6 == null)
                {
                    eventText = "ProcTc6(NCasProtocolBase protoBase) : TC6을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterinfoData = null;
                masterinfoData = BasicDataMng.GetAlarmMasterInfoData(tc6.Sector, tc6.BroadNetIdOrIp);
                if (masterinfoData == null)
                {
                    eventText = string.Format("ProcTc6(NCasProtocolBase protoBase) : TC6을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc6.BroadNetIdOrIp.ToString(), tc6.BroadNetIdOrIpByString, ((int)tc6.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                int areaCode = masterinfoData.Code;

                MasterInfoData masterInfoData2 = null;
                masterInfoData2 = BasicDataMng.GetMasterInfoDataToIp(tc6.RespNetIdOrIpByString);

                string query = string.Empty;
                query = QueryMng.GetCheckHaveBroadRespResultHistQuery(tc6.Mode, NCasDefineTcCode.TcBroadResult, tc6.OrderTimeByDateTime, (int)tc6.Source, masterInfoData2.Code);
                int queryCount = CountSelectQueryData(query);

                if (queryCount != 0)
                {
                    query = QueryMng.GetUpdateBroadRespResultHist(tc6.Mode, NCasDefineTcCode.TcBroadResult, tc6.OrderTimeByDateTime, areaCode, (int)tc6.CtrlKind, (int)tc6.Sector, (int)tc6.AlarmKind, (int)tc6.Media, tcData.RecvTime, masterInfoData2.Code, (int)tc6.Source, (int)tc6.RsltStatus);
                    InsertUpdateDeleteQueryData(query);
                }
                else
                {
                    query = QueryMng.GetInsertPreBroadRespResultHistQuery((int)tc6.Mode, NCasDefineTcCode.TcBroadResult, tc6.OrderTimeByDateTime, tcData.RecvTime, areaCode, (int)tc6.CtrlKind, (int)tc6.Sector, (int)tc6.Source, (int)tc6.AlarmKind, (int)tc6.Media, 1, (int)tc6.RsltStatus, masterInfoData2.Code, masterInfoData2.DevId);
                    InsertUpdateDeleteQueryData(query);
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc6(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 장비감시 결과 데이터를 입력한다.(사용안함)
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc8(TcData tcData)
        {
            try
            {
                string eventText = string.Empty;
                NCasProtocolTc8 tc8 = null;
                tc8 = tcData.ProtoBase as NCasProtocolTc8;
                int devKind;

                if (CheckMainSystem(tc8.TargetIpByString) && tc8.WatchIpByString != ConfigMng.LocalIp)
                {
                    eventText = "ProcTc8(NCasProtocolBase protoBase) : TC8을 수신하였으나 주제어의 장애 내역이고 다른시스템에서 넘어온 장애 내역이므로 처리하지 않음";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (tc8 == null)
                {
                    eventText = "ProcTc8(NCasProtocolBase protoBase) : TC8을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                MasterInfoData masterInfo = null;
                masterInfo = BasicDataMng.GetMasterInfoDataToIp(tc8.TargetIpByString);

                if (masterInfo == null)
                {
                    eventText = string.Format("ProcTc8(NCasProtocolBase protoBase) : TC8을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc8.TargetIp.ToString(), tc8.TargetIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (masterInfo.UseFlag == (int)NCasDefineUseStatus.Disuse)
                {
                    eventText = "ProcTc8(NCasProtocolBase protoBase) : TC8을 수신하였으나 사용하지 않는 장비이므로 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                int queryCount;

                devKind = (int)tc8.DevKind;
                switch (tc8.Status)
                {
                    case NCasDefineNormalStatus.Abnormal:
                        query = QueryMng.GetCheckHaveFaultDeviceDataQuery(true, masterInfo.Code, devKind, tcData.RecvTime);
                        queryCount = CountSelectQueryData(query);
                        if (queryCount > 0)
                        {
                            //eventText = string.Format("ProcTc8(NCasProtocolBase protoBase) : TC8을 수신하였으나 이미 장애가 발생한 장비의 정보이므로 넘어간다.\n[단말정보 - 발생시각 : {0}, 단말IP : {1}, 상태 : {2}]",
                            //    tcData.RecvTime.ToString("yyyy년MM월dd일 HH시mm분ss초"), tc8.TargetIpByString, tc8.Status);
                            //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                            //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                            return;
                        }
                        query = QueryMng.GetInsertFaultDeviceDataQuery(masterInfo.Code, devKind, tcData.RecvTime);
                        InsertUpdateDeleteQueryData(query);
                        break;
                        
                    case NCasDefineNormalStatus.Noraml:
                        query = QueryMng.GetFaultDeviceInfoDataQuery(masterInfo.Code, devKind);
                        DateTime occurTime1 = DateTime.MinValue;
                        occurTime1 = GetFaultInfoData(query);

                        if (tcData.RecvTime < occurTime1)
                        {
                            tcData.RecvTime = occurTime1 + TimeSpan.FromSeconds(1);
                        }

                        query = QueryMng.GetUpdateFaultDeviceDataQuery(true, tcData.RecvTime, masterInfo.Code, devKind);
                        InsertUpdateDeleteQueryData(query);
                        break;
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc8(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 장비감시 결과 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc88(TcData tcData)
        {
            try
            {
                string eventText = string.Empty;
                //Log
                //eventText = "TC88수신하여 ProcTc88()함수 진입함";
                //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                NCasProtocolTc88 tc88 = null;
                tc88 = tcData.ProtoBase as NCasProtocolTc88;
                int devKind;

                if (CheckMainSystem(tc88.TargetIpByString) && tc88.WatchIpByString != ConfigMng.LocalIp)
                {
                    eventText = "ProcTc88(NCasProtocolBase protoBase) : TC88을 수신하였으나 주제어의 장애 내역이고 다른시스템에서 넘어온 장애 내역이므로 처리하지 않음";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (tc88 == null)
                {
                    eventText = "ProcTc88(NCasProtocolBase protoBase) : TC88을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n####[데이터수신] IP : {0}, 상태 : {1}\n",tc88.TargetIpByString, tc88.Status.ToString()));
                //Log
                //eventText = string.Format("TC8수신-단말IP : {0}, 상태 : {1}, 발생시각 : {2}", tc88.TargetIpByString, tc88.Status, tcData.RecvTime.ToString("yyyy년MM월dd일 HH시mm분ss초"));
                //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                MasterInfoData masterInfo = null;
                masterInfo = BasicDataMng.GetMasterInfoDataToIp(tc88.TargetIpByString);

                if (masterInfo == null)
                {
                    eventText = string.Format("ProcTc88(NCasProtocolBase protoBase) : TC88을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc88.TargetIp.ToString(), tc88.TargetIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (masterInfo.UseFlag == (int)NCasDefineUseStatus.Disuse)
                {
                    eventText = "ProcTc88(NCasProtocolBase protoBase) : TC88을 수신하였으나 사용하지 않는 장비이므로 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                int queryCount;
                devKind = (int)tc88.DevKind;

                switch (tc88.Status)
                {
                    case NCasDefineNormalStatus.Abnormal:
                        query = QueryMng.GetCheckHaveFaultDeviceDataQuery(false, masterInfo.Code, devKind, tc88.TimeByDateTime);
                        queryCount = CountSelectQueryData(query);
                        if (queryCount > 0)
                        {
                            //eventText = string.Format("ProcTc88(NCasProtocolBase protoBase) : TC88을 수신하였으나 이미 장애가 발생한 장비의 정보이므로 넘어간다.\n[단말정보 - 발생시각 : {0}, 단말IP : {1}, 상태 : {2}]",
                            //    tcData.RecvTime.ToString("yyyy년MM월dd일 HH시mm분ss초"), tc88.TargetIpByString, tc88.Status);
                            //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                            //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                            return;
                        }
                        query = QueryMng.GetInsertFaultDeviceDataQuery(masterInfo.Code, devKind, tc88.TimeByDateTime);
                        InsertUpdateDeleteQueryData(query);
                        break;

                    case NCasDefineNormalStatus.Noraml:
                        query = QueryMng.GetFaultDeviceInfoDataQuery(masterInfo.Code, devKind);
                        DateTime occurTime1 = DateTime.MinValue;
                        occurTime1 = GetFaultInfoData(query);

                        if (tcData.RecvTime < occurTime1)
                        {
                            tcData.RecvTime = occurTime1 + TimeSpan.FromSeconds(1);
                        }

                        query = QueryMng.GetUpdateFaultDeviceDataQuery(false, tc88.TimeByDateTime, masterInfo.Code, devKind);
                        InsertUpdateDeleteQueryData(query);
                        break;
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc88(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void ProcTc89(TcData tcData)
        {
            try
            {
                string eventText = string.Empty;
                //Log
                //eventText = "TC89수신하여 ProcTc89()함수 진입함";
                //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                NCasProtocolTc89 tc89 = null;
                tc89 = tcData.ProtoBase as NCasProtocolTc89;
                int devKind;

                if (tc89 == null)
                {
                    eventText = "ProcTc89(NCasProtocolBase protoBase) : TC89을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                //Log
                //eventText = string.Format("TC8수신-단말IP : {0}, 상태 : {1}, 발생시각 : {2}", tc89.TargetIpByString, tc89.Status, tcData.RecvTime.ToString("yyyy년MM월dd일 HH시mm분ss초"));
                //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                MasterInfoData masterInfo = null;
                masterInfo = BasicDataMng.GetMasterInfoDataToIp(tc89.TargetIpByString);

                if (masterInfo == null)
                {
                    eventText = string.Format("ProcTc89(NCasProtocolBase protoBase) : TC89을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc89.TargetIp.ToString(), tc89.TargetIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (masterInfo.UseFlag == (int)NCasDefineUseStatus.Disuse)
                {
                    eventText = "ProcTc89(NCasProtocolBase protoBase) : TC89을 수신하였으나 사용하지 않는 장비이므로 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                int queryCount;
                devKind = (int)tc89.DevKind;

                switch (tc89.Status)
                {
                    case NCasDefineNormalStatus.Abnormal:
                        query = QueryMng.GetCheckHaveFaultDeviceDataQuery(false, masterInfo.Code, devKind, tc89.TimeByDateTime);
                        queryCount = CountSelectQueryData(query);
                        if (queryCount > 0)
                        {
                            //eventText = string.Format("ProcTc89(NCasProtocolBase protoBase) : TC89을 수신하였으나 이미 장애가 발생한 장비의 정보이므로 넘어간다.\n[단말정보 - 발생시각 : {0}, 단말IP : {1}, 상태 : {2}]",
                            //    tcData.RecvTime.ToString("yyyy년MM월dd일 HH시mm분ss초"), tc89.TargetIpByString, tc89.Status);
                            //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                            //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                            return;
                        }
                        query = QueryMng.GetInsertFaultDeviceDataQuery(masterInfo.Code, devKind, tc89.TimeByDateTime);
                        InsertUpdateDeleteQueryData(query);
                        break;

                    case NCasDefineNormalStatus.Noraml:
                        query = QueryMng.GetFaultDeviceInfoDataQuery(masterInfo.Code, devKind);
                        DateTime occurTime1 = DateTime.MinValue;
                        occurTime1 = GetFaultInfoData(query);

                        if (tcData.RecvTime < occurTime1)
                        {
                            tcData.RecvTime = occurTime1 + TimeSpan.FromSeconds(1);
                        }

                        query = QueryMng.GetUpdateFaultDeviceDataQuery(false, tc89.TimeByDateTime, masterInfo.Code, devKind);
                        InsertUpdateDeleteQueryData(query);
                        break;
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc898(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 위성 상태 요구 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc10(TcData tcData)
        {
            try
            {
                NCasProtocolTc10 tc10 = null;
                tc10 = tcData.ProtoBase as NCasProtocolTc10;
                string eventText = string.Empty;

                if (tc10 == null)
                {
                    eventText = "ProcTc10(NCasProtocolBase protoBase) : TC10을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                query = QueryMng.GetInsertSateTestOrderHistQuery(tc10.TimeByDateTime, (int)tc10.Source, tc10.TermIpByString, (int)tc10.DevKind, 0);//media = devKind
                InsertUpdateDeleteQueryData(query);

                int provCode = 0;
                
                provCode = BasicDataMng.GetAlarmAreaProvCode(tc10.TermIp, tc10.Source);
                if (provCode < 0)
                {
                    eventText = string.Format("ProcTc10(NCasProtocolBase protoBase) : TC10을 수신하였으나 IP에 해당하는 시도코드를 얻어오지 못해 넘어간다. [uint IP : {0}, string IP : {1}, Source : {2}]", tc10.TermIp.ToString(), tc10.TermIpByString, ((int)tc10.Source).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }
                query = QueryMng.GetInsertSateTestResultHistQuery(tc10.Source, tc10.TimeByDateTime, provCode);
                InsertUpdateDeleteQueryData(query);

                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc10(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 위성 상태 응답 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc11(TcData tcData)
        {
            try
            {
                NCasProtocolTc11 tc11 = null;

                tc11 = tcData.ProtoBase as NCasProtocolTc11;

                if (tc11 == null)
                {
                    string eventText = "ProcTc11(NCasProtocolBase protoBase) : TC11을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc11.TermIpByString);

                if (masterInfoData == null)
                {
                    string eventText = string.Format("ProcTc11(NCasProtocolBase protoBase) : TC11을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc11.TermIp.ToString(), tc11.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                //query = QueryMng.GetDeleteSateTestResultHistQuery(masterInfoData.Code, masterInfoData.DevId, tc11.TimeByDateTime);
                //InsertUpdateDeleteQueryData(query);

                query = QueryMng.GetUpdateSateTestResultHistQuery(masterInfoData.Code, masterInfoData.DevId, tcData.RecvTime);
                InsertUpdateDeleteQueryData(query);

                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc11(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 단말 주변기기 상태응답 데이터를 입력한다.
        /// TC14와 동일한 작업을 한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc13(TcData tcData)
        {
            try
            {
                NCasProtocolTc13 tc13 = null;
                tc13 = tcData.ProtoBase as NCasProtocolTc13;

                if (tc13 == null)
                {
                    string eventText = "ProcTc13(NCasProtocolBase protoBase) : TC13을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc13.TermIpByString);

                if (masterInfoData == null)
                {
                    string eventText = string.Format("ProcTc13(NCasProtocolBase protoBase) : TC13을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc13.TermIp.ToString(), tc13.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                int tmpCount;

                bool isBroadDept = false;
                if (masterInfoData.DevId == (int)NCasDefineDeviceKind.DeptSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.CentBroadSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.ProvBroadSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.PdeptSys)
                {
                    isBroadDept = true;
                }
                else
                {
                    isBroadDept = false;
                }

                if (isBroadDept)//방송국일 때 처리하는 내용
                {
                    #region 디지털
                    //tc13.Reset = 1
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Reset, (int)BroadAnalogDigitalPointInfo.Reset, masterInfoData.Code);
                    //tc13.Mcu1Status = 2
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Mcu1Status, (int)BroadAnalogDigitalPointInfo.Mcu1Status, masterInfoData.Code);
                    //tc13.Mcu2Status = 3
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Mcu2Status, (int)BroadAnalogDigitalPointInfo.Mcu2Status, masterInfoData.Code);
                    //tc13.Psu1Status = 4
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Psu1Status, (int)BroadAnalogDigitalPointInfo.Psu1Status, masterInfoData.Code);
                    //tc13.Psu2Status = 5
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Psu2Status, (int)BroadAnalogDigitalPointInfo.Psu2Status, masterInfoData.Code);
                    //tc13.AcFailStatus = 6
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.AcFailStatus, (int)BroadAnalogDigitalPointInfo.AcFailStatus, masterInfoData.Code);
                    //tc13.RackFanStatus = 9
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.RackFanStatus, (int)BroadAnalogDigitalPointInfo.RackFanStatus, masterInfoData.Code);
                    //tc13.CaseFanStatus = 10
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.CaseFanStatus, (int)BroadAnalogDigitalPointInfo.CaseFanStatus, masterInfoData.Code);

                    //단말 출입문 열림이 기존에 있으면 새로 입력하지 않는다.
                    //tc13.DoorStatus = 13
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.DoorStatus, (int)BroadAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                    
                    //tc13.SatelliteStatus = 15
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.SatelliteStatus, (int)BroadAnalogDigitalPointInfo.SatelliteStatus, masterInfoData.Code);
                    #endregion
                    #region 아날로그
                    #endregion
                }
                else//단말일 때 처리하는 내용
                {
                    #region 디지털
                    //tc13.DevRoomFireStatus 1
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.DevRoomFireStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomFireStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //******************장치실 출입문상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다. 2015/12/15
                    //tc13.DevRoomDoorStatus 2
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.DevRoomDoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc13.DevRoomDoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.DevRoomDoorStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomDoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }
                    
                    //tc13.BattRoomFireStatus 3
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.BattRoomFireStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomFireStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //******************밧데리실출입문 관련 상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다.
                    //tc13.BattRoomDoorStatus 4
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.BattRoomDoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc13.BattRoomDoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.BattRoomDoorStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomDoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }

                    //******************도어상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다.
                    //tc13.DoorStatus 5
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc13.DoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.DoorStatus, (int)AlarmAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }

                    //tc13.AcFailStatus 6
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.AcFailStatus, (int)AlarmAnalogDigitalPointInfo.AcFailStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu1Status 7
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Psu1Status, (int)AlarmAnalogDigitalPointInfo.Psu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu2Status 8
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Psu2Status, (int)AlarmAnalogDigitalPointInfo.Psu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.MonitorPsu1Status 9
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.MonitorPsu1Status, (int)AlarmAnalogDigitalPointInfo.MonitorPsu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.MonitorPsu2Status 10
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.MonitorPsu2Status, (int)AlarmAnalogDigitalPointInfo.MonitorPsu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenPowerStatus 11
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.SirenPowerStatus, (int)AlarmAnalogDigitalPointInfo.SirenPowerStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.RackFanStatus 12
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.RackFanStatus, (int)AlarmAnalogDigitalPointInfo.RackFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.CaseFanStatus 13
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.CaseFanStatus, (int)AlarmAnalogDigitalPointInfo.CaseFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.FanControlMode 14
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.FanControlMode, (int)AlarmAnalogDigitalPointInfo.FanControlMode, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //tc13.FrontPanelKey 16
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.FrontPanelKey, (int)AlarmAnalogDigitalPointInfo.FrontPanelKey, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.FrontPanelStatus 17
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.FrontPanelStatus, (int)AlarmAnalogDigitalPointInfo.FrontPanelStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.LocalBroad1 18
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.LocalBroad1, (int)AlarmAnalogDigitalPointInfo.LocalBroad1, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.LocalBroad2 19
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.LocalBroad2, (int)AlarmAnalogDigitalPointInfo.LocalBroad2, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.ManulOperSwitch 20
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.ManulOperSwitch, (int)AlarmAnalogDigitalPointInfo.ManulOperSwitch, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Reset 21
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Reset, (int)AlarmAnalogDigitalPointInfo.Reset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu1Status 22
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Mcu1Status, (int)AlarmAnalogDigitalPointInfo.Mcu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu2Status 23
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.Mcu2Status, (int)AlarmAnalogDigitalPointInfo.Mcu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DevRoomFanStatus 24
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.DevRoomFanStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.BattRoomFanStatus 25
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.BattRoomFanStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SatelliteStatus 34
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.SatelliteStatus, (int)AlarmAnalogDigitalPointInfo.SatelliteStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenAmpPowerSwitch 35
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.SirenAmpPowerSwitch, (int)AlarmAnalogDigitalPointInfo.SirenAmpPowerSwitch, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.TermDsuReset 36
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.TermDsuReset, (int)AlarmAnalogDigitalPointInfo.TermDsuReset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.TermAmpSetStatus 37
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc13.TermAmpSetStatus, (int)AlarmAnalogDigitalPointInfo.TermAmpSetStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    #endregion
                    #region 아날로그
                    //tc13.DevRoomTempStatus 26
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.DevRoomTempStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomTempStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DevRoomHumiStatus 27
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.DevRoomHumiStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomHumiStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    ////tc13.BattRoomTempStatus 28
                    //query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.BattRoomTempStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomTempStatus, masterInfoData.Code);
                    //InsertUpdateDeleteQueryData(query);
                    ////tc13.BattRoomHumiStatus 29
                    //query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.BattRoomHumiStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomHumiStatus, masterInfoData.Code);
                    //InsertUpdateDeleteQueryData(query);
                    //tc13.AcStatus 30
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.AcStatus, (int)AlarmAnalogDigitalPointInfo.AcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DcStatus 31
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.DcStatus, (int)AlarmAnalogDigitalPointInfo.DcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenAcStatus 32
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.SirenAcStatus, (int)AlarmAnalogDigitalPointInfo.SirenAcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenBattStatus 33
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc13.SirenBattStatus, (int)AlarmAnalogDigitalPointInfo.SirenBattStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    #endregion
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc13(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 단말 주변기기 상태 이벤트 데이터를 입력한다.★
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc14(TcData tcData)
        {
            try
            {
                NCasProtocolTc14 tc14 = null;
                tc14 = tcData.ProtoBase as NCasProtocolTc14;

                if (tc14 == null)
                {
                    string eventText = "ProcTc14(NCasProtocolBase protoBase) : TC14을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc14.TermIpByString);

                if (masterInfoData == null)
                {
                    string eventText = string.Format("ProcTc14(NCasProtocolBase protoBase) : TC14을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc14.TermIp.ToString(), tc14.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }
                
                string query = string.Empty;
                int tmpCount;

                bool isBroadDept = false;
                if (masterInfoData.DevId == (int)NCasDefineDeviceKind.DeptSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.CentBroadSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.ProvBroadSys ||
                    masterInfoData.DevId == (int)NCasDefineDeviceKind.PdeptSys)
                {
                    isBroadDept = true;
                }
                else
                {
                    isBroadDept = false;
                }

                if (isBroadDept)//방송국일 때 처리하는 내용
                {
                    byte[] broadTc = tcData.ProtoBase.GetDatas();

                    #region 디지털
                    //tc13.Reset = 1
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[5], (int)BroadAnalogDigitalPointInfo.Reset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu1Status = 2
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[6], (int)BroadAnalogDigitalPointInfo.Mcu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu2Status = 3
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[7], (int)BroadAnalogDigitalPointInfo.Mcu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu1Status = 4
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[8], (int)BroadAnalogDigitalPointInfo.Psu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu2Status = 5
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[9], (int)BroadAnalogDigitalPointInfo.Psu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.AcFailStatus = 6
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[10], (int)BroadAnalogDigitalPointInfo.AcFailStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.RackFanStatus = 9
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[13], (int)BroadAnalogDigitalPointInfo.RackFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.CaseFanStatus = 10
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[14], (int)BroadAnalogDigitalPointInfo.CaseFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DoorStatus = 13
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[17], (int)BroadAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SatelliteStatus = 15
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, broadTc[19], (int)BroadAnalogDigitalPointInfo.SatelliteStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);


                    /*
                    //tc13.Reset = 1
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Reset, (int)BroadAnalogDigitalPointInfo.Reset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu1Status = 2
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Mcu1Status, (int)BroadAnalogDigitalPointInfo.Mcu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu2Status = 3
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Mcu2Status, (int)BroadAnalogDigitalPointInfo.Mcu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu1Status = 4
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Psu1Status, (int)BroadAnalogDigitalPointInfo.Psu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu2Status = 5
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Psu2Status, (int)BroadAnalogDigitalPointInfo.Psu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.AcFailStatus = 6
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.AcFailStatus, (int)BroadAnalogDigitalPointInfo.AcFailStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.RackFanStatus = 9
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.RackFanStatus, (int)BroadAnalogDigitalPointInfo.RackFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.CaseFanStatus = 10
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.CaseFanStatus, (int)BroadAnalogDigitalPointInfo.CaseFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DoorStatus = 13
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.DoorStatus, (int)BroadAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SatelliteStatus = 15
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.SatelliteStatus, (int)BroadAnalogDigitalPointInfo.SatelliteStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                     */

                    #endregion
                    #region 아날로그
                    #endregion
                }
                else//단말일 때 처리하는 내용
                {
                    #region 디지털
                    //tc13.DevRoomFireStatus 1
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.DevRoomFireStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomFireStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //******************장치실 출입문상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다. 2015/12/15
                    //tc13.DevRoomDoorStatus 2
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.DevRoomDoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc14.DevRoomDoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.DevRoomDoorStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomDoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }

                    //tc13.BattRoomFireStatus 3
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.BattRoomFireStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomFireStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //******************밧데리실출입문 관련 상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다.
                    //tc13.BattRoomDoorStatus 4
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.BattRoomDoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc14.BattRoomDoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.BattRoomDoorStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomDoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }

                    //******************도어상태일 경우 기존에 발생한 이력이 있는지 확인 후 INSERT 한다.
                    //tc13.DoorStatus 5
                    query = QueryMng.GetCheckHaveTermStatusHistQuery(PointType.Digital, AlarmAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                    tmpCount = CountSelectQueryData(query);
                    if (tmpCount == 0 || tc14.DoorStatus == NCasDefineDoorStatus.DoorClose)
                    {
                        query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.DoorStatus, (int)AlarmAnalogDigitalPointInfo.DoorStatus, masterInfoData.Code);
                        InsertUpdateDeleteQueryData(query);
                    }

                    //tc13.AcFailStatus 6
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.AcFailStatus, (int)AlarmAnalogDigitalPointInfo.AcFailStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu1Status 7
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Psu1Status, (int)AlarmAnalogDigitalPointInfo.Psu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Psu2Status 8
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Psu2Status, (int)AlarmAnalogDigitalPointInfo.Psu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.MonitorPsu1Status 9
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.MonitorPsu1Status, (int)AlarmAnalogDigitalPointInfo.MonitorPsu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.MonitorPsu2Status 10
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.MonitorPsu2Status, (int)AlarmAnalogDigitalPointInfo.MonitorPsu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenPowerStatus 11
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.SirenPowerStatus, (int)AlarmAnalogDigitalPointInfo.SirenPowerStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.RackFanStatus 12
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.RackFanStatus, (int)AlarmAnalogDigitalPointInfo.RackFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.CaseFanStatus 13
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.CaseFanStatus, (int)AlarmAnalogDigitalPointInfo.CaseFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.FanControlMode 14
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.FanControlMode, (int)AlarmAnalogDigitalPointInfo.FanControlMode, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);

                    //tc13.FrontPanelKey 16
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.FrontPanelKey, (int)AlarmAnalogDigitalPointInfo.FrontPanelKey, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.FrontPanelStatus 17
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.FrontPanelStatus, (int)AlarmAnalogDigitalPointInfo.FrontPanelStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.LocalBroad1 18
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.LocalBroad1, (int)AlarmAnalogDigitalPointInfo.LocalBroad1, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.LocalBroad2 19
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.LocalBroad2, (int)AlarmAnalogDigitalPointInfo.LocalBroad2, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.ManulOperSwitch 20
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.ManulOperSwitch, (int)AlarmAnalogDigitalPointInfo.ManulOperSwitch, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Reset 21
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Reset, (int)AlarmAnalogDigitalPointInfo.Reset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu1Status 22
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Mcu1Status, (int)AlarmAnalogDigitalPointInfo.Mcu1Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.Mcu2Status 23
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.Mcu2Status, (int)AlarmAnalogDigitalPointInfo.Mcu2Status, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DevRoomFanStatus 24
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.DevRoomFanStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.BattRoomFanStatus 25
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.BattRoomFanStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomFanStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SatelliteStatus 34
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.SatelliteStatus, (int)AlarmAnalogDigitalPointInfo.SatelliteStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenAmpPowerSwitch 35
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.SirenAmpPowerSwitch, (int)AlarmAnalogDigitalPointInfo.SirenAmpPowerSwitch, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.TermDsuReset 36
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.TermDsuReset, (int)AlarmAnalogDigitalPointInfo.TermDsuReset, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.TermAmpSetStatus 37
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Digital, (int)tc14.TermAmpSetStatus, (int)AlarmAnalogDigitalPointInfo.TermAmpSetStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    #endregion
                    #region 아날로그
                    //tc13.DevRoomTempStatus 26
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.DevRoomTempStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomTempStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DevRoomHumiStatus 27
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.DevRoomHumiStatus, (int)AlarmAnalogDigitalPointInfo.DevRoomHumiStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    ////tc13.BattRoomTempStatus 28
                    //query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.BattRoomTempStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomTempStatus, masterInfoData.Code);
                    //InsertUpdateDeleteQueryData(query);
                    ////tc13.BattRoomHumiStatus 29
                    //query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.BattRoomHumiStatus, (int)AlarmAnalogDigitalPointInfo.BattRoomHumiStatus, masterInfoData.Code);
                    //InsertUpdateDeleteQueryData(query);
                    //tc13.AcStatus 30
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.AcStatus, (int)AlarmAnalogDigitalPointInfo.AcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.DcStatus 31
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.DcStatus, (int)AlarmAnalogDigitalPointInfo.DcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenAcStatus 32
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.SirenAcStatus, (int)AlarmAnalogDigitalPointInfo.SirenAcStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    //tc13.SirenBattStatus 33
                    query = QueryMng.GetTermStatusHistQuery(tcData.RecvTime, PointType.Analog, (int)tc14.SirenBattStatus, (int)AlarmAnalogDigitalPointInfo.SirenBattStatus, masterInfoData.Code);
                    InsertUpdateDeleteQueryData(query);
                    #endregion
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc14(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송 캡션 전송 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc20(TcData tcData)
        {
            try
            {
                NCasProtocolTc20 tc20 = null;
                tc20 = tcData.ProtoBase as NCasProtocolTc20;

                if (tc20 == null)
                {
                    string eventText = "ProcTc20(NCasProtocolBase protoBase) : TC20을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetAlarmMasterInfoData(tc20.Sector, tc20.DisplayIp);
                if (masterInfoData == null)
                {
                    string eventText = string.Format("ProcTc20(NCasProtocolBase protoBase) : TC20을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}, Sector : {2}]", tc20.DisplayIp.ToString(), tc20.DisplayIpByString, ((int)tc20.Sector).ToString());
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = QueryMng.GetCheckHaveBroadCaptionHistQuery(tc20.OrderTimeByDateTime, masterInfoData.Code, (int)tc20.Sector);
                int queryCount = CountSelectQueryData(query);

                if (queryCount > 0)
                {
                    string eventText = "ProcTc20(NCasProtocolBase protoBase) : TC20을 수신하였으나 이미 저장되어있는 데이터 이므로 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                query = QueryMng.GetInsertBroadCaptionHistQuery(tc20.OrderTimeByDateTime, masterInfoData.Code, (int)tc20.Mode, (int)tc20.Media, (int)tc20.Sector, (int)tc20.Kind, tc20.DisplayText);
                InsertUpdateDeleteQueryData(query);
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc20(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송회선 점검 요구 데이터를 입력한다.//해당 테이블이 없음
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc21(TcData tcData)
        {
            try
            {
                NCasProtocolTc21 tc21 = null;

                tc21 = tcData.ProtoBase as NCasProtocolTc21;

                string eventText = string.Empty;
                if (tc21 == null)
                {
                    eventText = "ProcTc21(NCasProtocolBase protoBase) : TC21을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc21.TermIpByString);

                if (masterInfoData == null)
                {
                    eventText = string.Format("ProcTc21(NCasProtocolBase protoBase) : TC21을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함 [uint IP : {0}, string IP : {1}]", tc21.TermIp.ToString(), tc21.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                query = QueryMng.GetInsertVoiceLineTestReqQuery(tc21.TimeByDateTime, masterInfoData.NetId, masterInfoData.Code, masterInfoData.DevId);
                InsertUpdateDeleteQueryData(query);
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc21(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송회선 점검 결과 수신 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc22(TcData tcData)
        {
            try
            {
                string ip = string.Empty;
                DateTime occurTime = tcData.RecvTime;
                int voiceLineStat = 0;

                NCasProtocolTc22 tc22 = null;
                tc22 = tcData.ProtoBase as NCasProtocolTc22;
                ip = tc22.TermIpByString;
                occurTime = tc22.TimeByDateTime;
                voiceLineStat = (int)tc22.VoiceLineStatus;

                string eventText = string.Empty;
                if (tc22 == null)
                {
                    eventText = "ProcTc22(NCasProtocolBase protoBase) : TC22을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(ip);

                if (masterInfoData == null)
                {
                    eventText = string.Format("ProcTc22(NCasProtocolBase protoBase) : TC22을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc22.TermIp.ToString(), tc22.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                query = QueryMng.GetInsertVoiceLineTestResultHistQuery(occurTime, ip, masterInfoData.Code, masterInfoData.DevId, voiceLineStat);
                InsertUpdateDeleteQueryData(query);
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc22(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 방송회선 점검 결과 수신 데이터를 입력한다.★
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc30(TcData tcData)
        {
            //프로토콜 모듈에서 추가해야 함
        }

        /// <summary>
        /// 단말 출입 상태 이벤트 데이터를 입력한다.
        /// </summary>
        /// <param name="protoBase"></param>
        private void ProcTc23(TcData tcData)
        {
            try
            {
                NCasProtocolTc23 tc23 = null;
                tc23 = tcData.ProtoBase as NCasProtocolTc23;

                string eventText = string.Empty;
                if (tc23 == null)
                {
                    eventText = "ProcTc23(NCasProtocolBase protoBase) : TC23을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                MasterInfoData masterInfoData = null;
                masterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc23.TermIpByString);

                if (masterInfoData == null)
                {
                    eventText = string.Format("ProcTc23(NCasProtocolBase protoBase) : TC23을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc23.TermIp.ToString(), tc23.TermIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                if (tc23.UserId == "XXXX")
                {
                    query = QueryMng.GetInsertUpdatetTermLoginHistQuery(LoginStatus.Trespass, masterInfoData.Code, tcData.RecvTime, tcData.RecvTime, tc23.UserId);
                    InsertUpdateDeleteQueryData(query);
                }

                if (tc23.Status == NCasDefineLoginStatus.Login)
                {
                    query = QueryMng.GetInsertUpdatetTermLoginHistQuery(LoginStatus.Login, masterInfoData.Code, tcData.RecvTime, tcData.RecvTime, tc23.UserId);
                    InsertUpdateDeleteQueryData(query);
                }
                else
                {
                    query = QueryMng.GetCheckHaveLoginHistQuery(masterInfoData.Code);
                    int queryCount = CountSelectQueryData(query);

                    if (queryCount == 0)
                    {
                        query = QueryMng.GetInsertUpdatetTermLoginHistQuery(LoginStatus.BeNotLogin, masterInfoData.Code, tcData.RecvTime, tcData.RecvTime, tc23.UserId);
                        InsertUpdateDeleteQueryData(query);
                    }
                    else
                    {
                        query = QueryMng.GetInsertUpdatetTermLoginHistQuery(LoginStatus.BeLogin, masterInfoData.Code, tcData.RecvTime, tcData.RecvTime, tc23.UserId);
                        InsertUpdateDeleteQueryData(query);
                    }
                }
                SetProcessingStandbyCount();
            }
            catch (Exception err)
            {
                string functionName = "ProcTc23(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        private void ProcTc65(TcData tcData)
        {
            try
            {
                NCasProtocolTc65 tc65 = null;
                tc65 = tcData.ProtoBase as NCasProtocolTc65;
                string eventText = string.Empty;

                if (tc65 == null)
                {
                    eventText = "ProcTc65(NCasProtocolBase protoBase) : TC65을 수신하였으나 null이어서 처리하지 못함";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                //MasterInfoData areaMasterInfoData = null;
                MasterInfoData devMasterInfoData = null;
                //areaMasterInfoData = BasicDataMng.GetBroadMasterInfoData(tc65.Sector, tc65.BroadAreaIp);
                devMasterInfoData = BasicDataMng.GetMasterInfoDataToIp(tc65.DisplayIpByString);

                if (devMasterInfoData == null)
                {
                    eventText = string.Format("ProcTc65(NCasProtocolBase protoBase) : TC65을 수신하였으나 등록되어 있는 단말이 아니므로 처리하지 못함[uint IP : {0}, string IP : {1}]", tc65.DisplayIp.ToString(), tc65.DisplayIpByString);
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                string query = string.Empty;
                query = QueryMng.GetBroadCaptionResultQuery(tc65.Mode, NCasDefineTcCode.TcBroadResponse, tc65.OrderTimeByDateTime, devMasterInfoData.Code, tc65.BroadResult);
                InsertUpdateDeleteQueryData(query);

                query = QueryMng.GetBroadCaptionResultQuery(tc65.Mode, NCasDefineTcCode.TcBroadResult, tc65.OrderTimeByDateTime, devMasterInfoData.Code, tc65.BroadResult);
                InsertUpdateDeleteQueryData(query);
            }
            catch (Exception err)
            {
                string functionName = "ProcTc65(TcData tcData)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        #region Utill Code
        /// <summary>
        /// 처리카운트를 반환한다.
        /// </summary>
        public int GetProcessingCount()
        {
            return processingCount;
        }

        private NCasProtocolTc4 GetTc4ToTc1(NCasProtocolTc1 protoClone)
        {
            NCasProtocolTc4 tc4 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadOrder) as NCasProtocolTc4;
            try
            {
                tc4.AlarmKind = protoClone.AlarmKind;
                tc4.AlarmNetIdOrIp = protoClone.AlarmNetIdOrIp;
                tc4.BroadNetIdOrIp = protoClone.BroadNetIdOrIp;
                tc4.CtrlKind = protoClone.CtrlKind;
                //tc4.Datas = protoClone.GetDatas();
                tc4.Media = protoClone.Media;
                tc4.Mode = protoClone.Mode;
                tc4.OrderTime = protoClone.OrderTime;
                tc4.RespReqFlag = protoClone.RespReqFlag;
                tc4.Sector = protoClone.Sector;
                tc4.Source = protoClone.Source;
            }
            catch (Exception err)
            {
                string functionName = "GetTc4ToTc1(NCasProtocolTc1 protoClone)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return tc4;
        }

        //2 - 5, 3 - 6
        private NCasProtocolTc5 GetTc5ToTc2(NCasProtocolTc2 protoClone)
        {
            NCasProtocolTc5 tc5 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadResponse) as NCasProtocolTc5;
            try
            {
                tc5.AlarmKind = protoClone.AlarmKind;
                tc5.AlarmNetIdOrIp = protoClone.AlarmNetIdOrIp;
                tc5.BroadNetIdOrIp = protoClone.BroadNetIdOrIp;
                tc5.CtrlKind = protoClone.CtrlKind;
                //tc5.Datas = protoClone.GetDatas();
                tc5.Media = protoClone.Media;
                tc5.Mode = protoClone.Mode;
                tc5.OrderTime = protoClone.OrderTime;
                tc5.RespNetIdOrIp = protoClone.RespNetIdOrIp;
                tc5.RespStatus = protoClone.RespStatus;
                tc5.Sector = protoClone.Sector;
                tc5.Source = protoClone.Source;
            }
            catch (Exception err)
            {
                string functionName = "GetTc5ToTc2(NCasProtocolTc2 protoClone)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return tc5;
        }

        private NCasProtocolTc6 GetTc6ToTc3(NCasProtocolTc3 protoClone)
        {
            NCasProtocolTc6 tc6 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcBroadResult) as NCasProtocolTc6;
            try
            {
                tc6.AlarmKind = protoClone.AlarmKind;
                tc6.AlarmNetIdOrIp = protoClone.AlarmNetIdOrIp;
                tc6.BroadNetIdOrIp = protoClone.BroadNetIdOrIp;
                tc6.CtrlKind = protoClone.CtrlKind;
                //tc6.Datas = protoClone.GetDatas();
                tc6.Media = protoClone.Media;
                tc6.Mode = protoClone.Mode;
                tc6.OrderTime = protoClone.OrderTime;
                tc6.RespNetIdOrIp = protoClone.RespNetIdOrIp;
                tc6.RsltStatus = protoClone.RsltStatus;
                tc6.Sector = protoClone.Sector;
                tc6.Source = protoClone.Source;
            }
            catch (Exception err)
            {
                string functionName = "GetTc6ToTc3(NCasProtocolTc3 protoClone)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return tc6;
        }

        /// <summary>
        /// 대기카운트를 반환한다.
        /// </summary>
        public int GetStandbyCount()
        {
            try
            {
                uint count = GetArchiveQueueCount();
                standbyCount = (int)count;
            }
            catch (Exception err)
            {
                string functionName = "GetStandbyCount()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return standbyCount;
        }

        /// <summary>
        /// 처리, 대기카운트를 설정한다.
        /// </summary>
        private void SetProcessingStandbyCount()
        {
            try
            {
                processingCount++;
                //standbyCount=큐카운트;
            }
            catch (Exception err)
            {
                string functionName = "SetProcessingStandbyCount()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 처리카운트를 초기화 한다.
        /// </summary>
        public void ResetProcessingCount()
        {
            try
            {
                processingCount = 0;
            }
            catch (Exception err)
            {
                string functionName = "ResetProcessingCount()";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// AlarmRespResultData를 반환한다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<AlarmRespResultData> GetAlarmRespResultData(string query)
        {
            List<AlarmRespResultData> lstAlarmRespResultData = null;
            AlarmRespResultData alarmRespResultData = null;

            OracleDataReader reader = null;
            try
            {
                NCasOracleDb oracleDb = BasicDataMng.OpenDataBase();
                oracleDb.QueryData(query, out reader);
                if (reader != null)
                {
                    lstAlarmRespResultData = new List<AlarmRespResultData>();
                    while (reader.Read())
                    {
                        alarmRespResultData = new AlarmRespResultData();
                        if (reader["RESULTTIME"] != DBNull.Value)
                        {
                            alarmRespResultData.RespResultTime = (DateTime)reader["RESULTTIME"];
                        }
                        if (reader["AREACODE"] != DBNull.Value)
                        {
                            alarmRespResultData.AreaCode = int.Parse(reader["AREACODE"].ToString());
                        }
                        if (reader["BROADCTRLFLAG"] != DBNull.Value)
                        {
                            alarmRespResultData.BroadCtrlFlag = int.Parse(reader["BROADCTRLFLAG"].ToString());
                        }
                        if (reader["SECTION"] != DBNull.Value)
                        {
                            alarmRespResultData.Section = int.Parse(reader["SECTION"].ToString());
                        }
                        if (reader["KIND"] != DBNull.Value)
                        {
                            alarmRespResultData.AlarmKind = int.Parse(reader["KIND"].ToString());
                        }
                        if (reader["MEDIA"] != DBNull.Value)
                        {
                            alarmRespResultData.Media = int.Parse(reader["MEDIA"].ToString());
                        }
                        if (reader["RESULT"] != DBNull.Value)
                        {
                            alarmRespResultData.RespResultFlag = int.Parse(reader["RESULT"].ToString());
                        }
                        if (reader["DEVCODE"] != DBNull.Value)
                        {
                            alarmRespResultData.DevCode = int.Parse(reader["DEVCODE"].ToString());
                        }
                        if (reader["DEVKIND"] != DBNull.Value)
                        {
                            alarmRespResultData.DevKind = int.Parse(reader["DEVKIND"].ToString());
                        }
                        if (reader["SOURCE"] != DBNull.Value)
                        {
                            alarmRespResultData.Source = int.Parse(reader["SOURCE"].ToString());
                        }
                        lstAlarmRespResultData.Add(alarmRespResultData);
                    }
                    reader.Dispose();
                }
            }
            catch (Exception err)
            {
                string functionName = "GetAlarmRespResultData(string query)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return lstAlarmRespResultData;
        }

        /// <summary>
        /// BroadRespResultData를 반환한다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<BroadRespResultData> GetBroadRespResultData(string query)
        {
            List<BroadRespResultData> lstBroadRespResultData = null;
            BroadRespResultData broadRespResultData = null;

            OracleDataReader reader = null;
            try
            {
                NCasOracleDb oracleDb = BasicDataMng.OpenDataBase();
                oracleDb.QueryData(query, out reader);
                if (reader != null)
                {
                    lstBroadRespResultData = new List<BroadRespResultData>();
                    while (reader.Read())
                    {
                        broadRespResultData = new BroadRespResultData();
                        if (reader["RESULTTIME"] != DBNull.Value)
                        {
                            broadRespResultData.RespResultTime = (DateTime)reader["RESULTTIME"];
                        }
                        if (reader["AREACODE"] != DBNull.Value)
                        {
                            broadRespResultData.AreaCode = int.Parse(reader["AREACODE"].ToString());
                        }
                        if (reader["BROADCTRLFLAG"] != DBNull.Value)
                        {
                            broadRespResultData.BroadCtrlFlag = int.Parse(reader["BROADCTRLFLAG"].ToString());
                        }
                        if (reader["SECTION"] != DBNull.Value)
                        {
                            broadRespResultData.Section = int.Parse(reader["SECTION"].ToString());
                        }
                        if (reader["KIND"] != DBNull.Value)
                        {
                            broadRespResultData.AlarmKind = int.Parse(reader["KIND"].ToString());
                        }
                        if (reader["MEDIA"] != DBNull.Value)
                        {
                            broadRespResultData.Media = int.Parse(reader["MEDIA"].ToString());
                        }
                        if (reader["RESULT"] != DBNull.Value)
                        {
                            broadRespResultData.RespResultFlag = int.Parse(reader["RESULT"].ToString());
                        }
                        if (reader["CODE"] != DBNull.Value)
                        {
                            broadRespResultData.DevCode = int.Parse(reader["CODE"].ToString());
                        }
                        if (reader["DEVKIND"] != DBNull.Value)
                        {
                            broadRespResultData.DevKind = int.Parse(reader["DEVKIND"].ToString());
                        }
                        if (reader["SOURCE"] != DBNull.Value)
                        {
                            broadRespResultData.Source = int.Parse(reader["SOURCE"].ToString());
                        }
                        if (reader["CAPTIONCODE"] != DBNull.Value)
                        {
                            broadRespResultData.CaptionCode = int.Parse(reader["CAPTIONCODE"].ToString());
                        }
                        if (reader["CLOSEPROCFLAG"] != DBNull.Value)
                        {
                            broadRespResultData.CloseProcFlag = int.Parse(reader["CLOSEPROCFLAG"].ToString());
                        }
                        lstBroadRespResultData.Add(broadRespResultData);
                    }
                    reader.Dispose();
                }
            }
            catch (Exception err)
            {
                string functionName = "GetBroadRespResultData(string query)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return lstBroadRespResultData;
        }

        /// <summary>
        /// 수량 확인을 수행하는 함수
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private int CountSelectQueryData(string query)
        {
            int count = 0;
            NCasOracleDb oracleDb = null;
            OracleDataReader reader = null;
            try
            {
                oracleDb = BasicDataMng.OpenDataBase();
                oracleDb.QueryData(query, out reader);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (reader["COUNT"] != DBNull.Value)
                        {
                            count = int.Parse(reader["COUNT"].ToString());
                        }
                    }
                    reader.Dispose();
                }
            }
            catch (Exception err)
            {
                string functionName = "CountSelectQueryData(string query)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return count;
        }

        private DateTime GetFaultInfoData(string query)
        {
            NCasOracleDb oracleDb = null;
            OracleDataReader reader = null;
            DateTime occurTime1 = DateTime.MinValue;
            try
            {
                oracleDb = BasicDataMng.OpenDataBase();
                oracleDb.QueryData(query, out reader);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (reader["OCCURTIME1"] != DBNull.Value)
                        {
                            occurTime1 = (DateTime)reader["OCCURTIME1"];
                        }
                    }
                    reader.Dispose();
                }
            }
            catch (Exception err)
            {
                string functionName = "GetErrInfoData(string query)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }

            return occurTime1;
        }

        /// <summary>
        /// Insert, Update, Delete쿼리를 실행하는 함수
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private int InsertUpdateDeleteQueryData(string query)
        {
            int queryNum = -1;
            if (query == string.Empty)
            {
                return queryNum;
            }

            NCasOracleDb oracleDb = null;
            try
            {
                int queryCount;
                oracleDb = BasicDataMng.OpenDataBase();
                oracleDb.BeginTransaction();
                queryCount = queryNum = oracleDb.QueryData(query);
                if (queryNum > 0)
                {
                    oracleDb.Commit();
                }
                else
                {
                    oracleDb.Rollback();
                }
            }
            catch (Exception err)
            {
                if (oracleDb != null)
                {
                    oracleDb.Rollback();
                }
                string functionName = "InsertUpdateDeleteQueryData(string query)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생(롤백됨) : {2} \n[Query : {3}]", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message, query));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
            return queryNum;
        }
        #endregion

        #region Network Code
        void tcpServerMng_RecvNetSessionClient(object sender, NCASBIZ.NCasNetSessionRecvEventArgs e)
        {
            try
            {
                string eventText = string.Empty;
                List<NCasProtocolBase> lstResult = null;
                NCasNetSessionContext context = tcpServerMng.GetNCasNetSessionContext(e.NCasProfile.IpAddr, e.NCasProfile.Port);
                NCasNetDataReceiver.ParseNCasPacket(e.Buff, e.Len, context.TcpClient, out lstResult);
                
                if (lstResult == null)
                {
                    //eventText = "tcpServerMng_RecvNetSessionClient(object sender, NCASBIZ.NCasNetSessionRecvEventArgs e) : TC정보를 수신하였으나 lstResult값이 null이므로 넘어간다.";
                    //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                foreach (NCasProtocolBase proto in lstResult)
                {
                    if (NCasProtocolFactory.GetTcValue(proto.GetDatas()) == NCasDefineTcCode.TcDevStsReq)
                    {
                        continue;
                    }
                    //Log
                    //if (proto.TcCode == NCasDefineTcCode.TcDevStsResult || proto.TcCode == NCasDefineTcCode.TcDevStsResult2)
                    //{
                    //    eventText = string.Format("\n[{0}] 데이터 수신 : TC{1}, 내용 : tcpServerMng_RecvNetSessionClient()에 데이터 들어와 AddBizData() 호출\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), (int)proto.TcCode);
                    //    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    //}
                    AddBizData(proto);
                }
            }
            catch (Exception err)
            {
                string functionName = "tcpServerMng_RecvNetSessionClient(object sender, NCASBIZ.NCasNetSessionRecvEventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        void tcpServerMng_AcceptNetSessionClient(object sender, NCASBIZ.NCasNetSessionAcceptEventArgs e)
        {
            try
            {
                isFirstConnect = true;

                string eventText = string.Empty;
                NCasProfile profile = e.NCasProfile;
                if (profile == null)
                {
                    eventText = "tcpServerMng_AcceptNetSessionClient(object sender, NCASBIZ.NCasNetSessionAcceptEventArgs e) : TCP연결 이벤트를 수신하였으나 해당하는 Profile이 없어 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }
                mainForm.SetProfileStatus(profile, true);
                eventText = string.Format("클라이언트 연결됨 - 클라이언트명 : {0}, 클라이언트IP : {1}", profile.Name, profile.IpAddr);
                mainForm.AddEventListText(eventText, NCasDefineNormalStatus.Noraml);
                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                MakeMainTc88(profile, true);
            }
            catch (Exception err)
            {
                string functionName = "tcpServerMng_AcceptNetSessionClient(object sender, NCASBIZ.NCasNetSessionAcceptEventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        void tcpServerMng_CloseNetSessionClient(object sender, NCASBIZ.NCasNetSessionCloseEventArgs e)
        {
            try
            {
                string eventText = string.Empty;
                NCasProfile profile = e.NCasProfile;
                if (profile == null)
                {
                    eventText = "tcpServerMng_CloseNetSessionClient(object sender, NCASBIZ.NCasNetSessionCloseEventArgs e) : TCP연결 종료 이벤트를 수신하였으나 해당하는 Profile이 없어 넘어간다.";
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }
                mainForm.SetProfileStatus(profile, false);
                eventText = string.Format("클라이언트 끊어짐 - 클라이언트명 : {0}, 클라이언트IP : {1}", profile.Name, profile.IpAddr);
                mainForm.AddEventListText(eventText, NCasDefineNormalStatus.Abnormal);
                NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);

                if (!isFirstConnect)
                {
                    return;
                }
                MakeMainTc88(profile, false);
            }
            catch (Exception err)
            {
                string functionName = "tcpServerMng_CloseNetSessionClient(object sender, NCASBIZ.NCasNetSessionCloseEventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        /// <summary>
        /// 주제어에 대한 TC88을 만드는 부분
        /// </summary>
        /// <param name="profile">해당 프로파일</param>
        /// <param name="isConnect">연결에 대한 이벤트 인지</param>
        private void MakeMainTc88(NCasProfile profile, bool isConnect)
        {
            if (!CheckMainSystem(profile.IpAddr))
            {
                return;
            }

            NCasProtocolTc88 tc88 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcDevStsResult2) as NCasProtocolTc88;
            tc88.DevKind = NCasDefineDeviceKind.ProvDevSys;
            if (isConnect)
            {
                tc88.Status = NCasDefineNormalStatus.Noraml;
            }
            else
            {
                tc88.Status = NCasDefineNormalStatus.Abnormal;
            }
            tc88.TargetIpByString = profile.IpAddr;
            tc88.WatchIpByString = ConfigMng.LocalIp;
            tc88.TimeByDateTime = DateTime.Now;

            AddBizData(tc88);
        }

        /// <summary>
        /// 넘어온 IP가 주제어 인지 확인하기 위한 함수
        /// </summary>
        /// <param name="netId"></param>
        /// <returns></returns>
        private bool CheckMainSystem(string netId)
        {
            bool isMainSystem = false;

            string[] strArr = netId.Split('.');
            string[] strArrMy = ConfigMng.LocalIp.Split('.');


            if (strArr.Length < 4 || strArrMy.Length < 4)
            {
                return isMainSystem;
            }

            string thisMainIp = string.Format("10.{0}.1.9", strArrMy[1]);

            //수신된 IP가 내 주제어인지 확인하기 위함
            //주제어일 경우 내가 TC88을 생성하여 DB에 저장한다.
            if (netId == thisMainIp)
            {
                isMainSystem = true;
            }
            return isMainSystem;
        }

        void udpSocket_ReceivedData(object sender, NCASFND.NCasUdpRecvEventArgs e)
        {
            try
            {
#if debug
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "----------UDP 수신----------"));
#endif
                byte[] udpBuff = new byte[e.Len];
                Array.Clear(udpBuff, 0, e.Len);
                Buffer.BlockCopy(e.Buff, 0, udpBuff, 0, e.Len);
                NCasProtocolBase proto = NCasProtocolFactory.ParseFrame(udpBuff);
                if (proto == null)
                {
                    return;
                }
                AddBizData(proto);
            }
            catch (Exception err)
            {
                string functionName = "udpSocket_ReceivedData(object sender, NCASFND.NCasUdpRecvEventArgs e)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }
        #endregion

        #region Virtual
        protected override void OnAsyncDataProcessing(NCasObject param)
        {
            try
            {
                string eventText = string.Empty;
                NCasProtocolBase proto = param as NCasProtocolBase;

                if(proto == null)
                {
                    //eventText = "OnAsyncDataProcessing(NCasObject param) : TC정보를 수신하였으나 proto값이 null이므로 넘어간다.";
                    //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), eventText));
                    //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    return;
                }

                if (proto.TcCode == NCasDefineTcCode.TcDevStsResult || proto.TcCode == NCasDefineTcCode.TcDevStsResult2)
                {
                    //eventText = string.Format("\n[{0}] 데이터 수신 : TC{1}, 내용 : OnAsyncDataProcessing()에 데이터 들어옴\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), (int)proto.TcCode);
                    //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                }

                TcData tcData = new TcData();
                tcData.RecvTime = DateTime.Now;
                tcData.ProtoBase = proto;

                if (TcOverlapCheckMng.TcOverlapCheck(proto))
                {
                    if (proto.TcCode == NCasDefineTcCode.TcDevStsResult || proto.TcCode == NCasDefineTcCode.TcDevStsResult2)
                    {
                        //eventText = string.Format("\n[{0}] 데이터 수신 : TC{1}, 내용 : AddOutputData() 호출\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), (int)proto.TcCode);
                        //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                    }

                    AddOutputData(tcData, true, true, true);
                }
                else
                {
                    //eventText = string.Format("\n[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "중복된 데이터 TcOverlapCheck에서 필터링 됨");
                    //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
#if debug
                    NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), "중복된 데이터 TcOverlapCheck에서 필터링 됨"));
#endif
                }
            }
            catch (Exception err)
            {
                string functionName = "OnAsyncDataProcessing(NCasObject param)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnAsyncArchiveProcessing(NCasObject param)
        {
            //tcpInsertAsyncArchiveCount++;
            //DebugDisplayCount();
            try
            {
                string eventText = string.Empty;

                base.OnAsyncArchiveProcessing(param);
                TcData tcData = param as TcData;

                if (tcData == null)
                {
                    return;
                }

                switch (tcData.ProtoBase.TcCode)
                {
                    case NCasDefineTcCode.TcAlarmOrder:
                    case NCasDefineTcCode.TcAutoAlarmOrder:
                    case NCasDefineTcCode.TcPublicAlarmOrder:
                        ProcTc1(tcData);
                        break;
                    case NCasDefineTcCode.TcAlarmResponse:
                    case NCasDefineTcCode.TcAutoAlarmResponse:
                    case NCasDefineTcCode.TcPublicAlarmResponse:
                        ProcTc2(tcData);
                        break;
                    case NCasDefineTcCode.TcAlarmResult:
                    case NCasDefineTcCode.TcAutoAlarmResult:
                    case NCasDefineTcCode.TcPublicAlarmResult:
                        ProcTc3(tcData);
                        break;
                    case NCasDefineTcCode.TcBroadOrder:
                        ProcTc4(tcData);
                        break;
                    case NCasDefineTcCode.TcBroadResponse:
                        ProcTc5(tcData);
                        break;
                    case NCasDefineTcCode.TcBroadResult:
                        ProcTc6(tcData);
                        break;
                    case NCasDefineTcCode.TcDevStsResult:
                        //Log
                        //eventText = "OnAsyncArchiveProcessing() 에서 ProcTc8()호출함";
                        //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                        ProcTc8(tcData);
                        break;
                    case NCasDefineTcCode.TcDevStsResult2:
                        //Log
                        //eventText = "OnAsyncArchiveProcessing() 에서 ProcTc88()호출함";
                        //NCasLoggingMng.ILogging.WriteLog(NCasProcName.NCasPDBManager.ToString(), eventText);
                        ProcTc88(tcData);
                        break;
                    case NCasDefineTcCode.TcDevStsResult2Ext:
                        ProcTc89(tcData);
                        break;
                    case NCasDefineTcCode.TcTermPointResp:
                        ProcTc13(tcData);
                        break;
                    case NCasDefineTcCode.TcTermPointEvent:
                        ProcTc14(tcData);
                        break;
                    case NCasDefineTcCode.TcSateStsReq:
                        ProcTc10(tcData);
                        break;
                    case NCasDefineTcCode.TcSateStsResp:
                        ProcTc11(tcData);
                        break;
                    case NCasDefineTcCode.TcBroadCaption:
                        ProcTc20(tcData);
                        break;
                    case NCasDefineTcCode.TcVoiceLineCheckReq:
                        //ProcTc21(tcData);
                        break;
                    case NCasDefineTcCode.TcVoiceLineCheckResult:
                        ProcTc22(tcData);
                        break;
                    case NCasDefineTcCode.TcLoginInfo:
                        ProcTc23(tcData);
                        break;
                    case NCasDefineTcCode.TcVoiceLineSelfCheckResult:
                        //ProcTc30(tcData);
                        break;
                    case NCasDefineTcCode.TcBroadCaptionResult:
                        ProcTc65(tcData);
                        break;
                }
            }
            catch (Exception err)
            {
                string functionName = "OnAsyncArchiveProcessing(NCasObject param)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnAsyncExternProcessing(NCasObject param)
        {
            try
            {
                base.OnAsyncExternProcessing(param);
                //asyncExternCount++;
            }
            catch (Exception err)
            {
                string functionName = "OnAsyncExternProcessing(NCasObject param)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }

        protected override void OnAsyncDispProcessing(NCasObject param)
        {
            try
            {
                base.OnAsyncDispProcessing(param);
                //asyncDispCount++;
            }
            catch (Exception err)
            {
                string functionName = "OnAsyncDispProcessing(NCasObject param)";
                NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n###AYJ###[{0}] {1} 오류발생 : {2}", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), functionName, err.Message));
                NCasLoggingMng.ILoggingException.WriteException(NCasProcName.NCasPDBManager.ToString(), functionName, err);
            }
        }
        #endregion

        #region Test Code
        public void TestRestRecvCount()
        {
            //tcpRecvCount = 0;
            //tcpInsertAsyncDataCount = 0;
            //tcpInsertAsyncArchiveCount = 0;
            //asyncDispCount = 0;
            //asyncExternCount = 0;
            //tc1Count = 0;
            //tc2Count = 0;
            //tc3Count = 0;
            DebugDisplayCount();
        }

        private void DebugDisplayCount()
        {
            //NCasLoggingMng.ILoggingDebug.WriteDebug(string.Format("\n[{0}] TCPRecvCCount : {1}, TCPInsertAsyncDataCount : {2},TCPInsertAsyncArchiveCount : {3}, asyncDispCount : {4}, asyncExternCount : {5}, TC1 : {6}, TC2 : {7}, TC3 : {8}\n", DateTime.Now.ToString("yyyy년MM월dd일 HH시mm분ss초"), tcpRecvCount, tcpInsertAsyncDataCount, tcpInsertAsyncArchiveCount, asyncDispCount, asyncExternCount, tc1Count, tc2Count, tc3Count));
        }
        #endregion

        private String MakeHornSpeakerStatusString(NCasDefineSpkStatus[] hornData)
        {
            String hornString = String.Empty;

            int hornMax = (int)NCasDefineTermPeripheral.HornNum42;
            for (int i = 0; i < hornMax; i++)
            {
                hornString += ((byte)hornData[i]).ToString();

                if (((i + 1) % 3 == 0))
                {
                    if ((i + 1) < hornMax)
                    {
                        hornString += "-";
                    }
                }
            }
            return hornString;
        }
    }
}
