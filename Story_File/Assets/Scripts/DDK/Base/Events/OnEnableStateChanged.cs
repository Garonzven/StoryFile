using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;


namespace DDK.Base.Events
{
    /// <summary>
    /// Allows executing some events when this component gets enabled/disabled.
    /// </summary>
	public class OnEnableStateChanged : MonoBehaviourExt
	{		
		public ComposedEvent onEnable = new ComposedEvent();
		public ComposedEvent onDisable = new ComposedEvent();


		void OnEnable()
		{
			onEnable.Invoke();
		}
		void OnDisable()
		{
			onDisable.Invoke();
		}
	}
}
