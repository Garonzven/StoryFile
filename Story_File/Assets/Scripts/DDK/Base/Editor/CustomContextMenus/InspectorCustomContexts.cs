//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using UnityEditor;
using DDK.Base.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


namespace DDK {

	public class InspectorCustomContexts {
		
		#region GRAPHIC
		[MenuItem ("CONTEXT/Graphic/Apply Alpha to Siblings")]
		static void Graphic_ApplyAlphaToSiblings() {
			
			ApplyAlphaToSiblings();		
		}
		
		[MenuItem ("CONTEXT/Graphic/Apply Alpha to Siblings - Include SubChildren")]
		static void Graphic_ApplyAlphaToSiblingsISC() {
			
			ApplyAlphaToSiblings( true );		
		}
		
		[MenuItem ("CONTEXT/Graphic/Apply Alpha to Siblings - Include Inactive")]
		static void Graphic_ApplyAlphaToSiblingsII() {
			
			ApplyAlphaToSiblings( false, true );		
		}
		
		[MenuItem ("CONTEXT/Graphic/Apply Alpha to Siblings - Include SubChildren and Inactive")]
		static void Graphic_ApplyAlphaToSiblingsISCI() {
			
			ApplyAlphaToSiblings( true, true );		
		}
		#endregion
		
		#region SPRITE RENDERER
		[MenuItem ("CONTEXT/SpriteRenderer/Apply Alpha to Siblings")]
		static void Graphic_ApplyAlphaToSiblingSprites() {
			
			ApplyAlphaToSiblings();		
		}
		
		[MenuItem ("CONTEXT/SpriteRenderer/Apply Alpha to Siblings - Include SubChildren")]
		static void Graphic_ApplyAlphaToSiblingSpritesISC() {
			
			ApplyAlphaToSiblings( true );		
		}
		
		[MenuItem ("CONTEXT/SpriteRenderer/Apply Alpha to Siblings - Include Inactive")]
		static void Graphic_ApplyAlphaToSiblingSpritesII() {
			
			ApplyAlphaToSiblings( false, true );		
		}
		
		[MenuItem ("CONTEXT/SpriteRenderer/Apply Alpha to Siblings - Include SubChildren and Inactive")]
		static void Graphic_ApplyAlphaToSiblingSpritesISCI() {
			
			ApplyAlphaToSiblings( true, true );		
		}
		#endregion

		#region BEHAVIOURS
		/*static List<Behaviour> copiedComps = new List<Behaviour>();

		[MenuItem ("CONTEXT/Behaviour/Copied Components: Include (copy)", false, 0)]
		static void Behaviour_CopiedComponentsAdd( MenuCommand command ) {
			
			CopiedComponentsAdd( command );		
		}
		[MenuItem ("CONTEXT/Behaviour/Copied Components: Remove from Copied (uncopy)", false, 1)]
		static void Behaviour_CopiedComponentsRemove( MenuCommand command ) {
			
			CopiedComponentsRemove( command );		
		}
		[MenuItem ("CONTEXT/Behaviour/Copied Components: Reset Copied (remove all)", false, 2)]
		static void Behaviour_CopiedComponentsRemoveAll() {
			
			CopiedComponentsRemoveAll();		
		}
		[MenuItem ("CONTEXT/Behaviour/Copied Components: Paste All", false, 3)]
		static void Behaviour_CopiedComponentsPasteAll() {
			
			CopiedComponentsPasteAll();		
		}*/
		#endregion

		#region COMPONENT
		static List<Component> copiedComps = new List<Component>();
		
		[MenuItem ("CONTEXT/Component/Copied Components: Include (copy)", false, 501)]
		static void Component_CopiedComponentsAdd( MenuCommand command ) {
			
			CopiedComponentsAdd( command );
		}
		[MenuItem ("CONTEXT/Component/Copied Components: Remove from Copied (uncopy)", false, 502)]
		static void Component_CopiedComponentsRemove( MenuCommand command ) {
			
			CopiedComponentsRemove( command );
		}
		[MenuItem ("CONTEXT/Component/Copied Components: Reset Copied (remove all)", false, 503)]
		static void Component_CopiedComponentsRemoveAll() {
			
			CopiedComponentsRemoveAll();		
		}
		[MenuItem ("CONTEXT/Component/Copied Components: Paste All", false, 504)]
		static void Component_CopiedComponentsPasteAll() {
			
			CopiedComponentsPasteAll();		
		}
		[MenuItem ("CONTEXT/Component/Select All Objects Holding This..", false, 551)]
		static void Component_SelectAllHoldingThis( MenuCommand command ) {

			Debug.Log ("This should take a while..");
			Component comp = command.context as Component;
			var allComps = GameObject.FindObjectsOfType( comp.GetType() ) as Component[];
			List<GameObject> newSelection = new List<GameObject>();
			foreach( Component _comp in allComps )
			{
				newSelection.Add( _comp.gameObject );
			}
			Selection.objects = newSelection.ToArray() as UnityEngine.Object[];
			Debug.Log ( newSelection.Count +" Objects have been selected" );
			/*var allObjs = GameObject.FindObjectsOfType<GameObject>();
			List<GameObject> newSelection = new List<GameObject>();
			foreach( var obj in allObjs )
			{
				if( obj.GetComponent( comp.GetType() ) )
				{
					newSelection.Add( obj );
				}
			}
			Selection.objects = newSelection.ToArray() as UnityEngine.Object[];*/
		}
		/*[MenuItem ("CONTEXT/Component/Combine Children Meshes", false, 551)]
		static void GameObject_CombineChildrenMeshes() {
			
			CombineChildrenMeshes();		
		}*/
		#endregion
		
				
		
		
		
		static void ApplyAlphaToSiblings ( bool includeSubChildren = false, bool includeInactive = false )	
		{
			
			GameObject obj = Selection.activeGameObject;
			Graphic g = obj.GetComponent<Graphic>();
			if( g )
			{
				g.SetSiblingGraphicsAlpha( g.color.a, includeSubChildren, includeInactive );
				Debug.Log ( "Alpha ("+g.color.a+") Applied!" );
				EditorUtility.SetDirty(g);
			}
			else
			{
				SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
				if( sr )
				{
					sr.SetSiblingSpritesAlpha( sr.color.a, includeSubChildren, includeInactive );
					Debug.Log ( "Alpha ("+sr.color.a+") Applied!" );
					EditorUtility.SetDirty(sr);
				}
			}
			
		}



		/// <summary>
		/// Returns true if a duplicated element was removed.
		/// </summary>
		/// <param name="command">Command.</param>
		static bool CheckCopiedComponentsRemoveDuplicates( MenuCommand command )
		{
			int iniCount = copiedComps.Count;
			copiedComps = copiedComps.RemoveDuplicates<Component>();
			if( iniCount == copiedComps.Count )
			{
				return false;
			}
			return true;
		}

		static void CopiedComponentsAdd ( MenuCommand command )	
		{			
			Component comp = command.context as Component;
			if( comp )
			{
				copiedComps.Add( comp );
			}
			if( CheckCopiedComponentsRemoveDuplicates( command ) )
			{
				Debug.LogWarning("The component had already been copied..");
			}
			else Debug.Log ( "Component of type "+comp.GetType()+" has being ADDED to Copied Components" );
		}

		static void CopiedComponentsRemove ( MenuCommand command )	
		{			
			Component comp = command.context as Component;
			if( comp )
			{
				if( copiedComps.Contains( comp ) )
				{
					copiedComps.Remove( comp );
					Debug.Log ( "Component of type "+comp.GetType()+" has being REMOVED from Copied Components" );
				}
				else Debug.LogWarning("The component has not been copied..");
			}
		}

		static void CopiedComponentsRemoveAll ()	
		{			
			copiedComps.Clear();
			Debug.Log ( "Copied Components have been RESET" );	
			validator = 0;
		}

		static byte validator;//PREVENTS MULTIPLE EXECUTIONS
		static void CopiedComponentsPasteAll()
		{
			GameObject[] destinations = Selection.gameObjects;
			if( validator == 0 )
			{
				if( copiedComps.Count == 0 )
				{
					Debug.Log ( "NO components to PASTE" );
					return;
				}
				List<Component> addedComps = new List<Component>( copiedComps.Count );
				if( destinations != null )
				{
					foreach( var dest in destinations )
					{
						foreach( var comp in copiedComps )
						{
							bool isEnabled = true;
							if( comp is Behaviour )
							{
								isEnabled = (comp as Behaviour).enabled;
							}
							if( true )
							{
								addedComps.Add( comp.CopyTo( dest, isEnabled ) );
							}
							EditorUtility.SetDirty( dest );
						}
					}
					Debug.Log ( "Copied Components successfully PASTED to destination objects, Copied Components must be reset manually with Reset Copied context menu option" );
				}
			}
			else if( validator == destinations.Length )
			{
				validator = 0;
			}
			if( destinations.Length > 1 )
				validator++;
		}

		/*static void CombineChildrenMeshes()
		{
			MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			//MeshRenderer mRenderer = Selection.activeGameObject.GetComponentInChildren<MeshRenderer>();
			int i = 0;
			while (i < meshFilters.Length) 
			{
				combine[i].mesh = meshFilters[i].sharedMesh;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive( false );
				i++;
			}
			Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			//Selection.activeGameObject.GetComponent<MeshRenderer>().sharedMaterials = mRenderer.sharedMaterials;
			Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
			Selection.activeGameObject.SetActive( true );
		}*/
		
		
		
		/*static Object[] GetSelectedAudioclips()
		
	{
		
		return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		
	}*/
		
	}

}
