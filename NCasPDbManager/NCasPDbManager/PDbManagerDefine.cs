using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCasPDbManager
{
    public delegate void InvokeVoid();
    public delegate void InvokeVoidOne<T>(T valueT);
    public delegate void InvokeVoidTwo<T, U>(T valueT, U valueU);
    /// <summary>
    /// Config화면에서 각 화면의 Kind를 정의한다.
    /// </summary>
    public enum ConfigViewKind : int
    {
        /// <summary>
        /// 없음
        /// </summary>
        None = 0,
        /// <summary>
        /// 데이터베이스 접속 설정
        /// </summary>
        dbConfig = 1,
        /// <summary>
        /// TCP 프로파일 설정 화면
        /// </summary>
        tcpConfig = 2,
        /// <summary>
        /// UDP설정 화면
        /// </summary>
        udpConfig = 3
    }

    /// <summary>
    /// 로그인이력에서 침입인지에 대한 설정을 한다.
    /// </summary>
    public enum LoginStatus : int
    {
        None = 0,
        Trespass = 1,
        Login = 2,
        BeLogin = 3,
        BeNotLogin = 4
    }

    public enum AlarmAnalogDigitalPointInfo : int
    {
        DevRoomFireStatus = 1,
        DevRoomDoorStatus = 2,
        BattRoomFireStatus = 3,
        BattRoomDoorStatus = 4,
        DoorStatus = 5,
        AcFailStatus = 6,
        Psu1Status = 7,
        Psu2Status = 8,
        MonitorPsu1Status = 9,
        MonitorPsu2Status = 10,
        SirenPowerStatus = 11,
        RackFanStatus = 12,
        CaseFanStatus = 13,
        FanControlMode = 14,
        FrontPanelKey = 16,
        FrontPanelStatus = 17,
        LocalBroad1 = 18,
        LocalBroad2 = 19,
        ManulOperSwitch = 20,
        Reset = 21,
        Mcu1Status = 22,
        Mcu2Status = 23,
        DevRoomFanStatus = 24,
        BattRoomFanStatus = 25,

        SatelliteStatus = 34,
        SirenAmpPowerSwitch = 35,
        TermDsuReset = 36,
        TermAmpSetStatus = 37,
        
        DevRoomTempStatus = 26,
        DevRoomHumiStatus = 27,
        BattRoomTempStatus = 28,
        BattRoomHumiStatus = 29,
        AcStatus = 30,
        DcStatus = 31,
        SirenAcStatus = 32,
        SirenBattStatus = 33
    }

    public enum BroadAnalogDigitalPointInfo : int
    {
        Reset = 1,
        Mcu1Status = 2,
        Mcu2Status = 3,
        Psu1Status = 4,
        Psu2Status = 5,
        AcFailStatus = 6,
        RackFanStatus = 9,
        CaseFanStatus = 10,
        DoorStatus = 13,
        SatelliteStatus = 15
    }

    public enum PointType : int
    {
        Digital = 1,
        Analog = 2
    }

    public enum AnalogPointValue : int
    {
        Normal = 0,
        Low = 1,
        High = 2
    }

    public enum DigitalPointValue : int
    {
        Normal = 0,
        AbNormal = 1,
        Active = 2
    }
}
