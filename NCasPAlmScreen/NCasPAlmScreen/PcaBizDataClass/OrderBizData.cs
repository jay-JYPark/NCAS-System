﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCASBIZ.NCasType;
using NCASBIZ.NCasProtocol;
using NCASBIZ.NCasDefine;
using NCASFND.NCasLogging;
using NCasMsgCommon.Tts;
using NCasMsgCommon.Std;
using NCasContentsModule.TTS;
using NCasContentsModule.StoMsg;

namespace NCasPAlmScreen
{
    public class OrderBizData : NCasObject
    {
        private NCasProtocolTc1 almProtocol = null;
        private bool allDestinationFlag = false;
        private bool orderGroupFlag = false;
        private bool orderDistFlag = false;
        private bool orderTermFlag = false;
        private PAlmScreenUIController.DisasterBroadKind selectedDisasterBroadKind = PAlmScreenUIController.DisasterBroadKind.None;
        private NCasDefineOrderKind lastOrderKind = NCasDefineOrderKind.None;
        private bool isLocal = true;
        private PAlmScreenUIController.OrderDataSendStatus isEnd = PAlmScreenUIController.OrderDataSendStatus.None;
        private bool ttsOrderFlag = false;
        private StoredMessageText selectedStoredMessage = new StoredMessageText();
        private int storedMessageRepeatCount = 1;
        private TtsMessageText selectedTtsMessage = new TtsMessageText();
        private List<string> groupName = new List<string>();
        private byte[] sendBuff = null;

        /// <summary>
        /// 경보발령 TC 프로퍼티
        /// </summary>
        public NCasProtocolTc1 AlmProtocol
        {
            get { return this.almProtocol; }
            set { this.almProtocol = value; }
        }

        /// <summary>
        /// 시도전체 발령 프로퍼티
        /// </summary>
        public bool AllDestinationFlag
        {
            get { return this.allDestinationFlag; }
            set { this.allDestinationFlag = value; }
        }

        /// <summary>
        /// 그룹 발령 프로퍼티
        /// </summary>
        public bool OrderGroupFlag
        {
            get { return this.orderGroupFlag; }
            set { this.orderGroupFlag = value; }
        }

        /// <summary>
        /// 시군 발령 프로퍼티
        /// </summary>
        public bool OrderDistFlag
        {
            get { return this.orderDistFlag; }
            set { this.orderDistFlag = value; }
        }

        /// <summary>
        /// 단말개별 발령 프로퍼티
        /// </summary>
        public bool OrderTermFlag
        {
            get { return this.orderTermFlag; }
            set { this.orderTermFlag = value; }
        }

        /// <summary>
        /// 재난경계 발령종류 프로퍼티
        /// </summary>
        public PAlmScreenUIController.DisasterBroadKind SelectedDisasterBroadKind
        {
            get { return this.selectedDisasterBroadKind; }
            set { this.selectedDisasterBroadKind = value; }
        }

        /// <summary>
        /// 마지막 발령 종류 프로퍼티
        /// </summary>
        public NCasDefineOrderKind LastOrderKind
        {
            get { return this.lastOrderKind; }
            set { this.lastOrderKind = value; }
        }

        /// <summary>
        /// 로컬발령인지 리모트발령인지 구분하기 위한 프로퍼티
        /// </summary>
        public bool IsLocal
        {
            get { return this.isLocal; }
            set { this.isLocal = value; }
        }

        /// <summary>
        /// 복수의 발령데이터를 전송하는 경우 마지막 발령데이터인지를 구분하기 위한 프로퍼티
        /// </summary>
        public PAlmScreenUIController.OrderDataSendStatus IsEnd
        {
            get { return this.isEnd; }
            set { this.isEnd = value; }
        }

        /// <summary>
        /// 가장 마지막 발령이 TTS발령이었는지를 구분하기 위한 프로퍼티
        /// </summary>
        public bool TtsOrderFlag
        {
            get { return this.ttsOrderFlag; }
            set { this.ttsOrderFlag = value; }
        }

        /// <summary>
        /// 선택한 저장메시지 정보
        /// </summary>
        public StoredMessageText SelectedStoredMessage
        {
            get { return this.selectedStoredMessage; }
            set { this.selectedStoredMessage = value; }
        }

        /// <summary>
        /// 선택한 저장메시지 반복횟수
        /// </summary>
        public int StoredMessageRepeatCount
        {
            get { return this.storedMessageRepeatCount; }
            set { this.storedMessageRepeatCount = value; }
        }

        /// <summary>
        /// 선택한 TTS메시지 정보
        /// </summary>
        public TtsMessageText SelectedTtsMessage
        {
            get { return this.selectedTtsMessage; }
            set { this.selectedTtsMessage = value; }
        }

        /// <summary>
        /// 그룹 정보
        /// </summary>
        public List<string> GroupName
        {
            get { return this.groupName; }
            set { this.groupName = value; }
        }

        /// <summary>
        /// 데이터
        /// </summary>
        public byte[] SendBuff
        {
            get { return this.sendBuff; }
            set { this.sendBuff = value; }
        }

        public override void CloneFrom(NCasObject obj)
        {
            OrderBizData newObject = obj as OrderBizData;
            this.almProtocol = newObject.almProtocol;
            this.allDestinationFlag = newObject.allDestinationFlag;
            this.orderGroupFlag = newObject.orderGroupFlag;
            this.orderDistFlag = newObject.orderDistFlag;
            this.orderTermFlag = newObject.orderTermFlag;
            this.selectedDisasterBroadKind = newObject.selectedDisasterBroadKind;
            this.lastOrderKind = newObject.lastOrderKind;
            this.isLocal = newObject.isLocal;
            this.isEnd = newObject.isEnd;
            this.ttsOrderFlag = newObject.ttsOrderFlag;
            this.selectedStoredMessage = newObject.selectedStoredMessage;
            this.storedMessageRepeatCount = newObject.storedMessageRepeatCount;
            this.selectedTtsMessage = newObject.selectedTtsMessage;
            this.groupName = newObject.groupName;
            this.sendBuff = newObject.sendBuff;
        }

        public override NCasObject CloneTo()
        {
            OrderBizData newObject = new OrderBizData();
            newObject.almProtocol = this.almProtocol;
            newObject.allDestinationFlag = this.allDestinationFlag;
            newObject.orderGroupFlag = this.orderGroupFlag;
            newObject.orderDistFlag = this.orderDistFlag;
            newObject.orderTermFlag = this.orderTermFlag;
            newObject.selectedDisasterBroadKind = this.selectedDisasterBroadKind;
            newObject.lastOrderKind = this.lastOrderKind;
            newObject.isLocal = this.isLocal;
            newObject.isEnd = this.isEnd;
            newObject.ttsOrderFlag = this.ttsOrderFlag;
            newObject.selectedStoredMessage = this.selectedStoredMessage;
            newObject.storedMessageRepeatCount = this.storedMessageRepeatCount;
            newObject.selectedTtsMessage = this.selectedTtsMessage;
            newObject.groupName = this.groupName;
            newObject.sendBuff = this.sendBuff;
            return newObject;
        }
    }
}