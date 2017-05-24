using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace DDK.Base.Classes 
{
	[Serializable]
	public class SerializableDictionary< K, V > : IEnumerable< KeyValuePair< K, V > > 
    {
		public V this[ K key ] 
        {
			get 
            {
				if( !dictionaryRestored )
					RestoreDictionary();
				return ValuesList[ Dictionary[ key ] ];
			}			
			set 
            {
				if( !dictionaryRestored )
					RestoreDictionary();
				int index;
				if( Dictionary.TryGetValue( key, out index ) ) 
                {
					ValuesList[ index ] = value;
				} 
                else 
                {
					Add( key, value  );
				}
			}
		}
		
		public void Add(K key, V value) 
        {
			Dictionary.Add( key, ValuesList.Count );
			KeysList.Add(key);
			ValuesList.Add(value);
		}
		
		public int Count { get { return ValuesList.Count; } }
		
		#region IEnumerable<KeyValuePair<K,V>> Members
		
		public IEnumerator< KeyValuePair< K, V > > GetEnumerator() { return new Enumerator( this ); }
		
		IEnumerator IEnumerable.GetEnumerator() { return new Enumerator( this ); }
		
		#endregion
		
		public V Get( K key, V default_value ) 
        {
			if( !dictionaryRestored )
				RestoreDictionary();
			
			int index;
			if( Dictionary.TryGetValue( key, out index ) )
				return ValuesList[ index ];
			else
				return default_value;
		}
		
		
		public bool TryGetValue( K key, out V value ) 
        {
			if( !dictionaryRestored )
				RestoreDictionary();
			
			int index;
			
			if( Dictionary.TryGetValue( key, out index ) ) 
            {
				value = ValuesList[ index ];
				return true;
			} 
            else 
            {
				value = default( V );
				return false;
			}
		}
		
		
		public bool Remove( K key ) 
        {
			if( !dictionaryRestored )
				RestoreDictionary();
			
			
			int index;
			if( Dictionary.TryGetValue( key, out index ) ) 
            {
				RemoveAt( index  );
				return true;
			}
			return false;
		}
		
		
		public void RemoveAt( int index ) 
        {
			if( !dictionaryRestored )
				RestoreDictionary();
			K key = KeysList[ index ];
			Dictionary.Remove( key );
			KeysList.RemoveAt( index );
			ValuesList.RemoveAt( index );
			
			for (int k = index; k < KeysList.Count; ++k)
				--Dictionary[KeysList[k]];
		}
		
		public KeyValuePair< K, V > GetAt( int index ) { return new KeyValuePair< K, V >( KeysList[ index ], ValuesList[ index ] ); }
		
		public V GetValueAt( int index ) { return ValuesList[ index ]; }
		
		public bool ContainsKey( K key ) 
        {
			if( !dictionaryRestored )
				RestoreDictionary();
			return Dictionary.ContainsKey( key );
		}
		
		
		public void Clear() 
        {
			Dictionary.Clear();
			KeysList.Clear();
			ValuesList.Clear();
		}
		
		
		private void RestoreDictionary() 
        {
			for( int i = 0 ; i < KeysList.Count ; ++i ) 
            {
				Dictionary[ KeysList[ i ] ] = i;
			}
			
			dictionaryRestored = true;
		}
		
		private Dictionary< K, int > Dictionary = new Dictionary< K, int >();
		
		[SerializeField] private List< K > KeysList = new List< K >();
		
		[SerializeField] private List< V > ValuesList = new List< V >();
		
		[NonSerialized] private bool dictionaryRestored = false;
		
		#region Nested type: Enumerator
		
		private class Enumerator : IEnumerator< KeyValuePair< K, V > > 
        {
			public Enumerator( SerializableDictionary< K, V > dictionary ) { Dictionary = dictionary; }
			#region IEnumerator<KeyValuePair<K,V>> Members
			
			public KeyValuePair< K, V > Current { get { return Dictionary.GetAt( current ); } }
			public void Dispose() { }
			object IEnumerator.Current { get { return Dictionary.GetAt( current ); } }
			public bool MoveNext() 
            {
				++current;
				return current < Dictionary.Count;
			}
			
			public void Reset() { current = -1; }
			#endregion
			
			private readonly SerializableDictionary< K, V > Dictionary;
			private int current = -1;
		}
		
		#endregion
	}
}