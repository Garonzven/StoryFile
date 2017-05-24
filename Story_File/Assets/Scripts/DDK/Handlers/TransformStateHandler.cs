//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using DDK.Base.ScriptableObjects;
using System.Collections.Generic;
using DDK.Base.Extensions;
using DDK.Base.Classes;
using DDK.Base.Statics;


namespace DDK.Handlers 
{
	/// <summary>
	/// Allows saving and setting up different Transform values in Editor or Play mode. This can be useful when having 
	/// to setup lots of objects like rocks that need to be placed on a terrain one by one, instead you can make them 
	/// dynamic (rigidbody with collider) and make them fall on the terrain in Play Mode, then you just click /save/ to
	/// store the current state (Transforms values). When you quit Play Mode, and click /set/, the stored values will 
	/// place the Transforms as they were when you clicked /save/
	/// </summary>
	[ExecuteInEditMode]
	public class TransformStateHandler : MonoBehaviour 
    {
        #if UNITY_EDITOR
		[HelpBoxAttribute( "_ShowInfo" )]
		public string info = "Allows saving and setting up different Transform values in Editor or Play mode. This can be useful when having " +
	 "to setup lots of objects like rocks that need to be placed on a terrain one by one, instead you can make them " +
	 "dynamic (rigidbody with collider) and make them fall on the terrain in Play Mode, then you just click /save/ to" +
	 "store the current state (Transforms values). When you quit Play Mode, and click /set/, the stored values will " +
	 "place the Transforms as they were when you clicked /save/";
		public bool hideInfo;
        #endif
        public Transforms[] states = new Transforms[0];
		[Header("Current State")]
		public int stateIndex;
		[Tooltip("This is executed in Edit Mode. Click it to save the current state/transform into the specified state index")]
		public bool save;
		[Tooltip("This is executed in Edit Mode. Click it to set the current state/transform")]
		public bool set;


        #if UNITY_EDITOR
		protected bool _ShowInfo()
		{
			return !hideInfo;
		}
        #endif


        private static bool _warningShown;
        private static bool _msgShown;


		// Use this for initialization
		void Start () { } //Allows enabling/disabling this component
		
#if UNITY_EDITOR
		// Update is called once per frame
		void Update () 
        {
			if( save )
			{
				_Save ();
				save = false;
			}
			if( set )
			{
				_Set ();
                set = false;
            }
		}
#endif



		void _Save()
		{
			if( stateIndex < states.Length && stateIndex >= 0 )
			{
				states[ stateIndex ].transforms[ GetInstanceID() ] = new SerializableTransform( transform );
				_ShowMsg( "Transform State Saved!" );
				return;
			}
			_ShowWarning( "You must set a valid state index" );
		}
		void _Set()
		{
			if( stateIndex < states.Length && stateIndex >= 0 )
			{
				if( !states[ stateIndex ].transforms.ContainsKey( GetInstanceID() ) && !_warningShown )
				{
					_ShowWarning( "The state you want to set hasn't been saved" );
					return;
				}
				states[ stateIndex ].transforms[ GetInstanceID() ].DeserializeInto( transform );
				_ShowMsg( "Transform State Set!" );
				return;
			}
			_ShowWarning( "You must set a valid state index" );
        }
		void _ShowWarning( string msg )
		{
			if( _warningShown )
				return;
            Utilities.LogWarning ( msg, gameObject );
			_warningShown = true;
			Invoke( "ResetWarningTime", 0.1f );
		}
		void _ShowMsg( string msg )
		{
			if( _msgShown )
				return;
            Utilities.Log( msg, gameObject );
			_msgShown = true;
			Invoke( "ResetMsgTime", 0.1f );
		}
		void _ResetWarningShown()
		{
			_warningShown = false;
		}
		void _ResetMsgShown()
		{
			_msgShown = false;
        }
    }
}
