///////////////////////////////////////////////////////////
//  ConfigForm.cs
//  Implementation of the Class ConfigForm
//  Generated by Enterprise Architect
//  Created on:      17-4-2015 오후 5:21:15
//  Original author: ahnyj ahnyj
///////////////////////////////////////////////////////////




using NCasPDbManager.ConfigContainers.ConfigViewContainers;
using NCasPDbManager;
namespace NCasPDbManager.ConfigContainers.ConfigViewContainers {
	/// <summary>
	/// 환경설정 창
	/// </summary>
	public class ConfigForm {

		/// <summary>
		/// 환경설정 화면을 담는 딕셔너리
		/// </summary>
		private Dictionary<ConfigViewKind, ConfigViewBase> dicConfigViewMng = new Dictionary<ConfigViewKind, ConfigViewBase>();
		public NCasPDbManager.ConfigContainers.ConfigViewContainers.ConfigViewBase m_ConfigViewBase;

		public ConfigForm(){

		}

		~ConfigForm(){

		}

		/// <summary>
		/// 저장버튼을 누르면 해당 화면의 데이터를 저장한다.
		/// 저장버튼 이벤트 시 해당 함수 호출
		/// </summary>
		/// <param name="viewKind"></param>
		private void SaveConfig(ConfigViewKind viewKind){

			//selectConfig.SaveConfig();


		}

		/// <summary>
		/// 종료
		/// </summary>
		/// <param name="e"></param>
		protected void OnClosing(CancelEventArgs e){

		}

		/// <summary>
		/// 넘어온 ConfigViewKind로 해당하는 Kind의 화면을 띄워준다.
		/// </summary>
		/// <param name="configViewKind"></param>
		private void OpenConfigView(ConfigViewKind configViewKind){

		}

		/// <summary>
		/// 환경설정창을 생성하여 딕셔너리에 저장한다.
		/// 초기에 DBConfig 화면이 보여지도록 한다.
		/// </summary>
		private void LoadCofnigView(){

		}

		/// <summary>
		/// 초기화 작업
		/// 모든 탭의 창을 생성한다.
		/// </summary>
		/// <param name="e"></param>
		protected void OnLoad(EventArgs e){

			//ConfigMng.LoadConfig();
			//LoadConfigView();


		}

	}//end ConfigForm

}//end namespace ConfigViewContainers