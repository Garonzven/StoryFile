//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using UnityEngine.Events;
using DDK.Base.Classes;


#if USE_FINAL_IK
using DDK.GamesDevelopment.Characters;
using RootMotion.FinalIK;
#endif


namespace DDK.GamesDevelopment.Characters.FinalIK {
	
	/// <summary>
	/// This allows handling multiple IK Components for multiple characters.
	/// </summary>
	public class IKsHandler : MonoBehaviourExt {
		
		#if USE_FINAL_IK
		
		[Tooltip("The characters that will be handled")]
		public string[] characters = new string[0];
		public FBBIKTargetHandler[] fbbIkTargetHandlers = new FBBIKTargetHandler[0];
		public AimIKTargetHandler[] aimIkTargetHandlers = new AimIKTargetHandler[0];
		[Header("Events")]
		/*[HelpBoxAttribute]
		public string msg = "The string param for the Call() function must be in the following format:" +
			"methodName/methodSplitter/value/argsSplitter/value/argsSplitter.... Ex: Talk#T:1. " +
				"NOTE: The allowed types are int, float, and bool (T, True, F, False).";
		[Space(50f)]
		[Tooltip("This will allow splitting the method Name for the Call() function")]
		public char methodSplitter = '#';*/
		[Tooltip("This will allow splitting the arguments for the SetXXXXAnimDuration() functions")]
		public char argsSplitter = ':';
		
		
		protected IK[] _characters;
		protected bool _proceed;		
		
		
		// Use this for initialization
		void Start () {
			
			_characters = characters.Find<IK>().ToArray();
			if( _characters.Length == 0 )
			{
				return;
			}
			_proceed = true;
		}	
		
		
		public void MoveToTarget( int fbbIkIndex )
		{
			_ActionToTarget( fbbIkTargetHandlers, fbbIkIndex );
		}
		public void SetFbbAnimDuration( string args = "handlerIndex:duration" )
		{
			SetAnimDuration( fbbIkTargetHandlers, args );
		}

		public void LookAtTarget( int aimIkIndex )
		{
			_ActionToTarget( aimIkTargetHandlers, aimIkIndex );
		}
		public void SetAimAnimDuration( string args = "handlerIndex:duration" )
		{
			SetAnimDuration( aimIkTargetHandlers, args );
		}
		public void SetAnimDuration( IKTargetHandler[] handlers, string args = "handlerIndex:duration" )
		{
			if( args.IndexOf( argsSplitter ) == -1 )
			{
				Debug.LogWarning ("Wrong arguments format..", gameObject );
				return;
			}
			int index;
			float duration;
			string[] _args = args.Split( argsSplitter );
			if( !int.TryParse( _args[0], out index ) || !float.TryParse( _args[1], out duration ) )
			{
				Debug.LogWarning ("The specified arguments can't be parsed into their target types (int "+argsSplitter+" float)", gameObject );
				return;
			}
			handlers[ index ].animation.animDuration = duration;
		}



		protected void _ActionToTarget( IKTargetHandler[] ikTargetHandlers, int ikIndex )
		{
			if( !_proceed )
				return;
			if( ikIndex >= ikTargetHandlers.Length )
				return;
			for( int i=0; i<_characters.Length; i++ )
			{
				ikTargetHandlers[ ikIndex ].ActionToTarget( _characters[ i ] );
			}
		}
		
		#else
		[HelpBoxAttribute( MessageType.Warning )]
		public string msg = "The USE_FINAL_IK scripting symbol must be defined";
		#endif
		
	}
	
}

