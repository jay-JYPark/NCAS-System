///////////////////////////////////////////////////////////
//  ConfigMng.cs
//  Implementation of the Class ConfigMng
//  Generated by Enterprise Architect
//  Created on:      17-4-2015 오후 4:59:59
//  Original author: ahnyj ahnyj
///////////////////////////////////////////////////////////




using NCasPDbManager.ConfigContainers.ConfigMngContainers;
namespace NCasPDbManager {
	/// <summary>
	/// Config관리자
	/// </summary>
	public class ConfigMng {

		/// <summary>
		/// 환경설정 값이 들어 있음
		/// </summary>
		private static NCasDmSerializableConfig cofnigNCasDmSerializableConfig = new NCasDmSerializableConfig;
		/// <summary>
		/// 환경설정 저장경로
		/// </summary>
		private static string filePath = NCasEnvironmentMng.NCasAppEnvPath;
		public NCasPDbManager.ConfigContainers.ConfigMngContainers.NCasDmSerializableConfig m_NCasDmSerializableConfig;

		public ConfigMng(){

		}

		~ConfigMng(){

		}

		/// <summary>
		/// 초기화 함수
		/// </summary>
		public static void Init(){

			//LoadConfig();


		}

		/// <summary>
		/// Config정보를 불러온다.
		/// </summary>
		private static void LoadConfig(){

			//XmlSerializer...


		}

		/// <summary>
		/// DB연결 IP
		/// </summary>
		public static string LocalDbServerIp(){

			return "";
		}

		/// <summary>
		/// DB연결 서비스네임
		/// </summary>
		public static string LocalDbServerSid(){

			return "";
		}

		/// <summary>
		/// DB연결 사용자 PW
		/// </summary>
		public static string LocalDbServerPw(){

			return "";
		}

		/// <summary>
		/// Tcp연결에 필요한 Profile정보를 담고있는 리스트
		/// </summary>
		public static List<TcpProfileData> LstTcpProfileDatas(){

			return null;
		}

		/// <summary>
		/// UDP수신 IP
		/// </summary>
		public static string UdpIp(){

			return "";
		}

		/// <summary>
		/// UDP수신 Port
		/// </summary>
		public static int UdpPort(){

			return 0;
		}

		/// <summary>
		/// DB연결 사용자ID
		/// </summary>
		public static string LocalDbServerUserId(){

			return "";
		}

		/// <summary>
		/// 환경설정값을 저장한다.
		/// </summary>
		public static bool SaveConfig(){

			//XmlSerializer...


			return false;
		}

	}//end ConfigMng

}//end namespace NCasPDbManager