using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using DDK.Base.Classes;

namespace DDK.GamesDevelopment.Characters._3D
{
    public class PathWalkerController : PathController<AgentGoalsController, Transform>
    {
        [Header("Agent Goals Parameters")]
        public bool setGoalsControllerValues = false;
        public float distanceTolerance = 0.5f;

		public SearchableGameObject searchableCharacter;

		public override GameObject currentCorrectItem
		{
			get
			{
				return _waypoints[_path[_currentRow]];
			}
		}

		private void GoalReachedHandler()
		{
			onGoalReached.InvokeAll();
		}

        // Use this for initialization
		IEnumerator Start ()
        {
			yield return _GetRef ();
            Movement.distanceTolerance = distanceTolerance;
			Movement.onGoalReached += GoalReachedHandler;
            _InitPaths();
            // delegate movement action
            moveAction += Movement.MoveToGoal;
        }

		protected override IEnumerator CorrectClick()
		{
			yield return base.CorrectClick ();
			if( HasGameEnded )
				Movement.onGoalReached -= GoalReachedHandler;
		}

		private IEnumerator _GetRef(){
			int retryCount = 0;
			while ( retryCount<10) {
				character = searchableCharacter.m_gameObject;
				retryCount++;

				if ( character != null && character == searchableCharacter.m_gameObject ){
					yield break;
				}

				yield return new WaitForEndOfFrame ();
			}
			if (retryCount > 10) {
				Debug.LogError ("There is no character of name " + searchableCharacter.objName);
			}
		}
    }
}
