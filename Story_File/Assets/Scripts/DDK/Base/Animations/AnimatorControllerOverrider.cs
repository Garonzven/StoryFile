using UnityEngine;
using System.Collections;
using DDK.Base.Classes;


namespace DDK.Base.Animations
{
	/// <summary>
	/// This will set an animator's runtime controller on awake, if the component is enabled.
	/// </summary>
	public class AnimatorControllerOverrider : MonoBehaviour 
	{		
		[Tooltip("If the controller is not found, it will be searched 3 more times with an interval of 1s")]
		public SearchableAnimator controller = new SearchableAnimator();
		public AnimatorOverrideController controllerOverrider;
		public float delay = 0f;
		[Tooltip("If true, the component will be destroyed after the controller is overriden")]
		public bool destroyAfterOverriden = true;


		protected bool _isOverriden;
        private WaitForSeconds _oneSecond = new WaitForSeconds( 1f );


		void Awake()
		{
			if( !enabled || delay > 0f )
				return;
			Override();
		}

		// Use this for initialization
		IEnumerator Start () 
        {
			if( _isOverriden )
				yield break;
			if( delay > 0f )
				yield return new WaitForSeconds( delay );
			byte searchCounts = 0;
			while( !controller.m_object && searchCounts < 3 )
			{
                yield return _oneSecond;
				searchCounts++;
			}
            Override();
		}


		public void Override()
		{
			if( !controller.m_object )
			{
				controller.m_object = GetComponentInChildren<Animator>();
			}
			if( !controller.m_object || !controllerOverrider )
				return;
			controller.m_object.runtimeAnimatorController = controllerOverrider;
			_isOverriden = true;
			if( destroyAfterOverriden )
			{
				Destroy ( this );
			}
		}
	}
}
