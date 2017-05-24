using DDK.Base.Extensions;
using System.Collections;
using UnityEngine;

namespace DDK.GamesDevelopment.Characters._3D
{
    public class PathJumperController : PathController<CharacterAutoJumpController, Transform>
    {
		public override GameObject currentCorrectItem
		{
			get
			{
				return _waypoints[_path[_currentRow]];
			}
		}

		void Start()
        {
			Movement.afterJumpEvent = onGoalReached;
            _InitPaths();
			// delegate movement action
			moveAction += Movement.JumpToGoal;
        }
    }
}
