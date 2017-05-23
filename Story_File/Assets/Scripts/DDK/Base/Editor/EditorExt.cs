//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using DDK.Base.Extensions;
using DDK.Base.Statics;


namespace DDK 
{	
	public static class EditorExt 
    {		
		public static void Center( this EditorWindow window )
		{
			window.position = new Rect( Screen.width * 0.5f + window.position.width * 0.5f, 
			                           Screen.height * 0.5f + window.position.height * 0.5f,
			                           window.position.width, window.position.height );
		}		
		public static void SetSize( this EditorWindow window, float width = 100f, float height = 100f )
		{
			window.position = new Rect( window.position.x, window.position.y, width, height );
		}
		/// <summary>
		/// Returns the object this property belongs to.
		/// </summary>
		/// <returns>The parent.</returns>
		/// <param name="prop">Property.</param>
		/// <param name="grandparentInstead">If the object has a grandparent, it will try to get it instead.</param>
		public static object GetParent( this SerializedProperty prop )
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			int count = 1;
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach( var element in elements.Take( elements.Length - count ) )
			{
				if( element.Contains("[")  )
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
					obj = obj.GetValue( elementName, index );
				}
				else
				{
					obj = obj.GetValue( element );
				}
			}
			return obj;
		}
		public static object GetValue( this object source, string name )
		{
			if(source == null)
				return null;
			var type = source.GetType();
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(f == null)
			{
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if(p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}
		
		public static object GetValue( this object source, string name, int index )
		{
			var enumerable = source.GetValue( name ) as IEnumerable;
            if( enumerable == null )
                return source;
			var enm = enumerable.GetEnumerator();
			while(index-- >= 0)
				enm.MoveNext();
            try{
                return enm.Current;
            } catch( System.InvalidOperationException )
            {
                return source;
            }
		}
		public static void LogProperties( this SerializedObject so, bool includeChildren = true ) 
		{
			// Shows all the properties in the serialized object with name and type
			// You can use this to learn the structure
			so.Update();
			SerializedProperty propertyLogger = so.GetIterator();
			while(true) {
				Debug.Log("name = " + propertyLogger.name + " type = " + propertyLogger.type);
				if(!propertyLogger.Next(includeChildren)) break;
			}
		}
		/// <summary>
		/// Count editable visible children of this property, including this property itself.
		/// </summary>
		/// <returns>The in property editables.</returns>
		/// <param name="property">Property.</param>
		public static int CountInPropertyEditables( this SerializedProperty property, bool siblingsInstead = false )
		{
			var prop = property.Copy();
			int count = 0;
			int children = property.CountInProperty() - 1;
			if( !siblingsInstead )
			{
				prop.NextVisible( true );
				prop = prop.Copy();
			}
			for( int i=0; i<children; i++ )
			{
				if( prop.editable )
				{
					count++;
				}
				prop.NextVisible( false );
			}
			return count;
		}
		/// <summary>
		/// Returns the sum of all children properties heights.
		/// </summary>
		public static float GetChildrenPropertiesHeightSum( this SerializedProperty property )
		{
			var prop = property.Copy();
			float totalHeight = 0f;
			int children = property.CountInProperty() - 1;

			if( !prop.NextVisible( true ) ) return 0f;
			
			for( int i=0; i<children; i++ )
			{
				totalHeight += EditorGUI.GetPropertyHeight( prop );
				
				if ( !prop.NextVisible(false) )
				{
					break;
				}
			}			
			return totalHeight;
		}
		/// <summary>
		/// Checks if a level is available (added to Build Settings and enabled). This returns true if not in the Editor.
		/// </summary>
		/// <param name="logMsgs">If set to <c>true</c> a message will be displayed if the scene hasn't been added
		/// to the Build Settings, or if it's disabled.</param>
		/// <param name="context"> The context object for the logged messages </parma>
		public static bool IsLevelAvailable( this Editor script, string levelName, bool logMsgs = false )
		{
			return Utilities.IsLevelAvailable( levelName, logMsgs, script.target );
		}		
	}	
}