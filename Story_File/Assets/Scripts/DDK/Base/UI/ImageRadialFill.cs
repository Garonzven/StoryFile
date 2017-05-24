using System.Collections;

using DDK.Base.Events.States;
using DDK.Base.Extensions;
using DDK.Base.Classes;

using UnityEngine;
using UnityEngine.UI;
using DDK.Base.Statics;

namespace DDK.Base.UI
{
    /// <summary>
    /// Animates An Image's Fill Amount. NOTE: This script should be named AnimateImageFill.
    /// </summary>
    public class ImageRadialFill : FinalState
    {
        public float duration = 5f;

        [Tooltip ( "The target fill amount when this component is disabled" )]
        [Range ( 0f, 1f )]
        public float onDisableTarget = 0;

        [Tooltip ( "The target fill amount when this component is enabled" )]
        [Range ( 0f, 1f )]
        public float onEnableTarget = 1;

        public Image radialImage;

        public DisableEnableAction events;


        // Use this for initialization
        private void Start()
        {
            if ( !radialImage )
            {
                radialImage = GetComponent<Image>();

                if ( !radialImage )
                {
                    Utilities.Log ( this + " needs Radial Image set to Work" );
                    return;
                }
            }

            // set as fill image
            radialImage.type = Image.Type.Filled;
            radialImage.enabled = true;
        }

        private void OnEnable()
        {
            if ( radialImage != null && gameObject.activeInHierarchy )
            {
                events.onEnable.InvokeAll();
                StartCoroutine ( _ImageFillTo ( onEnableTarget ) );
            }
        }

        private void OnDisable()
        {
            if ( radialImage != null && gameObject.activeInHierarchy )
            {
                events.onDisable.InvokeAll();
                StartCoroutine ( _ImageFillTo ( onDisableTarget ) );
            }
        }

        private IEnumerator _ImageFillTo ( float target )
        {
            events.beforeAction.InvokeAll();
            var timer = Time.deltaTime;
            var original = radialImage.fillAmount;

            while ( timer <= duration )
            {
                radialImage.fillAmount = Mathf.Lerp ( original, target, timer / duration );
                yield return null;
                timer += Time.deltaTime;
            }

            radialImage.fillAmount = target;
            events.afterAction.InvokeAll();
            _FinalStateAction();
        }
    }
}