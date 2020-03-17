using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCASBIZ.NCasType;

namespace NCasPDbManager
{
    public class MasterInfoData : NCasObject
    {
        #region Fields
        /// <summary>
        /// 장비코드
        /// </summary>
        private int code;
        /// <summary>
        /// 일련번호
        /// </summary>
        private int serialNo;
        /// <summary>
        /// 장비명
        /// </summary>
        private string name = string.Empty;
        /// <summary>
        /// 장비IP
        /// </summary>
        private string netId = string.Empty;
        /// <summary>
        /// 서브마스크
        /// </summary>
        private string subMask = string.Empty;
        /// <summary>
        /// 장비구분
        /// </summary>
        private int masterType;
        /// <summary>
        /// 소속상위지역코드
        /// </summary>
        private int parentCode;
        /// <summary>
        /// 소속분배소코드
        /// </summary>
        private int reptCode;
        /// <summary>
        /// 소속시도코드
        /// </summary>
        private int provCode;
        /// <summary>
        /// 소속시도명
        /// </summary>
        private string provName = string.Empty;
        /// <summary>
        /// 소속시군구코드
        /// </summary>
        private int distCode;
        /// <summary>
        /// 소속시군구명
        /// </summary>
        private string distName = string.Empty;
        /// <summary>
        /// 장비코드
        /// </summary>
        private int termCode;
        /// <summary>
        /// 장비명
        /// </summary>
        private string termName = string.Empty;
        /// <summary>
        /// 위성수신기사용여부
        /// </summary>
        private int sateFlag;
        /// <summary>
        /// 분배소소속여부
        /// </summary>
        private int reptFlag;
        /// <summary>
        /// 장비ID
        /// </summary>
        private int devId;
        /// <summary>
        /// 방송ID
        /// </summary>
        private int broadId;
        /// <summary>
        /// 방송IP
        /// </summary>
        private string broadNetId = string.Empty;
        /// <summary>
        /// 방송서브넷마스크
        /// </summary>
        private string broadSubMask = string.Empty;
        /// <summary>
        /// 주요기관IP
        /// </summary>
        private string deptNetId = string.Empty;
        /// <summary>
        /// 주요기관서브넷마스크
        /// </summary>
        private string deptSubMask = string.Empty;
        /// <summary>
        /// 분배소ID
        /// </summary>
        private string reptNetId = string.Empty;
        /// <summary>
        /// 분배소서브넷마스크
        /// </summary>
        private string reptSubMask = string.Empty;
        /// <summary>
        /// 음성회선번호
        /// </summary>
        private string voiceLineNo = string.Empty;
        /// <summary>
        /// 장비사용여부
        /// </summary>
        private int useFlag;
        private int oldSysFlag;
        private int termFlag;
        /// <summary>
        /// DSU셀번호
        /// </summary>
        private int dsuShellNo;
        /// <summary>
        /// DSU카드번호
        /// </summary>
        private int dsuCardNo;
        /// <summary>
        /// uint타입의 장비Ip
        /// </summary>
        private uint netIdToUint;
        #endregion

        #region Properties
        /// <summary>
        /// 장비코드
        /// </summary>
        public int Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }
        /// <summary>
        /// 일련번호
        /// </summary>
        public int SerialNo
        {
            get
            {
                return serialNo;
            }
            set
            {
                serialNo = value;
            }
        }
        /// <summary>
        /// 장비명
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// 장비IP
        /// </summary>
        public string NetId
        {
            get
            {
                return netId;
            }
            set
            {
                netId = value;
            }
        }

        /// <summary>
        /// 서브마스크
        /// </summary>
        public string SubMask
        {
            get
            {
                return subMask;
            }
            set
            {
                subMask = value;
            }
        }

        /// <summary>
        /// 장비구분
        /// </summary>
        public int MasterType
        {
            get
            {
                return masterType;
            }
            set
            {
                masterType = value;
            }
        }

        /// <summary>
        /// 소속상위지역코드
        /// </summary>
        public int ParentCode
        {
            get
            {
                return parentCode;
            }
            set
            {
                parentCode = value;
            }
        }

        /// <summary>
        /// 소속분배소코드
        /// </summary>
        public int ReptCode
        {
            get
            {
                return reptCode;
            }
            set
            {
                reptCode = value;
            }
        }

        /// <summary>
        /// 소속시도코드
        /// </summary>
        public int ProvCode
        {
            get
            {
                return provCode;
            }
            set
            {
                provCode = value;
            }
        }

        /// <summary>
        /// 소속시도명
        /// </summary>
        public string ProvName
        {
            get
            {
                return provName;
            }
            set
            {
                provName = value;
            }
        }

        /// <summary>
        /// 소속시군구코드
        /// </summary>
        public int DistCode
        {
            get
            {
                return distCode;
            }
            set
            {
                distCode = value;
            }
        }

        /// <summary>
        /// 소속시군구명
        /// </summary>
        public string DistName
        {
            get
            {
                return distName;
            }
            set
            {
                distName = value;
            }
        }

        /// <summary>
        /// 장비코드
        /// </summary>
        public int TermCode
        {
            get
            {
                return termCode;
            }
            set
            {
                termCode = value;
            }
        }

        /// <summary>
        /// 장비명
        /// </summary>
        public string TermName
        {
            get
            {
                return termName;
            }
            set
            {
                termName = value;
            }
        }

        /// <summary>
        /// 위성수신기사용여부
        /// </summary>
        public int SateFlag
        {
            get
            {
                return sateFlag;
            }
            set
            {
                sateFlag = value;
            }
        }

        /// <summary>
        /// 분배소소속여부
        /// </summary>
        public int ReptFlag
        {
            get
            {
                return reptFlag;
            }
            set
            {
                reptFlag = value;
            }
        }

        /// <summary>
        /// 장비ID
        /// </summary>
        public int DevId
        {
            get
            {
                return devId;
            }
            set
            {
                devId = value;
            }
        }

        /// <summary>
        /// 방송ID
        /// </summary>
        public int BroadId
        {
            get
            {
                return broadId;
            }
            set
            {
                broadId = value;
            }
        }

        /// <summary>
        /// 방송IP
        /// </summary>
        public string BroadNetId
        {
            get
            {
                return broadNetId;
            }
            set
            {
                broadNetId = value;
            }
        }

        /// <summary>
        /// 방송서브넷마스크
        /// </summary>
        public string BroadSubMask
        {
            get
            {
                return broadSubMask;
            }
            set
            {
                broadSubMask = value;
            }
        }

        /// <summary>
        /// 주요기관IP
        /// </summary>
        public string DeptNetId
        {
            get
            {
                return deptNetId;
            }
            set
            {
                deptNetId = value;
            }
        }

        /// <summary>
        /// 주요기관서브넷마스크
        /// </summary>
        public string DeptSubMask
        {
            get
            {
                return deptSubMask;
            }
            set
            {
                deptSubMask = value;
            }
        }

        /// <summary>
        /// 분배소ID
        /// </summary>
        public string ReptNetId
        {
            get
            {
                return reptNetId;
            }
            set
            {
                reptNetId = value;
            }
        }

        /// <summary>
        /// 분배소서브넷마스크
        /// </summary>
        public string ReptSubMask
        {
            get
            {
                return reptSubMask;
            }
            set
            {
                reptSubMask = value;
            }
        }

        /// <summary>
        /// 음성회선번호
        /// </summary>
        public string VoiceLineNo
        {
            get
            {
                return voiceLineNo;
            }
            set
            {
                voiceLineNo = value;
            }
        }

        /// <summary>
        /// 장비사용여부
        /// </summary>
        public int UseFlag
        {
            get
            {
                return useFlag;
            }
            set
            {
                useFlag = value;
            }
        }

        public int OldSysFlag
        {
            get
            {
                return oldSysFlag;
            }
            set
            {
                oldSysFlag = value;
            }
        }

        public int TermFlag
        {
            get
            {
                return termFlag;
            }
            set
            {
                termFlag = value;
            }
        }

        /// <summary>
        /// DSU셀번호
        /// </summary>
        public int DsuShellNo
        {
            get
            {
                return dsuShellNo;
            }
            set
            {
                dsuShellNo = value;
            }
        }

        /// <summary>
        /// DSU카드번호
        /// </summary>
        public int DsuCardNo
        {
            get
            {
                return dsuCardNo;
            }
            set
            {
                dsuCardNo = value;
            }
        }

        /// <summary>
        /// uint타입의 장비Ip
        /// </summary>
        public uint NetIdToUint
        {
            get
            {
                return netIdToUint;
            }
            set
            {
                netIdToUint = value;
            }
        }
        #endregion

        public override void CloneFrom(NCasObject obj)
        {
            MasterInfoData newObject = obj as MasterInfoData;
            this.broadId = newObject.broadId;
            this.broadNetId = newObject.broadNetId;
            this.broadSubMask = newObject.broadSubMask;
            this.code = newObject.code;
            this.deptNetId = newObject.deptNetId;
            this.deptSubMask = newObject.deptSubMask;
            this.devId = newObject.devId;
            this.distCode = newObject.distCode;
            this.distName = newObject.distName;
            this.dsuCardNo = newObject.dsuCardNo;
            this.dsuShellNo = newObject.dsuShellNo;
            this.masterType = newObject.masterType;
            this.name = newObject.name;
            this.netId = newObject.netId;
            this.netIdToUint = newObject.netIdToUint;
            this.oldSysFlag = newObject.oldSysFlag;
            this.parentCode = newObject.parentCode;
            this.provCode = newObject.provCode;
            this.provName = newObject.provName;
            this.reptCode = newObject.reptCode;
            this.reptFlag = newObject.reptFlag;
            this.reptNetId = newObject.reptNetId;
            this.reptSubMask = newObject.reptSubMask;
            this.sateFlag = newObject.sateFlag;
            this.serialNo = newObject.serialNo;
            this.subMask = newObject.subMask;
            this.termCode = newObject.termCode;
            this.termFlag = newObject.termFlag;
            this.termName = newObject.termName;
            this.useFlag = newObject.useFlag;
            this.voiceLineNo = newObject.voiceLineNo;
        }

        public override NCasObject CloneTo()
        {
            MasterInfoData newObject = new MasterInfoData();
            newObject.broadId = this.broadId;
            newObject.broadNetId = this.broadNetId;
            newObject.broadSubMask = this.broadSubMask;
            newObject.code = this.code;
            newObject.deptNetId = this.deptNetId;
            newObject.deptSubMask = this.deptSubMask;
            newObject.devId = this.devId;
            newObject.distCode = this.distCode;
            newObject.distName = this.distName;
            newObject.dsuCardNo = this.dsuCardNo;
            newObject.dsuShellNo = this.dsuShellNo;
            newObject.masterType = this.masterType;
            newObject.name = this.name;
            newObject.netId = this.netId;
            newObject.netIdToUint = this.netIdToUint;
            newObject.oldSysFlag = this.oldSysFlag;
            newObject.parentCode = this.parentCode;
            newObject.provCode = this.provCode;
            newObject.provName = this.provName;
            newObject.reptCode = this.reptCode;
            newObject.reptFlag = this.reptFlag;
            newObject.reptNetId = this.reptNetId;
            newObject.reptSubMask = this.reptSubMask;
            newObject.sateFlag = this.sateFlag;
            newObject.serialNo = this.serialNo;
            newObject.subMask = this.subMask;
            newObject.termCode = this.termCode;
            newObject.termFlag = this.termFlag;
            newObject.termName = this.termName;
            newObject.useFlag = this.useFlag;
            newObject.voiceLineNo = this.voiceLineNo;

            return newObject;
        }
    }
}
