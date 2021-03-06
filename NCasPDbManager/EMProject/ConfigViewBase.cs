///////////////////////////////////////////////////////////
//  ConfigViewBase.cs
//  Implementation of the Class ConfigViewBase
//  Generated by Enterprise Architect
//  Created on:      17-4-2015 오후 8:11:17
//  Original author: ahnyj ahnyj
///////////////////////////////////////////////////////////




using NCasPDbManager;
namespace NCasPDbManager.ConfigContainers.ConfigViewContainers {
	/// <summary>
	/// Config화면의 최상위
	/// </summary>
	public class ConfigViewBase {

		/// <summary>
		/// 컨트롤 탭이 어떤 탭인지 설정한다.
		/// </summary>
		private ConfigViewKind configViewKind = ConfigViewKind.None;

		public ConfigViewBase(){

		}

		~ConfigViewBase(){

		}

		/// <summary>
		/// 초기화 작업을 한다.
		/// </summary>
		public void Init(){

			//InitControl();


		}

		/// <summary>
		/// 환경설정 화면 컨트롤에 정보를 표시한다.
		/// </summary>
		private void InitControl(){

		}

		/// <summary>
		/// 변경된 내용을 저장한다.
		/// </summary>
		public void SaveConfig(){

			////ConfigMng에 값 대입
			//ConfigMng.SaveConfig();


		}

	}//end ConfigViewBase

}//end namespace ConfigViewContainers