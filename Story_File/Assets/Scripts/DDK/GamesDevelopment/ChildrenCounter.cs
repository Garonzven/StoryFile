//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.UI;
using DDK.Base.Extensions;
using DDK.Base.Managers;
using DDK.Base.Statics;
using DDK.Base.Classes;


namespace DDK.GamesDevelopment 
{
	/// <summary>
	/// Attach to an object containing fadeable children, such as the ones holding Image or SpriteRenderer components. 
	/// This allows counting the children one by one while their fade component gets enabled.
	/// </summary>
	public class ChildrenCounter : MonoBehaviour 
    {
		public float delay;
        public float delayBetweenChildren = 0f;
		public bool countOnStart = true;
		[Space(10f)]
		[Tooltip("A Component that allows to fade in/out when enabling/disabling it, this will be copied to every child. Ex: FadeImages.cs")]
		public Behaviour fadeInComp;
		[Tooltip("The path to the numbers sounds that will be played when counting.")]
		public string numbersPath = "Sfx/Numbers";
        public AudioClip[] numbers;
        /// <summary>
        /// The name of the SfxManager's source that will play the numbers clips.
        /// </summary>
        [Tooltip("The name of the SfxManager's source that will play the numbers clips.")]
		public string source = "Voices";
        // audio source params
        [Range(0, 1)]
        public float volume = 1f;
        [Range(-3, 3)]
        public float pitch = 1f;
		[Header("Events")]
		[Tooltip("If true, the static class's Game.ended variable will be set to true")]
		public bool setGameEndWhenAllCounted;
		public float enableActivateDelay = 0.5f;
		public GameObject[] activateWhenAllCounted;
		public GameObject[] deactivateWhenAllCounted;
		public MonoBehaviour[] enableWhenAllCounted;
		public MonoBehaviour[] disableWhenAllCounted;
		public DelayedAction[] onCount;
		internal GameObject[] _children;


		// Use this for initialization
		void Start () 
        {
            // numbers is empty so no previous audio clips have been associated
		    if ( numbers == null || numbers.Length == 0 && !string.IsNullOrEmpty(numbersPath))
		    {
                numbers = Resources.LoadAll<AudioClip>(numbersPath);
            }
			
			_children = gameObject.GetChildren();
			if( fadeInComp )
			{
				_children.CopyComponent( fadeInComp, false );
			}
			if( countOnStart )
			{
				StartCoroutine( Count() );
			}
		}


		public IEnumerator Count()
		{
			yield return new WaitForSeconds( delay );
			for( int i=0; i<_children.Length; i++ )
			{
				if( fadeInComp )
				{
					var comp = _children[i].GetComponent( fadeInComp.GetType() ) as Behaviour;
					//comp.enabled = false;//fade out
					yield return null;
					comp.enabled = true;//fade in
				}
				if( numbers.Length > i )
				{
                    var audioSource = SfxManager.GetSource(source);
                    audioSource.volume = volume;
                    audioSource.pitch = pitch;

					onCount.InvokeAll();
					SfxManager.PlayClip( source, numbers[i] );

					if( i == _children.Length - 1 )
					{
						break;
					}
					yield return new WaitForSeconds( numbers[i].length / audioSource.pitch );
				}

                if( i != _children.Length - 1 )
                {
					yield return new WaitForSeconds( delayBetweenChildren );
                }
			}
			yield return new WaitForSeconds( enableActivateDelay );
			disableWhenAllCounted.SetEnabled( false );
			deactivateWhenAllCounted.SetActiveInHierarchy( false );
			enableWhenAllCounted.SetEnabled();
			activateWhenAllCounted.SetActiveInHierarchy();
			if( setGameEndWhenAllCounted )
			{
				Game.ended = true;
			}
			yield return null;
			Game.ended = false;
		}
	}
}
