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

namespace NCasPAlmScreen
{
    public class PAlmScreenBiz : NCasBizProcess
    {
        #region element
        private MainForm mainForm = null;
        private ProvInfo provInfo = null;
        private readonly int SendUnicastOrderCount = 1;
        private int SendDelay = 2000; //화생방 or 재난경계(MIC/TTS) 일 때 사용하는 Delay
        private int TtsDelay = 5000; //TTS 발령일 때 사용하는 Delay
        private readonly string LoopBackIP = "127.0.0.1";
        private readonly string ProvBroadIP = "10.0.255.255";
        private NCasUdpSocket udpSoc = new NCasUdpSocket();
        private List<OrderBizData> lstOrderBizData = new List<OrderBizData>();
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public PAlmScreenBiz()
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mainForm">MainForm에서 넘겨주는 참조</param>
        public PAlmScreenBiz(MainForm mainForm)
            : this()
        {
            this.mainForm = mainForm;
            this.provInfo = this.mainForm.ProvInfo;
            this.TtsDelay = this.mainForm.TtsDelayTime;
            this.SendDelay = this.mainForm.TeleDelayTime;
        }
        #endregion

        #region UnInit
        /// <summary>
        /// PAlmScreenBiz UnInit 메소드
        /// </summary>
        public void UnInit()
        {
            this.udpSoc.Close();
        }
        #endregion

        #region private Method
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

        #region 시도전체 발령
        /// <summary>
        /// 시도전체 발령 명령을 처리하는 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        private void OrderProvAll(OrderBizData orderBizData)
        {
            int udpSendCount = 4;
            byte[] sendBuff = orderBizData.SendBuff;
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

            if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Mic) ||
                (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
            {
                udpSoc.SendTo(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, sendBuff);
                udpSoc.SendTo(this.ProvBroadIP, (int)NCasPortID.PortIdAlarm, sendBuff);
            }

            this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendBuff);
            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendBuff);
            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendBuff);

            if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
            {
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendBuff);
            }

            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendBuff);
            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendBuff);
            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendBuff);
            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendBuff);

            if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
            {
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendBuff);
            }

            udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendBuff);

            if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
            {
                for (int i = 0; i < udpSendCount; i++)
                {
                    udpSoc.SendTo(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, sendBuff);
                    udpSoc.SendTo(this.ProvBroadIP, (int)NCasPortID.PortIdAlarm, sendBuff);
                }
            }

            if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts)
            {
                byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                Thread.Sleep(this.TtsDelay);

                string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                    NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                    NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                    NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                    orderBizData.SelectedTtsMessage.Text;

                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
            }
            else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
            {
                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
            }
        }
        #endregion

        #region 시군 전체/개별 발령
        /// <summary>
        /// 시군전체 발령 명령을 처리하는 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        private void OrderDistAll()
        {
            string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
            List<string> reptAlarmServerIpAddr = new List<string>();

            for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
            {
                reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));
                udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, orderBizData.SendBuff);
                udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, orderBizData.SendBuff);

                for (int i = 0; i < reptAlarmServerIpAddr.Count; i++)
                {
                    udpSoc.SendTo(reptAlarmServerIpAddr[i], (int)NCasPortID.PortIdIntTeleRAlarm, orderBizData.SendBuff);
                }
            }

            if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Mic) ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                byte[] sendBuff = orderBizData.SendBuff;
                DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));
                this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendBuff);

                if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendBuff);
                }

                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendBuff);

                    if (distInfo.IsDisasterDist)
                    {
                        NCasProtocolBase baseProto = NCasProtocolFactory.ParseFrame(sendBuff);
                        NCasProtocolTc1 tc1 = baseProto as NCasProtocolTc1;
                        tc1.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(tc1.AlarmNetIdOrIpByString, 0, 2, 0, 0);
                        byte[] tmpBuff = NCasProtocolFactory.MakeUdpFrame(tc1);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, tmpBuff);
                    }
                }

                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
                {
                    foreach (TermInfo termInfo in distInfo.LstTerms)
                    {
                        if (termInfo.UseFlag == NCasDefineUseStatus.Use)
                        {
                            udpSoc.SendTo(termInfo.IpAddrToSring, (int)NCasPortID.PortIdAlarm, sendBuff);
                        }
                    }
                }

                //지진해일 시군
                if (distInfo.IsDisasterDist)
                {
                    orderBizData.AlmProtocol.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 2, 0, 0);
                    byte[] sendDisasterBuff = NCasProtocolFactory.MakeUdpFrame(orderBizData.AlmProtocol);
                    this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendDisasterBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendDisasterBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendDisasterBuff);

                    if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                    {
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendDisasterBuff);
                    }

                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendDisasterBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendDisasterBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendDisasterBuff);
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendDisasterBuff);

                    if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
                    {
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendDisasterBuff);
                    }

                    udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendDisasterBuff);

                    if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
                    {
                        foreach (TermInfo termInfo in distInfo.LstTerms)
                        {
                            if (termInfo.UseFlag == NCasDefineUseStatus.Use)
                            {
                                udpSoc.SendTo(termInfo.IpAddrToSring, (int)NCasPortID.PortIdAlarm, sendDisasterBuff);
                            }
                        }
                    }
                }

                if (orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.First || orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.FirstEnd)
                {
                    if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts)
                    {
                        byte[] tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        Thread.Sleep(this.TtsDelay);

                        string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                            NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                            NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                            NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                            orderBizData.SelectedTtsMessage.Text;

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                }
            }
        }
        #endregion

        #region 단말개별 발령
        /// <summary>
        /// 단말개별 발령 명령을 처리하는 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        private void OrderTerm()
        {
            byte[] tmpBuff = TtsControlDataMng.GetTeleStartData();
            string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
            List<string> reptAlarmServerIpAddr = new List<string>();

            for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
            {
                reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
            }

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

            if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Mic) ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                byte[] sendBuff = orderBizData.SendBuff;
                this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendBuff);

                if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendBuff);
                }

                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendBuff);
                }

                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
                {
                    udpSoc.SendTo(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, sendBuff);
                }

                if (orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.First || orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.FirstEnd)
                {
                    if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        Thread.Sleep(this.TtsDelay);

                        string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                            NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                            NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                            NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                            orderBizData.SelectedTtsMessage.Text;

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                }
            }
        }
        #endregion

        #region 그룹 발령
        /// <summary>
        /// 그룹 발령 명령을 처리하는 메소드
        /// </summary>
        /// <param name="orderBizData"></param>
        private void OrderGroup()
        {
            byte[] tmpBuff = TtsControlDataMng.GetTeleStartData();
            string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
            List<string> reptAlarmServerIpAddr = new List<string>();

            for (int i = 0; i < this.provInfo.LstRepts.Count; i++)
            {
                reptAlarmServerIpAddr.Add(NCasUtilityMng.INCasCommUtility.AddIpAddr(this.provInfo.LstRepts[i].NetIdToString, 0, 0, 1, 1));
            }

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

            if (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.AlarmBiochemist ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Mic) ||
                (this.lstOrderBizData[0].AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && this.lstOrderBizData[0].SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts))
            {
                Thread.Sleep(this.SendDelay);
            }

            //그룹발령 정보 전송 (TC 77)
            for (int i = 0; i < lstOrderBizData[0].GroupName.Count; i++)
            {
                NCasProtocolBase protoBase77 = NCasProtocolFactory.CreateCasProtocol(NCasDefineTcCode.TcGroupOrder);
                NCasProtocolTc77 protoTc77 = protoBase77 as NCasProtocolTc77;
                protoTc77.AlarmKind = this.lstOrderBizData[0].AlmProtocol.AlarmKind;
                protoTc77.AlarmNetIdOrIpByString = this.lstOrderBizData[0].AlmProtocol.AlarmNetIdOrIpByString;
                protoTc77.CtrlKind = this.lstOrderBizData[0].AlmProtocol.CtrlKind;
                protoTc77.GroupName = lstOrderBizData[0].GroupName[i];
                protoTc77.GroupNum = (byte)lstOrderBizData[0].GroupName.Count;
                protoTc77.Media = this.lstOrderBizData[0].AlmProtocol.Media;
                protoTc77.Mode = this.lstOrderBizData[0].AlmProtocol.Mode;
                protoTc77.OrderTimeByDateTime = this.lstOrderBizData[0].AlmProtocol.OrderTimeByDateTime;
                protoTc77.Sector = this.lstOrderBizData[0].AlmProtocol.Sector;
                protoTc77.Source = this.lstOrderBizData[0].AlmProtocol.Source;

                byte[] tc77Buff = NCasProtocolFactory.MakeUdpFrame(protoTc77);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, tc77Buff);

                string tmpLog = string.Format("GroupName - {0}, GroupNum - {1}, OrderTime - {2}", protoTc77.GroupName, protoTc77.GroupNum.ToString(), protoTc77.OrderTimeByDateTime.ToString());
                NCasLoggingMng.ILogging.WriteLog("PAlmScreenBiz", "그룹발령 정보(TC 77)를 NCasPipes.PipePcaProvMain로 전송 완료 - " + tmpLog);
                System.Diagnostics.Debug.WriteLine("### 그룹발령 정보(TC 77) - " + tmpLog);
            }

            foreach (OrderBizData orderBizData in this.lstOrderBizData)
            {
                byte[] sendBuff = orderBizData.SendBuff;
                this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);
                
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendBuff);

                if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendBuff);
                }

                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendBuff);
                this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
                {
                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendBuff);

                    if (orderBizData.AlmProtocol.Sector == NCasDefineSectionCode.SectionDist)
                    {
                        DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));

                        if (distInfo.IsDisasterDist)
                        {
                            NCasProtocolBase baseProto = NCasProtocolFactory.ParseFrame(sendBuff);
                            NCasProtocolTc1 tc1 = baseProto as NCasProtocolTc1;
                            tc1.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(tc1.AlarmNetIdOrIpByString, 0, 2, 0, 0);
                            byte[] disasterBuff = NCasProtocolFactory.MakeUdpFrame(tc1);
                            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, disasterBuff);
                        }
                    }
                }

                udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendBuff);

                if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
                {
                    if (orderBizData.AlmProtocol.Sector == NCasDefineSectionCode.SectionTerm)
                    {
                        udpSoc.SendTo(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, sendBuff);
                    }
                    else if (orderBizData.AlmProtocol.Sector == NCasDefineSectionCode.SectionDist)
                    {
                        DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));

                        foreach (TermInfo termInfo in distInfo.LstTerms)
                        {
                            if (termInfo.UseFlag == NCasDefineUseStatus.Use)
                            {
                                udpSoc.SendTo(termInfo.IpAddrToSring, (int)NCasPortID.PortIdAlarm, sendBuff);
                            }
                        }
                    }
                }

                //지진해일 시군
                if (orderBizData.AlmProtocol.Sector == NCasDefineSectionCode.SectionDist)
                {
                    DistInfo distInfo = this.mainForm.MmfMng.GetDistInfoByNetId(NCasUtilityMng.INCasCommUtility.SubtractIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 0, 0, 255));

                    if (distInfo.IsDisasterDist)
                    {
                        orderBizData.AlmProtocol.AlarmNetIdOrIpByString = NCasUtilityMng.INCasCommUtility.AddIpAddr(orderBizData.AlmProtocol.AlarmNetIdOrIpByString, 0, 2, 0, 0);
                        byte[] sendDisasterBuff = NCasProtocolFactory.MakeUdpFrame(orderBizData.AlmProtocol);
                        this.mainForm.MmfMng.WriteOrder(orderBizData.AlmProtocol);

                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentCAlm, sendDisasterBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvMain, sendDisasterBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnRAlm, sendDisasterBuff);

                        if ((orderBizData.AlmProtocol.AlarmNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                        {
                            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermDept, sendDisasterBuff);
                        }

                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaTermTerm, sendDisasterBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvDual, sendDisasterBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaRgnDAlm, sendDisasterBuff);
                        this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaProvPDAlm, sendDisasterBuff);

                        if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaSate)
                        {
                            this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipePcaCentSate, sendDisasterBuff);
                        }

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdExtCasMonitor, sendDisasterBuff);

                        if (orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.AlmProtocol.Media == NCasDefineOrderMedia.MediaLine)
                        {
                            foreach (TermInfo termInfo in distInfo.LstTerms)
                            {
                                if (termInfo.UseFlag == NCasDefineUseStatus.Use)
                                {
                                    udpSoc.SendTo(termInfo.IpAddrToSring, (int)NCasPortID.PortIdAlarm, sendDisasterBuff);
                                }
                            }
                        }
                    }
                }

                if (orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.First || orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.FirstEnd)
                {
                    if (orderBizData.AlmProtocol.AlarmKind == NCasDefineOrderKind.DisasterBroadcast && orderBizData.SelectedDisasterBroadKind == PAlmScreenUIController.DisasterBroadKind.Tts)
                    {
                        tmpBuff = TtsControlDataMng.GetTtsPlayData();
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                        Thread.Sleep(this.TtsDelay);

                        string ttsData = "TTS" + NCasContentsMng.ttsOption.SpeechSpeed.ToString().PadLeft(3, '0') +
                            NCasContentsMng.ttsOption.RepeatCount.ToString().PadLeft(2, '0') +
                            NCasContentsMng.ttsOption.SentenceInterval.ToString().PadLeft(4, '0') +
                            NCasContentsMng.ttsOption.RestInterval.ToString().PadLeft(4, '0') +
                            orderBizData.SelectedTtsMessage.Text;

                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsMessage, Encoding.Default.GetBytes(ttsData));
                    }
                    else if (orderBizData.AlmProtocol.AlarmKind != NCasDefineOrderKind.DisasterBroadcast)
                    {
                        udpSoc.SendTo(this.LoopBackIP, (int)NCasPortID.PortIdTtsControl, Encoding.Default.GetBytes("RES"));
                    }
                }
            }
        }
        #endregion

        #region OnAsyncDataProcessing Method
        protected override void OnAsyncDataProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is OrderBizData)
            {
                AddOutputData(param, false, true, false);
            }
            else if (param is KeyBizData)
            {
                AddOutputData(param, true, true, false);
            }
            else if (param is NCasPlcProtocolBase)
            {
                AddOutputData(param, false, true, false);
            }
        }
        #endregion

        #region OnAsyncDispProcessing Method
        protected override void OnAsyncDispProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is KeyBizData)
            {
                try
                {
                    KeyBizData keyBizData = param as KeyBizData;

                    if (keyBizData.IsLocal == true)
                        return;

                    this.mainForm.SetKeyDataFromDual(keyBizData.KeyData);
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PAlmScreenBiz", "PAlmScreenBiz.OnAsyncDispProcessing Method - KeyBizData", ex);
                }
            }
        }
        #endregion

        #region OnAsyncExternProcessing Method
        protected override void OnAsyncExternProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is OrderBizData)
            {
                try
                {
                    OrderBizData orderBizData = param as OrderBizData;

                    if (orderBizData.IsLocal == false)
                        return;

                    if (orderBizData.AlmProtocol.GetDatas() == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PAlmScreenBiz", "발령이 정상적으로 처리되지 않았습니다.",
                            "TC " + orderBizData.AlmProtocol.TcCode.ToString());
                        return;
                    }

                    if (orderBizData.TtsOrderFlag) //마지막 발령이 TTS발령이면..
                    {
                        byte[] tmpBuff = TtsControlDataMng.GetTtsStopData();
                        string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tmpBuff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tmpBuff);
                    }

                    if (orderBizData.AllDestinationFlag) //시도전체 발령(무조건 1개 패킷 전송)
                    {
                        this.OrderProvAll(orderBizData);
                    }
                    else //하나의 발령에 의해 여러개의 패킷을 전송해야 하는 경우..
                    {
                        if (orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.First || orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.None)
                        {
                            this.lstOrderBizData.Add(orderBizData);
                        }
                        else if (orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.End || orderBizData.IsEnd == PAlmScreenUIController.OrderDataSendStatus.FirstEnd)
                        {
                            this.lstOrderBizData.Add(orderBizData);

                            if (orderBizData.OrderDistFlag) //시군 전체/개별 발령
                            {
                                this.OrderDistAll();
                            }
                            else if (orderBizData.OrderTermFlag) //개별단말 발령
                            {
                                this.OrderTerm();
                            }
                            else if (orderBizData.OrderGroupFlag) //그룹 발령
                            {
                                this.OrderGroup();
                            }

                            this.lstOrderBizData.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PAlmScreenBiz", "PAlmScreenBiz.OnAsyncExternProcessing Method - OrderBizData", ex);
                }
            }
            else if (param is KeyBizData)
            {
                try
                {
                    KeyBizData keyBizData = param as KeyBizData;

                    if (keyBizData.IsLocal == false)
                        return;

                    this.mainForm.SendKeyDataToDual(keyBizData.KeyData);
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PAlmScreenBiz", "PAlmScreenBiz.OnAsyncExternProcessing Method - KeyBizData", ex);
                }
            }
            else if (param is NCasPlcProtocolBase)
            {
                try
                {
                    NCasPlcProtocolBase nCasPlcProtocolBase = param as NCasPlcProtocolBase;
                    byte[] buff = NCasPlcProtocolFactory.MakeFrame(nCasPlcProtocolBase);

                    if (nCasPlcProtocolBase.GetDatas() == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PAlmScreenBiz", "NCasPlcProtocolFactory.MakeFrame is null");
                        return;
                    }

                    if (buff == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PAlmScreenBiz", "buff is null");
                        return;
                    }

                    this.udpSoc.SendTo(this.LoopBackIP, (int)NCasPipes.PipeNccDevCcd, buff);
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PAlmScreenBiz", "PAlmScreenBiz.OnAsyncExternProcessing Method - NCasPlcProtocolBase", ex);
                }
            }
        }
        #endregion
    }
}