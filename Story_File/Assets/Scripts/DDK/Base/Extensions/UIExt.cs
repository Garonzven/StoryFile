//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DDK.Base.Events;
using DDK.Base.Statics;
using DDK.Base.Animations;
using DDK.Base.Classes;
using MovementEffects;


#if USE_SVG_IMPORTER
using SVGImporter;
#endif


namespace DDK.Base.Extensions 
{
    // <summary>
    /// Extension class for UI related actions and classes.
    /// </summary>
	public static class UIExt 
    {
		#region SET
		public static void SetAllButtonsInteractable( bool interactable )
		{
			var bts = GameObject.FindObjectsOfType<Button>();
			for( int i=0; i<bts.Length; i++ )
			{
				bts[i].interactable = interactable;
			}
		}		
		/// <summary>
		/// Sets the sibling buttons as interactable or not, including this button.
		/// </summary>
		/// <param name="bt">Button.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetSiblingButtonsInteractable( this Button bt, bool interactable )
		{
			var bts = bt.transform.parent.GetComponentsInChildren<Button>();
			for( int i=0; i<bts.Length; i++ )
			{
				bts[i].interactable = interactable;
			}
		}		
		/// <summary>
		/// Sets the sibling buttons as interactable or not, including this button.
		/// </summary>
		/// <param name="bt">Button.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetSiblingButtonsInteractable( this GameObject bt, bool interactable )
		{
			var button = bt.GetComponent<Button>();
			var bts = button.transform.parent.GetComponentsInChildren<Button>();
			for( int i=0; i<bts.Length; i++ )
			{
				bts[i].interactable = interactable;
			}
		}		
		/// <summary>
		/// Sets the sibling images color's alpha value, including this image.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="alpha">The alpha value.</param>
		public static void SetSiblingGraphicsAlpha( this Graphic graphic, float alpha, bool includeSiblingsSubChildren = false, 
		                                           bool includeInactive = false, bool includeThis = false )
		{
			var graphics = graphic.gameObject.GetParent().GetChildren( includeSiblingsSubChildren, includeInactive ).ToList();
			if( !includeThis )
			{
				graphics.Remove( graphic.gameObject );
			}
			for( int i=0; i<graphics.Count; i++ )
			{
				var g = graphics[i].GetComponent<Graphic>();
				if( g )
				{
					g.color = new Color( g.color.r, g.color.g, g.color.b, alpha );
				}
			}
		}		
		/// <summary>
		/// Sets the sibling images color's alpha value, including this image.
		/// </summary>
		/// <param name="Graphic">Graphic.</param>
		/// <param name="alpha">The alpha value.</param>
		public static void SetSiblingGraphicsAlpha( this GameObject graphic, float alpha, bool includeSiblingsSubChildren = false, 
		                                           bool includeInactive = false, bool includeThis = false )
		{
			var g = graphic.GetComponent<Graphic>();
			if( g )
			{
				g.SetSiblingGraphicsAlpha( alpha, includeSiblingsSubChildren, includeInactive, includeThis );
			}
			else Debug.LogWarning("The specified gameobject has no graphic component");
		}
		/// <summary>
		/// Sets the sibling images color's value, including this image.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="color">Color.</param>
		/// <param name="includeSiblingsSubChildren">If set to <c>true</c> include siblings sub children.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive objects.</param>
		/// <param name="includeThis">If set to <c>true</c> include this graphic.</param>
		public static void SetSiblingGraphicsColor( this Graphic graphic, Color color, bool includeSiblingsSubChildren = false, 
		                                           bool includeInactive = false, bool includeThis = false )
		{
			var graphics = graphic.gameObject.GetParent().GetChildren( includeSiblingsSubChildren, includeInactive ).ToList();
			if( !includeThis )
			{
				graphics.Remove( graphic.gameObject );
			}
			for( int i=0; i<graphics.Count; i++ )
			{
				var g = graphics[i].GetComponent<Graphic>();
				if( g )
				{
					g.color = color;
				}
			}
		}		
		/// <summary>
		/// Sets the sibling images color's value, including this image.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="color">Color.</param>
		/// <param name="includeSiblingsSubChildren">If set to <c>true</c> include siblings sub children.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive objects.</param>
		/// <param name="includeThis">If set to <c>true</c> include this graphic.</param>
		public static void SetSiblingGraphicsColor( this GameObject graphic, Color color, bool includeSiblingsSubChildren = false, 
		                                           bool includeInactive = false, bool includeThis = false )
		{
			var g = graphic.GetComponent<Graphic>();
			if( g )
			{
				g.SetSiblingGraphicsColor( color, includeSiblingsSubChildren, includeInactive, includeThis );
			}
			else Debug.LogWarning("The specified gameobject has no Graphic component");
		}
		public static void SetSize( this RectTransform rt, Vector2 size )
		{
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		}		
		public static void SetSize( this RectTransform rt, float x, float y )
		{
			rt.SetSize( new Vector2( x, y ) );
		}		
		public static void SetSize( this Image img, Vector2 size )
		{
			img.rectTransform.SetSize( size );
		}		
		public static void SetSize( this Image img, float x, float y )
		{
			img.rectTransform.SetSize( new Vector2( x, y ) );
		}
        #if USE_SVG_IMPORTER
        public static void SetSize( this SVGImage img, Vector2 size )
        {
            img.rectTransform.SetSize( size );
        }       
        public static void SetSize( this SVGImage img, float x, float y )
        {
            img.rectTransform.SetSize( new Vector2( x, y ) );
        }
        #endif
		/// <summary>
		/// Sets the image component's sprite.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="sprite">Sprite.</param>
		public static void SetImageSprite( this GameObject obj, Sprite sprite, bool nameObjAsSprite = false )
		{
			if( !obj )
				return;
			var img = obj.GetComponent<Image>();
			if( !img )
				return;
			img.sprite = sprite;
			if( nameObjAsSprite )
			{
				obj.name = sprite.name;
			}
		}		
		/// <summary>
		/// Sets the image component's sprite.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="path">The Sprite's path inside Resources folder.</param>
		public static void SetImageSprite( this GameObject obj, string path, bool nameObjAsSprite = false )
		{
			obj.SetImageSprite( Resources.Load<Sprite>( path ), nameObjAsSprite );
		}		
		/// <summary>
		/// Sets the sprite on this image by creating one from the specified texture.
		/// </summary>
		/// <param name="nameObjAsSprite">If set to <c>true</c> name the image's gameObject as the /texture/.</param>
		public static void SetSprite( this Image img, Texture2D texture, bool nameObjAsTexture = false )
		{
			if( !img || !texture )
				return;
			img.sprite = Sprite.Create( texture, new Rect( 0f, 0f, texture.width, texture.height ), new Vector2( 0.5f, 0.5f ) );
			if( nameObjAsTexture )
				img.name = texture.name;
		}	
		/// <summary>
		/// Sets the image's sprite.
		/// </summary>
		/// <param name="img">Image.</param>
		/// <param name="path">The Sprite's path inside Resources folder.</param>
		public static void SetSprite( this Image img, string path, bool nameObjAsSprite = false )
		{
			if( img )
			{
				img.sprite = Resources.Load<Sprite>( path );
				if( nameObjAsSprite )
				{
					img.gameObject.name = img.sprite.name;
				}
			}
		}		
		/// <summary>
		/// Sets the element's sprite.
		/// </summary>
		/// <param name="element">UI element.</param>
		/// <param name="path">The Sprite's path inside Resources folder.</param>
		public static void SetSprite( this Selectable element, string path, bool nameObjAsSprite = false )
		{
			element.SetSprite( Resources.Load<Sprite>( path ), nameObjAsSprite );
		}		
		/// <summary>
		/// Sets the element's sprite.
		/// </summary>
		/// <param name="element">UI element.</param>
		/// <param name="path">The Sprite's path inside Resources folder.</param>
		public static void SetSprite( this Selectable element, Sprite sprite, bool nameObjAsSprite = false )
		{
			if( element )
			{
				if( element.targetGraphic is Image )
				{
					( element.targetGraphic as Image ).sprite = sprite;
					if( nameObjAsSprite )
					{
						element.gameObject.name = sprite.name;
					}
				}
			}
		}		
		/// <summary>
		///This will set a graphic's Image or Text depending on both graphics's type, which should be the same.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="target">Target.</param>
		public static void SetGraphicMainValue( this Graphic graphic, Graphic target )
		{
			if( !graphic || !target )
				return;
			if( graphic.GetType() != target.GetType() )
			{
				Debug.LogWarning( "Graphics differ from type", graphic );
				return;
			}
			Image _target = target as Image;
			if( !_target )
			{
				Text _text = target as Text;
				Text _graphic = graphic as Text;
				_graphic.text = _text.text;
				return;
			}
			Image _image = graphic as Image;
			_image.sprite = _target.sprite;
		}
		/// <summary>
		/// Sets the interactable button property.
		/// </summary>
		/// <param name="obj">Object containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void SetInteractable( this GameObject obj, bool interactable, float delay = 0f )
		{
			obj._SetInteractableAfter( interactable, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property.
		/// </summary>
		/// <param name="obj">Object containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _SetInteractableAfter( this GameObject obj, bool interactable, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			if( !obj )
				yield break;
			var bt = obj.GetComponent<Button>();
			if( bt )
				bt.interactable = interactable;
		}			
		/// <summary>
		/// Checks if there is a Button component attached, if so, sets its interactable property.
		/// </summary>
		/// <param name="comp">Component attached to an object containing a Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetInteractable( this Behaviour comp, bool interactable, float delay = 0f )
		{
			comp.gameObject.SetInteractable( interactable, delay );
		}
		/// <summary>
		/// Sets the interactable button property for all active buttons in the scene.
		/// </summary>
		/// <param name="script">Any script.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void SetAllInteractableAfter( this Behaviour script, bool interactable, float delay = 0f )
		{
			script._SetAllInteractableAfter( interactable, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for all active buttons in the scene.
		/// </summary>
		/// <param name="script">Any script.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _SetAllInteractableAfter( this Behaviour script, bool interactable, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			var allButtons = GameObject.FindObjectsOfType<Button>();
			for( int i=0; i<allButtons.Length; i++ )
			{
				allButtons[i].interactable = interactable;
			}
		}		
		/// <summary>
		/// Sets the interactable button property for all active buttons in the scene.
		/// </summary>
		/// <param name="script">Any script.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void SetAllInteractable( this Behaviour script, bool interactable, List<Button> exclude = null )
		{
			var allButtons = GameObject.FindObjectsOfType<Button>();
			for( int i=0; i<allButtons.Length; i++ )
			{
				if( exclude != null )
					if( exclude.Contains( allButtons[i] ) )
						continue;
				allButtons[i].interactable = interactable;
			}
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetInteractables( this IList<GameObject> objs, bool interactable )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].GetComponent<Button>().interactable = interactable;
			}
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void SetInteractablesAfter( this IList<GameObject> objs, bool interactable, float delay = 0f )
		{
			objs._SetInteractablesAfter( interactable, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _SetInteractablesAfter( this IList<GameObject> objs, bool interactable, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].GetComponent<Button>().interactable = interactable;
			}
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		public static void SetInteractables( this IList<Button> objs, bool interactable )
		{
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].interactable = interactable;
			}
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void SetInteractablesAfter( this IList<Button> objs, bool interactable, float delay = 0f )
		{
			objs._SetInteractablesAfter( interactable, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _SetInteractablesAfter( this IList<Button> objs, bool interactable, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].interactable = interactable;
			}
		}
		public static void SetAlpha( this Graphic graphic, float alpha = 1f )
		{
			if( graphic )
			{
				graphic.color = new Color( graphic.color.r, graphic.color.g, graphic.color.b, alpha );
			}
		}
        public static void SetAlpha( this Text[] graphics, float alpha = 1f )
        {
            if( graphics == null )
                return;
            for( int i=0; i< graphics.Length; i++ )
            {
                graphics[i].SetAlpha( alpha );
            }
        }
        public static void SetAlpha( this List<Text> graphics, float alpha = 1f )
        {
            if( graphics == null )
                return;
            for( int i=0; i< graphics.Count; i++ )
            {
                graphics[i].SetAlpha( alpha );
            }
        }
		/// <summary>
		/// Sets the image's sprite order in the specified target. this parents the obj to the target and sets its 
		/// sibling index (last is on top of target's children, first is behing).
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Target.</param>
		/// <param name="siblingIndex">Sibling index, if left as -1 the index will be the last (on top of children).</param>
		public static void SetOrderInTarget( this GameObject obj, GameObject target, int siblingIndex = -1 )
		{
			if( obj != null && target != null )
			{
				if( siblingIndex < 0 )
				{
					siblingIndex = target.ChildCount() - 1;
				}
				obj.transform.SetParent (target.transform, true);
				obj.transform.SetSiblingIndex( siblingIndex.Clamp(0, target.ChildCount() - 1) );
			}
		}
		/// <summary>
		/// Sets the image's sprite order in the specified target. this parents the obj to the target and sets its 
		/// sibling index (last is on top of target's children, first is behing).
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="target">Target.</param>
		/// <param name="siblingIndex">Sibling index, if left as -1 the index will not be set.</param>
		public static IEnumerator SetOrderInTarget( this GameObject obj, GameObject target, int siblingIndex = -1,
		                                           float after = 1f )
		{
			if( obj != null && target != null )
			{
				obj.transform.SetParent (target.transform, true);
                if( after > 0f )
                {
                    yield return new WaitForSeconds( after );
                }
				if( siblingIndex >= 0 && siblingIndex < target.ChildCount() )
				{
					obj.transform.SetSiblingIndex( siblingIndex );
				}
			}
		}		
		public static void SetFont( this IList<Text> texts, Font font )
		{
			if( texts != null )
			{
				for( int i=0; i<texts.Count; i++ )
				{
					if( texts[i] )
					{
						texts[i].font = font;
					}
				}
			}
			else Debug.LogWarning("Texts list is null");
		}	
		public static void SetValues( this IList<Text> texts, IList<string> values )
		{
			if( texts != null )
			{
				Debug.LogWarning("Texts list is null");
				return;
			}
			if( values != null )
			{
				Debug.LogWarning("Values list is null");
				return;
			}
			if( texts.Count != values.Count )
			{
				Debug.LogWarning("The values list's count that you want to set is higher than the texts list's count");
				return;
			}
			for( int i=0; i<texts.Count; i++ )
			{
				if( texts[i] )
				{
					texts[i].text = values[i];
				}
			}
		}
		/// <summary>
		/// Enables or disables the Button's Animator if its transition is set to Animation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void SetAnimatorEnabled( this GameObject obj, bool enabled = true )
		{
			var bt = obj.GetComponent<Button>();
			if( bt )
			{
				var animator = obj.GetComponent<Animator>();
				if( animator != null && bt.transition == Selectable.Transition.Animation )
				{
					animator.enabled = enabled;
				}
			}
		}		
		/// <summary>
		/// Enables or disables the Button's Animator if its transition is set to Animation.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void SetAnimatorEnabled( this Graphic graphic, bool enabled = true )
		{
			var bt = graphic.GetComponent<Button>();
			if( bt )
			{
				var animator = graphic.GetComponent<Animator>();
				if( animator != null && bt.transition == Selectable.Transition.Animation )
				{
					animator.enabled = enabled;
				}
			}
		}
		/// <summary>
		/// Sets the color after last space using rich text.
		/// </summary>
		public static void SetColorAfterLastSpace( this Text uiText, Color color )
		{
			if( !uiText || uiText.text.IndexOf( " " ) < 0 )
				return;
			string hexColor = color.ToHex();
			uiText.supportRichText = true;
			string txtAfterLastSpace = uiText.text.GetLastEndPoint( false, " " );
			//CHECK IF IT ALREADY HAS A RICH TEXT COLOR
			if( txtAfterLastSpace.StartsWith( "<color=" ) )
			{
				//txtAfterLastSpace = txtAfterLastSpace.ReplaceBetweenMatches( "<color=#", ">", hexColor );
				txtAfterLastSpace = txtAfterLastSpace.RemoveBeforeMatch( 0, ">" );
				txtAfterLastSpace = "<color=#"+ hexColor + txtAfterLastSpace;
			}
			else
			{
				txtAfterLastSpace = "<color=#"+ hexColor +">" + txtAfterLastSpace + "</color>";
			}
			uiText.text = uiText.text.RemoveLastEndPoint( false, false, " " ) + txtAfterLastSpace;
		}
		/// <summary>
		/// Sets the color before the last space using rich text.
		/// </summary>
		public static void SetColorBeforeLastSpace( this Text uiText, Color color )
		{
			if( !uiText || uiText.text.IndexOf( " " ) < 0 )
				return;
			string hexColor = color.ToHex();
			uiText.supportRichText = true;
			string txtBeforeLastSpace = uiText.text.RemoveLastEndPoint( true, false, " " );
			//CHECK IF IT ALREADY HAS A RICH TEXT COLOR
			if( txtBeforeLastSpace.StartsWith( "<color=" ) )
			{
				//txtBeforeLastSpace = txtBeforeLastSpace.ReplaceBetweenMatches( "<color=#", ">", hexColor );
				txtBeforeLastSpace = txtBeforeLastSpace.RemoveBeforeMatch( 0, ">" );
				txtBeforeLastSpace = "<color=#"+ hexColor + txtBeforeLastSpace;
			}
			else
			{
				txtBeforeLastSpace = "<color=#"+ hexColor +">" + txtBeforeLastSpace + "</color>";
			}
			uiText.text = txtBeforeLastSpace + uiText.text.GetLastEndPoint( true, " " );
		}
		public static void SetInteractable( this CanvasGroup[] canvasGroup, bool interactable )
		{
			if( canvasGroup == null )
				return;
			for( int i=0; i<canvasGroup.Length; i++ )
			{
				if( !canvasGroup[ i ] )
					continue;
				canvasGroup[ i ].interactable = interactable;
			}
		}
		public static void SetBlocksRaycasts( this CanvasGroup[] canvasGroup, bool blocks )
		{
			if( canvasGroup == null )
				return;
			for( int i=0; i<canvasGroup.Length; i++ )
			{
				if( !canvasGroup[ i ] )
					continue;
				canvasGroup[ i ].blocksRaycasts = blocks;
			}
		}
		public static void SetIgnoreParentGroups( this CanvasGroup[] canvasGroup, bool ignore )
		{
			if( canvasGroup == null )
				return;
			for( int i=0; i<canvasGroup.Length; i++ )
			{
				if( !canvasGroup[ i ] )
					continue;
				canvasGroup[ i ].ignoreParentGroups = ignore;
			}
		}
		/// <summary>
		/// Animates the canvas group's alpha and enables/disables ( = active/!active) /blockRaycasts/.
		/// </summary>
		/// <param name="active">If set to <c>true</c> the alpha will be animated to 1f; otherwise to 0f.</param>
		/// <param name="affectInteractable">If set to <c>true</c> the /interactable/ will be also affected as the /blockRaycasts/.</param>
		public static void SetActiveInHierarchy( this CanvasGroup canvasGroup, bool active = true, float animDuration = 0.5f, bool affectInteractable = false )
		{
			if( !canvasGroup )
				return;
			canvasGroup.AlphaTo( active ? 1f : 0f, animDuration ).Start();
			canvasGroup.SetBlocksRaycasts( active, animDuration ).Run();
			if( affectInteractable )
				canvasGroup.SetInteractable( active, animDuration );
		}
		/// <summary>
		/// Animates the canvas group's alpha and enables/disables ( = active/!active) /blockRaycasts/.
		/// </summary>
		/// <param name="active">If set to <c>true</c> the alpha will be animated to 1f; otherwise to 0f.</param>
		/// <param name="affectInteractable">If set to <c>true</c> the /interactable/ will be also affected as the /blockRaycasts/.</param>
		public static void SetFields( this CanvasGroup canvasGroup, bool active = true, float animDuration = 0.5f, bool affectInteractable = false )
		{
			canvasGroup.SetActiveInHierarchy( active, animDuration, affectInteractable );
		}
		/// <summary>
		/// Sets the block raycasts. Checks if this group is null.
		/// </summary>
		public static IEnumerator<float> SetBlocksRaycasts( this CanvasGroup canvasGroup, bool block, float delay = 0f )
		{
			if( !canvasGroup )
				yield break;
			if( delay > 0 )
				yield return Timing.WaitForSeconds( delay );
			canvasGroup.blocksRaycasts = block;
		}
		#endregion SET

		#region GET
		public static Vector3 GetMousePosition( this Canvas canvas )
		{
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos );
			return canvas.transform.TransformPoint(pos);
		}
		/// <summary>
		/// Gets the world position from a UI object (WITH A RECT TRANSFORM).
		/// </summary>
		/// <returns>The world position.</returns>
		/// <param name="uiObj">User interface object.</param>
		/// <param name="canvas">The UI object's canvas. If null, The first Canvas to be returned in a search will be used. </param>
		/// <param name="cam">The camera related tu the canvas. If null, The Main Camera will be used, if no main camera 
		/// then the first camera to be returned in a search will be used. </param>
		public static Vector3 GetWorldPos( this GameObject uiObj, Canvas canvas = null, Camera cam = null )
		{
			var rt = uiObj.GetComponent<RectTransform>();
			if( !canvas )
			{
				canvas = GameObject.FindObjectOfType<Canvas>();
			}
			if( !cam )
			{
				cam = Camera.main;
				if( !cam )
				{
					cam = GameObject.FindObjectOfType<Camera>();
				}
			}
			
			var rMode = canvas.renderMode;
			if( rMode == RenderMode.ScreenSpaceOverlay )
			{
				return cam.ScreenToWorldPoint( rt.position );
			}
			else
			{
				return rt.position;
			}
		}		
		/// <summary>
		/// Gets the world position from a UI object (WITH A RECT TRANSFORM).
		/// </summary>
		/// <returns>The world position.</returns>
		/// <param name="uiObj">User interface object.</param>
		/// <param name="canvas">The UI object's canvas. If null, The first Canvas to be returned in a search will be used. </param>
		/// <param name="cam">The camera related tu the canvas. If null, The Main Camera will be used, if no main camera 
		/// then the first camera to be returned in a search will be used. </param>
		public static Vector2 GetWorldPos2( this GameObject uiObj, Canvas canvas = null, Camera cam = null )
		{
			return (Vector2) uiObj.GetWorldPos( canvas, cam );
		}		
		/// <summary>
		/// Gets the world position from a graphic.
		/// </summary>
		/// <returns>The world position.</returns>
		/// <param name="uiElement">Graphic.</param>
		/// <param name="inParent"> If true, the world position in the parent element is returned. </param>
		/// <param name="canvas">The UI object's canvas. If null, The first Canvas to be returned in a search will be used. </param>
		/// <param name="cam">The camera related tu the canvas. If null, The Main Camera will be used, if no main camera 
		/// then the first camera to be returned in a search will be used. </param>
		public static Vector3 GetWorldPos( this Graphic uiElement, bool inParent = false, Canvas canvas = null, Camera cam = null )
		{
			if( inParent )
			{
				return uiElement.rectTransform.parent.gameObject.GetWorldPos( canvas, cam );
			}
			return uiElement.gameObject.GetWorldPos( canvas, cam );
		}		
		public static RectTransform GetParentTransform( this Graphic t )
		{
			return t.rectTransform.parent.GetComponent<RectTransform>();
		}
		public static RectTransform GetParentTransform( this RectTransform t )
		{
			return t.parent.GetComponent<RectTransform>();
		}		
		/// <summary>
		/// Returns a string array with the text value of each text component in the specified Text reference parameter.
		/// </summary>
		/// <returns>The texts.</returns>
		/// <param name="texts">Texts.</param>
		public static string[] GetTexts( this Text[] texts )
		{
			if( texts != null )
			{
				string[] txts = new string[texts.Length];
				for( int i=0; i<texts.Length; i++ )
				{
					txts[i] = texts[i].text;
				}
				return txts;
			}
			return null;
		}		
		/// <summary>
		/// Returns a string that is the result of concatenating each text value of each Text element inside the specified array.
		/// </summary>
		/// <returns>The texts as a string.</returns>
		/// <param name="texts">Texts.</param>
		public static string GetAsString( this Text[] texts )
		{
			string s = "";
			for( int i=0; i<texts.Length; i++ )
			{
				s += texts[i].text;
			}
			return s;
		}		
		public static GameObject GetParent( this Graphic uiElement )
		{
			if( uiElement )
			{
				return uiElement.transform.parent.gameObject;
			}
			else return null;
		}		
		/// <summary>
		/// Gets the component of the specified type, if no comp is found in the specified object, its children will be 
		/// searched until one returns it.
		/// </summary>
		/// <returns>The first found component.</returns>
		/// <param name="element">The UI element.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentIncludeChildren<T>( this Selectable element )
		{
			return element.gameObject.GetComponentIncludeChildren<T>();
		}		
		/// <summary>
		/// Gets the component of the specified type, if no comp is found in the specified object, its children will be 
		/// searched until one returns it.
		/// </summary>
		/// <returns>The first found component.</returns>
		/// <param name="element">The UI element.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentIncludeChildren<T>( this Graphic element )
		{
			return element.gameObject.GetComponentIncludeChildren<T>();
		}		
		/// <summary>
		/// Gets the sprite from image component. It also checks if the references object is a prefab, if so, the sprite 
		/// from the referenced gameobject's prefab instance is returned, in case there is one.
		/// </summary>
		/// <returns>The sprite from image.</returns>
		/// <param name="image">Image.</param>
		public static Sprite GetSpriteFromImage( this GameObject image )
		{
			if( image )
			{
				if( image.IsPrefab() )
				{
					var prefab = image.GetPrefabInstance();
					return prefab.GetComponent<Image>().sprite;
				}
				return image.GetComponent<Image>().sprite;
			}
			else return null;
		}		
		/// <summary>
		/// Gets the sprites from the images components. It also checks if the referenced objects are prefabs, if so, the sprites 
		/// from the referenced gameobjects prefab instances are returned, in case there is one for the respective.
		/// </summary>
		/// <returns>The sprites from the images.</returns>
		/// <param name="image">Image.</param>
		public static List<Sprite> GetSpritesFromImages( this IList<GameObject> images )
		{
			List<Sprite> sprites = new List<Sprite>();
			if( images == null )
				return sprites;
			for( int i=0; i<images.Count; i++ )
			{
				sprites.Add( images[i].GetSpriteFromImage() );
			}
			return sprites;
		}
		/// <summary>
		/// Returns the pixel coordinates of the image's texture where the mouse position is located.
		/// </summary>
		/// <returns>The raycast location.</returns>
		/// <param name="img">Image.</param>
		/// <param name="eventCamera">Event camera.</param>
		public static Vector2 GetRaycastLocation( this Image img, Camera eventCamera)
		{
			var sprite = img.sprite;
			
			var rectTransform = (RectTransform)img.transform;
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle( (RectTransform)img.transform, Input.mousePosition, eventCamera, out localPositionPivotRelative );
			
			// convert to bottom-left origin coordinates
			var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x*rectTransform.rect.width,
			                                localPositionPivotRelative.y + rectTransform.pivot.y*rectTransform.rect.height);
			
			var spriteRect = sprite.textureRect;
			var maskRect = rectTransform.rect;
			
			var x = 0;
			var y = 0;
			// convert to texture space
			switch (img.type)
			{
				
			case Image.Type.Sliced:
			{
				var border = sprite.border;
				// x slicing
				if (localPosition.x < border.x)
				{
					x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
				}
				else if (localPosition.x > maskRect.width - border.z)
				{
					x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
				}
				else
				{
					x = Mathf.FloorToInt(spriteRect.x + border.x +
					                     ((localPosition.x - border.x)/
					 (maskRect.width - border.x - border.z)) *
					                     (spriteRect.width - border.x - border.z));
				}
				// y slicing
				if (localPosition.y < border.y)
				{
					y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
				}
				else if (localPosition.y > maskRect.height - border.w)
				{
					y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
				}
				else
				{
					y = Mathf.FloorToInt(spriteRect.y + border.y +
					                     ((localPosition.y - border.y) /
					 (maskRect.height - border.y - border.w)) *
					                     (spriteRect.height - border.y - border.w));
				}
			}
				break;
			case Image.Type.Simple:
			default:
			{
				// conversion to uniform UV space
				x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
				y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
			}
				break;
			}
			
			return new Vector2( x, y );
		}
		/// <summary>
		/// Returns the texts from the toggles which /isOn/ value equals the specified. 
		/// </summary>
		public static List<string> GetTextsFrom( this Toggle[] toggles, bool isOn = true )
		{
			if( toggles == null )
				return null;
			List<string> list = new List<string>( toggles.Length );
			for( int i=0; i<toggles.Length; i++ )
			{
				if( !toggles[ i ] )
					continue;
				if( toggles[ i ].isOn == isOn )
				{
					list.Add( toggles[ i ].GetComponentInChildren<Text>().text );
				}
			}
			return list;
		}
		/// <summary>
		/// Returns the index of the first toggle that /isOn/. This returns -1 if no toggle is on or this array is 
		/// null, empty, or with not even a single valid toggle reference.
		/// </summary>
		public static int GetIndex( this Toggle[] toggles, bool isOn = true )
		{
			if( toggles == null )
				return -1;
			for( int i=0; i<toggles.Length; i++ )
			{
				if( !toggles[ i ] )
					continue;
				if( toggles[ i ].isOn == isOn )
				{
					return i;
				}
			}
			return -1;
		}
		#endregion GET

		#region ANIMATIONS
		/// <summary>
		/// Animates the sibling images color's alpha value.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
		/// <param name="includeSiblingsSubChildren">If set to <c>true</c> include siblings sub children.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive objects.</param>
		/// <param name="includeThis">If set to <c>true</c> include this graphic.</param>
		public static void AnimateSiblingGraphicsAlpha( this GameObject graphic, float target, float duration, 
		                                               bool includeSiblingsSubChildren = false, bool includeInactive = false, bool includeThis = false )
		{
			var graphics = graphic.GetParent().GetChildren( includeSiblingsSubChildren, includeInactive ).ToList();
			if( !includeThis )
			{
				graphics.Remove( graphic );
			}
			for( int i=0; i<graphics.Count; i++ )
			{
				var g = graphics[i].GetComponent<Graphic>();
				if( g )
				{
					g.AnimAlpha( target, duration );
				}
			}
		}		
		/// <summary>
		/// Animates the sibling images color's value.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="avoidAlpha">If set to <c>true</c> the alpha channel is not taken into account.</param>
		/// <param name="includeSiblingsSubChildren">If set to <c>true</c> include siblings sub children.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive objects.</param>
		/// <param name="includeThis">If set to <c>true</c> include this graphic.</param>
		public static void AnimateSiblingGraphicsColor( this GameObject graphic, Color target, float duration, bool avoidAlpha = true, 
		                                               bool includeSiblingsSubChildren = false, bool includeInactive = false, bool includeThis = false )
		{
			var graphics = graphic.GetParent().GetChildren( includeSiblingsSubChildren, includeInactive ).ToList();
			if( !includeThis )
			{
				graphics.Remove( graphic );
			}
			for( int i=0; i<graphics.Count; i++ )
			{
				var g = graphics[i].GetComponent<Graphic>();
				if( g )
				{
                    g.AnimColor( target, duration, avoidAlpha ).Run();
				}
			}
		}
		/// <summary>
		/// Animates the scale of the specified ui element
		/// </summary>
		/// <param name="uiElement">User interface element.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator<float> AnimScale( this Graphic uiElement, Vector3 targetScl, float duration, 
		                                    bool toggleAnimator = true, UnityAction callback = null )
		{
			if( !uiElement )
				yield break;
			if( toggleAnimator )
			{
				uiElement.ToggleAnimator();
			}
			Vector3 currentScl = uiElement.transform.localScale;
            Vector3 initialScl = currentScl;
            float t = Time.deltaTime;
            while ( t <= duration )
			{
                currentScl = Vector3.Lerp( initialScl, targetScl, t / duration );
                yield return 0f;
				if( uiElement )//IT MIGHT GET DESTROYED WHILE ANIMATING
					uiElement.transform.localScale = currentScl;
				else break;
                t += Time.deltaTime;
			}
            if( uiElement )
                uiElement.transform.localScale = targetScl;
			if( callback == null )
				yield break;
			callback.Invoke();
		}		
		/// <summary>
		/// Animates the color of the specified ui element
		/// </summary>
		/// <param name="uiElement">User interface element.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
        /// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColor( this Graphic uiElement, Color target, float duration = 0.4f,
		                                    bool avoidAlpha = true, UnityAction callback = null )
		{
			if( !uiElement )
				yield break;

            Color iniColor = uiElement.color;
            Color current = iniColor;
            float t = Time.deltaTime;
            float wait = (duration * 0.1f).Clamp( 0.05f, 0.2f );//This will improve this coroutine's performance
            while( t <= duration )
            {
                if( !uiElement )//IT MIGHT GET DESTROYED WHILE ANIMATING
                {
                    yield break;
                }
                current = iniColor.Lerp( target, t / duration, avoidAlpha ); 
                uiElement.color = current;
                yield return Timing.WaitForSeconds( wait );
                t += wait;
            }
            if( uiElement )
            {
                if( avoidAlpha )
                {
                    target.a = iniColor.a;
                }
                uiElement.color = target;
            }
            if( callback == null )
				yield break;
			callback.Invoke();
		}
        /// <summary>
        /// Animates the color (back and forth) of the specified ui element
        /// </summary>
        /// <param name="uiElement">User interface element.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColorPingPong( this Graphic uiElement, Color target, float duration = 0.4f,
            bool avoidAlpha = true, int repeat = 0, UnityAction callback = null )
        {
            if( !uiElement )
                yield break;
            
            Color iniColor = uiElement.color;
            for( int i=0; i<repeat + 1; i++ )
            {
                yield return Timing.WaitUntilDone( uiElement.AnimColor( target, duration * 0.5f, avoidAlpha ).Run() );
                yield return Timing.WaitUntilDone( uiElement.AnimColor( iniColor, duration * 0.5f, avoidAlpha ).Run() );
            }

            if( !uiElement || callback == null )
                yield break;
            callback.Invoke();
        }
		/// <summary>
		/// Animates the alpha value of the specified ui element
		/// </summary>
		/// <param name="uiElement">User interface element.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
		public static void AnimAlpha( this Graphic uiElement, float target, float duration = 0.4f, UnityAction callback = null )
		{
            uiElement.AnimAlphaCo( target, duration, callback ).Run();
		}
		/// <summary>
        /// Animates the alpha value of the specified ui element. Use _AnimAlphaCo() which is optimized for performance.
		/// </summary>
		/// <param name="uiElement">User interface element.</param>
		/// <param name="target">Target.</param>
		/// <param name="duration">Animation Duration.</param>
        public static IEnumerator<float> AnimAlphaCo( this Graphic uiElement, float target, float duration = 0.4f, UnityAction callback = null )
		{
			Color current = uiElement.color;
			var targetColor = new Color( current.r, current.g, current.b, target );
            yield return Timing.WaitUntilDone( uiElement.AnimColor( targetColor, duration, false, callback ).Run() );
		}
        /// <summary>
        /// Animates the alpha value of the specified ui element
        /// </summary>
        /// <param name="uiElement">User interface element.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        public static IEnumerator<float> _AnimAlphaCo( this Graphic uiElement, float target, float duration = 0.4f, UnityAction callback = null )
        {
            Color current = uiElement.color;
            var targetColor = new Color( current.r, current.g, current.b, target );
            yield return Timing.WaitUntilDone( uiElement.AnimColor( targetColor, duration, false, callback ).Run() );
        }
        /// <summary>
        /// Animates the alpha of the specified object's graphic until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this Graphic obj, float targetValue, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( !obj )
                yield break;
            if( toggleAnimator )
            {
                obj.gameObject.ToggleAnimator();
            }

            ValidationFlag validationFlags = obj.gameObject.AddGetComponent<ValidationFlag>();
            if( validationFlags.IsFlagged( ValidationFlag.Flags.Alpha ) )
                yield break;
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, true );

            if( multiplier == null || multiplier.keys.Length <= 1 )
                multiplier = AnimationCurve.Linear( 0f, 1f, duration, 1f );
            targetValue *= multiplier.keys[ multiplier.keys.Length - 1 ].value;

            float initialValue = obj.color.a;
            float time = 0;
            Color color;
            while( time < duration )
            {
                time += Time.deltaTime;
                float multiply = multiplier.Evaluate( Mathf.Lerp( 0f, 
                    multiplier.keys[ multiplier.keys.Length - 1 ].time, time / duration ) );
                color = obj.color;
                color.a = Mathf.Lerp( initialValue, targetValue, time / duration ) * multiply;
                obj.color = color;
                yield return null;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Alpha ) )
                    yield break;
                if( !obj )//IT MIGHT GET DESTROYED WHILE ANIMATING
                    break;
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        /// <summary>
        /// Animates the alpha of the specified object's graphic until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this Graphic[] objs, float targetValue, float duration, 
            AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].AnimAlphaWithMultiplier( targetValue, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }
        /// <summary>
        /// Animates the alpha of the specified object's graphic until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this SearchableGraphic[] objs, float targetValue, 
            float duration, AnimationCurve multiplier, bool toggleAnimator = false )
        {
            if( objs == null )
                yield break;
            for( int i=0; i<objs.Length; i++ )
            {
                objs[ i ].m_gameObject.AnimAlphaWithMultiplier( targetValue, duration, multiplier, toggleAnimator ).Start();
            }
            yield return new WaitForSeconds( duration );
        }

		/// <summary>
		/// Blinks the specified graphic, until it reaches the specified target value the specified amount of /repeat/.
		/// </summary>
		/// <param name="blinkDuration"> The duration of each blink </param>
		public static void Blink( this Graphic graphic, float target, float blinkDuration, int repeat = 1 )
		{
			if( !graphic )
				return;
            graphic.BlinkCo( target, blinkDuration, repeat ).Run();
		}
		/// <summary>
		/// Blinks the specified graphic, until it reaches the specified target value the specified amount of /repeat/.
		/// </summary>
		/// <param name="blinkDuration"> The duration of each blink </param>
        public static IEnumerator<float> BlinkCo( this Graphic graphic, float target, float blinkDuration, int repeat = 1 )
		{
			if( !graphic )
				yield break;
			float iniAlpha = graphic.color.a;
			for( int i=0; i<repeat + 1; i++ )
			{
                yield return Timing.WaitUntilDone( graphic.AnimAlphaCo( target, blinkDuration * 0.5f ).Run() );
                yield return Timing.WaitUntilDone( graphic.AnimAlphaCo( iniAlpha, blinkDuration * 0.5f ).Run() );
			}
		}	
		/// <summary>
		/// Fades the specified graphics, until they reach the specified target value.
		/// </summary>
		public static void Blink( this IList<Graphic> graphics, float target, float blinkDuration, int repeat = 1 )
		{
			if( graphics == null )
				return;
			graphics.BlinkCo( target, blinkDuration, repeat ).Start();
		}
		/// <summary>
		/// Fades the specified graphics, until they reach the specified target value.
		/// </summary>
        public static IEnumerator<float> BlinkCo( this IList<Graphic> graphics, float target, float blinkDuration, int repeat = 1 )
		{
			if( graphics == null )
				yield break;
			for( int i=0; i<graphics.Count; i++ )
			{
                graphics[i].BlinkCo( target, blinkDuration, repeat ).Run();
			}
			yield return Timing.WaitForSeconds( blinkDuration * ( repeat + 1 ) );
		}
		/// <summary>
		/// Animation the specified text /from/ /to/ in the specified amount of time. The /updateRate/ indicates the
		/// text's text update frequency in frames, lower values might cause overhead.
		/// </summary>
		public static void Anim( this Text text, int from, int to, float duration = 1f, int updateRate = 4 )
		{
			text.AnimCo( from, to, duration, updateRate ).Start();
		}
		/// <summary>
		/// Animation the specified text /from/ /to/ in the specified amount of time. The /updateRate/ indicates the
		/// text's text update frequency in frames, lower values might cause overhead.
		/// </summary>
		public static IEnumerator AnimCo( this Text text, int from, int to, float duration = 1f, int updateRate = 4 )
		{
			float time = 0f;
			int currentFrame = 0;
			while( time < duration )
			{
				time += Time.deltaTime;
				currentFrame++;
				int next = (int) Mathf.Lerp( from, to, time / duration );
				if( currentFrame % updateRate == 0 )
				{
					text.text = next.ToString();
				}
				yield return null;
			}
			text.text = to.ToString();
		}
        public static void AnimSize( this RectTransform rt, Vector2 size, float duration )
        {
            rt.AnimSizeCo( size, duration ).Start();
        }
        public static IEnumerator AnimSizeCo( this RectTransform rt, Vector2 size, float duration )
        {
            float t = 0f;
            Vector2 _size = rt.sizeDelta;
            while( t < duration )
            {
                t += Time.deltaTime;
                rt.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, Mathf.Lerp( _size.x, size.x, t / duration ) );
                rt.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, Mathf.Lerp( _size.y, size.y, t / duration ) );
                yield return null;
            }
        }       
        public static void AnimSize( this RectTransform rt, float x, float y, float duration )
        {
            rt.AnimSize( new Vector2( x, y ), duration );
        }
        public static IEnumerator AnimSizeCo( this RectTransform rt, float x, float y, float duration )
        {
            yield return rt.AnimSizeCo( new Vector2( x, y ), duration ).Start();
        }
		/// <summary>
		/// Animates the specified Text's words's color one by one until the last word has the specified /targetColor/. 
        /// This uses Rich Text
		/// </summary>
        public static IEnumerator<float> AnimTextWordsColor( this Text text, Color initialColor, Color targetColor, float duration )
		{
			if( !text )
				yield break;
			text.supportRichText = true;
			string[] words = text.text.GetWords();
            float t = Time.deltaTime;
            float wait = (duration * 0.1f).Clamp( 0.05f, 0.2f );
			for( int i=0; i<words.Length; i++ )
			{
				if( string.IsNullOrEmpty( words[ i ] ) )
					continue;
                while( true )
                {
                    words[ i ] = words[ i ].LerpColor( initialColor, targetColor, t );
                    text.text = "";
                    for( int j=0; j<words.Length; j++ )
                    {
                        text.text += words[ j ] + " ";
                    }
                    if( t > duration )
                        break;
                    yield return Timing.WaitForSeconds( wait );
                    t += wait;
                }
			}
		}
		/// <summary>
		/// Animates the specified Text's words's color one by one towards /targetcolor/ and back to /initialColor/. 
        /// This uses Rich Text
		/// </summary>
		/// <param name="stayDuration"> The amount of time the word will stay with the /targetColor/ before going back to the /initialColor/ </param>
        public static IEnumerator<float> AnimTextWordsColor( this Text text, Color initialColor, Color targetColor, 
		                                             float duration, float stayDuration )
		{
            if( !text )
                yield break;
            text.supportRichText = true;
            string[] words = text.text.GetWords();
            float t = Time.deltaTime;
            float wait = (duration * 0.1f).Clamp( 0.05f, 0.2f );
            for( int i=0; i<words.Length; i++ )
            {
                if( string.IsNullOrEmpty( words[ i ] ) )
                    continue;
                while( true )
                {
                    words[ i ] = words[ i ].LerpColor( initialColor, targetColor, t );
                    text.text = "";
                    for( int j=0; j<words.Length; j++ )
                    {
                        text.text += words[ j ] + " ";
                    }
                    if( t > duration )
                        break;
                    yield return Timing.WaitForSeconds( wait );
                    t += wait;
                }
                yield return Timing.WaitForSeconds( stayDuration );
                while( true )
                {
                    words[ i ] = words[ i ].LerpColor( targetColor, initialColor, t );
                    text.text = "";
                    for( int j=0; j<words.Length; j++ )
                    {
                        text.text += words[ j ] + " ";
                    }
                    if( t > duration )
                        break;
                    yield return Timing.WaitForSeconds( wait );
                    t += wait;
                }
            }
		}
		/// <summary>
		/// Animates the specified Text's words's size one by one towards /from/ and back to /to/. This uses Rich Text
		/// </summary>
		/// <param name="stayDuration"> The amount of time the word will stay with the /to/ before going back to the /from/ </param>
		public static IEnumerator AnimTextWordsSize( this Text text, int from, int to, 
		                                             float duration, float stayDuration )
		{
			if( !text )
				yield break;
			text.supportRichText = true;
			string[] words = text.text.GetWords();
			for( int i=0; i<words.Length; i++ )
			{
				if( string.IsNullOrEmpty( words[ i ] ) )
					continue;
				foreach( string scaledWord in words[ i ].AnimSizeCo( from, to, duration ) )
				{
					words[ i ] = scaledWord;
					text.text = "";
					for( int j=0; j<words.Length; j++ )
					{
						text.text += words[ j ] + " ";
					}
					yield return null;
				}
				yield return new WaitForSeconds( stayDuration );
				foreach( string scaledWord in words[ i ].AnimSizeCo( to, from, duration ) )
				{
					words[ i ] = scaledWord;
					text.text = "";
					for( int j=0; j<words.Length; j++ )
					{
						text.text += words[ j ] + " ";
					}
					yield return null;
				}
			}
		}
		/// <summary>
		/// Animates the specified Text's words's color one by one towards /targetcolor/ and back to /initialColor/. This uses Rich Text
		/// </summary>
		/// <param name="stayDuration"> The amount of time the word will stay with the /targetColor/ before going back to the /initialColor/ </param>
		public static IEnumerator AnimTextWordsColorAndSize( this Text text, Color initialColor, Color targetColor,
		                                                    int from, int to, float duration, float stayDuration )
		{
			if( !text )
				yield break;
			text.supportRichText = true;
			string[] words = text.text.GetWords();
			for( int i=0; i<words.Length; i++ )
			{
				if( string.IsNullOrEmpty( words[ i ] ) )
					continue;
				foreach( string newWord in words[ i ].AnimSizeAndColorCo( from, to, initialColor, targetColor, duration ) )
				{
					words[ i ] = newWord;
					text.text = "";
					for( int j=0; j<words.Length; j++ )
					{
						text.text += words[ j ] + " ";
					}
					yield return null;
				}
				yield return new WaitForSeconds( stayDuration );
				foreach( string newWord in words[ i ].AnimSizeAndColorCo( to, from, targetColor, initialColor, duration ) )
				{
					words[ i ] = newWord;
					text.text = "";
					for( int j=0; j<words.Length; j++ )
					{
						text.text += words[ j ] + " ";
					}
					yield return null;
				}
			}
		}
		public static void Animate( this Slider slider, float target, float duration = 1f )
		{
			if( !slider )
				return;
			slider.AnimateCo( target, duration ).Run();
		}
		public static IEnumerator<float> AnimateCo( this Slider slider, float target, float duration = 1f )
		{
			if( !slider )
				yield break;
			float time = 0f;
			float a = slider.value;
			while( time < duration )
			{
				time += Time.deltaTime;
				slider.value = Mathf.Lerp( a, target, time / duration );
				if( !slider )
					break;
				yield return 0f;
			}
			slider.value = target;
		}
		#endregion ANIMATIONS

		#region BLOCK
		/// <summary>
		/// Sets the interactable button property for the specified button.
		/// </summary>
		/// <param name="obj">The object containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void BlockInteractableFor( this GameObject obj, float seconds, float delay = 0f )
		{
			obj._BlockInteractableFor( seconds, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for the specified button.
		/// </summary>
		/// <param name="obj">The object containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _BlockInteractableFor( this GameObject obj, float seconds, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			obj.GetComponent<Button>().interactable = false;
			yield return new WaitForSeconds( seconds );
			if( obj )
			{
				obj.GetComponent<Button>().interactable = true;
			}
		}
		/// <summary>
		/// Sets the interactable button property for all active buttons in the scene.
		/// </summary>
		/// <param name="script">Any script.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void BlockAllInteractablesFor( this Behaviour script, float seconds, float delay = 0f )
		{
			script._BlockAllInteractablesFor( seconds, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for all active buttons in the scene.
		/// </summary>
		/// <param name="script">Any script.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _BlockAllInteractablesFor( this Behaviour script, float seconds, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			var allButtons = GameObject.FindObjectsOfType<Button>();
			for( int i=0; i<allButtons.Length; i++ )
			{
				allButtons[i].interactable = false;
			}
			yield return new WaitForSeconds( seconds );
			for( int i=0; i<allButtons.Length; i++ )
			{
				if( allButtons[i] != null )
				{
					allButtons[i].interactable = true;
				}
			}
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		public static void BlockInteractablesFor( this IList<GameObject> objs, float seconds, float delay = 0f )
		{
			objs._BlockInteractablesFor( seconds, delay ).Start();
		}
		/// <summary>
		/// Sets the interactable button property for all buttons in the specified list.
		/// </summary>
		/// <param name="obj">The objects containing the Button component.</param>
		/// <param name="interactable">If set to <c>true</c> interactable.</param>
		/// <param name="delay">Delay.</param>
		private static IEnumerator _BlockInteractablesFor( this IList<GameObject> objs, float seconds, float delay = 0f )
		{
			yield return new WaitForSeconds( delay );
			for( int i=0; i<objs.Count; i++ )
			{
				objs[i].GetComponent<Button>().interactable = false;
			}
			yield return new WaitForSeconds( seconds );
			for( int i=0; i<objs.Count; i++ )
			{
				if( objs[i] != null )
				{
					objs[i].GetComponent<Button>().interactable = true;
				}
			}
		}
		#endregion BLOCK

		#region MISC
		/// <summary>
		/// Returns true if this was the last selected gameobject.
		/// </summary>
		/// <param name="graphic">Graphic.</param>
		public static bool WasLastSelected( this Button graphic )
		{
			#if UNITY_5
			return ( graphic.gameObject == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject );
			#else
			return ( graphic.gameObject == UnityEngine.EventSystems.EventSystem.current.lastSelectedGameObject );
			#endif
		}
		public static void ToggleAlpha( this Graphic img )
		{
			if( img )
			{
				var alpha = ( img.color.a == 1f ) ? 0.5f : 1f;
				img.color = new Color( img.color.r, img.color.g, img.color.b, alpha );
			}
		}	
		/// <summary>
		/// Enables or disables the Button's Animator if its transition is set to Animation.
		/// </summary>
		/// <param name="graphic">Object.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public static void ToggleAnimator( this Graphic graphic )
		{
			var bt = graphic.GetComponent<Button>();
			if( bt )
			{
				var animator = graphic.GetComponent<Animator>();
				if( animator != null && bt.transition == Selectable.Transition.Animation )
				{
					animator.enabled = !animator.enabled;
				}
			}
		}
		public static CanvasGroup IgnoreElement( this Graphic uiElement, bool ignore = true )
		{
			if( ignore )
			{
				var ignorer = uiElement.GetOrAddComponent<CanvasGroup>();
				ignorer.blocksRaycasts = false;
				ignorer.interactable = false;
				return ignorer;
			}
			else
			{
				var ignorer = uiElement.GetComponent<CanvasGroup>();
				if( ignorer )
				{
					MonoBehaviour.Destroy( ignorer );
				}
				return null;
			}
		}		
        /// <summary>
        /// Sets the /uiElement/'s CanvasGroup (add one if it doesn't have it) to blocksRaycasts = false, andinteractable = false.
        /// </summary>
		public static CanvasGroup IgnoreElement( this GameObject uiElement, bool ignore = true )
		{
			if( ignore )
			{
				var ignorer = uiElement.AddGetComponent<CanvasGroup>();
				ignorer.blocksRaycasts = false;
				ignorer.interactable = false;
				return ignorer;
			}
			else
			{
				var ignorer = uiElement.GetComponent<CanvasGroup>();
				if( ignorer )
				{
					MonoBehaviour.Destroy( ignorer );
				}
				return null;
			}
		}
		/// <summary>
		/// Returns the position of the UI element.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="world">If set to <c>true</c> returns the world space position.</param>
		public static Vector3 Position( this UIBehaviour element, bool world = true )
		{
			if( world )
			{
				return element.transform.position;
			}
			else return element.transform.localPosition;
		}		
		/// <summary>
		/// Returns the Rect of the UI element.
		/// </summary>
		/// <param name="element">Element.</param>
		public static Rect Rect( this UIBehaviour element )
		{
			return element.GetComponent<RectTransform>().rect;
		}		
		public static float Width( this UIBehaviour element )
		{
			return element.Rect().width;
		}		
		public static float Height( this UIBehaviour element )
		{
			return element.Rect().height;
		}
		/// <summary>
		/// Determines if raycast location is valid on the specified image. This also checks for transparent pixels
		/// </summary>
		/// <returns><c>true</c> if raycast location is valid for the specified image; otherwise, <c>false</c>.</returns>
		/// <param name="img">Image.</param>
		/// <param name="eventCamera">Event camera.</param>
		public static bool IsRaycastPosValid( this Image img, Camera eventCamera )
		{
			var sprite = img.sprite;
			
			var rectTransform = (RectTransform)img.transform;
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) img.transform, Input.mousePosition, eventCamera, out localPositionPivotRelative);
			
			// convert to bottom-left origin coordinates
			var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x*rectTransform.rect.width,
			                                localPositionPivotRelative.y + rectTransform.pivot.y*rectTransform.rect.height);
			
			var spriteRect = sprite.textureRect;
			var maskRect = rectTransform.rect;
			
			var x = 0;
			var y = 0;
			// convert to texture space
			switch (img.type)
			{
				
			case Image.Type.Sliced:
			{
				var border = sprite.border;
				// x slicing
				if (localPosition.x < border.x)
				{
					x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
				}
				else if (localPosition.x > maskRect.width - border.z)
				{
					x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
				}
				else
				{
					x = Mathf.FloorToInt(spriteRect.x + border.x +
					                     ((localPosition.x - border.x)/
					 (maskRect.width - border.x - border.z)) *
					                     (spriteRect.width - border.x - border.z));
				}
				// y slicing
				if (localPosition.y < border.y)
				{
					y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
				}
				else if (localPosition.y > maskRect.height - border.w)
				{
					y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
				}
				else
				{
					y = Mathf.FloorToInt(spriteRect.y + border.y +
					                     ((localPosition.y - border.y) /
					 (maskRect.height - border.y - border.w)) *
					                     (spriteRect.height - border.y - border.w));
				}
			}
				break;
			case Image.Type.Simple:
			default:
			{
				// conversion to uniform UV space
				x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
				y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
			}
				break;
			}
			
			try
			{
				return sprite.texture.GetPixel(x,y).a > 0;
			}
			catch (UnityException)
			{
				Debug.LogError("The specified image's texture is not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
				return false;
			}
		}
		/// <summary>
		/// Determines if this UI element is inside its canvas.
		/// </summary>
		public static bool IsInsideItsCanvas( this GameObject uiObj )
		{
			if( !uiObj )
				return false;
			var rt = uiObj.GetComponent<RectTransform>();
			if( !rt )
				return false;
			var canvas = uiObj.GetCompInParent<Canvas>().GetComponent<RectTransform>();
			
            if( canvas.rect.Contains( rt.localPosition ) )
			{
                return true;
			}
            return false;
		}
		/// <summary>
		/// Returns a new list without the Non-Interactable buttons.
		/// </summary>
		public static List<Button> RemoveNonInteractable( this IList<Button> buttons )
		{
			List<Button> newList = new List<Button>(buttons);
			for( int i=0; i<buttons.Count; i++ )
			{
				if( !buttons[i].interactable )
				{
					newList.Remove( buttons[i] );
				}
			}
			return newList;
		}
		public static void InvokeAllOnClick( this IList<Button> buttons )
		{
			for( int i=0; i<buttons.Count; i++ )
			{
				if( !buttons[i] )
					continue;
				buttons[i].onClick.Invoke();
			}
		}
		/// <summary>
		/// Adds an eventTrigger component with the specified action to the specified element. NOTE: Draggable Events are
		/// not supported.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="action">Action.</param>
		/// <param name="eventTriggerType">Event trigger type.</param>
		public static void AddEvent( this UIBehaviour element, UnityAction<BaseEventData> action, 
		                            EventTriggerType eventTriggerType = EventTriggerType.PointerUp )
		{
			if( element == null )
				return;
			EventTriggersPointers pointerEvents;
			EventTriggersDraggers dragEvents;
			
			switch( eventTriggerType )
			{
			case EventTriggerType.BeginDrag: dragEvents = element.GetOrAddComponent<EventTriggersDraggers>();
				dragEvents.m_onBeginDrag += action;
				Utilities.LogWarning("Drag Handlers should be avoided since they give issues with ScrollRects");
				break;
            case EventTriggerType.Cancel: Utilities.LogWarning( "Event Trigger Cancel: Not yet implemented" );
				break;
            case EventTriggerType.Deselect: Utilities.LogWarning( "Event Trigger Deselect: Not yet implemented" );
				break;
			case EventTriggerType.Drag: dragEvents = element.GetOrAddComponent<EventTriggersDraggers>();
				dragEvents.m_onDrag += action;
                Utilities.LogWarning("Drag Handlers should be avoided since they give issues with ScrollRects");
				break;
            case EventTriggerType.Drop: Utilities.LogWarning( "Event Trigger Drop: Not yet implemented" );
				break;
			case EventTriggerType.EndDrag: dragEvents = element.GetOrAddComponent<EventTriggersDraggers>();
				dragEvents.m_onEndDrag += action;
                Utilities.LogWarning("Drag Handlers should be avoided since they give issues with ScrollRects");
				break;
			case EventTriggerType.InitializePotentialDrag: dragEvents = element.GetOrAddComponent<EventTriggersDraggers>();
				dragEvents.m_onInitializePotentialDrag += action;
                Utilities.LogWarning("Drag Handlers should be avoided since they give issues with ScrollRects");
				break;
            case EventTriggerType.Move: Utilities.LogWarning( "Event Trigger Move: Not yet implemented" );
				break;
			case EventTriggerType.PointerClick: pointerEvents = element.GetOrAddComponent<EventTriggersPointers>();
				break;
			case EventTriggerType.PointerDown: pointerEvents = element.GetOrAddComponent<EventTriggersPointers>();
				pointerEvents.m_onPointerDown += action;
				break;
			case EventTriggerType.PointerEnter: pointerEvents = element.GetOrAddComponent<EventTriggersPointers>();
				pointerEvents.m_onPointerEnter += action;
				break;
			case EventTriggerType.PointerExit: pointerEvents = element.GetOrAddComponent<EventTriggersPointers>();
				pointerEvents.m_onPointerExit += action;
				break;
			case EventTriggerType.PointerUp: pointerEvents = element.GetOrAddComponent<EventTriggersPointers>();
				pointerEvents.m_onPointerUp += action;
				break;
            case EventTriggerType.Scroll: Utilities.LogWarning( "Event Trigger Scroll: Not yet implemented" );
				break;
            case EventTriggerType.Select: Utilities.LogWarning( "Event Trigger Select: Not yet implemented" );
				break;
            case EventTriggerType.Submit: Utilities.LogWarning( "Event Trigger Submit: Not yet implemented" );
				break;
            case EventTriggerType.UpdateSelected: Utilities.LogWarning( "Event Trigger Update Selected: Not yet implemented" );
				break;
			}
		}
		/// <summary>
		/// Adds an eventTrigger component to the specified element.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="action">Action.</param>
		/// <param name="eventTriggerType">Event trigger type.</param>
		public static void AddEvents( this IList<UIBehaviour> elements, UnityAction<BaseEventData> action, 
		                             EventTriggerType eventTriggerType = EventTriggerType.PointerUp )
		{
			for( int i=0; i<elements.Count; i++ )
			{
				elements[i].AddEvent( action, eventTriggerType );
			}
		}
		/// <summary>
		/// Gets and/or Adds an EventTrigger component to the specified element. The event is returned so a listener
		/// can be added. NOTE: EventTrigger are known to have issues with ScrollRects.
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="element">Element.</param>
		/// <param name="eventTriggerType">Event trigger type.</param>
		public static EventTrigger.TriggerEvent AddGetEventTrigger( this UIBehaviour element, EventTriggerType eventTriggerType = EventTriggerType.PointerUp )
		{
			
			if( element == null )
				return null;
			EventTrigger eventTrigger = element.GetOrAddComponent<EventTrigger>();
			
			EventTrigger.Entry pointerEntry = new EventTrigger.Entry();
			EventTrigger.TriggerEvent pointerEvent = new EventTrigger.TriggerEvent();
			
			pointerEntry.eventID = eventTriggerType;
			pointerEntry.callback = pointerEvent;
			
			if( eventTrigger.triggers == null )
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}	
			eventTrigger.triggers.Add(pointerEntry);
			return pointerEvent;
		}		
		public static void RemoveAllListeners( this Button button )
		{
			if( !button )
				return;
			button.onClick.RemoveAllListeners();
		}
		public static void RemoveAllListenersInChildren( this GameObject btsParent )
		{
			if( !btsParent )
				return;
			Button[] children = btsParent.GetComponentsInChildren<Button>();
			for( int i=0; i<children.Length; i++ )
			{
				children[i].RemoveAllListeners();
			}
		}
		/// <summary>
		/// Returns true if all toggle buttons are off.
		/// </summary>
		public static bool AreAllOff( this IList<Toggle> toggleButtons )
		{
			return toggleButtons.AreAllOff( null );
		}
		/// <summary>
		/// Returns true if all toggle buttons are off.
		/// </summary>
		public static bool AreAllOff( this IList<Toggle> toggleButtons, Toggle skip )
		{
			if( toggleButtons == null )
				return true;
			for( int i=0; i<toggleButtons.Count; i++ )
			{
				if( skip == toggleButtons[ i ] )
					continue;
				if( toggleButtons[i].isOn )
					return false;
			}
			return true;
		}
		/// <summary>
		/// Returns false if this Text or its Font is null.
		/// </summary>
		public static bool ValidateFont( this Text text )
		{
			if( !text )
			{
                Utilities.LogWarning( Constants.NULL_TEXT_MSG );
				return false;
			}
			if( !text.font )
			{
                Utilities.LogWarning( "You're missing the Font on: "+text.name, text.gameObject );
				return false;
			}
			return true;
		}
		#endregion MISC		
	}
}