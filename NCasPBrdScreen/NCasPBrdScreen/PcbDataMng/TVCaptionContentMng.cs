using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NCASFND.NCasLogging;
using NCASBIZ.NCasEnv;
using NCASBIZ.NCasDefine;

namespace NCasPBrdScreen
{
    public static class TVCaptionContentMng
    {
        private static TVCaptionContainer lstTvCaptionContent = new TVCaptionContainer();
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasBrdTVCaption.xml";

        /// <summary>
        /// TV자막정보 데이터 리스트
        /// </summary>
        public static List<TVCaptionContent> LstTvCaptionContent
        {
            get { return lstTvCaptionContent.LstTVCaptionContent; }
        }

        /// <summary>
        /// TV자막 정보 로드
        /// </summary>
        public static void LoadTvCaptionContents()
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TVCaptionContainer));
                    lstTvCaptionContent = (TVCaptionContainer)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("TVCaptionContentMng", "LoadTvCaptionContents() Method", ex);
            }
        }

        /// <summary>
        /// TV자막 정보 저장
        /// </summary>
        public static void SaveTvCaptionContents()
        {
            try
            {
                #region 임시 TV자막 정보 파일 생성
                #region 경북
                //TVCaptionContent tvCaptionContent = new TVCaptionContent();
                //tvCaptionContent.ProvCode = 1678; //경북

                //TVCaptionData tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmWatch;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [CYAN]경계경보[WHITE]를 발령합니다. 현재시각 경북 지역에 실제 [CYAN]경계경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmAttack;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [RED]공습경보[WHITE]를 발령합니다. 현재시각 경북 지역에 실제 [RED]공습경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmBiochemist;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [YELLOW]화생방경보[WHITE]를 발령합니다. 현재시각 경북 지역에 실제 [YELLOW]화생방경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmCancel;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. 경북 지역에 발령했던 [RED]실제경보[WHITE]를 해제합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmClose;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 이상은 민방위재해통제본부에서 알려드렸습니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmWatch;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [CYAN]경계경보[WHITE]를 발령합니다. 현재시각 경북 지역에 [BLUE]훈련 [CYAN]경계경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmAttack;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [RED]공습경보[WHITE]를 발령합니다. 현재시각 경북 지역에 [BLUE]훈련 [RED]공습경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmBiochemist;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [YELLOW]화생방경보[WHITE]를 발령합니다. 현재시각 경북 지역에 [BLUE]훈련 [YELLOW]화생방경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmCancel;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. 경북 지역에 발령했던 [BLUE]훈련경보[WHITE]를 해제합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmClose;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 이상은 민방위재해통제본부에서 알려드렸습니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //lstTvCaptionContent.LstTVCaptionContent.Add(tvCaptionContent);
                #endregion

                #region 대구
                //tvCaptionContent = new TVCaptionContent();
                //tvCaptionContent.ProvCode = 1677; //대구

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmWatch;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [CYAN]경계경보[WHITE]를 발령합니다. 현재시각 대구 지역에 실제 [CYAN]경계경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmAttack;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [RED]공습경보[WHITE]를 발령합니다. 현재시각 대구 지역에 실제 [RED]공습경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmBiochemist;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [RED]실제 [YELLOW]화생방경보[WHITE]를 발령합니다. 현재시각 대구 지역에 실제 [YELLOW]화생방경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmCancel;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. 대구 지역에 발령했던 [RED]실제경보[WHITE]를 해제합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.RealTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmClose;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 이상은 민방위재해통제본부에서 알려드렸습니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmWatch;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [CYAN]경계경보[WHITE]를 발령합니다. 현재시각 대구 지역에 [BLUE]훈련 [CYAN]경계경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmAttack;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [RED]공습경보[WHITE]를 발령합니다. 현재시각 대구 지역에 [BLUE]훈련 [RED]공습경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmBiochemist;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. [BLUE]훈련 [YELLOW]화생방경보[WHITE]를 발령합니다. 현재시각 대구 지역에 [BLUE]훈련 [YELLOW]화생방경보[WHITE]를 발령합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmCancel;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 여기는 민방위재해통제본부입니다. 대구 지역에 발령했던 [BLUE]훈련경보[WHITE]를 해제합니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //tvCaptionData = new TVCaptionData();
                //tvCaptionData.TvCaptionMode = NCasDefineCaption.TestTvCaption;
                //tvCaptionData.OrderKind = NCasDefineOrderKind.AlarmClose;
                //tvCaptionData.TvText = "[WHITE]국민 여러분 이상은 민방위재해통제본부에서 알려드렸습니다.";
                //tvCaptionContent.TVCaptionData.Add(tvCaptionData);

                //lstTvCaptionContent.LstTVCaptionContent.Add(tvCaptionContent);
                #endregion
                #endregion

                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(TVCaptionContainer));
                    ser.Serialize(stream, lstTvCaptionContent);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("TVCaptionContentMng", "SaveTvCaptionContents() Method", ex);
            }
        }

        /// <summary>
        /// 단일 시도 TV자막 정보 저장
        /// </summary>
        /// <param name="captionData"></param>
        public static void AddTvCaptionContent(TVCaptionContent captionData)
        {
        }

        /// <summary>
        /// 단일 시도 TV자막 정보 가져오기
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static TVCaptionContent GetTvCaptionContent(int provCode)
        {
            TVCaptionContent rstTvCaptionData = new TVCaptionContent();

            foreach (TVCaptionContent content in lstTvCaptionContent.LstTVCaptionContent)
            {
                if (content.ProvCode == provCode)
                {
                    rstTvCaptionData = content;
                    break;
                }
            }

            return rstTvCaptionData;
        }

        /// <summary>
        /// 단일 TV자막 정보 가져오기
        /// </summary>
        /// <param name="provCode">시도 Code</param>
        /// <param name="tvCaptionMode">TV자막 모드</param>
        /// <param name="defineOrderKind">경보 모드</param>
        /// <returns></returns>
        public static TVCaptionData GetTvCaptionData(int provCode, NCasDefineCaption tvCaptionMode, NCasDefineOrderKind defineOrderKind)
        {
            TVCaptionContent rstTvCaptionData = new TVCaptionContent();
            TVCaptionData rstTvCaption = new TVCaptionData();

            foreach (TVCaptionContent content in lstTvCaptionContent.LstTVCaptionContent)
            {
                if (content.ProvCode == provCode)
                {
                    rstTvCaptionData = content;
                    break;
                }
            }

            foreach (TVCaptionData tvCaption in rstTvCaptionData.TVCaptionData)
            {
                if (tvCaption.TvCaptionMode == tvCaptionMode)
                {
                    if (tvCaption.OrderKind == defineOrderKind)
                    {
                        rstTvCaption = tvCaption;
                        break;
                    }
                }
            }

            return rstTvCaption;
        }

        /// <summary>
        /// 단일 시도 TV자막 정보 삭제
        /// </summary>
        /// <param name="captionData"></param>
        public static void RemoveTvCaptionContent(TVCaptionContent captionData)
        {
        }
    }
}