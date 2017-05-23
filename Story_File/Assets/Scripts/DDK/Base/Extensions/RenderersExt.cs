using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DDK.Base.Components;
using DDK.Base.Animations;
using DDK.Base.Classes;
using UnityEngine.Events;
using MovementEffects;


namespace DDK.Base.Extensions
{
    public static class RenderersExt 
    {
        /// <summary>
        /// Enables or disables the specified component, if it references a prefab the prefab instance will be searched;
        /// make sure the instance has a unique name if multiple instances are active.
        /// </summary>
        /// <param name="comp">Comp.</param>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        /// <param name="prefabCompId">Prefab comp identifier.</param>
        public static void SetEnabled( this Renderer comp, bool enabled = true, int prefabCompId = 0 )
        {
            if( comp.IsPrefab() )
            {
                var instance = comp.name.Find();
                if( instance )
                {
                    var cmps = instance.GetComponents( comp.GetType() );
                    if( cmps != null )
                    {
                        for( int j=0; j<cmps.Length; j++ )
                        {
                            if( j == prefabCompId )
                            {
                                var c = cmps[j] as Renderer;
                                c.enabled = enabled;
                            }
                        }
                    }
                }
            }
            else if( comp ) { 
                comp.enabled = enabled;
            }
        }
        public static void SetEnabled( this IList<Renderer> comps, bool enabled = true, int prefabCompId = 0 )
        {
            if( comps == null )
                return;
            for( int i=0; i<comps.Count; i++ )
            {
                comps[i].SetEnabled( enabled, prefabCompId );
            }
        }
        public static void SetAlpha( this SpriteRenderer sr, float alpha = 1f )
        {
            if( sr )
            {
                sr.color = new Color( sr.color.r, sr.color.g, sr.color.b, alpha );
            }
        }

        public static void SetAlpha( this SpriteRenderer[] renderers, float alpha = 1f )
        {
            for( int i=0; i<renderers.Length; i++ )
            {
                renderers[i].SetAlpha( alpha );
            }
        }
        /// <summary>
        /// Sets the sibling sprite renderer color's alpha value, including this renderer.
        /// </summary>
        /// <param name="graphic">Graphic.</param>
        /// <param name="alpha">The alpha value.</param>
        public static void SetSiblingSpritesAlpha( this SpriteRenderer spriteRenderer, float alpha, bool includeSiblingsSubChildren = false, 
            bool includeInactive = false, bool includeThis = false )
        {
            var graphics = spriteRenderer.gameObject.GetParent().GetChildren( includeSiblingsSubChildren, includeInactive ).ToList();
            if( !includeThis )
            {
                graphics.Remove( spriteRenderer.gameObject );
            }
            for( int i=0; i<graphics.Count; i++ )
            {
                var g = graphics[i].GetComponent<SpriteRenderer>();
                if( g )
                {
                    g.color = new Color( g.color.r, g.color.g, g.color.b, alpha );
                }
            }
        }
        public static void SetAlpha( this Renderer ren, float alpha = 1f, bool sharedMaterial = false )
        {
            if( !ren )
                return;
            if( ren is SpriteRenderer )
            {
                ( ( SpriteRenderer )ren ).SetAlpha( alpha );
            }
            else 
            {
                Material[] mats = ( sharedMaterial ) ? ren.sharedMaterials : ren.materials;
                mats.SetAlpha( alpha );
            }
        }

        public static void SetAlpha( this IList<Renderer> renderers, float alpha = 1f, bool sharedMaterial = false )
        {
            if( renderers == null )
                return;
            for( int i=0; i<renderers.Count; i++ )
            {
                renderers[i].SetAlpha( alpha, sharedMaterial );
            }
        }
        /// <summary>
        /// Fades the specified renderer, until it reaches the specified target value.
        /// </summary>
        public static IEnumerator AlphaTo( this Renderer renderer, float target, float duration, bool sharedMaterial = false )
        {
            if( !renderer )
                yield break;
            if( renderer is SpriteRenderer )
            {
                SpriteRenderer ren = ( renderer as SpriteRenderer );
                float iniAlpha = ren.color.a;
                float time = 0f;
                while( ren.color.a != target )
                {
                    time += Time.unscaledDeltaTime;
                    Debug.Log ( time / duration );
                    ren.SetAlpha( Mathf.Lerp( iniAlpha, target, time / duration ) );
                    yield return null;
                }
            }
            else
            {
                Material[] mats = ( sharedMaterial ) ? renderer.sharedMaterials : renderer.materials;
                for( int i=0; i<mats.Length; i++ )
                {
                    float iniAlpha = mats[i].color.a;
                    float time = 0f;
                    while( mats[i].color.a != target )
                    {
                        time += Time.unscaledDeltaTime;
                        mats.SetAlpha( Mathf.Lerp( iniAlpha, target, time / duration ) );
                        yield return null;
                    }
                }
            }
        }
        /// <summary>
        /// Fades the specified renderers, until they reaches the specified target value.
        /// </summary>
        public static void AlphaTo( this IList<Renderer> renderers, float target, float duration, bool sharedMaterial = false )
        {
            if( renderers == null )
                return;
            for( int i=0; i<renderers.Count; i++ )
            {
                renderers[i].AlphaTo( target, duration, sharedMaterial ).Start();
            }
        }
        /// <summary>
        /// Animates the alpha of the specified object's RendererGroup's until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this RendererGroup obj, float targetValue, float duration, 
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

            float initialValue = obj.m_Alpha;
            float time = 0f;
            while( time < duration )
            {
                time += Time.deltaTime;
                float multiply = multiplier.Evaluate( Mathf.Lerp( 0f, 
                    multiplier.keys[ multiplier.keys.Length - 1 ].time, time / duration ) );
                obj.m_Alpha = Mathf.Lerp( initialValue, targetValue, time / duration ) * multiply;
                yield return null;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Alpha ) )
                    yield break;
                if( !obj )//IT MIGHT GET DESTROYED WHILE ANIMATING
                    break;
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        /// <summary>
        /// Animates the alpha of the specified renderer's material until the target is reached or this object is destroyed.
        /// </summary>
        /// <param name="obj">The renderer.</param>
        /// <param name="targetValue">Target alpha.</param>
        /// <param name="duration">Animation duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this Renderer obj, float targetValue, float duration, 
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

            Material[] materials = obj.GetTrueMaterials();
            float initialValue = materials[0].color.a;
            float time = 0f;
            while( time < duration )
            {
                time += Time.deltaTime;
                float multiply = multiplier.Evaluate( Mathf.Lerp( 0f, 
                    multiplier.keys[ multiplier.keys.Length - 1 ].time, time / duration ) );
                Color color;
                for( int i=0; i<materials.Length; i++ )
                {
                    color = materials[i].color;
                    color.a = Mathf.Lerp( initialValue, targetValue, time / duration ) * multiply;
                    materials[i].color = color;
                }
                yield return null;
                if ( !validationFlags.IsFlagged ( ValidationFlag.Flags.Alpha ) )
                    yield break;
                if( !obj )//IT MIGHT GET DESTROYED WHILE ANIMATING
                    break;
            }
            validationFlags.SetFlagged( ValidationFlag.Flags.Alpha, false );
        }
        /// <summary>
        /// Animates the alpha of the specified renderers materials until the target are reached.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this Renderer[] objs, float targetValue, float duration, 
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
        /// Animates the alpha of the specified objects materials until the target are reached.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <param name="targetValue">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="multiplier">This curve's values will be multiplied by the target on each specific time.</param>
        /// <param name="toggleAnimator">If true, Animator will be toggled (enabled/disabled).</param>
        public static IEnumerator AnimAlphaWithMultiplier( this SearchableRenderer[] objs, float targetValue, 
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
        /// Returns the renderer's materials unless this is called in Editor Mode, which will return the sharedMaterials
        /// to prevent creating undesired materials instances.
        /// </summary>
        public static Material[] GetTrueMaterials( this Renderer ren )
        {
            if( Application.isPlaying )
            {
                return ren.materials;
            }
            return ren.sharedMaterials;
        }
        /// <summary>
        /// Returns the renderer's materias unless this is called in Editor Mode, which will return the sharedMaterial
        /// to prevent creating undesired material instances.
        /// </summary>
        public static Material GetTrueMaterial( this Renderer ren )
        {
            if( Application.isPlaying )
            {
                return ren.material;
            }
            return ren.sharedMaterial;
        }
        /// <summary>
        /// Animates all colors of the specified Renderer's materials.
        /// </summary>
        public static IEnumerator AnimColors( this Renderer ren, Color target, float duration = 0.4f,
            bool avoidAlpha = true, UnityAction callback = null )
        {
            if( !ren )
                yield break;
            ren.GetTrueMaterials().AnimColors( target, duration, avoidAlpha, callback ).Run();
            yield return new WaitForSeconds( duration );
        }   
        /// <summary>
        /// Animates all colors of the specified Renderer's materials.
        /// </summary>
        public static IEnumerator<float> AnimColor( this Renderer ren, Color target, float duration = 0.4f,
            bool avoidAlpha = true, UnityAction callback = null )
        {
            if( !ren )
                yield break;
            ren.GetTrueMaterial().AnimColor( target, duration, avoidAlpha, callback ).Run();
            yield return Timing.WaitForSeconds( duration );
        }   
        /// <summary>
        /// Animates the color (back and forth) of the specified material
        /// </summary>
        /// <param name="uiElement">User interface element.</param>
        /// <param name="target">Target.</param>
        /// <param name="duration">Animation Duration.</param>
        /// <param name="avoidAlpha">If true, the alpha channel is not taken in account.</param>
        public static IEnumerator<float> AnimColorPingPong( this Renderer ren, Color target, float duration = 0.4f,
            bool avoidAlpha = true, int repeat = 0, UnityAction callback = null )
        {
            if( !ren )
                yield break;

            yield return Timing.WaitUntilDone( ren.GetTrueMaterial().AnimColorPingPong( 
                target, duration, avoidAlpha, repeat, callback ).Run() );
        }
    }

}