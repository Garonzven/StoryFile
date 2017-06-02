//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;


namespace DDK.ObjManagement.Spawning 
{
    /// <summary>
    /// Allows instantiating multiple gameObjects on Awake() or OnEnable() 
    /// </summary>
	public class InstantiateOnIni : MonoBehaviour 
    {
		[System.Serializable]
		public class Obj 
        {
			public int instances = 1;
			public GameObject obj;
			public bool parentToThis;
			[IndentAttribute(1)]
			public bool worldPosStays;
			public bool activeOnInstantiation = true;
		}


		public Obj[] onAwake;
		public Obj[] onEnable;			
		
		
		void Awake()
		{
			CreateInstances( onAwake );
		}		
		// Use this for initialization
		void Start () {} //Allows enabling/disabling this component
		void OnEnable()
		{
			CreateInstances( onEnable );
		}


		protected void CreateInstances( Obj[] objs )
		{
            if( objs == null )
                return;
            int length = objs.Length;//caching
            int instances;//caching
            GameObject instance;//caching
            for( int i=0; i<length; i++ )
			{
                if( objs[i] == null )
                    continue;
                instances = objs[i].instances;
                for( int j=0; j<instances; j++ )
				{
					instance = objs[i].obj.SetActiveInHierarchy();
					if( !instance )
					{
						break;
					}
					if( objs[i].parentToThis )
					{
						instance.SetParent( transform, objs[i].worldPosStays );
					}
					instance.SetActive( objs[i].activeOnInstantiation );
				}
			}
		}
	}
}