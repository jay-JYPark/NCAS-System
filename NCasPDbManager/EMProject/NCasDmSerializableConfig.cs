///////////////////////////////////////////////////////////
//  NCasDmSerializableConfig.cs
//  Implementation of the Class NCasDmSerializableConfig
//  Generated by Enterprise Architect
//  Created on:      17-4-2015 오후 5:00:42
//  Original author: ahnyj ahnyj
///////////////////////////////////////////////////////////




using NCasPDbManager.ConfigContainers.ConfigMngContainers;
namespace NCasPDbManager.ConfigContainers.ConfigMngContainers {
	/// <summary>
	/// 환경설정 데이터
	/// </summary>
	public class NCasDmSerializableConfig {

		/// <summary>
		/// DB서버IP
		/// </summary>
		private string localDbServerIp = string.Empty;
		/// <summary>
		/// DB 서비스명(SID)
		/// </summary>
		private string localDbServerSid = string.Empty;
		/// <summary>
		/// DB접속 사용자 ID
		/// </summary>
		private string localDbServerUserId = string.Empty;
		/// <summary>
		/// DB서버 접속 사용자 PW
		/// </summary>
		private string localDbServerPw = string.Empty;
		/// <summary>
		/// UDP 수신 IP
		/// </summary>
		private string udpIp = string.Empty;
		/// <summary>
		/// UDP수신 포트
		/// </summary>
		private int udpPort;
		/// <summary>
		/// TCP설정에 필요한 TcpProfileData데이터
		/// </summary>
		private List<TcpProfileData> lstTcpProfileDatas = null;

		public NCasDmSerializableConfig(){

		}

		~NCasDmSerializableConfig(){

		}

		/// <summary>
		/// DB서버IP
		/// </summary>
		public string LocalDbServerIp{
			get{
				return localDbServerIp;
			}
			set{
				localDbServerIp = value;
			}
		}

		/// <summary>
		/// DB 서비스명(SID)
		/// </summary>
		public string LocalDbServerSid{
			get{
				return localDbServerSid;
			}
			set{
				localDbServerSid = value;
			}
		}

		/// <summary>
		/// DB접속 사용자 ID
		/// </summary>
		public string LocalDbServerUserId{
			get{
				return localDbServerUserId;
			}
			set{
				localDbServerUserId = value;
			}
		}

		/// <summary>
		/// DB서버 접속 사용자 PW
		/// </summary>
		public string LocalDbServerPw{
			get{
				return localDbServerPw;
			}
			set{
				localDbServerPw = value;
			}
		}

		/// <summary>
		/// UDP 수신 IP
		/// </summary>
		public string UdpIp{
			get{
				return udpIp;
			}
			set{
				udpIp = value;
			}
		}

		/// <summary>
		/// UDP수신 포트
		/// </summary>
		public int UdpPort{
			get{
				return udpPort;
			}
			set{
				udpPort = value;
			}
		}

		/// <summary>
		/// TCP설정에 필요한 TcpProfileData데이터
		/// </summary>
		public List<TcpProfileData> LstTcpProfileDatas{
			get{
				return lstTcpProfileDatas;
			}
			set{
				lstTcpProfileDatas = value;
			}
		}

	}//end NCasDmSerializableConfig

}//end namespace ConfigMngContainers