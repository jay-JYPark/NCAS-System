using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NCASFND.NCasLogging;
using NCASFND.NCasNet;
using NCASBIZ.NCasBiz;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasPlcProtocol;
using NCASBIZ.NCasData;
using NCASBIZ.NCasUtility;
using NCasAppCommon.Define;
using NCasMsgCommon.Tts;
using NCasContentsModule;
using NCasContentsModule.TTS;

namespace NCasPDAlmScreen
{
    public class PDAlmScreenBiz : NCasBizProcess
    {
        #region element
        private MainForm mainForm = null;
        private ProvInfo provInfo = null;
        private int TtsDelay = 5000; //TTS 발령일 때 사용하는 Delay
        private NCasUdpSocket udpSoc = new NCasUdpSocket();
        private readonly int SendDelay = 2000; //화생방 or 재난경계(MIC/TTS) 일 때 사용하는 Delay
        private readonly string LoopBackIP = "127.0.0.1";
        private readonly string ProvBroadIP = "10.0.255.255";
        private List<OrderBizData> lstOrderBizData = new List<OrderBizData>();
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public PDAlmScreenBiz()
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mainForm">MainForm에서 넘겨주는 참조</param>
        public PDAlmScreenBiz(MainForm mainForm)
            : this()
        {
            this.mainForm = mainForm;
            this.provInfo = this.mainForm.ProvInfo;
            this.TtsDelay = this.mainForm.TtsDelayTime;
        }
        #endregion

        #region UnInit
        /// <summary>
        /// PDAlmScreenBiz UnInit 메소드
        /// </summary>
        public void UnInit()
        {
            this.udpSoc.Close();
        }
        #endregion

        #region private Method
        #region 시도 전체 발령
        /// <summary>
        /// 시도 전체 발령
        /// </summary>
        /// <param name="orderBizData"></param>
        private void OrderProvAll(OrderBizData orderBizData)
        {
            NCasProtocolBase baseProto = NCasProtocolFactory.ParseFrame(orderBizData.SendBuff);
            NCasProtocolTc1 tc1 = baseProto as NCasProtocolTc1;
            tc1.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(tc1.AlarmNetIdOrIpByString, 0, 7, 0, 0);
            byte[] sendBuff = NCasProtocolFactory.MakeUdpFrame(tc1);
            NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcPublicAlarmOrder);
            NCasProtocolTc151 protoTc151 = protoBase as NCasProtocolTc151;

            if (orderBizData.SelectedDisasterBroadKind != OrderView19201080.DisasterBroadKind.StroredMessage)
            {
                string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                List<string> reptAlarmServerIpAddr = new List<string>();

                for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
                {
                    reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
                }

                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, sendBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, sendBuff);

                for (int i = 0; i < reptAlarmServerIpAddr.Count; i++)
                {
                    udpSoc.SendTo(reptAlarmServerIpAddr[i], (int)NCasPortID.PortIdIntTeleRAlarm, sendBuff);
                }
            }

            if (tc1.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (tc1.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic) ||
                (tc1.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            byte[] tmpStoBuff = null;

            if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage)
            {
                protoTc151.AlarmKind = NCasDefineOrderKind.BroadMessage;
                protoTc151.AlarmNetIdOrIpByString = tc1.AlarmNetIdOrIpByString;
                protoTc151.AuthenFlag = tc1.AuthenFlag;
                protoTc151.CtrlKind = tc1.CtrlKind;
                protoTc151.Media = tc1.Media;
                protoTc151.Mode = tc1.Mode;
                protoTc151.Source = tc1.Source;
                protoTc151.Sector = tc1.Sector;
                protoTc151.RespReqFlag = tc1.RespReqFlag;
                protoTc151.OrderTimeByDateTime = tc1.OrderTimeByDateTime;
                protoTc151.MsgNum1 = GetStoredMsgHeaderNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                protoTc151.MsgNum2 = int.Parse(orderBizData.SelectedStoredMessage.MsgNum);
                protoTc151.MsgNum3 = GetStoredMsgTailNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                protoTc151.RepeatNum = (byte)orderBizData.StoredMessageRepeatCount;

                tmpStoBuff = NCasProtocolFactory.MakeUdpFrame(protoTc151);
            }

            this.mainForm.MmfMng.WriteOrder(tc1);

            if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage) //저장메시지 발령인 경우..
            {
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, tmpStoBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, tmpStoBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmpStoBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmpStoBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, tmpStoBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, tmpStoBuff);
            }
            else
            {
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, sendBuff);
            }

            if (tc1.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts)
            {
                byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                Thread.Sleep(this.TtsDelay);

                string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                    NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                    NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                    NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                    orderBizData.SelectedTtsMessage.Text;

                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
            }
            else if (tc1.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic)
            {
                byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
            }
            else if (tc1.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
            }
        }
        #endregion

        #region 단말개별 발령
        /// <summary>
        /// 단말개별 발령
        /// </summary>
        private void OrderTerm()
        {
            NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcPublicAlarmOrder);
            NCasProtocolTc151 protoTc151 = protoBase as NCasProtocolTc151;
            byte[] tmpBuff = TtsControlDataMng.GetTeleStartData();
            string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
            List<string> reptAlarmServerIpAddr = new List<string>();

            for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
            {
                reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
            }

            if (this.lstOrderBizData[0].SelectedDisasterBroadKind != OrderView19201080.DisasterBroadKind.StroredMessage)
            {
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);

                foreach (OrderBizData orderBizData in this.lstOrderBizData)
                {
                    udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, orderBizData.SendBuff);
                    udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, orderBizData.SendBuff);

                    for (int i = 0; i < reptAlarmServerIpAddr.Count; i++)
                    {
                        udpSoc.SendTo(reptAlarmServerIpAddr[i], (int)NCasPortID.PortIdIntTeleRAlarm, orderBizData.SendBuff);
                    }
                }

                tmpBuff = TtsControlDataMng.GetTeleStopData();
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
            }

            if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic) ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            //그룹정보 전송
            if (this.lstOrderBizData[0].OrderTermGroupFlag)
            {
                for (int i = 0; i < lstOrderBizData[0].GroupName.Count; i++)
                {
                    NCasProtocolBase protoBase77 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcGroupOrder);
                    NCasProtocolTc77 protoTc77 = protoBase77 as NCasProtocolTc77;
                    protoTc77.AlarmKind = this.lstOrderBizData[0].AlmProtocol.AlarmKind;
                    protoTc77.AlarmNetIdOrIpByString = this.lstOrderBizData[0].AlmProtocol.AlarmNetIdOrIpByString;
                    protoTc77.CtrlKind = this.lstOrderBizData[0].AlmProtocol.CtrlKind;
                    protoTc77.GroupName = lstOrderBizData[0].GroupName[i];
                    protoTc77.Media = this.lstOrderBizData[0].AlmProtocol.Media;
                    protoTc77.Mode = this.lstOrderBizData[0].AlmProtocol.Mode;
                    protoTc77.OrderTimeByDateTime = this.lstOrderBizData[0].AlmProtocol.OrderTimeByDateTime;
                    protoTc77.Sector = this.lstOrderBizData[0].AlmProtocol.Sector;
                    protoTc77.Source = this.lstOrderBizData[0].AlmProtocol.Source;
                    byte[] tmpBuff77 = NCasProtocolFactory.MakeUdpFrame(protoTc77);

                    string disasterMainIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 232);
                    string main = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 9);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmpBuff77);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmpBuff77);
                    this.udpSoc.SendTo(disasterMainIpAddr, 7003, tmpBuff77);
                    this.udpSoc.SendTo(main, 7003, tmpBuff77);

                    Console.WriteLine("##### 단말그룹 정보 전송 - " + lstOrderBizData[0].GroupName[i]);
                }

                if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmCancel)
                {
                    this.mainForm.SetGroupListClear();
                }
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                byte[] storedBuff = null;

                if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage)
                {
                    protoTc151.AlarmKind = NCasDefineOrderKind.BroadMessage;
                    protoTc151.AlarmNetIdOrIpByString = orderBizData.AlmProtocol.AlarmNetIdOrIpByString;
                    protoTc151.AuthenFlag = orderBizData.AlmProtocol.AuthenFlag;
                    protoTc151.CtrlKind = orderBizData.AlmProtocol.CtrlKind;
                    protoTc151.Media = orderBizData.AlmProtocol.Media;
                    protoTc151.Mode = orderBizData.AlmProtocol.Mode;
                    protoTc151.Source = orderBizData.AlmProtocol.Source;
                    protoTc151.Sector = orderBizData.AlmProtocol.Sector;
                    protoTc151.RespReqFlag = orderBizData.AlmProtocol.RespReqFlag;
                    protoTc151.OrderTimeByDateTime = orderBizData.AlmProtocol.OrderTimeByDateTime;
                    protoTc151.MsgNum1 = GetStoredMsgHeaderNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                    protoTc151.MsgNum2 = int.Parse(orderBizData.SelectedStoredMessage.MsgNum);
                    protoTc151.MsgNum3 = GetStoredMsgTailNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                    protoTc151.RepeatNum = (byte)orderBizData.StoredMessageRepeatCount;

                    storedBuff = NCasProtocolFactory.MakeUdpFrame(protoTc151);
                }

                this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage) //저장메시지 발령인 경우..
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, storedBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, storedBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, storedBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, storedBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, storedBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, storedBuff);
                }
                else
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, orderBizData.SendBuff);
                }

                if (orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.First || orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.FirstEnd)
                {
                    if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                        (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic) ||
                        (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts))
                    {
                        Thread.Sleep(this.SendDelay);
                    }

                    if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts)
                    {
                        tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        Thread.Sleep(this.TtsDelay);

                        string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                            NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                            NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                            NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                            orderBizData.SelectedTtsMessage.Text;

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic)
                    {
                        tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                }
            }
        }
        #endregion

        #region 시군 발령
        /// <summary>
        /// 시군 발령
        /// </summary>
        private void OrderDist()
        {
            NCasProtocolBase protoBase = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcPublicAlarmOrder);
            NCasProtocolTc151 protoTc151 = protoBase as NCasProtocolTc151;
            string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
            List<string> reptAlarmServerIpAddr = new List<string>();

            for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
            {
                reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
            }

            if (this.lstOrderBizData[0].SelectedDisasterBroadKind != OrderView19201080.DisasterBroadKind.StroredMessage)
            {
                foreach (OrderBizData orderBizData in this.lstOrderBizData)
                {
                    udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, orderBizData.SendBuff);
                    udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, orderBizData.SendBuff);

                    for (int i = 0; i < reptAlarmServerIpAddr.Count; i++)
                    {
                        udpSoc.SendTo(reptAlarmServerIpAddr[i], (int)NCasPortID.PortIdIntTeleRAlarm, orderBizData.SendBuff);
                    }
                }
            }

            if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic) ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            //그룹정보 전송
            if (this.lstOrderBizData[0].OrderDistGroupFlag)
            {
                for (int i = 0; i < lstOrderBizData[0].GroupName.Count; i++)
                {
                    NCasProtocolBase protoBase77 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcGroupOrder);
                    NCasProtocolTc77 protoTc77 = protoBase77 as NCasProtocolTc77;
                    protoTc77.AlarmKind = this.lstOrderBizData[0].AlmProtocol.AlarmKind;
                    protoTc77.AlarmNetIdOrIpByString = this.lstOrderBizData[0].AlmProtocol.AlarmNetIdOrIpByString;
                    protoTc77.CtrlKind = this.lstOrderBizData[0].AlmProtocol.CtrlKind;
                    protoTc77.GroupName = lstOrderBizData[0].GroupName[i];
                    protoTc77.Media = this.lstOrderBizData[0].AlmProtocol.Media;
                    protoTc77.Mode = this.lstOrderBizData[0].AlmProtocol.Mode;
                    protoTc77.OrderTimeByDateTime = this.lstOrderBizData[0].AlmProtocol.OrderTimeByDateTime;
                    protoTc77.Sector = this.lstOrderBizData[0].AlmProtocol.Sector;
                    protoTc77.Source = this.lstOrderBizData[0].AlmProtocol.Source;
                    byte[] tmp77Buff = NCasProtocolFactory.MakeUdpFrame(protoTc77);

                    string disasterMainIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 232);
                    string main = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 9);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmp77Buff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmp77Buff);
                    this.udpSoc.SendTo(disasterMainIpAddr, 7003, tmp77Buff);
                    this.udpSoc.SendTo(main, 7003, tmp77Buff);

                    Console.WriteLine("##### 시군그룹 정보 전송 - " + lstOrderBizData[0].GroupName[i]);
                }

                if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmCancel)
                {
                    this.mainForm.SetGroupListClear();
                }
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));
                byte[] tmpStoredBuff = null;

                if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage)
                {
                    protoTc151.AlarmKind = NCasDefineOrderKind.BroadMessage;
                    protoTc151.AlarmNetIdOrIpByString = orderBizData.AlmProtocol.AlarmNetIdOrIpByString;
                    protoTc151.AuthenFlag = orderBizData.AlmProtocol.AuthenFlag;
                    protoTc151.CtrlKind = orderBizData.AlmProtocol.CtrlKind;
                    protoTc151.Media = orderBizData.AlmProtocol.Media;
                    protoTc151.Mode = orderBizData.AlmProtocol.Mode;
                    protoTc151.Source = orderBizData.AlmProtocol.Source;
                    protoTc151.Sector = orderBizData.AlmProtocol.Sector;
                    protoTc151.RespReqFlag = orderBizData.AlmProtocol.RespReqFlag;
                    protoTc151.OrderTimeByDateTime = orderBizData.AlmProtocol.OrderTimeByDateTime;
                    protoTc151.MsgNum1 = GetStoredMsgHeaderNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                    protoTc151.MsgNum2 = int.Parse(orderBizData.SelectedStoredMessage.MsgNum);
                    protoTc151.MsgNum3 = GetStoredMsgTailNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                    protoTc151.RepeatNum = (byte)orderBizData.StoredMessageRepeatCount;
                    tmpStoredBuff = NCasProtocolFactory.MakeUdpFrame(protoTc151);
                }

                this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage) //저장메시지 발령인 경우..
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, tmpStoredBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, tmpStoredBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmpStoredBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmpStoredBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, tmpStoredBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, tmpStoredBuff);
                }
                else
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, orderBizData.SendBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, orderBizData.SendBuff);
                }

                //지진해일 시군
                if (distInfo.IsDisasterDist)
                {
                    orderBizData.AlmProtocol.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 2, 0, 0);
                    byte[] tmpDistBuff = NCasProtocolFactory.MakeUdpFrame(orderBizData.AlmProtocol);
                    byte[] tmpDistStoredBuff = null;

                    if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage)
                    {
                        protoTc151.AlarmKind = NCasDefineOrderKind.BroadMessage;
                        protoTc151.AlarmNetIdOrIpByString = orderBizData.AlmProtocol.AlarmNetIdOrIpByString;
                        protoTc151.AuthenFlag = orderBizData.AlmProtocol.AuthenFlag;
                        protoTc151.CtrlKind = orderBizData.AlmProtocol.CtrlKind;
                        protoTc151.Media = orderBizData.AlmProtocol.Media;
                        protoTc151.Mode = orderBizData.AlmProtocol.Mode;
                        protoTc151.Source = orderBizData.AlmProtocol.Source;
                        protoTc151.Sector = orderBizData.AlmProtocol.Sector;
                        protoTc151.RespReqFlag = orderBizData.AlmProtocol.RespReqFlag;
                        protoTc151.OrderTimeByDateTime = orderBizData.AlmProtocol.OrderTimeByDateTime;
                        protoTc151.MsgNum1 = GetStoredMsgHeaderNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                        protoTc151.MsgNum2 = int.Parse(orderBizData.SelectedStoredMessage.MsgNum);
                        protoTc151.MsgNum3 = GetStoredMsgTailNumber(int.Parse(orderBizData.SelectedStoredMessage.MsgNum));
                        protoTc151.RepeatNum = (byte)orderBizData.StoredMessageRepeatCount;
                        tmpDistStoredBuff = NCasProtocolFactory.MakeUdpFrame(protoTc151);
                    }

                    this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                    if (orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.StroredMessage) //저장메시지 발령인 경우..
                    {
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, tmpDistStoredBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, tmpDistStoredBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmpDistStoredBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmpDistStoredBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, tmpDistStoredBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, tmpDistStoredBuff);
                    }
                    else
                    {
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdIntAuthorityDAlarm, tmpDistBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, tmpDistBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvAlm, tmpDistBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaProvPDMain, tmpDistBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaTermTerm, tmpDistBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePdaCentSate, tmpDistBuff);
                    }
                }

                if (orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.First || orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.FirstEnd)
                {
                    if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Tts)
                    {
                        byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        Thread.Sleep(this.TtsDelay);

                        string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                            NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                            NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                            NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                            orderBizData.SelectedTtsMessage.Text;

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == OrderView19201080.DisasterBroadKind.Mic)
                    {
                        byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 방송할 저장메시지에 해당되는 Header 메시지 번호를 반환한다.
        /// </summary>
        /// <param name="storedMsgNumber">방송할 저장메시지 번호</param>
        /// <returns>방송할 저장메시지에 해당되는 Header 메시지 번호</returns>
        private int GetStoredMsgHeaderNumber(int storedMsgNumber)
        {
            int resultNum = 0;

            if ((storedMsgNumber % 2 == 0) && (storedMsgNumber > 201 && storedMsgNumber < 219))
            {
                resultNum = 954;
            }
            else if (storedMsgNumber == 157 || storedMsgNumber == 158)
            {
                resultNum = 954;
            }
            else
            {
                resultNum = 951;
            }

            return resultNum;
        }

        /// <summary>
        /// 방송할 저장메시지에 해당되는 Tail 메시지 번호를 반환한다.
        /// </summary>
        /// <param name="storedMsgNumber">방송할 저장메시지 번호</param>
        /// <returns>방송할 저장메시지에 해당되는 Tail 메시지 번호</returns>
        private int GetStoredMsgTailNumber(int storedMsgNumber)
        {
            int resultNum = 0;

            if (storedMsgNumber > 155 && storedMsgNumber < 170)
            {
                resultNum = 509;
            }
            else
            {
                resultNum = 502;
            }

            return resultNum;
        }
        #endregion

        #region OnAsyncDataProcessing
        protected override void OnAsyncDataProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is OrderBizData)
            {
                AddOutputData(param, false, true, false);
            }
            else if (param is NCasProtocolBase)
            {
                AddOutputData(param, false, true, false);
            }
        }
        #endregion

        #region OnAsyncDispProcessing
        protected override void OnAsyncDispProcessing(NCASBIZ.NCasType.NCasObject param)
        {
        }
        #endregion

        #region OnAsyncExternProcessing
        protected override void OnAsyncExternProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is OrderBizData)
            {
                try
                {
                    OrderBizData orderBizData = param as OrderBizData;

                    if (orderBizData.IsLocal == false)
                        return;

                    if (orderBizData.SendBuff == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PDAlmScreenBiz", "발령이 정상적으로 처리되지 않았습니다.",
                            "TC " + orderBizData.AlmProtocol.TcCode.ToString() + " - " + NCasUtilityMng.INCasEtcUtility.Bytes2HexString(orderBizData.SendBuff));
                        return;
                    }

                    if (orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.First)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        Thread.Sleep(500);
                    }

                    if (orderBizData.TtsOrderFlag) //마지막 발령이 TTS발령이면..
                    {
                        byte[] tmpBuff = TtsControlDataMng.GetTtsStopData();
                        string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                    }

                    if (orderBizData.AllDestinationFlag || orderBizData.OrderTermAllFlag) //시도전체 발령(무조건 1개 패킷 전송)
                    {
                        this.OrderProvAll(orderBizData);
                    }
                    else //하나의 발령에 의해 여러개의 패킷을 전송해야 하는 경우..
                    {
                        if (orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.First || orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.None)
                        {
                            this.lstOrderBizData.Add(orderBizData);
                        }
                        else if (orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.End || orderBizData.IsEnd == OrderView19201080.OrderDataSendStatus.FirstEnd)
                        {
                            this.lstOrderBizData.Add(orderBizData);

                            if (orderBizData.OrderTermFlag || orderBizData.OrderTermGroupFlag || orderBizData.OrderDistTermFlag) //단말개별 발령
                            {
                                this.OrderTerm();
                            }
                            else if (orderBizData.OrderDistFlag || orderBizData.OrderDistGroupFlag || orderBizData.OrderDistTermAllFlag) //시군 발령
                            {
                                this.OrderDist();
                            }

                            this.lstOrderBizData.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PDAlmScreenBiz", "PDAlmScreenBiz.OnAsyncExternProcessing Method - OrderBizData", ex);
                }
            }
            else if (param is NCasProtocolBase)
            {
                try
                {
                    NCasProtocolBase nCasPlcProtocolBase = param as NCasProtocolBase;
                    byte[] buff = NCasProtocolFactory.MakeUdpFrame(nCasPlcProtocolBase);

                    if (nCasPlcProtocolBase.TcCode == NCasDefineTcCode.None)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PDAlmScreenBiz", "NCasPlcProtocolFactory.TcCode is NCasDefineTcCode.None");
                        return;
                    }

                    if (buff == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PDAlmScreenBiz", "buff is null");
                        return;
                    }

                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipeNccDevAlmKey, buff);
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PDAlmScreenBiz", "PDAlmScreenBiz.OnAsyncExternProcessing Method - NCasPlcProtocolBase", ex);
                }
            }
        }
        #endregion
    }
}