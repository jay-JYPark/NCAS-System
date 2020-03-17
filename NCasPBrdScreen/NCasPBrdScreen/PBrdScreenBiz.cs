using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASFND.NCasLogging;
using NCASFND.NCasNet;
using NCASBIZ.NCasBiz;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasPlcProtocol;
using NCASBIZ.NCasData;
using NCASBIZ.NCasUtility;
using NCasAppCommon.Define;

namespace NCasPBrdScreen
{
    public class PBrdScreenBiz : NCasBizProcess
    {
        #region element
        private MainForm mainForm = null;
        private ProvInfo provInfo = null;
        private readonly int SendUnicastOrderCount = 4;
        private NCasUdpSocket udpSoc = new NCasUdpSocket();
        private readonly string IP_LOOPBACK = "127.0.0.1";
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public PBrdScreenBiz()
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="main">MainForm에서 넘겨주는 참조</param>
        public PBrdScreenBiz(MainForm mainForm)
            : this()
        {
            this.mainForm = mainForm;
            this.provInfo = this.mainForm.ProvInfo;
        }
        #endregion

        #region UnInit
        /// <summary>
        /// PBrdScreenBiz UnInit 메소드
        /// </summary>
        public void UnInit()
        {
            this.udpSoc.Close();
        }
        #endregion

        #region Virtual Method
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
            else if (param is NCasProtocolTc20)
            {
                AddOutputData(param, false, true, false);
            }
        }

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
                    NCasLoggingMng.ILoggingException.WriteException("PBrdScreenBiz", "PBrdScreenBiz.OnAsyncDispProcessing Method - KeyBizData", ex);
                }
            }
        }

        protected override void OnAsyncExternProcessing(NCASBIZ.NCasType.NCasObject param)
        {
            if (param is OrderBizData)
            {
                try
                {
                    OrderBizData orderBizData = param as OrderBizData;

                    if (orderBizData.IsLocal == false)
                        return;

                    byte[] buff = NCasProtocolFactory.MakeUdpFrame(orderBizData.BrdProtocol);

                    if (buff == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PBrdScreenBiz", "발령이 정상적으로 처리되지 않았습니다.",
                            "TC " + orderBizData.BrdProtocol.TcCode.ToString() + " - " + NCasUtilityMng.INCasEtcUtility.Bytes2HexString(orderBizData.BrdProtocol.GetDatas()));
                        return;
                    }

                    if (orderBizData.BrdProtocol.Sector == NCasDefineSectionCode.SectionBroadShare) //광역시 전체 발령
                    {
                        NCasProtocolBase baseProto = NCasProtocolFactory.ParseFrame(buff);
                        NCasProtocolTc4 tc4 = baseProto as NCasProtocolTc4;
                        tc4.Sector = NCasDefineSectionCode.SectionProv;
                        NCasProtocolFactory.MakeUdpFrame(tc4);
                        this.mainForm.MmfMng.WriteOrder(tc4);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbRgnPBrdMan, tc4.GetDatas());
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbProvMain, tc4.GetDatas());
                        string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, tc4.GetDatas());
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, tc4.GetDatas());
                    }
                    else //시도전체 또는 시도개별 발령
                    {
                        if (orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaLine)
                        {
                            udpSoc.SendTo(orderBizData.BrdProtocol.BroadNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, buff);
                        }

                        this.mainForm.MmfMng.WriteOrder(orderBizData.BrdProtocol);
                        string teleServerIpAddr = NCasUtilityMng.INCasCommUtility.AddIpAddr(NCasUtilityMng.INCasCommUtility.MakeNetIdByAnyIp(this.provInfo.NetIdToString), 0, 0, 1, 111);
                        udpSoc.SendTo(teleServerIpAddr, (int)NCasPortID.PortIdExtCallPgServer, buff);
                        udpSoc.SendTo(NCasUtilityMng.INCasEtcUtility.GetIPv4(), (int)NCasPortID.PortIdExtCallPgMan, buff);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbCentCBrd, buff);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbProvMain, buff);

                        if (orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaSate)
                        {
                            this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbCentSate, buff);
                        }

                        if ((orderBizData.BrdProtocol.BroadNetIdOrIp & 0x0000ffff) == 0x0000ffff) //x.x.255.255
                        {
                            this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbTermBroad, buff);
                            this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbTermDept, buff);
                        }
                        else if ((orderBizData.BrdProtocol.BroadNetIdOrIp & 0x0000ff00) == 0x0000fe00) //x.x.254.x
                        {
                            this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbTermBroad, buff);
                        }
                        else if ((orderBizData.BrdProtocol.BroadNetIdOrIp & 0x0000ff00) == 0x0000fd00) //x.x.253.x
                        {
                            this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbTermDept, buff);
                        }

                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbRgnPBrdCom, buff);
                        udpSoc.SendTo("127.0.0.1", (int)NCasPortID.PortIdExtCasMonitor, buff);

                        if (orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaAll || orderBizData.BrdProtocol.Media == NCasDefineOrderMedia.MediaLine)
                        {
                            for (int i = 0; i < this.SendUnicastOrderCount; i++)
                            {
                                udpSoc.SendTo(orderBizData.BrdProtocol.BroadNetIdOrIpByString, (int)NCasPortID.PortIdAlarm, buff);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PBrdScreenBiz", "PBrdScreenBiz.OnAsyncExternProcessing Method - OrderBizData", ex);
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
                    NCasLoggingMng.ILoggingException.WriteException("PBrdScreenBiz", "PBrdScreenBiz.OnAsyncExternProcessing Method - KeyBizData", ex);
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
                        NCasLoggingMng.ILogging.WriteLog("PBrdScreenBiz", "NCasPlcProtocolFactory.MakeFrame is null");
                        return;
                    }

                    if (buff == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PBrdScreenBiz", "buff is null");
                        return;
                    }

                    this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipeNccDevCcd, buff);
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PBrdScreenBiz", "PBrdScreenBiz.OnAsyncExternProcessing Method - NCasPlcProtocolBase", ex);
                }
            }
            else if (param is NCasProtocolTc20)
            {
                try
                {
                    NCasProtocolTc20 protocolTc20 = param as NCasProtocolTc20;

                    if (protocolTc20.GetDatas() == null)
                    {
                        NCasLoggingMng.ILogging.WriteLog("PBrdScreenBiz", "TV자막이 정상적으로 처리되지 않았습니다.",
                            "TC " + protocolTc20.TcCode.ToString() + " - " + NCasUtilityMng.INCasEtcUtility.Bytes2HexString(protocolTc20.GetDatas()));
                        return;
                    }

                    NCasProtocolBase protoBase = NCasProtocolFactory.ParseFrame(protocolTc20.GetDatas());
                    NCasProtocolTc20 proto20 = protoBase as NCasProtocolTc20;

                    if (protocolTc20.Sector == NCasDefineSectionCode.SectionBroadShare) //광역시 전체
                    {
                        proto20.Sector = NCasDefineSectionCode.SectionProv;
                        byte[] tmpProto20 = NCasProtocolFactory.MakeUdpFrame(proto20);

                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbRgnPBrdMan, tmpProto20);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbProvMain, tmpProto20);
                        this.mainForm.MmfMng.WriteBroadCaptionOrder(proto20);
                    }
                    else //시도전체 또는 시도개별
                    {
                        byte[] tmpProto20 = NCasProtocolFactory.MakeUdpFrame(proto20);

                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbCentCBrd, tmpProto20);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbProvMain, tmpProto20);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbCentCBrd, tmpProto20);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbTermBroad, tmpProto20);
                        this.udpSoc.SendTo(this.IP_LOOPBACK, (int)NCasPipes.PipePcbRgnPBrdCom, tmpProto20);
                        this.mainForm.MmfMng.WriteBroadCaptionOrder(proto20);
                    }
                }
                catch (Exception ex)
                {
                    NCasLoggingMng.ILoggingException.WriteException("PBrdScreenBiz", "PBrdScreenBiz.OnAsyncExternProcessing Method - NCasProtocolTc20", ex);
                }
            }
        }
        #endregion
    }
}