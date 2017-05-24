//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DDK.Base.Fx.Transitions;
using DDK.Base.Components;
using DDK.Base.Misc;
using UnityEngine.Events;
using DDK.Base.Classes;
using DDK.Base.ScriptableObjects;
using DDK.Base.Events;
using DDK.Base.Animations;
using DDK.Base.Statics;
using MovementEffects;


#if USE_GAMES_DEVELOPMENT
using DDK.GamesDevelopment.Characters;
#endif

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

#if USE_PARSE
using Parse;
#endif


namespace DDK.Base.Extensions 
{
    /// <summary>
    /// Extension class for miscellaneous classes.
    /// </summary>
	public static class OtherExt 
    {						
		#region GUI
		public static void SetAlpha( this GUIText guiText, float alpha )
		{
			if( guiText )
			{
				guiText.color = new Color( guiText.color.r, guiText.color.g, guiText.color.b, alpha );
			}
		}
		#endregion

		#region RANDOMS
        /// <summary>
        /// Returns a random object from the list and removes it.
        /// </summary>
        /// <returns>The random object.</returns>
        /// <param name="obj">Object.</param>
        public static T PopRandom<T>( this IList<T> objs )
        {
            int count = objs.Count;
            if( count == 0 )
            {
                Debug.LogWarning ("objs list is empty.. Returning default type");
                return default(T);
            }
            int ranIndex = Random.Range( 0, count );
            T element = objs[ ranIndex ];
            objs.Remove( element );
            return element;
        }
		/// <summary>
        /// Returns a random object from the list. Use PopRandom() to also remove the returned object.
		/// </summary>
		/// <returns>The random object.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="removeReturned">If true, the returned object is removed from the list.</param>
        /// <seealso cref="PopRandom"/>
		public static T GetRandom<T>( this IList<T> objs )
		{
			if( objs.Count == 0 )
			{
				Debug.LogWarning ("obj list is empty.. Returning default type");
				return default(T);
			}
			int ranIndex = Random.Range( 0, objs.Count );
            return objs[ ranIndex ];
		}
		/// <summary>
		/// Returns a list of random objects from the list. The returning list won't have repeated objects unless they 
		/// were already repeated in different indexes inside the specified list.
		/// </summary>
		/// <returns>The random objects.</returns>
		/// <param name="obj">Objects List.</param>
		/// <param name="amount">The amount of random objects to return. The objects list must have a larger length than
		/// the specified amount.</param>
		public static List<T> GetRandoms<T>( this IList<T> objs, int amount )
		{
            List<T> objsCopy = new List<T>( objs );
            int count = objs.Count;
            List<T> objsList = new List<T>( count );
			
            if( count >= amount )
			{
				for( int i=0; i<amount; i++ )
				{
                    objsList.Add( objsCopy.PopRandom() );
				}
			}
			else Debug.LogWarning("The specified amount is larger than the obj list's length");
			return objsList;
		}
            
        /// <summary>
        /// Returns a random object from the list.
        /// </summary>
        /// <returns>The random object.</returns>
        public static T GetRandom<T>( this T[] objs )
        {
            int count = objs.Count<T>();
            if( count == 0 )
            {
                Debug.LogWarning ("objs array is empty.. Returning default type");
                return default(T);
            }
            int ranIndex = Random.Range( 0, count );
            return objs[ ranIndex ];
        }		
		/// <summary>
		/// Returns a random object from the array.
		/// </summary>
		/// <returns>The random object.</returns>
		/// <param name="obj">Object.</param>
		public static object GetRandom( this object[] obj )
		{
			return obj[ Random.Range( 0, obj.Length ) ];
		}
		
		/// <summary>
		/// Returns a random object from the array.
		/// </summary>
		/// <returns>The random object.</returns>
		/// <param name="obj">Object.</param>
		public static object GetRandom( this ArrayList obj )
		{
			return obj[ Random.Range( 0, obj.Count ) ];
		}
		#endregion

		#region TIME SPAN
		/// <summary>
		/// Adds a 0 before the value if lower than 10.
		/// </summary>
		/// <param name="ts">Time Span.</param>
		public static string GetSeconds0( this System.TimeSpan ts )
		{
			if( ts.Seconds < 10 ) {
				return "0"+ts.Seconds;
			}
			return ""+ts.Seconds;
		}
		/// <summary>
		/// Adds a 0 before the value if lower than 10.
		/// </summary>
		/// <param name="ts">Time Span.</param>
		public static string GetMinutes0( this System.TimeSpan ts )
		{
			if( ts.Minutes < 10 ) {
				return "0"+ts.Minutes;
			}
			return ""+ts.Minutes;
		}
		/// <summary>
		/// Adds a 0 before the value if lower than 10.
		/// </summary>
		/// <param name="ts">Time Span.</param>
		public static string GetHours0( this System.TimeSpan ts )
		{
			if( ts.Hours < 10 ) {
				return "0"+ts.Hours;
			}
			return ""+ts.Hours;
		}
		#endregion

		#region DATETIME
		/// <summary>
		/// Compares to dates without taking into account the time. Returns 1 if a > b, -1 if a < b and 0 if both are equal.
		/// </summary>
		public static int CompareToSkipTime( this System.DateTime a, System.DateTime b )
		{
			var _a = new System.DateTime( a.Year, a.Month, a.Day );
			var _b = new System.DateTime( b.Year, b.Month, b.Day );
			return _a.CompareTo( _b );
		}
		#endregion

		#region RENDERER GROUP
		/// <summary>
		/// Fades the specified canvas groups, until they reaches the specified target value.
		/// </summary>
		public static void AlphaTo( this IList<RendererGroup> groups, float target, float duration )
		{
			if( groups == null )
				return;
            RendererGroup rendererGroup;
			for( int i=0; i<groups.Count; i++ )
			{
                rendererGroup = groups[i];
                if( !rendererGroup )
                {
                    continue;
                }
                rendererGroup.AlphaTo( target, duration ).Start();
			}
		}
		/// <summary>
		/// Fades the specified renderer groups, until they reach the specified target value.
		/// </summary>
		public static void Blink( this IList<RendererGroup> groups, float target, float blinkDuration, int repeat = 1 )
		{
			if( groups == null )
				return;
			groups.BlinkCo( target, blinkDuration, repeat ).Start();
		}
		/// <summary>
		/// Fades the specified renderer groups, until they reach the specified target value.
		/// </summary>
		public static IEnumerator BlinkCo( this IList<RendererGroup> groups, float target, float blinkDuration, int repeat = 1 )
		{
			if( groups == null )
				yield break;
			for( int i=0; i<groups.Count; i++ )
			{
				groups[i].BlinkCo( target, blinkDuration, repeat ).Start();
			}
			yield return new WaitForSeconds( blinkDuration * ( repeat + 1 ) );
		}
		#endregion

		#region COMPONENT
		/// <summary>
		/// Copies a non built in component's values to this component. The component's properties values are not copied.
		/// </summary>
		public static void CopyFrom( this Component original, Component from )
		{
			System.Type type = original.GetType();
			if( from.GetType() != type )
			{
				Debug.LogWarning ("The component you want to copy is from a different type..");
			}
			// Copied fields can be restricted with BindingFlags
			FieldInfo[] fields = type.GetFields( /*BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic*/ ); 
			foreach ( FieldInfo field in fields )
			{
				field.SetValue( original, field.GetValue( from ) );
			}
		}		
		/// <summary>
		/// Copies a non built in component's values to this component. The component's properties values are not copied.
		/// </summary>
		public static void CopyFrom( this Component original, GameObject from )
		{
			System.Type type = original.GetType();
			Component _from = from.GetComponent(type);
			// Copied fields can be restricted with BindingFlags
			FieldInfo[] fields = type.GetFields( /*BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic*/ ); 
			foreach ( FieldInfo field in fields )
			{
				field.SetValue( original, field.GetValue( _from ) );
			}
		}
		/// <summary>
		/// Copies all non built in components from another object. The component's properties values are not copied.
		/// </summary>
		/// <param name="original">Original.</param>
        /// <param name="from">Source.</param>
		public static void CopyAllCompsFrom( this GameObject original, GameObject from )
		{
			if( !original || !from )
				return;
			Component[] comps = from.GetComponents<Component>();
			for( int i=0; i<comps.Length; i++ )
			{
				bool enabled = true;
				if( comps[i] is Behaviour )
				{
					enabled = ( (Behaviour)comps[i] ).enabled;
				}
				if( comps[i] is Collider )
				{
					enabled = ( (Collider)comps[i] ).enabled;
				}
				comps[i].CopyTo( original, enabled );
			}
		}		
        /// <summary>
        /// Copies all non built in components to another object. The component's properties values are not copied.
        /// </summary>
        /// <param name="original">Original.</param>
        /// <param name="target">Destination.</param>
        public static void CopyAllCompsTo( this GameObject original, GameObject target )
        {
            //target.CopyAllCompsFrom( original );
            if( !original || !target )
                return;
            Component[] comps = original.GetComponents<Component>();
            for( int i=0; i<comps.Length; i++ )
            {
                if( comps[i].GetType().Name.Equals( "CopyCompsTo" ) )
                {
                    continue;
                }
                bool enabled = true;
                if( comps[i] is Behaviour )
                {
                    enabled = ( (Behaviour)comps[i] ).enabled;
                }
                if( comps[i] is Collider )
                {
                    enabled = ( (Collider)comps[i] ).enabled;
                }
                comps[i].CopyTo( target, enabled );
            }
        }
		/// <summary>
		/// Copies a non built in component to another object. The component's properties values are not copied.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="original">Original.</param>
		/// <param name="destination">Destination.</param>
		public static Component CopyTo( this Component original, GameObject destination, bool enabled = true)
		{
			System.Type type = original.GetType();
			if( typeof( Transform ) == type )
			{
				destination.CopyTransformFrom( original.gameObject );
				return destination.transform;
			}
			Component copy = destination.AddGetComponent(type);
			if( copy is Behaviour )
			{
				((Behaviour)copy).enabled = enabled;
			}
			else if( copy is Collider )
			{
				((Collider)copy).enabled = enabled;
			}
			//BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			FieldInfo[] fields = type.GetFields(  ); 
			foreach ( FieldInfo field in fields )
			{
				field.SetValue( copy, field.GetValue( original ) );
			}
			/*PropertyInfo[] props = type.GetProperties(  ); 
			foreach ( PropertyInfo prop in props )
			{
				if( prop.CanRead && prop.CanWrite )
					prop.SetValue( copy, prop.GetValue( original, null ), null );
			}*/
			return copy;
		}
		/// <summary>
		/// Copies a non built in component to another object. The component's properties values are not copied.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="original">Original.</param>
		/// <param name="destination">Destination.</param>
		public static Component CopyCompTo( this GameObject original, GameObject destination, bool enabled = true )
		{
			return original.GetComponent<Component>().CopyTo( destination, enabled );
		}
		public static bool IsAnyAncestorNamed( this Component obj, string name )
		{
			if( !obj )
				return false;
			return obj.gameObject.IsAnyAncestorNamed( name );
		}
        public static T InstantiateOnTargetWithSameScale<T>( this T obj, Transform target, Transform parent ) where T : Component
        {
            return obj.gameObject.InstantiateOnTargetWithSameScale<T>( target, parent );
        }
        public static T InstantiateOnTargetWithSameScaleAndParent<T>( this T obj, Transform target ) where T : Component
        {
            return obj.InstantiateOnTargetWithSameScale<T>( target, target.parent );
        }
		/// <summary>
		/// Returns the first found object with the specified layer, or null if no object has the specified layer.
		/// </summary>
		public static T GetFirstFromLayer<T>( this T[] comps, string layerName ) where T : Component
		{
			if( comps == null )
			{
				//Debug.LogWarning ("The comps array is null");
				return null;
			}
			for( int i=0; i<comps.Length; i++ )
			{
				if( comps[ i ].gameObject.layer.Equals( LayerMask.NameToLayer( layerName ) ) )
				{
					return comps[ i ];
				}
			}
			return null;
		}
		/// <summary>
		/// Orders the components inside the array by sibling index ascending.
		/// </summary>
		public static T[] OrderBySiblingIndexAscending<T>( this T[] comps ) where T : Component
		{			
			for( int i=0; i<comps.Length; i++ )
			{
				if( !comps[i] || i == 0 )
					continue;
				if( comps[i].transform.GetSiblingIndex() < comps[i-1].transform.GetSiblingIndex() )
				{
					var temp = comps[i];
					comps[i] = comps[i-1];
					comps[i-1] = temp;
				}
			}
			return comps;
		}
		/// <summary>
		/// Orders the components inside the list by sibling index ascending. NOTE: If the list large, use the array version to improve performance.
		/// </summary>
		public static List<T> OrderBySiblingIndexAscending<T>( this List<T> comps ) where T : Component
		{
			for( int i=0; i<comps.Count; i++ )
			{
				if( !comps[i] || i == 0 )
					continue;
				if( comps[i].transform.GetSiblingIndex() < comps[i-1].transform.GetSiblingIndex() )
				{
					var temp = comps[i];
					comps[i] = comps[i-1];
					comps[i-1] = temp;
				}
			}
			return comps;
		}
		/// <summary>
		/// Orders the components inside the array by sibling index ascending.
		/// </summary>
		public static T[] OrderBySiblingIndexDescending<T>( this T[] comps ) where T : Component
		{			
			for( int i=0; i<comps.Length; i++ )
			{
				if( !comps[i] || i == 0 )
					continue;
				if( comps[i].transform.GetSiblingIndex() > comps[i-1].transform.GetSiblingIndex() )
				{
					var temp = comps[i];
					comps[i] = comps[i-1];
					comps[i-1] = temp;
				}
			}
			return comps;
		}
		/// <summary>
		/// Orders the components inside the list by sibling index ascending. NOTE: If the list large, use the array version to improve performance.
		/// </summary>
		public static List<T> OrderBySiblingIndexDescending<T>( this List<T> comps ) where T : Component
		{
			for( int i=0; i<comps.Count; i++ )
			{
				if( !comps[i] || i == 0 )
					continue;
				if( comps[i].transform.GetSiblingIndex() > comps[i-1].transform.GetSiblingIndex() )
				{
					var temp = comps[i];
					comps[i] = comps[i-1];
					comps[i-1] = temp;
				}
			}
			return comps;
		}
		#endregion

		#region T
		/// <summary>
		/// Copies the specified component of the specified type, to the specified objects.
		/// </summary>
		/// <returns>The new components.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static IList<T> CopyTo<T>( this T original, IList<GameObject> destinations, bool enabled = true ) where T : Component
		{
			List<T> comps = new List<T>();
			if( destinations != null && original != null )
			{
				for( int i=0; i<destinations.Count; i++ )
				{
					comps.Add( (T)original.CopyTo( destinations[i], enabled ) );
				}
			}
			return comps;
		}
		/// <summary>
		/// Creates a new instance of the specified type for each object in the collection. This is useful to fix Unity's bug
		/// related to custom serializable class arrays not taking the default values. You call this function inside a context menu
		/// to fix the issue.
		/// </summary>
		public static void SetDefaults<T>( this IList<T> objs ) where T : new()
		{
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i] = new T();
			}
		}
		public static string Serialize<T>( this T obj )
		{
			return Persistent.Serialize( obj );
		}
		public static T Deserialize<T>( this string data )
		{
			return Persistent.Deserialize<T>( data );
		}
		#endregion

		#region SEARCHABLE COMPONENTS/GAMEOBJECTS
		/// <summary>
		/// Returns the /m_object/ of each SearchableComponent in this list.
		/// </summary>
		public static T[] GetComponents<T>( this IList<SearchableComponent<T>> comps ) where T : Component
		{
			if( comps == null )
				return null;
			List<T> _comps = new List<T>( comps.Count );
			for( int i=0; i<comps.Count; i++ )
			{
				if( !comps[i].m_gameObject )
					continue;
				_comps.Add( comps[i].m_object );
			}
			return _comps.ToArray();
		}
		#endregion

		#region BOOL
		public static float ToFloat( this bool flag )
		{
			return ( flag ) ? 1f : 0f;
		}
		#endregion

		#region ILIST
		/// <summary>
		/// Copies each of the specified components of the specified type, to the specified object.
		/// </summary>
		/// <returns>The new components.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static IList<T> CopyTo<T>( this IList<T> originals, GameObject destination, bool enabled = true ) where T : Component
		{
			List<T> comps = new List<T>();
			if( destination != null && originals != null )
			{
				for( int i=0; i<originals.Count; i++ )
				{
					comps.Add( (T)originals[i].CopyTo( destination, enabled ) );
				}
			}
			return comps;
		}
		
		/// <summary>
		/// Copies each of the specified components of the specified type, to each of the specified objects.
		/// </summary>
		/// <returns>The new components.</returns>
		/// <param name="obj">Objects.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static IList<T> CopyTo<T>( this IList<T> originals, IList<GameObject> destinations, bool enabled = true ) where T : Component
		{
			List<T> comps = new List<T>();
			if( destinations != null && originals != null )
			{
				for( int i=0; i<destinations.Count; i++ )
				{
					comps.AddRange( originals.CopyTo<T>( destinations[i], enabled ) );
				}
			}
			return comps;
		}
		#endregion

		#region IENUMERATOR - IENUMERABLE
		static Dictionary<string, GameObject> _coHolders;

		/// <summary>
		/// Starts the coroutine on the Runtime Coroutines Holder game object. This prevents the coroutine from stopping when an 
		/// object is destroyed.
		/// </summary>
		/// <param name="This">This.</param>
		/// <param name="suffix">The Runtime Coroutines Holder's name suffix. This allows to access a specific holder </param>
		public static Coroutine Start( this IEnumerator This, string suffix = "DEFAULT" )
		{
			string name = "Runtime Coroutines Holder " + suffix;
			if( _coHolders == null )
			{
				_coHolders = new Dictionary<string, GameObject>();
			}
			if( !_coHolders.ContainsKey( suffix ) )
			{
				_coHolders[ suffix ] = new GameObject( name );
			}
			var coRunner = _coHolders[ suffix ].AddGetComponent<CoroutineRunner>();
			if( !coRunner )//ON APPLICATION QUIT IT BECOMES NULL
				return null;
			return coRunner.StartCoroutine( This );
		}		
        public static IEnumerator<float> Run( this IEnumerator<float> This )
        {
            return Timing.RunCoroutine( This );
        } 
        public static float RunWaitUntilDone( this IEnumerator<float> This )
        {
            return Timing.WaitUntilDone( Timing.RunCoroutine( This ) );
        } 
        public static IEnumerator<float> Run( this IEnumerator<float> This, string tag )
        {
            return Timing.RunCoroutine( This, tag );
        } 
        public static int KillCoroutines( this string tag )
        {
            return Timing.KillCoroutines( tag );
        }
		/// <summary>
		/// Starts the ienumerable as an IEnumerator (coroutine) on the Runtime Coroutines Holder game object. This prevents
		/// the coroutine from stopping when an object is destroyed.
		/// </summary>
		/// <param name="This">This.</param>
		/// <param name="suffix">The Runtime Coroutines Holder's name suffix. This allows to access a specific holder </param>
		public static Coroutine Start( this IEnumerable This, string suffix = "DEFAULT" )
		{
			return This.GetEnumerator().Start( suffix );
		}
		/// <summary>
		/// Starts the coroutine on the Runtime Coroutines Holder game object. This prevents the coroutine from stopping when an 
		/// object is destroyed.
		/// </summary>
		/// <param name="This">This.</param>
		/// <param name="delay">Delay.</param>
		/// <param name="suffix">The Runtime Coroutines Holder's name suffix. This allows to access a specific holder </param>
		public static Coroutine Start( this IEnumerator This, float delay, string suffix = "DEFAULT" )
		{
			return This._Start( delay, suffix ).Start( suffix );
		}
		
		private static IEnumerator _Start( this IEnumerator This, float delay, string suffix = "DEFAULT" )
		{
			if( delay > 0f )
				yield return new WaitForSeconds( delay );
			This.Start( suffix );
		}
        /// <summary>
        /// Starts the ienumerable as an IEnumerator (coroutine) on the Runtime Coroutines Holder game object. This prevents
        /// the coroutine from stopping when an object is destroyed.
        /// </summary>
        public static IEnumerator<float> Run( this IEnumerable<float> This )
        {
            return This.GetEnumerator().Run();
        }
		/// <summary>
		/// THIS IS NOT WORKING.
		/// </summary>
		public static void StopRuntimeCoroutinesHolder( this MonoBehaviour This, string suffix )
		{
			Debug.LogWarning ("This isn't working..");
			if( _coHolders == null )
				return;
			if( !_coHolders.ContainsKey( suffix ) )
				return;
			_coHolders[ suffix ].GetComponent<CoroutineRunner>().StopAllCoroutines();
			_coHolders[ suffix ].DestroyImmediate();
			/*_coHolders[ suffix ].GetComponent<CoroutineRunner>().StopAllCoroutines();
			_coHolders[ suffix ].Destroy( 0.01f );*/
			_coHolders.Remove( suffix );
			//This.StopAllCoroutines();
		}
		#endregion

		#region BYTE[]
		public static T Decode<T>( this byte[] data, string fileName, string possibleObjName = "" ) where T : class
		{
			string ext = fileName.GetFileExtension();
			ext = ext.ToLower();
			switch( ext )
			{
			case ".png": return data.DecodeTexture() as T;
			case ".jpg": return data.DecodeTexture() as T;
			case ".jpeg": return data.DecodeTexture() as T;
			case ".wav": return data.DecodeFromWav( possibleObjName ) as T;
				/*case ".aif": return typeof( AudioClip );
				break;*/
				/*case ".mp3": return typeof( AudioClip );
				break;
			case ".ogg": return typeof( AudioClip );
				break;
			case ".xm": return typeof( AudioClip );
				break;
			case ".mod": return typeof( AudioClip );
				break;
			case ".it": return typeof( AudioClip );
				break;
			case ".s3m": return typeof( AudioClip );
				break;*/
			}
			return default(T);
		}
		#endregion

		#region CAMERA
		/// <summary>
		/// Zoom the specified camera, by the specified /targetFieldOfView/ value.
		/// </summary>
		public static IEnumerator Zoom( this Camera cam, float targetFieldOfView, float duration )
		{
			foreach( float value in cam.fieldOfView.MoveFromTo( targetFieldOfView, duration ) )
			{
				cam.fieldOfView = value;
				yield return null;
				if( cam == null )
				{
					break;
				}
				if( cam.fieldOfView.CloseTo( targetFieldOfView ) )
				{
					cam.fieldOfView = targetFieldOfView;
					break;
				}
			}
		}
		/// <summary>
		/// Determines if this camera is active enabled and pre rendered. Do not call this in a loop cause it uses
		/// GetOrAddComponent which might impact performance.
		/// </summary>
		public static bool IsActiveEnabledAndPreRendered( this Camera camera )
		{
			if( !camera || !camera.isActiveAndEnabled )
				return false;
			var cameraRenderEvents = camera.GetOrAddComponent<OnCameraRenderEvents>();
			if( !cameraRenderEvents || !cameraRenderEvents.enabled )
				return false;
			return cameraRenderEvents.IsPreRendered;
		}
		/// <summary>
		/// Determines if this camera is active enabled and pre culled. Do not call this in a loop cause it uses
		/// GetOrAddComponent which might impact performance.
		/// </summary>
		public static bool IsActiveEnabledAndPreCulled( this Camera camera )
		{
			if( !camera || !camera.isActiveAndEnabled )
				return false;
			var cameraRenderEvents = camera.GetOrAddComponent<OnCameraRenderEvents>();
			if( !cameraRenderEvents || !cameraRenderEvents.enabled )
				return false;
			return cameraRenderEvents.IsPreCulled;
		}
		/// <summary>
		/// Determines if this camera is active enabled and post rendered. Do not call this in a loop cause it uses
		/// GetOrAddComponent which might impact performance.
		/// </summary>
		public static bool IsActiveEnabledAndPostRendered( this Camera camera )
		{
			if( !camera || !camera.isActiveAndEnabled )
				return false;
			var cameraRenderEvents = camera.GetOrAddComponent<OnCameraRenderEvents>();
			if( !cameraRenderEvents || !cameraRenderEvents.enabled )
				return false;
			return cameraRenderEvents.IsPostRendered;
		}
		#endregion

		#region OBJECT

		/// <summary>
		/// Instantiates all objects.
		/// </summary>
		public static Object[] Instantiate( this Object[] objs )
		{
			Object[] objects = new Object[objs.Length];
			for( int i=0; i<objs.Length; i++ )
			{
				if( objs[i] )
				{
					objects[i] = UnityEngine.Object.Instantiate( objs[i] );
				}
			}
			return objects;
		}
		/// <summary>
		/// Call the specified function/method and send the specified params/args all in a single string that will be split. NOTE: 
		/// args can only have bool, float, int or string values.
		/// </summary>
		/// <param name="comp">The component in which the method/function resides.</param>
		/// <param name="args">Arguments. Supported types are bool, float, int, and string.</param>
		/// <param name="methodSplitter">Method splitter.</param>
		/// <param name="argsSplitter">Arguments splitter.</param>
		public static object Call( this Object comp, string methodName, string[] args )
		{
			string _args = methodName + "#";
			for( int i=0; i<args.Length; i++ )
			{
				if( string.IsNullOrEmpty( args[i] ) )
					continue;
				_args += args[i];
				if( i < args.Length - 1 )
					_args += ":";
			}
			return comp.Call( _args );
		}
		/// <summary>
		/// Call the specified function/method and send the specified params/args all in a single string that will be split. NOTE: 
		/// args can only have bool, float, int or string values.
		/// </summary>
		/// <param name="comp">The component in which the method/function resides.</param>
		/// <param name="args">Arguments. Supported types are bool, float, int, and string.</param>
		/// <param name="methodSplitter">Method splitter.</param>
		/// <param name="argsSplitter">Arguments splitter.</param>
		public static object Call( this Object comp, string args = "methodName#arg0:arg1", char methodSplitter = '#', char argsSplitter = ':' )
		{
			if( !comp )
				return null;
			string[] splitted = args.Split( methodSplitter );
			string methodName = splitted[0];
			if( string.IsNullOrEmpty( methodName ) )
			{
				Debug.LogWarning("The /methodName/ hasn't been specified");
				return null;
			}
			var method = comp.GetType().GetMethod( methodName );
			var parameters = method.GetParameters();
			List<object> arguments = new List<object>( parameters.Length );
			if( splitted.Length <= 1 )
			{
				for( int i=0; i<arguments.Capacity; i++ )
				{
					arguments.Add( parameters[i].DefaultValue );
				}
			}
			else
			{
				string[] _args = splitted[1].Split( argsSplitter );
				for( int i=0; i<arguments.Capacity; i++ )
				{
					if( i >= _args.Length )
					{
						arguments.Add( parameters[i].DefaultValue );
					}
					int iVal;
					float fVal;
					if( int.TryParse( _args[i], out iVal ) )
					{
						arguments.Add( iVal );
					}
					else if( float.TryParse( _args[i], out fVal ) )
					{
						arguments.Add( fVal );
					}
					else if( _args.ContainsAny( "T", "True", "F", "False", "t", "f", "true", "false" ) ) 
					{
						arguments.Add( _args[i].ToBool() );
					}
					else arguments.Add( _args[i] );
				}
			}
			return method.Invoke( comp, arguments.ToArray() );
		}
        public static int CompareAlphanumFast( this object x, object y )
        {
            string s1 = x as string;
            if (s1 == null)
            {
                return 0;
            }
            string s2 = y as string;
            if (s2 == null)
            {
                return 0;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    int thisNumericChunk = int.Parse(str1);
                    int thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }
		#endregion

		#region LANGUAGE DICTIONARY
		/// <summary>
		/// Returns the current language's LanguageDictionary.
		/// </summary>
		public static LanguageDictionary GetCurrentLanguageDictionary( this IList<LanguageDictionary> languagesDic, int defaultLanguageIndex = 0 )
		{
			return languagesDic.GetLanguageDictionary( Application.systemLanguage, defaultLanguageIndex );
		}
		/// <summary>
		/// Returns the current language's LanguageDictionary.
		/// </summary>
		public static LanguageDictionary GetLanguageDictionary( this IList<LanguageDictionary> languagesDic, 
			SystemLanguage language, int defaultLanguageIndex = 0 )
		{
			if( languagesDic == null || languagesDic.Count == 0 )
			{
				Debug.LogWarning ("This /languagesDic/ is null or empty. Returning null");
				return null;
			}
			for( int i=0; i<languagesDic.Count; i++ )
			{
				if( languagesDic[i].language == language )
					return languagesDic[i];
			}
			if( languagesDic.Count <= defaultLanguageIndex )
			{
				Debug.LogWarning ("The specified /defaultLanguageIndex/ exceeds the /languagesDic/'s count. Returning the " + languagesDic[0].language + " dictionary" );
				return languagesDic[0];
			}
			return languagesDic[ defaultLanguageIndex ];
		}
		#endregion

		#region GENERIC CHARACTERS
#if USE_CHARACTERS
		public static void SitFloor( this Dictionary< string, GenericCharacter > characters, bool sitting = true, params string[] exclude )
		{
			var _characters = new List<GenericCharacter>( characters.Values );
			var keys = new List<string>( characters.Keys );
			for( int i=0; i<characters.Count; i++ )
			{
				if( exclude.Contains( keys[i] ) )
					continue;
				if( _characters[i] == null )
				{
					Debug.LogWarning ("The GenericCharacter value for the Key \""+keys[i]+"\" is null");
					continue;
				}
				_characters[i].SitFloor( sitting );
			}
		}
#endif
        #endregion


#if USE_PARSE
		/// <summary>
		/// Gets the file. This returns a WWW object until it finishes retrieving the file, consequently returning the 
		/// file name (including its extension) and the actual file as a byte array (byte[]).
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="key">Key.</param>
		public static IEnumerable<object> GetFile( this ParseObject obj, string key )
		{
			var file = obj.Get<ParseFile>( key );
			var wwwFile = new WWW(file.Url.AbsoluteUri);
			yield return wwwFile;
			int fileNameStartIndex = file.Url.AbsoluteUri.LastIndexOf("-");
			yield return file.Url.AbsoluteUri.Substring( fileNameStartIndex );
			yield return wwwFile.bytes;
		}
#endif

        /*/// <summary>
		/// Orders by descending value. The specified key's value must be an integer.
		/// </summary>
		/// <param name="dic">Dic.</param>
		/// <param name="key">Key.</param>
		public static void OrderDescending( this IDictionary[] dic, object key )
		{
			//var temp = new Dictionary<object, object>();
			int previousVal;
			for( int i=0; i<dic.Length; i++ )
			{
				if( i != 0 )
				{
					//temp[ key ] = dic[i][ key ];
					var previous = dic[i-1];
					previousVal = (int)dic[i-1][ key ];
					if( previousVal < (int)dic[i][key] )//is previous value lower than actual value?
					{//Yes, switch
						var temp = dic[i];
						dic[i] = previous;
						dic[i-1] = temp;
					}
				}
			}
		}*/
        #region UNITY EVENTS / ACTIONS
        public static void StateMatchingTarget(this UnityEvent evnt, GameObject target, UnityEventCallState callState)
        {
            var count = evnt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                var t = evnt.GetPersistentTarget(i) as GameObject;

                if (t == target)
                {
                    evnt.SetPersistentListenerState(i, callState);
                }
            }
        }
        public static void StateUnmatchingTarget(this UnityEvent evnt, GameObject target, UnityEventCallState callState)
        {
            var count = evnt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                var t = evnt.GetPersistentTarget(i) as GameObject;

                if (t != target)
                {
                    evnt.SetPersistentListenerState(i, callState);
                }
            }
        }
        public static void StatePersistentEvents(this UnityEvent evnt, UnityEventCallState callState)
        {
            var count = evnt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                evnt.SetPersistentListenerState(i, callState);
            }
        }
        public static void InvokeAfter( this UnityAction action, float delay )
        {
            action.InvokeAfterCo( delay ).Start();
        }
        public static IEnumerator InvokeAfterCo( this UnityAction action, float delay )
        {
            if( action == null )
                yield break;
            if( delay > 0f )
                yield return new WaitForSeconds( delay );
            action.Invoke();
        }
        #endregion

		#region DELAYED ACTION
		public static void StateMatchingTarget(this DelayedAction[] evnts, GameObject target, UnityEventCallState callState)
		{
			for (int i = 0; i < evnts.Length; i++)
			{
				evnts[i].StateMatchingTarget(target, callState);
			}
		}
		public static void StateUnmatchingTarget(this DelayedAction[] evnts, GameObject target, UnityEventCallState callState)
		{
			for (int i = 0; i < evnts.Length; i++)
			{
				evnts[i].StateUnmatchingTarget(target, callState);
			}
		}
		public static void StatePersistentEvents(this DelayedAction[] evnts, UnityEventCallState callState)
		{
			for (int i = 0; i < evnts.Length; i++)
			{
				evnts[i].StatePersistentEvents(callState);
			}
		}
		#endregion

        #region ACTION
        public static void InvokeAfter( this System.Action action, float delay )
        {
            action.InvokeAfterCo( delay ).Start();
        }
        public static IEnumerator InvokeAfterCo( this System.Action action, float delay )
        {
            if( action == null )
                yield break;
            if( delay > 0f )
                yield return new WaitForSeconds( delay );
            action.Invoke();
        }
        #endregion

        #region ANIMATION CURVES
        /// <summary>
        /// Ensures that this curve has the specified duration. This will automatically shrink or scale the curve if necessary.
        /// </summary>
        public static void Validate( this AnimationCurve curve, float duration )
        {
            if( duration == 0f )
                return;
            bool durationExceeded = false;
            float time;
            if( curve == null || curve.keys.Length <= 1 )
            {
                curve = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            }
            else for( int i = 0; i<curve.keys.Length; i++ )
            {
                if( curve.keys[ i ].time > duration && i != curve.keys.Length - 1 )//CAN'T EXCEED DURATION, DON'T CHECK LAST KEY
                {
                    durationExceeded = true;
                    curve.RemoveKey( i-- );
                }
                //LAST MUST REACH DURATION
                else if( i == curve.keys.Length - 1 && curve.keys[ i ].time != duration )
                {
                    for( int j = 1; j<curve.keys.Length; j++ )//FIX ALL KEYFRAMES (shrink or scale curve)
                    {
                        time = duration * curve.keys[ j ].time / curve.keys[ i ].time;
                        curve.MoveKey( j, new Keyframe( time, curve.keys[ j ].value ) );
                    }
                }
            }
            if( durationExceeded )
            {
                if( curve.keys.Length > 1 )
                    curve.AddKey( duration, 1f );
                else curve = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            }
        }
        /// <summary>
        /// Ensures that this curve has the specified duration. This will automatically shrink or scale the curve if necessary.
        /// </summary>
        public static Keyframe GetLastKeyframe( this AnimationCurve curve )
        {
            return curve.keys[ curve.keys.Length - 1 ];
        }
        public static void ScaleTime( this AnimationCurve curve, float scale, bool affectDuration = false )
        {
            if( scale <= 0f )
                return;
            float curveDuration = curve.GetLastKeyframe().time;
            bool scalingUp = scale > curveDuration;
            float maxTime;
            float time;
            if( affectDuration )
            {
                for( int i = 0; i<curve.keys.Length; i++ )
                {
                    time = scale * curve.keys[ i ].time;
                    curve.MoveKey( i, new Keyframe( time, curve.keys[ i ].value ) );
                }
            }
            else for( int i = 1; i<curve.keys.Length - 1; i++ )
            {
                if( scalingUp )
                {
                    maxTime = curveDuration - curve.keys[ i ].time;
                    time = curve.keys[ i ].time * Mathf.Clamp( scale, 0.01f, maxTime );
                }
                else time = scale * curve.keys[ i ].time;
                curve.MoveKey( i, new Keyframe( time, curve.keys[ i ].value ) );
            }
        }
        #endregion

        #region PARTICLE SYSTEM
        public static void SetParticlesColorOverLifetime(this ParticleSystem system, Gradient gradient)
        {
            var colorModule = system.colorOverLifetime;
            colorModule.color = new UnityEngine.ParticleSystem.MinMaxGradient( gradient );
        }
        public static void SetParticlesColorOverLifetime(this ParticleSystem system, Color min, Color max )
        {
            var minMaxGradient = system.colorOverLifetime.color;
            minMaxGradient.colorMin = min;
            minMaxGradient.colorMax = max;
        }
        /// <summary>
        /// Pauses the specified particle system for the specified amount of time (duration)
        /// </summary>
        /// <param name="system">This Particle ystem.</param>
        /// <param name="duration">The pause's duration.</param>
        /// <param name="realTime">If true, the pause's duration won't take into account the time scale.</param>
        public static IEnumerator PauseForCo( this ParticleSystem system, float duration, bool realTime = false )
        {
            if( !system )
                yield break;
            system.Pause();
            if( realTime )
            {
                yield return new WaitForSecondsRealtime( duration );
            }
            else yield return new WaitForSeconds( duration );
            if( !system )//It might have been destroyed
                yield break;
            system.Play();
        }
        /// <summary>
        /// Pauses the specified particle system for the specified amount of time (duration)
        /// </summary>
        /// <param name="system">This Particle ystem.</param>
        /// <param name="duration">The pause's duration.</param>
        /// <param name="realTime">If true, the pause's duration won't take into account the time scale.</param>
        public static void PauseFor( this ParticleSystem system, float duration, bool realTime = false )
        {
            system.PauseForCo( duration, realTime ).Start();
        }
        #endregion
    }


}