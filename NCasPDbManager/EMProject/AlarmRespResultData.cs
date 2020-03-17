///////////////////////////////////////////////////////////
//  AlarmRespResultData.cs
//  Implementation of the Class AlarmRespResultData
//  Generated by Enterprise Architect
//  Created on:      21-4-2015 ���� 10:55:51
//  Original author: ahnyj ahnyj
///////////////////////////////////////////////////////////




using Business.NCasType;
namespace NCasPDbManager.DataContainers {
	/// <summary>
	/// �̸��ö�� ���������� ��� ������ Ŭ����
	/// </summary>
	public class AlarmRespResultData : NCasObject {

		/// <summary>
		/// ����/��� �ð�
		/// </summary>
		private DateTime respResultTime = null;
		/// <summary>
		/// �߷ɴ������
		/// </summary>
		private int areaCode;
		private int broadCtrlFlag;
		/// <summary>
		/// �߷ɱ���
		/// </summary>
		private int section;
		/// <summary>
		/// �溸����
		/// </summary>
		private int alarmKind;
		/// <summary>
		/// �߷ɸ�ü
		/// </summary>
		private int media;
		/// <summary>
		/// �ܸ��ڵ�
		/// </summary>
		private int devCode;
		/// <summary>
		/// �ܸ�����
		/// </summary>
		private int devKind;
		/// <summary>
		/// �߷ɿ�
		/// </summary>
		private int source;
		/// <summary>
		/// ����/��� ����
		/// </summary>
		private int respResultFlag;

		public AlarmRespResultData(){

		}

		~AlarmRespResultData(){

		}

		public override NCasObject CopyTo(){

			return null;
		}

		/// <summary>
		/// ����/��� �ð�
		/// </summary>
		public DateTime RespResultTime{
			get{
				return respResultTime;
			}
			set{
				respResultTime = value;
			}
		}

		/// <summary>
		/// �߷ɴ������
		/// </summary>
		public int AreaCode{
			get{
				return areaCode;
			}
			set{
				areaCode = value;
			}
		}

		public int BroadCtrlFlag{
			get{
				return broadCtrlFlag;
			}
			set{
				broadCtrlFlag = value;
			}
		}

		/// 
		/// <param name="obj"></param>
		public override void CopyFrom(NCasObject obj){

		}

		/// <summary>
		/// �߷ɱ���
		/// </summary>
		public int Section{
			get{
				return section;
			}
			set{
				section = value;
			}
		}

		/// <summary>
		/// �溸����
		/// </summary>
		public int AlarmKind{
			get{
				return alarmKind;
			}
			set{
				alarmKind = value;
			}
		}

		/// <summary>
		/// �߷ɸ�ü
		/// </summary>
		public int Media{
			get{
				return media;
			}
			set{
				media = value;
			}
		}

		/// <summary>
		/// �ܸ��ڵ�
		/// </summary>
		public int DevCode{
			get{
				return devCode;
			}
			set{
				devCode = value;
			}
		}

		/// <summary>
		/// �ܸ�����
		/// </summary>
		public int DevKind{
			get{
				return devKind;
			}
			set{
				devKind = value;
			}
		}

		/// <summary>
		/// �߷ɿ�
		/// </summary>
		public int Source{
			get{
				return source;
			}
			set{
				source = value;
			}
		}

		/// <summary>
		/// ����/��� ����
		/// </summary>
		public int RespResultFlag{
			get{
				return respResultFlag;
			}
			set{
				respResultFlag = value;
			}
		}

	}//end AlarmRespResultData

}//end namespace DataContainers