using System;
using System.Collections.Generic;

using DDK.Base.Classes;
using DDK.Base.Extensions;

using UnityEngine;

namespace DDK.ObjManagement
{
    /// <summary>
    /// Copies a transform to another in Editor Mode.
    /// The script is destroyed as soon the game start.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class MirrorTransform : MonoBehaviour
    {
		/// <summary>
		///Stores the name of the object in case the reference gets lost.
		/// </summary>
		[HideInInspector]
		public string sourceName;
		[Tooltip("THE NAME OF THE OBJECT MUST BE UNIQUE")]
        public Transform source;

        public bool position = true;
        public bool rotation = true;
        public bool scale = true;

        public bool sourceToTransform = false;
        public bool transformToSource = true;

        public bool destroyOnPlay = true;

		int _findSourceMaxRetryFrames = 10;
		int _retryCount = 0;

        void Start()
        {
            if ( Application.isPlaying && destroyOnPlay )
            {
                Destroy ( this );
                return;
            }
        }

        void Update()
        {
            if ( !enabled )
            {
                return;
            }
			#if UNITY_EDITOR
			if( !Application.isPlaying )
			{
				if( source )
				{
					sourceName = source.name;
				}
				else if( !string.IsNullOrEmpty( sourceName ) )
				{
                    source = sourceName.Find<Transform>();
                }
            }
            #endif

			if (source != null) 
            {
				if (transformToSource) 
                {
					_CopyTransform (transform, source);
				} 
                else if (sourceToTransform) 
                {
					_CopyTransform (source, transform);
				}
			} 
            else if (_retryCount < _findSourceMaxRetryFrames) 
            {
				source = sourceName.Find<Transform> ();
				_retryCount++;
			}
			else if ( Application.isPlaying ) enabled = false;//Prevent multiple GameObject.Find() calls at runtime
        }

        private void _CopyTransform ( Transform src, Transform dst )
        {
            if ( position )
            {
                dst.position = src.position;
            }

            if ( rotation )
            {
                dst.rotation = src.rotation;
            }

            if ( scale )
            {
                dst.localScale = src.localScale;
            }
        }
#if UNITY_EDITOR
        [EditorButton]
        public void MirrorComponents()
        {
            gameObject.CopyAllCompsFrom ( source.gameObject );
        }
#endif
    }
}