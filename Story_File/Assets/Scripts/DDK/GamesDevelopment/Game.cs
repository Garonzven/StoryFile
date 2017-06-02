//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Statics;
using DDK.Base.Classes;


namespace DDK.GamesDevelopment {

	/// <summary>
	/// Global game settings with only 1 instance.
	/// </summary>
	public class Game : Singleton<Game> {

		public enum DIFFICULTIES {veryEasy, easy, medium, hard, veryHard}
		public enum MODES {classic, survival}
		
		//PUBLIC VARIABLES----------------------------------------------------------
		public DIFFICULTIES difficulty = DIFFICULTIES.medium;
		public MODES mode = MODES.classic;
		public string playerName = "DefaultPlayer";
		public string appName = "DefaultApp";
		//STATIC----------------------------------------------------
		/// <summary>
		/// This gets invoked when Game.ended is set to true.
		/// </summary>
		public static ComposedEvent onGameEnded;
		private static bool _ended;
		/// <summary>
		/// This can be used to trigger game end events.This has to be manually reset, maybe by using GameEndedReset component.
		/// </summary>
		public static bool ended {
			get{
				return _ended;
			}
			set{
				if( onGameEnded != null && value )
					onGameEnded.Invoke();
				_ended = value;
				Debug.Log ( "Game.ended ( Frame: " + Time.frameCount + " ): " + value );
			}
		}
		public static string identifier {
			get
			{
				return Instance.appName + "_" + Instance.playerName;
			}
		}
		//--------------------------------------------------------------------------
		
		//PROPS---------------------------------------------------------------------
		public static float deltaTime{
			get{
				return Device.deltaTime;
			}
		}
		/// <summary>
		/// Gets the x screen bounds.
		/// </summary>
		/// <value>The x screen bounds.</value>
		public static float xBound{
			get{
				return Device.xBound;
			}
		}
		/// <summary>
		/// Gets the y screen bounds.
		/// </summary>
		/// <value>The y screen bounds.</value>
		public static float yBound{
			get{
				return Device.yBound;
			}
		}
		/// <summary>
		/// Gets the -x screen bounds.
		/// </summary>
		/// <value>The -x screen bounds.</value>
		public static float _xBound{
			get{
				return Device._xBound;
			}
		}
		/// <summary>
		/// Gets the -y screen bounds.
		/// </summary>
		/// <value>The -y screen bounds.</value>
		public static float _yBound{
			get{
				return Device._yBound;
			}
		}
		/// <summary>
		/// Returns a random screen point as world point.
		/// </summary>
		/// <value>A random screen point.</value>
		public static Vector2 getRandomScreenPoint {
			get{
				return Device.getRandomScreenPoint;
			}
		}
        //-------------------------------------------------------------------------

        /// <summary>
        ///     Coroutine to wait till <see cref="ended" /> is set to true
        /// </summary>
        /// <param name="restart">
        ///     If set to <c>true</c> [restart] will
        ///     set Game.ended to false once waiting is done.
        /// </param>
        /// <returns></returns>
        public static IEnumerator WaitForEnd(bool restart = false)
        {
            while (!ended)
            {
                yield return null;
            }

            ended = !restart && ended;
        }

    }


}