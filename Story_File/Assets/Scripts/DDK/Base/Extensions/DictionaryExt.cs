using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace DDK.Base.Extensions
{
    public static class DictionaryExt 
    {
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns default(Tvalue).
        /// </summary>
        public static Tvalue Get<Tkey, Tvalue>( this Dictionary<Tkey, Tvalue> dic, Tkey key )
        {
            return dic.Get( key, default( Tvalue ) );
        }
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns default(T).
        /// </summary>
        public static T Get<T>( this Dictionary<string, T> dic, string key )
        {
            return dic.Get<string, T>( key );
        }
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns default(T).
        /// </summary>
        public static List<T> Get<T>( this Dictionary<string, T> dic, IList<string> keys )
        {
            return dic.Get( keys, default( T ) );
        }
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns default(Tvalue).
        /// </summary>
        public static Tvalue Get<Tkey, Tvalue>( this Dictionary<Tkey, Tvalue> dic, Tkey key, Tvalue defaultValue )
        {
            if( dic.ContainsKey( key ) )
            {
                return dic[key];
            }
            return defaultValue;
        }
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns /defaultValue/.
        /// </summary>
        public static T Get<T>( this Dictionary<string, T> dic, string key, T defaultValue )
        {
            return dic.Get<string, T>( key, defaultValue );
        }
        /// <summary>
        /// This ensures the key is present in the dic, if not, it returns /defaultValue/.
        /// </summary>
        public static List<T> Get<T>( this Dictionary<string, T> dic, IList<string> keys, T defaultValue )
        {
            List<T> list = new List<T>();
            for( int i=0; i<keys.Count; i++ )
            {
                list.Add( dic.Get<string, T>( keys[i], default( T ) ) );
            }
            return list;
        }
        /// <summary>
        /// Adds the specified list to the dictionary. The names of the list items will be used as keys.
        /// </summary>
        public static void AddRange<T>( this Dictionary<string, T> dic, IList<T> list ) where T : Object
        {
            dic.AddRange<T>( string.Empty, list );
        }
        /// <summary>
        /// Adds the specified list to the dictionary. The names of the list items will be used as keys and the /mainPath/ as the key prefix.
        /// </summary>
        public static void AddRange<T>( this Dictionary<string, T> dic, string mainPath, IList<T> list ) where T : Object
        {
            for( int i=0; i<list.Count; i++ )
            {
                string key = mainPath + list[i].name;
                if( dic.ContainsKey( key ) )
                {
                    continue;
                }
                dic.Add( key, list[i] );
            }
        }
        /// <summary>
        /// Returns all objects of type /T/ in which their Key starts with the specified string.
        /// </summary>
        public static List<T> GetAll<T>( this Dictionary<string, Object> dic, string keyStartsWith ) where T : Object
        {
            List<T> objs = new List<T>();
            var keys = new List<string>( dic.Keys );
            for( int i=0; i<dic.Count; i++ )
            {
                if( keys[i].StartsWith( keyStartsWith ) )//MATCH!
                {
                    objs.Add( dic.Values.ElementAt(i) as T );
                }
            }
            return objs;
        }
        /// <summary>
        /// Removes all objects of type /T/ in which their Key starts with the specified string.
        /// </summary>
        /// <param name="removeFromThis">If true, this dictionary's objects will be removed; otherwise, a new dictionary without the objects is returned.</param>
        public static Dictionary<string, T> Remove<T>( this Dictionary<string, T> dic, string keyStartsWith, bool removeFromThis = true ) where T : Object
        {
            Dictionary<string, T> newDic = new Dictionary<string, T>( dic );
            List<T> objs = new List<T>();
            var keys = new List<string>( newDic.Keys );
            for( int i=0; i<newDic.Count; i++ )
            {
                if( keys[i].StartsWith( keyStartsWith ) )//MATCH!
                {
                    objs.Remove( newDic.Values.ElementAt(i) as T );
                    i--;
                }
            }
            if( removeFromThis )
            {
                dic = newDic;
            }
            return newDic;
        }
    }

}