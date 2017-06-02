//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DDK.Base.Extensions;
using DDK.Base.Statics;
using DDK.Base.Classes;


namespace DDK.Base.Animations 
{
	/// <summary>
	/// Animates a Grid Layout Group's Spacing value towards a target or back to its initial value.
	/// </summary>
	public class AnimateGridLayout : MonoBehaviourExt 
    {
		[Tooltip("If null and this object doesn't have one, a search will be done and the first found will be used")]
		public GridLayoutGroup layout;
		public Vector2 spacingTarget;
		public float animDuration = 1f;
		[Tooltip("If false, the animation will be done OnDisable() instead")]
		public bool toTargetOnEnable = true;
		public bool destroyWhenDone;


		internal Vector2 _originalSpacing;



		// Use this for initialization
		void Awake ()
        {
			//VALIDATE
			if( !layout )
			{
				layout = GetComponent<GridLayoutGroup>();
				if( !layout )
				{
					layout = FindObjectOfType<GridLayoutGroup>();
				}
				if( !layout )
				{
					Utilities.LogWarning("No Grid layout was found.. Destroying component");
					Destroy ( this );
				}
			}
			//INIT
			_originalSpacing = layout.spacing;
		}
        /// <summary>
        /// Invoked when this component is enabled. Animates the specified layout's spacing values.
        /// </summary>
		protected void OnEnable()
		{
			if( toTargetOnEnable )
			{
				AnimateSpacing ( spacingTarget ).Start();
			}
			else 
            {
				AnimateSpacing ( _originalSpacing ).Start();
			}
		}
        /// <summary>
        /// Invoked when this component is disabled. Animates the specified layout's spacing values.
        /// </summary>
		protected void OnDisable()
		{
            if( destroyWhenDone )
                return;
			if( toTargetOnEnable )
			{
				AnimateSpacing ( _originalSpacing ).Start();
			}
			else 
            {
				AnimateSpacing ( spacingTarget ).Start();
			}
		}


        /// <summary>
        /// Animates the specified layout's spacing values to the specified /target/ values. The component might be 
        /// destroyed afterwards if /destroyWhenDone/ equals true.
        /// </summary>
		public IEnumerator AnimateSpacing( Vector2 target )
		{
            Vector2 initial = layout.spacing;
            float time = 0f;
            while( initial != target )
            {
                time += Time.deltaTime;
                if( !layout )
                {
                    Utilities.LogError("Layout element became null", gameObject);
                    yield break;
                }
                layout.spacing = Vector2.Lerp( initial, target, time / animDuration );
                yield return null;
            }
            layout.spacing = target;
			if( destroyWhenDone )
			{
				Destroy( this );
			}
		}
	}
}