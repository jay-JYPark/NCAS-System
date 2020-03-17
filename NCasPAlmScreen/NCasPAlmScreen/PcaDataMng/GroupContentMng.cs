using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NCASFND.NCasLogging;
using NCASBIZ.NCasEnv;
using NCasAppCommon.Define;
using NCasAppCommon.Type;

namespace NCasPAlmScreen
{
    public class GroupContentMng
    {
        private static GroupContentContainer lstGroupContent = new GroupContentContainer();
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasAlmGroupData.xml";

        /// <summary>
        /// 그룹 정보 컨테이너 프로퍼티
        /// </summary>
        public static List<GroupContent> LstGroupContent
        {
            get { return lstGroupContent.LstGroupContent; }
        }

        /// <summary>
        /// 그룹 정보 로드
        /// </summary>
        public static void LoadGroupContent()
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GroupContentContainer));
                    lstGroupContent = (GroupContentContainer)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("GroupContentMng", "LoadGroupContent() Method", ex);
            }
        }

        /// <summary>
        /// 그룹 정보 저장
        /// </summary>
        public static void SaveGroupContent()
        {
            try
            {
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(GroupContentContainer));
                    ser.Serialize(stream, lstGroupContent);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("GroupContentMng", "SaveGroupContent() Method", ex);
            }
        }

        /// <summary>
        /// 단일 그룹 정보 저장
        /// </summary>
        /// <param name="groupContent"></param>
        public static void AddGroupContent(GroupContent groupContent)
        {
            foreach (GroupContent Content in lstGroupContent.LstGroupContent)
            {
                if (Content.Title == groupContent.Title) //기존에 있는 그룹이면..
                {
                    Content.LstGroupData.Clear();
                    Content.LstGroupData = groupContent.LstGroupData;
                    SaveGroupContent();
                    return;
                }
            }

            lstGroupContent.LstGroupContent.Add(groupContent);
            SaveGroupContent();
        }

        /// <summary>
        /// 단일 그룹 정보 삭제
        /// </summary>
        /// <param name="groupContent"></param>
        public static void RemoveGroupContent(GroupContent groupContent)
        {
        }

        /// <summary>
        /// 단일 그룹 정보 가져오기
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static GroupContent GetGroupContent(string title)
        {
            GroupContent groupContent = new GroupContent();

            foreach (GroupContent eachGroupContent in lstGroupContent.LstGroupContent)
            {
                if (eachGroupContent.Title == title)
                {
                    groupContent = eachGroupContent;
                    break;
                }
            }

            return groupContent;
        }
    }
}