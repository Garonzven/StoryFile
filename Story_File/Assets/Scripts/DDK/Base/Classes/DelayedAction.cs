using System;
using System.Collections;

using DDK.Base.Extensions;

using UnityEngine;
using UnityEngine.Events;

namespace DDK.Base.Classes
{
    /// <summary>
    /// Class to call UnityEvent with a delay and describe
    /// its action
    /// </summary>
    [Serializable]
    public class DelayedAction
    {
        public string description;
        public float delay;
        public UnityEvent actions;

        public IEnumerator InvokeCo ( bool withDelay = true )
        {
            if ( withDelay )
            {
                yield return new WaitForSeconds ( delay );
            }

            actions.Invoke();
        }

        public void Invoke (  bool withDelay = true )
        {
            InvokeCo ( withDelay ).Start();
        }

        public void StateMatchingTarget(GameObject target, UnityEventCallState callState)
        {
            var count = actions.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                var t = actions.GetPersistentTarget(i) as GameObject;

                if (t == target)
                {
                    actions.SetPersistentListenerState(i, callState);
                }
            }
        }
        public void StateUnmatchingTarget(GameObject target, UnityEventCallState callState)
        {
            var count = actions.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                var t = actions.GetPersistentTarget(i) as GameObject;

                if (t != target)
                {
                    actions.SetPersistentListenerState(i, callState);
                }
            }
        }
        public void StatePersistentEvents(UnityEventCallState callState)
        {
            var count = actions.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                actions.SetPersistentListenerState(i, callState);
            }
        }
    }
}