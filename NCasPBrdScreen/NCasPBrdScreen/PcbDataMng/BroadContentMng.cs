using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NCASBIZ.NCasEnv;
using NCASFND.NCasLogging;

namespace NCasPBrdScreen
{
    public static class BroadContentMng
    {
        private static BroadContentContainer lstBroadContent = new BroadContentContainer();
        private static string filePath = NCasEnvironmentMng.NCasAppEnvPath + "\\NCasBrdBroadContent.xml";

        /// <summary>
        /// 방송문안 리스트 프로퍼티
        /// </summary>
        public static List<BroadContent> LstBroadContent
        {
            get { return lstBroadContent.LstBroadContents; }
            set { lstBroadContent.LstBroadContents = value; }
        }

        /// <summary>
        /// 단일 BroadContent를 추가
        /// </summary>
        /// <param name="content"></param>
        public static void AddBroadContent(BroadContent content)
        {

        }

        /// <summary>
        /// 단일 BroadContent 제공
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BroadContent GetBroadContent(string key)
        {
            BroadContent rstBroadContent = new BroadContent();
            return rstBroadContent;
        }

        /// <summary>
        /// 단일 BroadContent를 삭제
        /// </summary>
        /// <param name="content"></param>
        public static void RemoveBroadContent(BroadContent content)
        {

        }

        /// <summary>
        /// 방송문안 로드
        /// </summary>
        public static void LoadBroadContents()
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BroadContentContainer));
                    lstBroadContent = (BroadContentContainer)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("BroadContentMng", "LoadBroadContents() Method", ex);
            }
        }

        /// <summary>
        /// 방송문안 저장
        /// </summary>
        public static void SaveBroadContents()
        {
            try
            {
                #region 임시 방송문안 정보 파일 생성
                #region 경계 경보
                //BroadContent broadContent = new BroadContent();
                //broadContent.Name = "경계 경보";

                //BroadText broadText = new BroadText();
                //broadText.Title = "주간";
                //broadText.LstText.Add("***** 경계 경보 (주간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 경계경보를 발령합니다.\n" +
                //    "현재시간 (우리나라 전역)에 실제 경계경보를 발령합니다.\n         (  ○ ○  지역)\n\n  - 경계경보 사이렌 음 ~\n\n 국민 여러분!\n" +
                //    "         여기는 민방위 재난통제본부입니다.\n실제 경계경보를 발령합니다.\n실제 경계경보를 발령합니다\n\n이 방송은 실제 상황입니다.\n" +
                //    "현재시간 (우리나라 전역)에  적기의 공습이 예상되므로\n         (  ○ ○  지역)\n\n 실제 경계경보를 발령합니다\n\n" +
                //    "국민 여러분께서는 방송을 들으면서 민방위 재난통제본부의\n지시에 따라 행동하시기 바랍니다.");
                //broadText.LstText.Add("○ 모든 행정기관에서는 비상 근무태세를 갖추고 자체경계를\n   강화 하시기 바라며, 경찰관서에서는 주민의 안전보호와\n" +
                //    "   교통통제를 강화하시기 바랍니다.\n\n○ 민방위대장은 대원을 지휘하여 민방위시설과 장비를 점검\n   하시기 바랍니다.\n\n" +
                //    "○ 국민 여러분께서는 즉시 대피할 준비를 하시고, 어린이와\n    노약자는 미리 대피시키기 바랍니다.\n\n○ 화생방공격에 대비하여 방독면 등 화생방 개인장비와\n " +
                //    "   대체활용 가능한 장비를 점검하시기 바랍니다.\n\n○ 대피하시기 전에 화재의 위험이 있는 유류와 가스는\n    안전한 곳으로 옮기고 전열기는 코드를 뽑아 두시기\n " +
                //    "   바랍니다.\n\n○ 극장, 운동장, 터미널, 백화점 등 사람이 많이 모인\n    곳에서는 영업을 중단하고, 손님들에게 경보내용을\n    알린 다음 순차적으로 대피시켜 주시기 바랍니다.\n\n" +
                //    "○ 운행중인 자동차는 대피할 준비를 하면서 천천히 운행하\n   시고 고가도로나 도심지 진입을 삼가시기 바랍니다.\n\n○ 이 방송은 실제 상황입니다.\n" +
                //    "   국민 여러분께서는 당황하지말고 질서있게 행동하시기\n    바랍니다.\n   이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //broadText = new BroadText();
                //broadText.Title = "야간";
                //broadText.LstText.Add("***** 경계 경보 (야간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 경계경보를 발령합니다.\n" +
                //    "현재시간 (우리나라 전역)에 실제 경계경보를 발령합니다.\n         (  ○ ○  지역)\n\n - 경계경보 사이렌 음 ~\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n" +
                //    "실제 경계경보를 발령합니다.\n실제 경계경보를 발령합니다\n\n이 방송은 실제 상황입니다.\n현재시간 (우리나라 전역)에  적기의 공습이 예상되므로\n         (  ○ ○  지역)\n\n" +
                //    "경계경보를 발령합니다\n\n국민 여러분께서는 방송을 들으면서 민방위 재난통제본부의\n지시에 따라 행동하시기 바랍니다.");
                //broadText.LstText.Add("○ 모든 가정과 직장에서는 즉시 대피할 준비를 하시고\n   어린이와 노약자는 미리 대피시킨 다음, 옥내·외의\n" +
                //    "   등은 모두 꺼 주시기 바랍니다.\n\n○ 극장, 운동장, 터미널, 백화점 등 사람이 많이 모인\n    곳에서는  영업을 중단하고, 손님들에게 경보내용을\n" +
                //    "   알린 다음 순차적으로 대피시키고 등을 모두 꺼 주시기\n   바랍니다.\n\n○ 응급 환자실, 중요산업시설 등 불가피한 곳에서는\n" +
                //    "   불빛이 밖으로 새어 나가지 않도록 완전히 가려 주시기\n   바랍니다.\n\n○ 운행중인 자동차는 불빛을 줄이고 대피할 준비를 하면서\n" +
                //    "   천천히 운행하시기 바랍니다.\n\n○ 민방위대장과 지도요원은 각 가정과 건물의 소등을\n    지도하여 주시기 바랍니다.\n\n" +
                //    "○ 화생방공격에 대비하여 방독면 등 화생방 개인보호장비나\n   대체 활용 가능한 장비를 점검하시기 바랍니다.\n\n" +
                //    "○ 이 방송은 실제 상황입니다.\n   국민여러분께서는 당황하지말고 질서있게 행동하시기\n   바랍니다.\n\n" +
                //    "   이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //lstBroadContent.LstBroadContents.Add(broadContent);
                #endregion

                #region 공습 경보
                //broadContent = new BroadContent();
                //broadContent.Name = "공습 경보";

                //broadText = new BroadText();
                //broadText.Title = "주간";
                //broadText.LstText.Add("***** 공습 경보 (주간) ******\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 공습경보를 발령합니다.\n현재시간 (우리나라 전역)에 실제 공습경보를 발령합니다.\n" +
                //    "         (  ○ ○  지역)\n\n  - 공습경보 사이렌 음 ~\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 공습경보를 발령합니다.\n실제 공습경보를 발령합니다\n\n" +
                //    "이 방송은 실제 상황입니다.\n\n현재시간 (우리나라 전역)에  실제 공습경보를 발령합니다.\n         (  ○ ○  지역)\n\n국민 여러분께서는 즉시 안전한 곳으로 대피하시고 방송을\n " +
                //    "들으면서 민방위 재난통제본부의 지시에 따라 행동하시기\n 바랍니다.");
                //broadText.LstText.Add("○ 국민 여러분께서는 지하대피소나 지형지물을 이용하여\n    신속하고 질서있게 대피하시기 바랍니다.\n\n ○ 극장, 운동장, 터미널, 백화점 등 사람이 많이 모인\n " +
                //    "   곳에서는 영업을 중단하고 손님을 대피시켜 주시기\n    바랍니다.\n\n○ 운행중인 자동차는 가까운 빈터나 도로 우측에 정차하여\n   승객을 안전한 곳으로 대피시켜 주시기 바랍니다.\n\n" +
                //    "○ 대피 하실때는 화생방공격에 대비하여 방독면 등\n    보호장비와 대체활용 가능한 장비를 착용하시고\n   대피하시기 바랍니다.\n\n○ 모든 행정기관에서는 비상 근무태세를 갖추고 자체경계를\n" +
                //    "   강화하시기 바랍니다.\n\n○ 대피장소에서는 질서를 지키고, 계속 방송을 들으면서\n    민방위 재난통제본부의 지시에 따라 주시기 바랍니다.\n\n  ○ 이 방송은 실제 상황입니다\n" +
                //    "   국민 여러분께서는 신속하고 질서있게 행동하시기\n    바랍니다.\n\n   이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //broadText = new BroadText();
                //broadText.Title = "야간";
                //broadText.LstText.Add("***** 공습 경보(야간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 공습경보를 발령합니다.\n현재시간 (우리나라 전역)에 실제 공습경보를 발령합니다.\n" +
                //    "         (  ○ ○  지역)\n\n  - 공습경보 사이렌 음 ~\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 공습경보를 발령합니다.\n실제 공습경보를 발령합니다\n\n" +
                //    "이 방송은 실제 상황입니다.\n\n현재시간 (우리나라 전역)에  실제 공습경보를 발령합니다.\n         (  ○ ○  지역)\n\n 국민 여러분께서는 즉시 옥내·외의 등을 모두 끈 다음,\n " +
                //    "안전한 곳으로 대피하시고 방송을 들으면서\n 민방위 재난통제본부의 지시에 따라 행동하시기 바랍니다.");
                //broadText.LstText.Add("○ 모든 가정과 직장에서는 지금 곧 옥내·외의 등을 모두\n   끈 다음 신속하고 질서있게 대피하시기 바랍니다.\n\n○ 극장, 운동장, 터미널, 백화점 등 사람이 많이 모인\n " +
                //    "   곳에서는 영업을 중단하고, 모든 등을 끈 다음 손님들을\n    대피시키기 바랍니다.\n\n○ 응급환자실, 중요산업시설 등 불가피한 곳에서는 불빛이\n   밖으로 새어 나가지 않도록 완전히 가려 주시기 바랍니다\n\n" +
                //    "   ○ 운행중인 자동차는 가까운 빈터나 도로 우측에 정차하여\n    등을 끈 다음, 승객을 대피시켜 주시기 바랍니다.\n\n○ 대피하실 때는 화생방공격에 대비하여 방독면 등\n    보호장비와 대체활용 가능한 장비를 착용하고\n " +
                //    "   대피하시기 바랍니다.\n\n ○ 민방위대장, 지도요원, 교통경찰관은 건물과 차량의\n    등을 모두 끄도록 하고, 주민대피 유도와 차량운행\n    통제에 임하시기 바랍니다.\n\n" +
                //    " ○ 이 방송은 실제 상황입니다.\n   국민 여러분께서는 당황하지말고 질서있게 행동하시기\n   바랍니다.\n\n   이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //lstBroadContent.LstBroadContents.Add(broadContent);
                #endregion

                #region 재차 경계 경보
                //broadContent = new BroadContent();
                //broadContent.Name = "재차 경계 경보";

                //broadText = new BroadText();
                //broadText.Title = "주간";
                //broadText.LstText.Add("***** 재차 경계경보(주간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 경계경보를 발령합니다.\n현재시간 (우리나라 전역)에 실제 경계경보를 발령합니다.\n" +
                //    "         (  ○ ○  지역)\n\n   - 경계경보 사이렌 음 ~\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n현재시간 (우리나라 전역)에 발령했던 실제 공습경보를\n          (  ○ ○  지역)\n  " +
                //    "경계경보로 바꾸어 발령합니다.\n\n현재시간 (우리나라 전역)에. 발령했던 실제 공습경보를\n          (  ○ ○  지역)\n  경계경보로 바꾸어 발령합니다\n\n이 방송은 실제 상황입니다.\n\n" +
                //    "국민 여러분께서는 계속 경계태세를 유지하면서 대피소에서\n 나와 사태수습에 참여하시기 바랍니다.\n\n이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //broadText = new BroadText();
                //broadText.Title = "야간";
                //broadText.LstText.Add("***** 재차 경계경보(야간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n실제 경계경보를 발령합니다.\n현재시간 (우리나라 전역)에 실제 경계경보를 발령합니다.\n" +
                //    "         (  ○ ○  지역)\n\n  - 경계경보 사이렌 음~\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n현재시간 (우리나라 전역)에 발령했던 실제 공습경보를\n         (  ○ ○  지역)\n  " +
                //    "경계경보로 바꾸어 발령합니다.\n\n현재시간 (우리나라 전역)에. 발령했던 실제 공습경보를\n          (  ○ ○  지역)\n  경계경보로 바꾸어 발령합니다\n\n이 방송은 실제 상황입니다.\n\n" +
                //    "국민 여러분께서는 계속 등화관제를 하면서 경계태세를\n 유지하여 주시기 바랍니다.\n\n이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //lstBroadContent.LstBroadContents.Add(broadContent);
                #endregion

                #region 경보 해제
                //broadContent = new BroadContent();
                //broadContent.Name = "경보 해제";

                //broadText = new BroadText();
                //broadText.Title = "주간";
                //broadText.LstText.Add("***** 경보 해제(주간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n\n현재시간 (우리나라 전역)에 발령했던 실제 경보를 해제\n" +
                //    "         (  ○ ○  지역)\n 합니다. 실제 경보를 해제합니다.\n\n 국민 여러분께서는 정상업무에 임하시기 바랍니다.\n\n-2회 반복방송 -\n\n이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //broadText = new BroadText();
                //broadText.Title = "야간";
                //broadText.LstText.Add("***** 경보 해제(야간) *****\n\n국민 여러분!\n여기는 민방위 재난통제본부입니다.\n\n현재시간 (우리나라 전역)에 발령했던 실제 경보를 해제\n" +
                //    "         (  ○ ○  지역)\n 합니다.   실제 경보를 해제합니다.\n\n 국민 여러분께서는 정상업무에 임하시기 바랍니다.\n\n- 2회 반복방송 -\n\n이상 민방위 재난통제본부에서 알려드렸습니다.");
                //broadContent.LstBroadText.Add(broadText);

                //lstBroadContent.LstBroadContents.Add(broadContent);
                #endregion
                #endregion

                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(BroadContentContainer));
                    ser.Serialize(stream, lstBroadContent);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                NCasLoggingMng.ILoggingException.WriteException("BroadContentMng", "SaveBroadContents() Method", ex);
            }
        }
    }
}