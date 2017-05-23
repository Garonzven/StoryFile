//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;

namespace DDK.Base.Misc 
{
	/// <summary>
	/// This object allows the programmer to write a description for the scene explaining anything he wants to allow other 
	/// programmers to easily understand the scene structure.
	/// </summary>
	public class SceneDescriptor : MonoBehaviour //THIS HAS AN EDITOR CLASS
	{
		#if UNITY_EDITOR
		public Texture2D customIcon;
		public string description = "";
		public UnityEditor.MessageType messageType = UnityEditor.MessageType.Info;
		public GameObject[] objs;

		public bool Lock;
		public bool Ref;
		public bool CustomIcon;


		[ContextMenu("Toggle Lock")]
		void _Lock()
		{
			Lock = !Lock;
		}
		[ContextMenu("Toggle References")]
		void _Ref()
		{
			Ref = !Ref;
		}
		[ContextMenu("Toggle Custom Icon")]
		void _CustomIcon()
		{
			CustomIcon = !CustomIcon;
		}
		#endif
	}

}
