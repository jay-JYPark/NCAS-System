using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasDefine;
using NCASBIZ.NCasProtocol;

namespace NCasPDbManager
{
    public class QueryMng
    {
        /// <summary>
        /// 문자방송 메세지를 저장한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="section"></param>
        public static string GetCheckHaveBroadCaptionHistQuery(DateTime occurTime, int areaCode, int section)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM BroadCaptionHist WHERE OCCURTIME1 = TO_DATE({0}, 'yyyymmddhh24miss') AND AREACODE = {1} AND SECTION = {2}",
                occurTime.ToString("yyyyMMddHHmmss"), areaCode, section);
            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송 응답 결과 데이터를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="section"></param>
        /// <param name="source"></param>
        /// <param name="kind"></param>
        /// <param name="media"></param>
        /// <param name="closeProcFlag"></param>
        /// <param name="respResultFlag"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetInsertPreBroadRespResultHistQuery(int orderMode, NCasDefineTcCode respResultKind, DateTime occurTime, DateTime respResultTime, int areaCode, int broadCtrlFlag, int section, int source, int kind, int media, int closeProcFlag, int respResultFlag, int devCode, int devKind)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                strBuilder.Append("INSERT INTO PreBroadRespHist (ORDERMODE, OCCURTIME1, OCCURTIME2, RESPTIME, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, DEVKIND, KIND, CODE, MEDIA, CAPTIONCODE, CLOSEPROCFLAG, RESPFLAG) \n");
                strBuilder.AppendFormat("VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, NULL, 1, {12})",
                    orderMode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, source, devKind, kind, devCode, media, respResultFlag);
            }
            else
            {
                strBuilder.Append("INSERT INTO PreBroadResultHist (ORDERMODE, OCCURTIME1, OCCURTIME2, RESULTTIME, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, DEVKIND, KIND, CODE, MEDIA, SIRENRESULTFLAG, CAPTIONCODE, CLOSEPROCFLAG) \n");
                strBuilder.AppendFormat("VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, NULL, 1)",
                    orderMode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, source, devKind, kind, devCode, media, respResultFlag);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송발령 데이터를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="oldSection"></param>
        /// <param name="newSection"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="source"></param>
        /// <param name="kind"></param>
        /// <param name="media"></param>
        /// <param name="respReqFlag"></param>
        /// <param name="closeProcFlag"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetInsertBroadOrderHistQuery(NCasProtocolTc4 tc4, MasterInfoData areaMasterInfoData, MasterInfoData devMasterInfoData)
        {
            StringBuilder strBuilder = new StringBuilder();
            NCasDefineSectionCode sectionVal = NCasDefineSectionCode.None;
            NCasDefineDeviceKind devKind = NCasDefineDeviceKind.None;
            string query = string.Empty;
            string tableName = string.Empty;
            if (tc4.Mode == NCasDefineOrderMode.RealMode)
            {
                tableName = "REALBROADORDERHIST";
            }
            else
            {
                tableName = "TESTBROADORDERHIST";
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
                        if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9} FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind);
                        }
                        else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9} FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind);
                        }
                    }
                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)//전국기관
                    {
                        devKind = NCasDefineDeviceKind.DeptSys;

                        if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9} FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind);
                        }
                        else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, Code, {9} FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind);
                        }
                    }

                    if ((((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) || (tc4.BroadNetIdOrIp & 0x0000FF00) == 0x00004300) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))//시도특정방송
                    {
                        int areaCode = 1670;
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10} FROM DUAL",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, devMasterInfoData.Code, devMasterInfoData.DevId);
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FD00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10} FROM DUAL",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, devMasterInfoData.Code, devMasterInfoData.DevId);
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
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9} FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {10} AND UseFlag = 1",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, areaMasterInfoData.Code);
                    }

                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9} FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {10} AND UseFlag = 1",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, areaMasterInfoData.Code);
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.ProvBroadSys;
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10} FROM DUAL",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, areaMasterInfoData.Code, (int)devKind);
                    }
                    break;

                case NCasDefineSectionCode.SectionBroad:
                    devKind = NCasDefineDeviceKind.ProvBroadSys;
                    strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, {9}, {10} FROM DUAL",
                        tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, areaMasterInfoData.Code, (int)devKind);
                    break;

                case NCasDefineSectionCode.SectionDist:
                    devKind = NCasDefineDeviceKind.PdeptSys;
                    strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, RespReqFlag, CloseProcFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, {8}, 0, 1, MasterInfo.Code, {9} FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {10} AND UseFlag = 1",
                        tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)tc4.Media, (int)devKind, areaMasterInfoData.Code);
                    break;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송 응답 결과 정보를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetInsertBroadRespResultHistQuery(NCasProtocolTc4 tc4, NCasDefineTcCode respResultKind, MasterInfoData areaMasterInfoData, MasterInfoData devMasterInfoData)
        {
            StringBuilder strBuilder = new StringBuilder();
            NCasDefineSectionCode sectionVal = NCasDefineSectionCode.None;
            NCasDefineDeviceKind devKind = NCasDefineDeviceKind.None;
            string query = string.Empty;
            string respResultTableName = string.Empty;
            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                if (tc4.Mode == NCasDefineOrderMode.RealMode)
                {
                    respResultTableName = "REALBROADRESPHIST";
                }
                else
                {
                    respResultTableName = "TESTBROADRESPHIST";
                }
            }
            else
            {
                if (tc4.Mode == NCasDefineOrderMode.RealMode)
                {
                    respResultTableName = "REALBROADRESULTHIST";
                }
                else
                {
                    respResultTableName = "TESTBROADRESULTHIST";
                }
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
                        if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                        {
                            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                            else
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 12 OR MASTERTYPE = 13) AND UseFlag = 1",
                                       respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                        }
                        else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                        {
                            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                            else
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 12 AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                        }
                    }
                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)//전국기관
                    {
                        devKind = NCasDefineDeviceKind.DeptSys;

                        if (tc4.Sector == NCasDefineSectionCode.SectionGlobal)
                        {
                            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                            else
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                        }
                        else if (tc4.Sector == NCasDefineSectionCode.SectionProv || tc4.Sector == NCasDefineSectionCode.SectionRegn1)
                        {
                            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.Devid FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                            else
                            {
                                strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 10 AND UseFlag = 1",
                                        respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind);
                            }
                        }
                    }

                    if ((((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) || (tc4.BroadNetIdOrIp & 0x0000FF00) == 0x00004300) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))//시도특정방송
                    {
                        int areaCode = 1670;
                        if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaCode, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                        }
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FD00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, devMasterInfoData.Code, devMasterInfoData.DevId);
                        }
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
                        if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {8} AND UseFlag = 1",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {8} AND UseFlag = 1",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        }
                    }

                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                   respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                        }
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.ProvBroadSys;
                        if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, code, {8} FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {9} AND UseFlag = 1",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)devKind, areaMasterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, code, {8} FROM MasterInfo WHERE MASTERTYPE = 13 AND ParentCode = {9} AND UseFlag = 1",
                                    respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, (int)devKind, areaMasterInfoData.Code);
                        }
                    }
                    break;

                case NCasDefineSectionCode.SectionBroad:
                    devKind = NCasDefineDeviceKind.ProvBroadSys;
                    if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code, (int)devKind);
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, {8}, {9} FROM DUAL",
                                respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code, (int)devKind);
                    }
                    break;

                case NCasDefineSectionCode.SectionDist:
                    devKind = NCasDefineDeviceKind.PdeptSys;
                    if (respResultKind == NCasDefineTcCode.TcBroadResponse)
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2,	AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, RespFlag, Code, DevKind) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.Devid FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                respResultTableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OccurTime1, OccurTime2, AreaCode, BroadCtrlFlag, Section, Source, Kind, Media, CloseProcFlag, SirenResultFlag, Code, DevKind)	SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, MasterInfo.Code, MasterInfo.DEVID FROM MasterInfo WHERE (MASTERTYPE = 10 OR MASTERTYPE = 11) AND ParentCode = {8} AND UseFlag = 1",
                                tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), areaMasterInfoData.Code, (int)tc4.CtrlKind, (int)tc4.Sector, (int)tc4.Source, (int)tc4.AlarmKind, areaMasterInfoData.Code);
                    }
                    break;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// BroadOrderHist테이블에 해당 발령이 있는는지 확인하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="section"></param>
        /// <param name="devKind"></param>
        public static string GetCheckHaveBroadOrderHistQuery(NCasProtocolTc4 tc4, MasterInfoData areaMasterInfoData, MasterInfoData devMasterInfoData)
        {
            StringBuilder strBuilder = new StringBuilder();
            NCasDefineSectionCode sectionVal = NCasDefineSectionCode.None;
            NCasDefineDeviceKind devKind = NCasDefineDeviceKind.None;
            string query = string.Empty;
            string tableName = string.Empty;
            if(tc4.Mode == NCasDefineOrderMode.RealMode)
            {
                tableName = "REALALARMORDERHIST";
            }
            else
            {
                tableName = "TESTALARMORDERHIST";
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
                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FEFF)
                    {
                        devKind = NCasDefineDeviceKind.CentBroadSys;
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector);
                    }
                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)
                    {
                        devKind = NCasDefineDeviceKind.DeptSys;
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND DevKind = {3}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind);
                    }

                    if ((((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) || (tc4.BroadNetIdOrIp & 0x0000FF00) == 0x00004300) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Code = {2}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), devMasterInfoData.Code);
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FD00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Code = {2}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), devMasterInfoData.Code);
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
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} A, ProvBroadInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                    }

                    if ((tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FFFF || (tc4.BroadNetIdOrIp & 0x0000FFFF) == 0x0000FDFF)
                    {
                        devKind = NCasDefineDeviceKind.PdeptSys;
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} A, ProvDeptInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                    }

                    if (((tc4.BroadNetIdOrIp & 0x0000FF00) == 0x0000FE00) && ((tc4.BroadNetIdOrIp & 0x000000FF) != 0x000000FF))
                    {
                        devKind = NCasDefineDeviceKind.ProvBroadSys;
                        strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND CODE = {3}",
                            tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, areaMasterInfoData.Code);
                    }
                    break;

                case NCasDefineSectionCode.SectionBroad:
                    devKind = NCasDefineDeviceKind.ProvBroadSys;
                    strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} WHERE OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND Section = {2} AND CODE = {3}",
                        tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, areaMasterInfoData.Code);
                    break;

                case NCasDefineSectionCode.SectionDist:
                    devKind = NCasDefineDeviceKind.PdeptSys;
                    strBuilder.AppendFormat("SELECT COUNT(*) FROM {0} A, ProvDeptInfo B WHERE A.OccurTime1 = TO_DATE({1}, 'yyyymmddhh24miss') AND A.SECTION = {2} AND A.DEVKIND = {3} AND A.CODE = B.CODE AND B.PROVCODE = {4}",
                        tableName, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), (int)tc4.Sector, (int)devKind, areaMasterInfoData.Code);
                    break;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 장비상태 값을 저장한다.(이상 발생 시)
        /// </summary>
        /// <param name="devCode"></param>
        /// <param name="occurTime"></param>
        /// <param name="pointCode"></param>
        /// <param name="pointValue"></param>
        public static string GetInsertTermStatusHistQuery(DateTime occurTime, int pointType, int occurVal, int pointCode, int devCode)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("INSERT INTO TERMSTATUSHIST (CODE ,OCCURTIME1 ,OCCURTIME2 ,RECOVERTIME1 ,RECOVERTIME2 ,POINTCODE ,POINTTYPE ,OCCURVAL ,RECOVERVAL) VALUES ( {0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, NULL, NULL, {3}, {4}, {5}, NULL)",
                devCode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), pointCode, pointType, occurVal);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 이상이 발생한 장비값을 정상 또는 다른 값으로 업데이트하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="recoverTime"></param>
        /// <param name="recoverValue"></param>
        /// <param name="devCode"></param>
        /// <param name="pointCode"></param>
        public static string GetUpdateTermStatusHistQuery(DateTime recoverTime, int pointType, int recoverVal, int pointCode, int devCode)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("UPDATE TERMSTATUSHIST SET RECOVERTIME1 = TO_DATE({0}, 'yyyymmddhh24miss'), RECOVERTIME2 = {1}, RECOVERVAL = {2} WHERE CODE = {3} AND POINTCODE = {4} AND RECOVERTIME1 IS NULL",
                recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), recoverVal, devCode, pointCode);

            return strBuilder.ToString();
        }

        public static string GetTermStatusHistQuery(DateTime occurTime, PointType pointType, int eventVal, int pointCode, int devCode)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (pointType == PointType.Digital)//디지털
            {
                if (eventVal == (int)DigitalPointValue.AbNormal)//이상
                {
                    strBuilder.AppendFormat("INSERT INTO TERMSTATUSHIST (CODE ,OCCURTIME1 ,OCCURTIME2 ,RECOVERTIME1 ,RECOVERTIME2 ,POINTCODE ,POINTTYPE ,OCCURVAL ,RECOVERVAL) VALUES ( {0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, NULL, NULL, {3}, {4}, {5}, NULL)",
                        devCode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), pointCode, (int)pointType, eventVal);
                }
                else if(eventVal == (int)DigitalPointValue.Active || eventVal == (int)DigitalPointValue.Normal)//정상, ACTIVE
                {
                    strBuilder.AppendFormat("UPDATE TERMSTATUSHIST SET RECOVERTIME1 = TO_DATE({0}, 'yyyymmddhh24miss'), RECOVERTIME2 = {1}, RECOVERVAL = {2} WHERE CODE = {3} AND POINTCODE = {4} AND RECOVERTIME1 IS NULL",
                        occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), eventVal, devCode, pointCode);
                }
            }
            else//아날로그
            {
                if(eventVal == (int)AnalogPointValue.High || eventVal == (int)AnalogPointValue.Low)
                {
                    strBuilder.AppendFormat("INSERT INTO TERMSTATUSHIST (CODE ,OCCURTIME1 ,OCCURTIME2 ,RECOVERTIME1 ,RECOVERTIME2 ,POINTCODE ,POINTTYPE ,OCCURVAL ,RECOVERVAL) VALUES ( {0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, NULL, NULL, {3}, {4}, {5}, NULL)",
                        devCode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), pointCode, (int)pointType, eventVal);
                }
                else if (eventVal == (int)AnalogPointValue.Normal)//정상
                {
                    strBuilder.AppendFormat("UPDATE TERMSTATUSHIST SET RECOVERTIME1 = TO_DATE({0}, 'yyyymmddhh24miss'), RECOVERTIME2 = {1}, RECOVERVAL = {2} WHERE CODE = {3} AND POINTCODE = {4} AND RECOVERTIME1 IS NULL",
                        occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), eventVal, devCode, pointCode);
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 해당 데이터 단말의 로그인 이력이 있는지 확인하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="code"></param>
        public static string GetCheckHaveLoginHistQuery(int code)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM TermLoginHist WHERE CODE = {0} AND OutTime IS NULL",
                code);
            return strBuilder.ToString();
        }

        ///// <summary>
        ///// 로그아웃 시 업데이트 하는 쿼리문을 반환한다.
        ///// </summary>
        ///// <param name="logoutTime"></param>
        ///// <param name="devCode"></param>
        //public static string GetUpdateTermLoginHistQuery(DateTime logoutTime, int devCode)
        //{

        //    return "";
        //}

        /// <summary>
        /// 로그인 했을 경우 데이터를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="isInvasion"></param>
        /// <param name="devCode"></param>
        /// <param name="logInTime"></param>
        /// <param name="logOutTime"></param>
        /// <param name="loginId"></param>
        public static string GetInsertUpdatetTermLoginHistQuery(LoginStatus loginStatus, int devCode, DateTime logInTime, DateTime logOutTime, string loginId)
        {
            StringBuilder strBuilder = new StringBuilder();
            switch(loginStatus)
            {
                case LoginStatus.Trespass :
                    strBuilder.AppendFormat("INSERT INTO TermLoginHist (Code, InTime, OutTime, LoginID) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), TO_DATE({2}, 'yyyymmddhh24miss'), '{3}')",
                        devCode, logInTime.ToString("yyyyMMddHHmmss"), logOutTime.ToString("yyyyMMddHHmmss"), loginId);
                    break;
                case LoginStatus.Login:
                    strBuilder.AppendFormat("INSERT INTO TermLoginHist (Code, InTime, LoginID) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), '{2}')",
                        devCode, logInTime.ToString("yyyyMMddHHmmss"), loginId);
                    break;
                case LoginStatus.BeLogin:
                    strBuilder.AppendFormat("UPDATE TermLoginHist SET OutTime = TO_DATE({0}, 'yyyymmddhh24miss') WHERE CODE = {1} AND OutTime IS NULL",
                        logOutTime.ToString("yyyyMMddHHmmss"), devCode);
                    break;
                case LoginStatus.BeNotLogin:
                    strBuilder.AppendFormat("INSERT INTO TermLoginHist (Code, InTime, OutTime, LoginID) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), TO_DATE({2}, 'yyyymmddhh24miss'), '{3}')",
                        devCode, logInTime.ToString("yyyyMMddHHmmss"), logOutTime.ToString("yyyyMMddHHmmss"), loginId);
                    break;
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 음성회선감시 결과를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="ip"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="status"></param>
        public static string GetInsertVoiceLineTestResultHistQuery(DateTime occurTime, string ip, int devCode, int devKind, int status)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("INSERT INTO VOICETESTRESULTHIST (OccurTime1, OccurTime2, DevCode, DevKind, RESULT) VALUES (TO_DATE({0}, 'yyyymmddhh24miss'), {1}, '{2}', {3}, {4})",
                occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), devCode, devKind, status);
            
            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송회선 점검 요구 데이터 입력 쿼리문을 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="ip"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetInsertVoiceLineTestReqQuery(DateTime occurTime, string ip, int devCode, int devKind)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("INSERT INTO VoiceLineTestReq (OccurTime1, OccurTime2, NetID, DevCode, DevKind) VALUES (TO_DATE({0}, 'yyyymmddhh24miss'), {1}, {2}, {3}, {4})",
                occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), devCode, devKind);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 이상이었던 데이터를 정상으로 업데이트 하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetUpdateSateTestResultHistQuery(int devCode, int devKind, DateTime recoverTime)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("UPDATE SateTestResultHist SET RecoverTime1 = TO_DATE({0}, 'yyyymmddhh24miss'), RecoverTime2 = {1}, Result = 1 WHERE DevCode = {2} AND DevKind = {3} AND RecoverTime1 is NULL",
                recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), devCode, devKind);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 정상으로 올라온 데이터의 결과를 삭제한다.
        /// </summary>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="occurTime"></param>
        public static string GetDeleteSateTestResultHistQuery(int devCode, int devKind, DateTime occurTime)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("DELETE SateTestResultHist WHERE DevCode = {0} AND DevKind = {1} AND OccurTime2 = {2} AND RecoverTime1 is NULL",
                devCode, devKind, occurTime.ToString("yyyyMMddHHmmss"));

            return strBuilder.ToString();
        }

        /// <summary>
        /// 위성 수신기 감시 요구에 따른 결과를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="isCenter"></param>
        /// <param name="occurTime"></param>
        /// <param name="recoverTime"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="result"></param>
        /// <param name="provCode"></param>
        public static string GetInsertSateTestResultHistQuery(NCasDefineOrderSource source, DateTime occurTime, int provCode)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (source == NCasDefineOrderSource.CentCtrlRoom)
            {
                strBuilder.AppendFormat("INSERT INTO SateTestResultHist (OccurTime1, OccurTime2, RecoverTime1, RecoverTime2, DevCode, DevKind, Result) (SELECT TO_DATE({0}, 'yyyymmddhh24miss'), {1}, NULL, NULL, A.CODE, A.DEVID, 0 FROM MASTERINFO A WHERE (A.CODE, A.DEVID) NOT IN (SELECT B.CODE, B.KIND FROM SateFaultDev B) AND SATEFLAG = 1 AND UseFlag = 1)",
                    occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                strBuilder.AppendFormat("INSERT INTO SateTestResultHist (OccurTime1, OccurTime2, RecoverTime1, RecoverTime2, DevCode, DevKind, Result) SELECT TO_DATE({0}, 'yyyymmddhh24miss'), {1}, NULL, NULL, CODE, DEVID, 0 FROM MASTERINFO WHERE PROVCODE = {2} AND DEVID IN(6, 9, 50) AND USEFLAG = 1 AND SATEFLAG = 1",
                    occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), provCode);

                //strBuilder.AppendFormat("INSERT INTO SateTestResultHist (OccurTime1, OccurTime2, RecoverTime1, RecoverTime2, DevCode, DevKind, Result) (SELECT TO_DATE({0}, 'yyyymmddhh24miss'), {1}, NULL, NULL, A.CODE, A.DEVID, 0 FROM MASTERINFO A WHERE (A.CODE, A.DEVID) NOT IN (SELECT B.CODE, B.KIND FROM SateFaultDev B) AND SATEFLAG = 1 AND ( Provcode = {2} OR ParentCode = {2} OR REPTCODE = {2}) AND UseFlag = 1)",
                //    occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), provCode);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 위성수신기 감시요구 데이터를 입력한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="source"></param>
        /// <param name="ip"></param>
        /// <param name="media"></param>
        /// <param name="closeProcFlag"></param>
        public static string GetInsertSateTestOrderHistQuery(DateTime occurTime, int source, string ip, int media, int closeProcFlag)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO SATETESTORDERHIST (OCCURTIME1, OCCURTIME2, NETID, DEVIPADDR, MEDIA, CLOSEPROCFLAG) VALUES (TO_DATE({0}, 'yyyymmddhh24miss'), {1}, {2}, '{3}', {4}, {5})",
                occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), source, ip, media, closeProcFlag);
            return strBuilder.ToString();
        }

        /// <summary>
        /// 문자방송 메세지 이력을 저장한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="orderMode"></param>
        /// <param name="media"></param>
        /// <param name="section"></param>
        /// <param name="kind"></param>
        /// <param name="caption"></param>
        public static string GetInsertBroadCaptionHistQuery(DateTime occurTime, int areaCode, int orderMode, int media, int section, int kind, string caption)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO BroadCaptionHist (OCCURTIME1, OCCURTIME2, AREACODE, ORDERMODE, MEDIA, SECTION, KIND, CAPTIONCODE, CAPTION) VALUES (TO_DATE({0}, 'yyyymmddhh24miss'), {1}, {2}, {3}, {4}, {5}, {6}, NULL, '{7}')",
                occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), areaCode, orderMode, media, section, kind, caption);
            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송 응답결과 데이터가 테이블에 있는지 확인하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="source"></param>
        /// <param name="devCode"></param>
        public static string GetCheckHaveBroadRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, DateTime occurTime, int source, int devCode)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESPHIST";
                }
                else
                {
                    tableName = "TESTBROADRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESULTHIST";
                }
                else
                {
                    tableName = "TESTBROADRESULTHIST";
                }
            }

            strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM {0} WHERE OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss') AND SOURCE = {2} AND CODE = {3}",
                tableName, occurTime.ToString("yyyyMMddHHmmss"), source, devCode);

            return strBuilder.ToString();
        }

        public static string GetCheckHaveTermStatusHistQuery(PointType pointType, AlarmAnalogDigitalPointInfo pointInfo, int devCode)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM TERMSTATUSHIST WHERE CODE = {0} AND POINTCODE = {1} AND POINTTYPE = {2} AND RECOVERTIME1 IS NULL",
                devCode, (int)pointInfo, (int)pointType);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 장비이상 정상 데이터가 들어왔을 때 정상값으로 업데이트하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="recoverTime"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetUpdateFaultDeviceDataQuery(bool isTc8, DateTime recoverTime, int devCode, int devKind)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (isTc8)
            {
                strBuilder.AppendFormat("UPDATE  DevStatusHist SET RecoverTime1 = TO_DATE({0}, 'YYYYMMDDHH24MISS'), RecoverTime2 = {1}, FaultTerm = (TO_DATE({2}, 'YYYYMMDDHH24MISS') - OCCURTIME1) * 1440 WHERE EXISTS ( SELECT * FROM DevStatusHist WHERE CODE = {3} AND DevKind = {4} AND RecoverTime1 is NULL ) AND Code = {5} AND DevKind = {6} AND RECOVERTIME1 IS NULL",
                    //recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), devCode, devKind, devCode, devKind);
                    recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), devCode, devKind, devCode, devKind);
            }
            else
            {
                strBuilder.AppendFormat("UPDATE DevStatusHist SET RecoverTime1 = TO_DATE({0}, 'YYYYMMDDHH24MISS'), RecoverTime2 = {1}, FaultTerm = (TO_DATE({2}, 'YYYYMMDDHH24MISS') - OCCURTIME1) * 1440 WHERE EXISTS (SELECT * FROM DevStatusHist WHERE CODE = {3} AND DevKind = {4} AND RecoverTime1 is NULL) AND Code = {5} AND DevKind = {6} AND RecoverTime1 IS NULL",
                    recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), recoverTime.ToString("yyyyMMddHHmmss"), devCode, devKind, devCode, devKind);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 장비이상 데이터를 입력하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="occurTime"></param>
        /// <param name="recoverTime"></param>
        /// <param name="faultTime"></param>
        /// <param name="devUseFlag"></param>
        public static string GetInsertFaultDeviceDataQuery(int devCode, int devKind, DateTime occurTime)
        {
            StringBuilder strBuilder = new StringBuilder();
    
                strBuilder.AppendFormat("INSERT INTO DevStatusHist (CODE, DEVKIND, OCCURTIME1, OCCURTIME2, RECOVERTIME1,	RECOVERTIME2,	FAULTTERM, USEFLAG) SELECT {0}, {1}, TO_DATE({2}, 'YYYYMMDDHH24MISS'), {3}, NULL, NULL, NULL, 1 FROM DUAL WHERE NOT EXISTS ( SELECT CODE FROM DevStatusHist WHERE  CODE = {4} AND DevKind = {5} AND RecoverTime1 is NULL )",
                    devCode, devKind, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), devCode, devKind);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 장애가 발생한 단말 데이터가 들어있는지 확인하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetCheckHaveFaultDeviceDataQuery(bool isTc8, int devCode, int devKind, DateTime eventTime)
        {
            StringBuilder strBuilder = new StringBuilder();

                strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM DevStatusHist WHERE  CODE = {0} AND DEVKIND = {1} AND  RECOVERTIME1 IS NULL",
                    devCode, devKind);

            return strBuilder.ToString();
        }

        public static string GetFaultDeviceInfoDataQuery(int devCode, int devKind)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendFormat("SELECT OCCURTIME1 FROM DevStatusHist WHERE  CODE = {0} AND DEVKIND = {1} AND  RECOVERTIME1 IS NULL",
                devCode, devKind);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 해당 발령이 테이블에 있는지 확인한다.
        /// 이미 발령된 발령 데이터인지 확인한다.
        /// 실제일지 시험인지 NCasDefineOrderMode를 넘겨준다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="orderControlKind"></param>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="source"></param>
        public static string GetCheckHaveAlarmOrderHistQuery(NCasDefineOrderMode orderMode, DateTime occurTime, int areaCode, int source)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;
            if (orderMode == NCasDefineOrderMode.RealMode)
            {
                tableName = "REALALARMORDERHIST";
            }
            else
            {
                tableName = "TESTALARMORDERHIST";
            }

            strBuilder.Append("SELECT COUNT(*) COUNT \n");
            strBuilder.AppendFormat("FROM {0} \n", tableName);
            strBuilder.Append("WHERE \n");
            strBuilder.AppendFormat("OCCURTIME2 = '{0}' \n", occurTime.ToString("yyyyMMddHHmmss"));
            strBuilder.AppendFormat("AND AREACODE = {0} \n", areaCode);
            strBuilder.AppendFormat("AND SOURCE = {0} \n", source);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 방송 응답 결과 정보를 업데이트하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="NCasDefineOrderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="section"></param>
        /// <param name="kind"></param>
        /// <param name="media"></param>
        /// <param name="cloaseProcFlag"></param>
        /// <param name="respFlag"></param>
        /// <param name="respResultTime"></param>
        /// <param name="devCode"></param>
        /// <param name="source"></param>
        public static string GetUpdateBroadRespResultHist(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, DateTime occurTime, int areaCode, int broadCtrlFlag, int section, int kind, int media, DateTime respResultTime, int devCode, int source, int respResultFlag)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESPHIST";
                }
                else
                {
                    tableName = "TESTBROADRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESULTHIST";
                }
                else
                {
                    tableName = "TESTBROADRESULTHIST";
                }
            }

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                strBuilder.AppendFormat("UPDATE {0} SET RESPTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, CLOSEPROCFLAG = 1, RESPFLAG = {7} WHERE OCCURTIME1 = TO_DATE({8}, 'yyyymmddhh24miss') AND CODE = {9} AND SOURCE = {10}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, kind, media, respResultFlag, occurTime.ToString("yyyyMMddHHmmss"), devCode, source);
            }
            else
            {
                strBuilder.AppendFormat("UPDATE {0} SET RESULTTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, CLOSEPROCFLAG = 1, SIRENRESULTFLAG = {7} WHERE OCCURTIME1 = TO_DATE({8}, 'yyyymmddhh24miss') AND CODE = {9} AND SOURCE = {10}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, kind, media, respResultFlag, occurTime.ToString("yyyyMMddHHmmss"), devCode, source);
            }
            return strBuilder.ToString();
        }

        public static string GetBroadCaptionResultQuery(NCasDefineCaption mode, NCasDefineTcCode respResultKind, DateTime occurTime, int devCode, NCasDefineNormalStatus result)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                if (mode == NCasDefineCaption.RealTvCaption)
                {
                    tableName = "REALBROADRESPHIST";
                }
                else
                {
                    tableName = "TESTBROADRESPHIST";
                }
            }
            else
            {
                if (mode == NCasDefineCaption.RealTvCaption)
                {
                    tableName = "REALBROADRESULTHIST";
                }
                else
                {
                    tableName = "TESTBROADRESULTHIST";
                }
            }

            strBuilder.AppendFormat("UPDATE {0} SET CAPTIONCODE = {1} WHERE CODE = {2} AND OCCURTIME1 = TO_DATE({3}, 'yyyymmddhh24miss')", tableName, (int)result, devCode, occurTime.ToString("yyyyMMddHHmmss")); 

            return strBuilder.ToString();
        }

        /// <summary>
        /// 해당 응답결과 데이터가 들어있는지 확인하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="respResultKind"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="source"></param>
        public static string GetCheckHaveAlarmRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, MasterInfoData masterInfoData, DateTime occurTime, int source)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESPHIST";
                }
                else
                {
                    tableName = "TESTALARMRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESULTHIST";
                }
                else
                {
                    tableName = "TESTALARMRESULTHIST";
                }
            }

            strBuilder.AppendFormat("SELECT COUNT(*) COUNT FROM {0} WHERE OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss') AND DEVCODE = {2} AND DEVKIND = {3} AND SOURCE = {4}",
                tableName, occurTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, masterInfoData.DevId, source);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 발령정보가 없는 응답 결과 데이터를 Pre테이블에 저장하는 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="respResultKind"></param>
        /// <param name="occurTime"></param>
        /// <param name="respResultTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="section"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="source"></param>
        /// <param name="kind"></param>
        /// <param name="media"></param>
        /// <param name="closeProcFlag"></param>
        /// <param name="respResultFlag"></param>
        public static string GetInsertPreAlarmRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, MasterInfoData masterinfoData, DateTime occurTime, DateTime respResultTime, int broadCtrlFlag, int section, int source, int alarmKind, int media, string hornStatus)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                strBuilder.AppendFormat("INSERT INTO PREALARMRESPHIST (ORDERMODE, OCCURTIME1, OCCURTIME2, RESPTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, 1, 1)",
                    (int)orderMode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), respResultTime.ToString("yyyyMMddHHmmss"), masterinfoData.ProvCode, broadCtrlFlag, section, masterinfoData.Code, masterinfoData.DevId, source, alarmKind, media);
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    strBuilder.AppendFormat("INSERT INTO PREALARMRESULTHIST (ORDERMODE, OCCURTIME1, OCCURTIME2, RESULTTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, SIRENRESULTFLAG, CLOSEPROC, HORNSTATUS) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, 1, 1, '{12}')",
                    (int)orderMode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), respResultTime.ToString("yyyyMMddHHmmss"), masterinfoData.ProvCode, broadCtrlFlag, section, masterinfoData.Code, masterinfoData.DevId, source, alarmKind, media, hornStatus);
                }
                else
                {
                    strBuilder.AppendFormat("INSERT INTO PREALARMRESULTHIST (ORDERMODE, OCCURTIME1, OCCURTIME2, RESULTTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, SIRENRESULTFLAG, CLOSEPROC) VALUES ({0}, TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, 1, 1)",
                    (int)orderMode, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), respResultTime.ToString("yyyyMMddHHmmss"), masterinfoData.ProvCode, broadCtrlFlag, section, masterinfoData.Code, masterinfoData.DevId, source, alarmKind, media);
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Pre테이블에 있는 데이터를  처리한 후 해당 Pre테이블에 있는 데이터를 삭제하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="source"></param>
        public static string GetDeletePreAlarmRespResultHistQuery(NCasDefineTcCode respResultKind, NCasProtocolTc1 tc1, AlarmRespResultData alarmRespResultData)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                strBuilder.AppendFormat("DELETE FROM PREALARMRESPHIST WHERE OCCURTIME1 = TO_DATE({0}, 'yyyymmddhh24miss') AND DEVCODE = {1} AND DEVKIND = {2} AND SOURCE = {3}",
                    tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), alarmRespResultData.DevCode, alarmRespResultData.DevKind, alarmRespResultData.Source);
            }
            else
            {
                strBuilder.AppendFormat("DELETE FROM PREALARMRESULTHIST WHERE OCCURTIME1 = TO_DATE({0}, 'yyyymmddhh24miss') AND DEVCODE = {1} AND DEVKIND = {2} AND SOURCE = {3}",
                    tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), alarmRespResultData.DevCode, alarmRespResultData.DevKind, alarmRespResultData.Source);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Pre테이블에 있는 데이터를  처리한 후 해당 Pre테이블에 있는 데이터를 삭제하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="occurTime"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        /// <param name="source"></param>
        public static string GetDeletePreBroadRespResultHistQuery(NCasDefineTcCode respResultKind, NCasProtocolTc4 tc4, BroadRespResultData broadRespResultData)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                strBuilder.AppendFormat("DELETE FROM PREBROADRESPHIST WHERE OCCURTIME1 = TO_DATE({0}, 'yyyymmddhh24miss') AND CODE = {1} AND DEVKIND = {2} AND SOURCE = {3}",
                    tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), broadRespResultData.DevCode, broadRespResultData.DevKind, broadRespResultData.Source);
            }
            else
            {
                strBuilder.AppendFormat("DELETE FROM PREBROADRESULTHIST WHERE OCCURTIME1 = TO_DATE({0}, 'yyyymmddhh24miss') AND CODE = {1} AND DEVKIND = {2} AND SOURCE = {3}",
                    tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), broadRespResultData.DevCode, broadRespResultData.DevKind, broadRespResultData.Source);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// PreResult, PreResp테이블에 해당 발령에 대한 정보가 있으면 업데이트를 하는 쿼리문을 반환한다.(경보)
        /// </summary>
        public static string GetUpdateAlarmRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, DateTime occurTime, DateTime respResultTime, int areaCode, int broadCtrlFlag, int section, int alarmKind, int media, int respResultFlag, int source, int devCode, int devId, string hornStatus)
        {
            StringBuilder strBuilder = new StringBuilder();
            int provCode = BasicDataMng.GetLocalCode();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESPHIST";
                }
                else
                {
                    tableName = "TESTALARMRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESULTHIST";
                }
                else
                {
                    tableName = "TESTALARMRESULTHIST";
                }
            }

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)//응답
            {
                strBuilder.AppendFormat("UPDATE {0} SET RESPTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, RESPFLAG = {7} WHERE OCCURTIME1 = TO_DATE({8}, 'yyyymmddhh24miss') AND DEVCODE = {9} AND DEVKIND = {10} AND SOURCE = {11}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, alarmKind, media, respResultFlag, occurTime.ToString("yyyyMMddHHmmss"), devCode, devId, source);
            }
            else//결과
            {
                if (orderMode == NCasDefineOrderMode.RealMode)//TO DO : 실제일때는 스피커 상태도 입력해줘야 한다.
                {
                    strBuilder.AppendFormat("UPDATE {0} SET RESULTTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, SIRENRESULTFLAG = {7}, HORNSTATUS = '{8}' WHERE OCCURTIME1 = TO_DATE({9}, 'yyyymmddhh24miss') AND DEVCODE = {10} AND DEVKIND = {11} AND SOURCE = {12}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, alarmKind, media, respResultFlag, hornStatus, occurTime.ToString("yyyyMMddHHmmss"), devCode, devId, source);
                }
                else
                {
                    strBuilder.AppendFormat("UPDATE {0} SET RESULTTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, SIRENRESULTFLAG = {7} WHERE OCCURTIME1 = TO_DATE({8}, 'yyyymmddhh24miss') AND DEVCODE = {9} AND DEVKIND = {10} AND SOURCE = {11}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, alarmKind, media, respResultFlag, occurTime.ToString("yyyyMMddHHmmss"), devCode, devId, source);
                }
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// PreResult, PreResp테이블에 해당 발령에 대한 정보가 있으면 업데이트를 하는 쿼리문을 반환한다.(방송)
        /// </summary>
        public static string GetUpdateBroadRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, DateTime occurTime, DateTime respResultTime, int areaCode, int broadCtrlFlag, int section, int alarmKind, int media, int respResultFlag, int source, int devCode, int devId, int captionCode, int closeProc)
        {
            StringBuilder strBuilder = new StringBuilder();
            int provCode = BasicDataMng.GetLocalCode();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESPHIST";
                }
                else
                {
                    tableName = "TESTBROADRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALBROADRESULTHIST";
                }
                else
                {
                    tableName = "TESTBROADRESULTHIST";
                }
            }

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                strBuilder.AppendFormat("UPDATE {0} SET RESPTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, RESPFLAG = {7}, CAPTIONCODE = {8}, CLOSEPROCFLAG = {9} WHERE OCCURTIME1 = TO_DATE({10}, 'yyyymmddhh24miss') AND CODE = {11} AND DEVKIND = {12} AND SOURCE = {13}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, alarmKind, media, respResultFlag, captionCode, closeProc, occurTime.ToString("yyyyMMddHHmmss"), devCode, devId, source);
            }
            else
            {
                strBuilder.AppendFormat("UPDATE {0} SET RESULTTIME = TO_DATE({1}, 'yyyymmddhh24miss'), AREACODE = {2}, BROADCTRLFLAG = {3}, SECTION = {4}, KIND = {5}, MEDIA = {6}, SIRENRESULTFLAG = {7}, CAPTIONCODE = {8}, CLOSEPROCFLAG = {9} WHERE OCCURTIME1 = TO_DATE({10}, 'yyyymmddhh24miss') AND CODE = {11} AND DEVKIND = {12} AND SOURCE = {13}",
                    tableName, respResultTime.ToString("yyyyMMddHHmmss"), areaCode, broadCtrlFlag, section, alarmKind, media, respResultFlag, captionCode, closeProc, occurTime.ToString("yyyyMMddHHmmss"), devCode, devId, source);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 미리들어온 응답, 결과가 있는지 확인하는 쿼리문을 반환한다.(경보)
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="respResultKind"></param>
        /// <param name="orderMode2"></param>
        /// <param name="occurTime"></param>
        public static string GetCheckHavePreAlarmRespResultHistQuery(NCasProtocolTc1 tc1, NCasDefineTcCode respResultKind)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                strBuilder.AppendFormat("SELECT A.RESPTIME AS RESULTTIME, A.AREACODE, A.BROADCTRLFLAG, A.SECTION, A.KIND, A.MEDIA, A.RESPFLAG AS RESULT, A.DEVCODE, A.DEVKIND, A.SOURCE FROM PREALARMRESPHIST A WHERE A.ORDERMODE = {0} AND A.OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss')",
                    (int)tc1.Mode, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                strBuilder.AppendFormat("SELECT A.RESULTTIME, A.AREACODE, A.BROADCTRLFLAG, A.SECTION, A.KIND, A.MEDIA, A.SIRENRESULTFLAG AS RESULT, A.DEVCODE, A.DEVKIND, A.SOURCE FROM PREALARMRESULTHIST A WHERE A.ORDERMODE = {0} AND A.OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss')",
                   (int)tc1.Mode, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"));
            }
            return strBuilder.ToString();
        }

        /// 미리들어온 응답, 결과가 있는지 확인하는 쿼리문을 반환한다.(방송)
        public static string GetCheckHavePreBroadRespResultHistQuery(NCasProtocolTc4 tc4, NCasDefineTcCode respResultKind)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (respResultKind == NCasDefineTcCode.TcBroadResponse)
            {
                strBuilder.AppendFormat("SELECT A.RESPTIME AS RESULTTIME, A.AREACODE, A.BROADCTRLFLAG, A.SECTION, A.KIND, A.MEDIA, A.RESPFLAG AS RESULT, A.CODE, A.DEVKIND, A.SOURCE, A.CAPTIONCODE, A.CLOSEPROCFLAG FROM PREBROADRESPHIST A WHERE A.ORDERMODE = {0} AND A.OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss')",
                    (int)tc4.Mode, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                strBuilder.AppendFormat("SELECT A.RESULTTIME, A.AREACODE, A.BROADCTRLFLAG, A.SECTION, A.KIND, A.MEDIA, A.SIRENRESULTFLAG AS RESULT, A.CODE, A.DEVKIND, A.SOURCE, A.CAPTIONCODE, A.CLOSEPROCFLAG FROM PREBROADRESULTHIST A WHERE A.ORDERMODE = {0} AND A.OCCURTIME1 = TO_DATE({1}, 'yyyymmddhh24miss')",
                   (int)tc4.Mode, tc4.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"));
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 응답 결과 데이터를 저장하기 위한 쿼리문을 반환한다.
        /// </summary>
        /// <param name="orderMode"></param>
        /// <param name="orderSource"></param>
        /// <param name="respResultKind"></param>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="section"></param>
        /// <param name="source"></param>
        /// <param name="kind"></param>
        /// <param name="media"></param>
        /// <param name="closeProcFlag"></param>
        /// <param name="respFlag"></param>
        /// <param name="devCode"></param>
        /// <param name="devKind"></param>
        public static string GetInsertAlarmRespResultHistQuery(NCasProtocolTc1 tc1, NCasDefineTcCode respResultKind, MasterInfoData masterInfoData)
        {
            StringBuilder strBuilder = new StringBuilder();
            int provCode = BasicDataMng.GetLocalCode();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                if (tc1.Mode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESPHIST";
                }
                else
                {
                    tableName = "TESTALARMRESPHIST";
                }
            }
            else
            {
                if (tc1.Mode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESULTHIST";
                }
                else
                {
                    tableName = "TESTALARMRESULTHIST";
                }
            }

            switch (tc1.Sector)
            {
                case NCasDefineSectionCode.SectionGlobal:

                    if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE (MASTERTYPE = 9 OR MASTERTYPE = 4) AND (PROVCODE = {8} OR PARENTCODE = {9} OR REPTCODE = {10}) AND (CODE <> {11}) AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, provCode, provCode, provCode, provCode);
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE MASTERTYPE = 9 AND (PROVCODE = {8} OR PARENTCODE = {9} OR REPTCODE = {10}) AND (CODE <> {11}) AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, provCode, provCode, provCode, provCode);
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
                    if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                    {
                        if (tc1.Source == NCasDefineOrderSource.CentCtrlRoom)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE CODE = {8} OR (PARENTCODE IN (SELECT CODE FROM MASTERINFO WHERE PARENTCODE = {9} AND MASTERTYPE = 6)) AND USEFLAG = 1",
                                tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code, masterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE IN (SELECT CODE FROM MASTERINFO WHERE PARENTCODE = {9} AND MASTERTYPE = 6) AND USEFLAG = 1",
                                tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code, masterInfoData.Code);
                        }
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE IN (SELECT CODE FROM MASTERINFO WHERE PARENTCODE = {8} AND MASTERTYPE = 6) AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                    }
                    break;
                case NCasDefineSectionCode.SectionRept:
                    if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, DEVID FROM MASTERINFO WHERE REPTFLAG = 1 AND REPTCODE = {8} AND USEFLAG = 1 AND MASTERTYPE = 9 AND DISTCODE = {9}",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.ReptCode, masterInfoData.Code);
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, DEVID FROM MASTERINFO WHERE REPTFLAG = 1 AND REPTCODE = {8} AND USEFLAG = 1 AND MASTERTYPE = 9 AND DISTCODE = {9}",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.ReptCode, masterInfoData.Code);
                    }
                    break;
                case NCasDefineSectionCode.SectionDist:
                    if (0x0000FDFF == (tc1.AlarmNetIdOrIp & 0x0000FDFF))
                    {
                        if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE = {8} AND MASTERTYPE = 11 AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE = {8} AND MASTERTYPE = 11 AND USEFLAG = 1",
                                tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                        }
                    }
                    else
                    {
                        if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE = {8} AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                        }
                        else
                        {
                            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE PARENTCODE = {8} AND USEFLAG = 1",
                                tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                        }
                    }
                    break;
                case NCasDefineSectionCode.SectionTerm:
                    if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE CODE = {8} AND USEFLAG = 1",
                        tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                    }
                    else
                    {
                        strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, AREACODE, BROADCTRLFLAG, SECTION, SOURCE, KIND, MEDIA, CLOSEPROC, SIRENRESULTFLAG, DEVCODE, DEVKIND) SELECT TO_DATE({1}, 'yyyymmddhh24miss'), {2}, {3}, {4}, {5}, {6}, {7}, 0, 1, 0, CODE, MASTERINFO.DEVID FROM MASTERINFO WHERE CODE = {8} AND USEFLAG = 1",
                            tableName, tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), tc1.OrderTimeByDateTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, (int)tc1.CtrlKind, (int)tc1.Sector, (int)tc1.Source, (int)tc1.AlarmKind, masterInfoData.Code);
                    }
                    break;
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// 응답 결과 데이터를 저장하기 위한 쿼리문을 반환한다.
        /// </summary>
        /// <param name="tc2"></param>
        /// <param name="respResultKind"></param>
        /// <param name="masterInfoData"></param>
        /// <returns></returns>
        public static string GetInsertAlarmRespResultHistQuery(NCasDefineOrderMode orderMode, NCasDefineTcCode respResultKind, MasterInfoData masterInfoData, DateTime occurTime, DateTime respResultTime, int areaCode, int broadCtrlFlag, int section, int source, int alarmKind, int media, int respResultFlag, string hornStatus)
        {
            StringBuilder strBuilder = new StringBuilder();
            int provCode = BasicDataMng.GetLocalCode();
            string tableName = string.Empty;

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESPHIST";
                }
                else
                {
                    tableName = "TESTALARMRESPHIST";
                }
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    tableName = "REALALARMRESULTHIST";
                }
                else
                {
                    tableName = "TESTALARMRESULTHIST";
                }
            }

            if (respResultKind == NCasDefineTcCode.TcAlarmResponse)
            {
                strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, RESPTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, CLOSEPROCFLAG, RESPFLAG) VALUES (TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, 1, {12})",
                    tableName, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, broadCtrlFlag, section, masterInfoData.Code, masterInfoData.DevId, source, alarmKind, media, respResultFlag);
            }
            else
            {
                if (orderMode == NCasDefineOrderMode.RealMode)
                {
                    strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, RESULTTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, SIRENRESULTFLAG, CLOSEPROC, HORNSTATUS) VALUES (TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, 1, '{13}')",
                    tableName, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, broadCtrlFlag, section, masterInfoData.Code, masterInfoData.DevId, source, alarmKind, media, respResultFlag, hornStatus);
                }
                else
                {
                    strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, OCCURTIME2, RESULTTIME, AREACODE, BROADCTRLFLAG, SECTION, DEVCODE, DEVKIND, SOURCE, KIND, MEDIA, SIRENRESULTFLAG, CLOSEPROC) VALUES (TO_DATE({1}, 'yyyymmddhh24miss'), {2}, TO_DATE({3}, 'yyyymmddhh24miss'), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, 1)",
                    tableName, occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), occurTime.ToString("yyyyMMddHHmmss"), masterInfoData.Code, broadCtrlFlag, section, masterInfoData.Code, masterInfoData.DevId, source, alarmKind, media, respResultFlag);
                }
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// AlarmOrderHist 테이블에 Insert하는 쿼리문을 불러온다.
        /// </summary>
        /// <param name="NCasDefineOrderMode"></param>
        /// <param name="occurTime"></param>
        /// <param name="areaCode"></param>
        /// <param name="broadCtrlFlag"></param>
        /// <param name="source"></param>
        /// <param name="kind"></param>
        /// <param name="section"></param>
        /// <param name="media"></param>
        /// <param name="respreqflag"></param>
        /// <param name="closeProcFlag"></param>
        public static string GetInsertAlarmOrderHistQuery(NCasDefineOrderMode orderMode , DateTime occurTime, int areaCode, int broadCtrlFlag, int source, int kind, int section, int media, int respreqflag, int closeProcFlag)
        {
            StringBuilder strBuilder = new StringBuilder();
            string tableName = string.Empty;

            if (orderMode == NCasDefineOrderMode.RealMode)
            {
                tableName = "REALALARMORDERHIST";
            }
            else
            {
                tableName = "TESTALARMORDERHIST";
            }

            strBuilder.AppendFormat("INSERT INTO {0} (OCCURTIME1, AREACODE, SOURCE, OCCURTIME2, BROADCTRLFLAG, KIND, SECTION, MEDIA, RESPREQFLAG, CLOSEPROCFLAG)\n", tableName);
            strBuilder.AppendFormat("VALUES(TO_DATE('{0}', 'yyyymmddhh24miss'), {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                occurTime.ToString("yyyyMMddHHmmss"), areaCode, source, occurTime.ToString("yyyyMMddHHmmss"), broadCtrlFlag, kind, section, media, respreqflag, closeProcFlag);

            return strBuilder.ToString();
        }

        /// <summary>
        /// 장비데이터를 불러오는 쿼리문을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetLoadMasterInfoDataQuery()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT \n");
            strBuilder.Append("CODE, SERIALNO, NAME, NETID, SUBMASK, MASTERTYPE, PARENTCODE, REPTCODE, PROVCODE, PROVNAME, DISTCODE, DISTNAME, TERMCODE, TERMNAME, SATEFLAG, REPTFLAG, DEVID, BROADID, BROADNETID, BROADSUBMASK, DEPTNETID, DEPTSUBMASK, REPTNETID, REPTSUBMASK, VOICELINENO, USEFLAG, OLDSYSFLAG, TERMFLAG, DSUSHELLNO, DSUCARDNO \n");
            strBuilder.Append("FROM MASTERINFO \n");
            //strBuilder.Append("WHERE USEFLAG = 1 \n");

            return strBuilder.ToString();
        }
    }
}
