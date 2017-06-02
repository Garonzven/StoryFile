#if UNITY_EDITOR
//By: Daniel Soto
//4dsoto@gmail.com
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DDK.Base.Misc;
using DDK.Base.Extensions;
using System.Globalization;
#if USE_NODE_CANVAS
using NodeCanvas.Framework;
#endif


namespace DDK
{
	[InitializeOnLoad]
	class HierarchyIcons
	{
		static Object[] _icons;
		
		const string PATH_1 = "Assets/Code/DDK/Base/BaseIcons";
		const string PATH_2 = "Assets/Scripts/DDK/Base/BaseIcons";
        const string PATH_3 = "Assets/BaseCode/Code/DDK/Base/BaseIcons";
		const string INFO = "/info.png";
		const string WARNING = "/warning.png";
		const string ERROR = "/error.png";
		
		static HierarchyIcons ()
		{
			EditorApplication.hierarchyWindowItemOnGUI += ShowIcons;
		}
		
		static void ShowIcons (int instanceID, Rect selectionRect)
		{
            var go = (GameObject) EditorUtility.InstanceIDToObject( instanceID );
			if ( go == null ) return;
			selectionRect.x = selectionRect.xMax - 18;
			selectionRect.width = 18;
			
			var descriptor = go.GetComponent<SceneDescriptor>();
			if( descriptor )
			{
				Texture2D icon = null;
				if( descriptor.CustomIcon && descriptor.customIcon != null )
				{
					GUI.Label( selectionRect, descriptor.customIcon );
				}
				else if( descriptor.description.ContainsIgnoreCase("THIS WILL BE AN ASSET BUNDLE") )
				{
#if USE_NODE_CANVAS
                    if( descriptor.GetComponent<GraphOwner>() )
                    {
                        selectionRect.x = selectionRect.xMax - 30;
                        selectionRect.width = 30;
                        GUI.Label( selectionRect, "AB  " );
                    }
                    else 
#endif
                    {
                        GUI.Label( selectionRect, "AB" );
                    }
				}
				else if( descriptor.messageType == MessageType.Info )
				{
					if( AssetDatabase.IsValidFolder( PATH_1 ) )
					{
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D>( string.Concat( PATH_1, INFO ) );
					}
                    else if( AssetDatabase.IsValidFolder( PATH_2 ) ) 
                    {
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_2, INFO ) );
                    }
                    else icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_3, INFO ) );
					if( icon != null )
					{
						GUI.Label( selectionRect, icon );
					}
					else GUI.Label( selectionRect, "i" );
				}
				else if( descriptor.messageType == MessageType.Warning )
				{
					if( AssetDatabase.IsValidFolder( PATH_1 ) )
					{
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D>( string.Concat( PATH_1, WARNING ) );
					}
                    else if( AssetDatabase.IsValidFolder( PATH_2 ) ) 
                    {
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_2, INFO ) );
                    }
                    else icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_3, INFO ) );
					if( icon != null )
					{
						GUI.Label( selectionRect, icon );
					}
					else GUI.Label( selectionRect, "!" );
				}
				else if( descriptor.messageType == MessageType.Error )
				{
					if( AssetDatabase.IsValidFolder( PATH_1 ) )
					{
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D>( string.Concat( PATH_1, ERROR ) );
					}
                    else if( AssetDatabase.IsValidFolder( PATH_2 ) ) 
                    {
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_2, INFO ) );
                    }
                    else icon = AssetDatabase.LoadAssetAtPath<Texture2D> ( string.Concat( PATH_3, INFO ) );
					if( icon != null )
					{
						GUI.Label( selectionRect, icon );
					}
					else GUI.Label( selectionRect, "!!!" );
				}
			}
            else
            {
                var monoExt = go.GetComponent<MonoBehaviourExt>();
                if( monoExt )
                {
                    if( monoExt.hierarchyIcon != null )
                    {
                        GUI.Label( selectionRect, monoExt.hierarchyIcon );
                    }
                    else _CheckCanvasGroup( go, selectionRect );
                }
                else _CheckCanvasGroup( go, selectionRect );
            }
		}

        static void _CheckCanvasGroup( GameObject go, Rect selectionRect )
        {
            var canvasGroup = go.GetComponent<UnityEngine.CanvasGroup>();
            if( canvasGroup )
            {
                selectionRect.x = selectionRect.xMax - 20;
                selectionRect.width = 20;
                string alpha = canvasGroup.alpha.ToString("F1");
                GUI.Label( selectionRect, alpha );
            }
        }
	}
}
#endif