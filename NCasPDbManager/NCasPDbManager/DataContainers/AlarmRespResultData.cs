using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasType;

namespace NCasPDbManager
{
    public class AlarmRespResultData : NCasObject
    {
        #region Fields
        /// <summary>
        /// 응답/결과 시각
        /// </summary>
        private DateTime respResultTime;
        /// <summary>
        /// 발령대상지역
        /// </summary>
        private int areaCode;
        private int broadCtrlFlag;
        /// <summary>
        /// 발령구분
        /// </summary>
        private int section;
        /// <summary>
        /// 경보종류
        /// </summary>
        private int alarmKind;
        /// <summary>
        /// 발령매체
        /// </summary>
        private int media;
        /// <summary>
        /// 단말코드
        /// </summary>
        private int devCode;
        /// <summary>
        /// 단말종류
        /// </summary>
        private int devKind;
        /// <summary>
        /// 발령원
        /// </summary>
        private int source;
        /// <summary>
        /// 응답/결과 여부
        /// </summary>
        private int respResultFlag;
        #endregion

        #region Properties
        /// <summary>
        /// 응답/결과 시각
        /// </summary>
        public DateTime RespResultTime
        {
            get
            {
                return respResultTime;
            }
            set
            {
                respResultTime = value;
            }
        }

        /// <summary>
        /// 발령대상지역
        /// </summary>
        public int AreaCode
        {
            get
            {
                return areaCode;
            }
            set
            {
                areaCode = value;
            }
        }

        public int BroadCtrlFlag
        {
            get
            {
                return broadCtrlFlag;
            }
            set
            {
                broadCtrlFlag = value;
            }
        }

        /// <summary>
        /// 발령구분
        /// </summary>
        public int Section
        {
            get
            {
                return section;
            }
            set
            {
                section = value;
            }
        }

        /// <summary>
        /// 경보종류
        /// </summary>
        public int AlarmKind
        {
            get
            {
                return alarmKind;
            }
            set
            {
                alarmKind = value;
            }
        }

        /// <summary>
        /// 발령매체
        /// </summary>
        public int Media
        {
            get
            {
                return media;
            }
            set
            {
                media = value;
            }
        }

        /// <summary>
        /// 단말코드
        /// </summary>
        public int DevCode
        {
            get
            {
                return devCode;
            }
            set
            {
                devCode = value;
            }
        }

        /// <summary>
        /// 단말종류
        /// </summary>
        public int DevKind
        {
            get
            {
                return devKind;
            }
            set
            {
                devKind = value;
            }
        }

        /// <summary>
        /// 발령원
        /// </summary>
        public int Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        /// <summary>
        /// 응답/결과 여부
        /// </summary>
        public int RespResultFlag
        {
            get
            {
                return respResultFlag;
            }
            set
            {
                respResultFlag = value;
            }
        }
        #endregion

        public override void CloneFrom(NCasObject obj)
        {
            AlarmRespResultData newObject = obj as AlarmRespResultData;
            this.alarmKind = newObject.alarmKind;
            this.areaCode = newObject.areaCode;
            this.broadCtrlFlag = newObject.broadCtrlFlag;
            this.devCode = newObject.devCode;
            this.devKind = newObject.devKind;
            this.media = newObject.media;
            this.respResultFlag = newObject.respResultFlag;
            this.respResultTime = newObject.respResultTime;
            this.section = newObject.section;
            this.source = newObject.source;
        }

        public override NCasObject CloneTo()
        {
            AlarmRespResultData newObject = new AlarmRespResultData();
            newObject.alarmKind = this.alarmKind;
            newObject.areaCode = this.areaCode;
            newObject.broadCtrlFlag = this.broadCtrlFlag;
            newObject.devCode = this.devCode;
            newObject.devKind = this.devKind;
            newObject.media = this.media;
            newObject.respResultFlag = this.respResultFlag;
            newObject.respResultTime = this.respResultTime;
            newObject.section = this.section;
            newObject.source = this.source;

            return newObject;
        }
    }
}
