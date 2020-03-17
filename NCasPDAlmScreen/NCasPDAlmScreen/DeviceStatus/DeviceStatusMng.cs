﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NCASBIZ.NCasEnv;
using NCASBIZ.NCasDefine;
using NCASFND.NCasLogging;
using NCasAppCommon.Define;

namespace NCasPDAlmScreen
{
    public static class DeviceStatusMng
    {
        private static DeviceStatusDataContainer lstDeviceStatusData = new DeviceStatusDataContainer();
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasPDAlmDeviceStatusData.xml";

        /// <summary>
        /// 장비 데이터 리스트 프로퍼티
        /// </summary>
        public static List<DeviceStatusData> LstDeviceStatusData
        {
            get { return lstDeviceStatusData.LstDeviceStatusData; }
        }

        /// <summary>
        /// 장비 정보 데이터 클래스 추가
        /// </summary>
        /// <param name="statusData"></param>
        public static void AddDeviceStatusData(DeviceStatusData statusData)
        {
        }

        /// <summary>
        /// 단밀 장비 정보 데이터 가져오기
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <returns></returns>
        public static DeviceStatusData GetDeviceStatusData(string ipAddr)
        {
            DeviceStatusData deviceStatusData = new DeviceStatusData();

            foreach (DeviceStatusData eachDeviceData in lstDeviceStatusData.LstDeviceStatusData)
            {
                if (eachDeviceData.IpAddr == ipAddr)
                {
                    deviceStatusData = eachDeviceData;
                    break;
                }
            }

            return deviceStatusData;
        }

        /// <summary>
        /// 장비 정보 데이터 클래스 삭제
        /// </summary>
        /// <param name="statusData"></param>
        public static void RemoveDeviceStatusData(DeviceStatusData statusData)
        {
        }

        /// <summary>
        /// 장비 정보 데이터 로드
        /// </summary>
        public static void LoadDeviceStatusDatas()
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeviceStatusDataContainer));
                    lstDeviceStatusData = (DeviceStatusDataContainer)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("DeviceStatusMng", "DeviceStatusMng.LoadDeviceStatusDatas() Method", ex);
            }
        }

        /// <summary>
        /// 장비 정보 데이터 저장
        /// </summary>
        public static void SaveDeviceStatusDatas()
        {
            try
            {
                #region 임시 장비정보 파일 생성
                //DeviceStatusData tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.1.1.1";
                //tmp.Name = "중앙경보대#1";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.1.1.2";
                //tmp.Name = "중앙경보대#2";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.2.1.1";
                //tmp.Name = "2중앙경보대#1";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.2.1.2";
                //tmp.Name = "2중앙경보대#2";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.96.1.3";
                //tmp.Name = "PLC";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.96.1.9";
                //tmp.Name = "주제어";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.96.1.2";
                //tmp.Name = "DUAL";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.96.1.5";
                //tmp.Name = "방송대#1";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);

                //tmp = new DeviceStatusData();
                //tmp.IpAddr = "10.96.1.6";
                //tmp.Name = "방송대#2";
                //lstDeviceStatusData.LstDeviceStatusData.Add(tmp);
                #endregion

                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(DeviceStatusDataContainer));
                    ser.Serialize(stream, lstDeviceStatusData);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("DeviceStatusMng", "DeviceStatusMng.SaveDeviceStatusDatas() Method", ex);
            }
        }
    }
}