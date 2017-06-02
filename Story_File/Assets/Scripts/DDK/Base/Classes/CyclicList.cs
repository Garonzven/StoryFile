//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;


namespace DDK.Base.Classes 
{
	/// <summary>
	/// A cyclic list with the first element linked with the last.
	/// </summary>
	[System.Serializable]
	public class CyclicList<T> 
    {		
		[System.Serializable]
		class Node 
		{
			public Node previous;
			public T element = default(T);
			public Node next;
			
			public Node() {}
			
			public Node(T element) 
			{
				this.element = element;
			}
			
		}
		
		//------------------------------------------------------------
		
		Node node;
		Node first;
		Node last;
		
		public CyclicList()
		{
			node = new Node();
		}
		
		/// <summary>
		/// Push the specified obj (it is positioned after the last one).
		/// </summary>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public void Push(T obj)
		{
			if(node.element == null)//first push
			{
				node.element = obj;
				node.previous = node;
				node.next = node;
				first = node;
				last = node;
			}
			else if (node.next == node)//second push
			{
				Node newNode = new Node(obj);
				node.next = newNode;
				node.previous = newNode;
				newNode.previous = node;
				newNode.next = node;
				
				last = newNode;
			}
			else //After more than 2 pushes
			{
				Node newNode = new Node(obj);
				newNode.previous = last;
				newNode.next = first;
				last.next = newNode;
				first.previous = newNode;
				last = newNode;
			}
			
		}
		
		/// <summary>
		/// Removes the specified element (it is deleted from the list).
		/// </summary>
		/// <param name='element'>
		/// Element to be removed.
		/// </param>
		public void Remove(T element)
		{
			int count = Count;
			node = first;
			for( int i=0; i<count; i++ )
			{
				if( !Equals(node.element, element) ){
					Next ();
				}
				else
				{
					if( count == 1 )
						Clear();
					else if( count == 2 )
					{
						node.previous.next = node.previous;
						node.previous.previous = node.previous;
						node = node.previous;
						
						first = node;
						last = node;
					}
					else 
					{
						node.previous.next = node.next;
						node.next.previous = node.previous;
						Next();
						
						if( i == count - 1 )
						{
							last = node.previous;
						}
						else if( i == 0 )
						{
							first = node;
						}
						else 
						{
							node = first;
							break;
						}
					}
					
					//var e = node.element = default(T);
				}
			}
		}
		
		/// <summary>
		/// Removes the specified element (it is deleted from the list).
		/// </summary>
		/// <param name='element'>
		/// The element's index.
		/// </param>
		public void Remove(int index)
		{
			int count = Count;
			node = first;
			for( int i=0; i<count; i++ )
			{
				if( i != index ){
					Next ();
				}
				else
				{
					if( count == 1 )
						Clear();
					else if( count == 2 )
					{
						node.previous.next = node.previous;
						node.previous.previous = node.previous;
						node = node.previous;
						
						first = node;
						last = node;
					}
					else 
					{
						node.previous.next = node.next;
						node.next.previous = node.previous;
						Next();
						
						if( i == count - 1 )
						{
							last = node.previous;
						}
						else if( i == 0 )
						{
							first = node;
						}
						else 
						{
							node = first;
							break;
						}
					}
					
					//var e = node.element = default(T);
				}
			}
		}
		
		/// <summary>
		/// Pops the last element of the list, if last = true; otherwise pops the first element.
		/// </summary>
		public T Pop(bool popLast = true)
		{
			if( popLast ){
				if(last != null && Count > 1)
				{
					T Last = last.element;
					last.previous.next = first;
					last = last.previous;
					return Last;
				}
				else if(node != null) //last object in list
				{
					T temp = node.element;
					Clear();
					return temp;
				}
				else return default(T);
			}
			else{
				if(first != null && Count > 1)
				{
					T First = first.element;
					first.next.previous = last;
					first = first.next;
					return First;
				}
				else if(node != null) //last object in list
				{
					T temp = node.element;
					Clear();
					return temp;
				}
				else return default(T);
			}
		}
		
		/// <summary>
		/// Peeks the last element of the list, if last = true; otherwise peeks the first element.
		/// </summary>
		public T Peek(bool peekLast = true)
		{
			if( peekLast ){
				if(last != null)
				{
					return last.element;
				}
				else if(node != null) //last object in list
				{
					return node.element;
				}
				else return default(T);
			}
			else{
				if(first != null)
				{
					return first.element;
				}
				else if(node != null) //last object in list
				{
					return node.element;
				}
				else return default(T);
			}
		}
		
		/// <summary>
		/// Clear this list.
		/// </summary>
		public void Clear()
		{
			node = new Node();
			first = node;
			last = node;
		}
		
		/// <summary>
		/// Make the next element the actual element.
		/// </summary>
		public void Next()
		{
			node = node.next;
		}
		
		/// <summary>
		/// Calls Next() the specified count.
		/// </summary>
		/// <param name='count'>
		/// Count.
		/// </param>
		public void Next(int count)
		{
			for(int i=0; i<count; i++)
				Next ();
		}
		
		/// <summary>
		/// Makes the previous element the actual element.
		/// </summary>
		public void Previous()
		{
			node = node.previous;
		}
		
		/// <summary>
		/// Calls Previous() the specified count.
		/// </summary>
		/// <param name='count'>
		/// Count.
		/// </param>
		public void Previous(int count)
		{
			for(int i=0; i<count; i++)
				Previous ();
		}
		
		/// <summary>
		/// Gets the actual element.
		/// </summary>
		/// <returns>
		/// The actual element.
		/// </returns>
		public T GetActualElement()
		{
			if(node != null) return node.element;
			return default(T);
		}
		
		/// <summary>
		/// Gets the next element.
		/// </summary>
		/// <returns>
		/// The next element.
		/// </returns>
		public T GetNextElement()
		{
			if(node != null) 
				if(node.next != null) return node.next.element;
			return default(T);
		}
		
		/// <summary>
		/// Gets the previous element.
		/// </summary>
		/// <returns>
		/// The previous element.
		/// </returns>
		public T GetPreviousElement()
		{
			if(node != null) 
				if(node.previous != null) return node.previous.element;
			return default(T);
		}
		
		/// <summary>
		/// Gets the last element.
		/// </summary>
		/// <returns>
		/// The last element.
		/// </returns>
		public T GetLastElement()
		{
			if(last != null) return last.element;
			return default(T);
		}
		
		/// <summary>
		/// Gets the first element.
		/// </summary>
		/// <returns>
		/// The first element.
		/// </returns>
		public T GetFirstElement()
		{
			if(first != null) return first.element;
			return default(T);
		}
		
		/// <summary>
		/// Sets the actual element's value.
		/// </summary>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public void SetActualElement(T obj)
		{
			if(node != null) node.element = obj;
		}
		
		/// <summary>
		/// Sets the next element's value.
		/// </summary>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public void SetNextElement(T obj)
		{
			if(node != null)
				if(node.next != null) node.next.element = obj;
		}
		
		/// <summary>
		/// Sets the previous element's value.
		/// </summary>
		/// <param name='obj'>
		/// Object.
		/// </param>
		public void SetPreviousElement(T obj)
		{
			if(node != null)
				if(node.previous != null) node.previous.element = obj;
		}
		
		/// <summary>
		/// Sets the actual element.
		/// </summary>
		/// <param name='element'>
		/// Element.
		/// </param>
		/// <returns> The actual index; otherwise, -1 if the element couldn't be set (doesn't exist). </returns>
		public int SetActual(T element)
		{
			for( int i=0; i<Count; i++ )
			{
				if( !Equals(node.element, element) ){
					Next ();
				}
				else return i;
			}
			return -1;
		}
		
		/// <summary>
		/// Sets the actual element.
		/// </summary>
		/// <param name='index'>
		/// The index.
		/// </param>
		/// <returns> The actual index; otherwise, -1 if the element couldn't be set (doesn't exist). </returns>
		public T SetActual(int index)
		{
			if( Count < index+1 ) return default(T);//throw new System.IndexOutOfRangeException();
			node = first;
			for( int i=0; i<index; i++ )
			{
				Next();
			}
			return node.element;
		}
		
        /// <summary>
        /// Sets the last element as the actual/current element.
        /// </summary>
		public void SetLastAsActual()
		{
			last = node;
		}
		
		public int Count
		{
			get{
				int count = 0;
				if( node.element == null/*default(T)*/ )
					return count;
				T act = node.element;
				
				while( true )
				{
					count++;
					Next ();
					if ( Equals(node.element, act) )
						break;
				}
				return count;
			}
		}
		
		/// <summary>
		/// Print the list elements.
		/// </summary>
		public void Print()
		{
			T act = node.element;
			
			do{
				Debug.Log(node.element);
				Next ();
			}while( !Equals(node.element, act) );//node.element != act
		}
		
	}

}


