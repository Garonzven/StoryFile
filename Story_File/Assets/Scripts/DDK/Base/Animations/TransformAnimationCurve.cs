using System;
using System.Collections;
using System.Collections.Generic;
using DDK.Base.Events.States;
using DDK.Base.Extensions;
using DDK.Base.Classes;

using UnityEngine;

namespace DDK.Base.Animations
{
    /// <summary>
    /// Allows executing multiple Transform related operations by also using AnimationCurves.
    /// </summary>
    public class TransformAnimationCurve : FinalState
    {
        public enum AnimationPreset
        {
            AlignWith,
            LookTo,
            MoveTo,
            RotateTo,
            ScaleTo
        }

        protected Dictionary<string, AnimationPreset> _presetDictionary;

        [Header ( "Transform Begin / End" )]
        public LateReference source;
        public LateReference target;

        private Transform _source;
        private Transform _target;

        [Header ( "Transform Animation Curves Setup" )]
        public CustomTransformAnimation animationSetup;

        public bool useLocal;

        [Header ( "Events" )]
        public DelayedAction[] actionBefore;

        public DelayedAction[] actionAfter;

        [Header ( "State Events" )]
        public bool playOnEnable = true;

        public bool revertOnDisable = false;

        [Header ( "Preset Setup" )]
        public AnimationPreset preset;
        [InspectorButton ( "AddSetPreset", true )]
        public bool addPreset;
        [InspectorButton ( "TruncateSetPreset", true )]
        public bool truncatePreset;

        private StoredTransform _initialTransform = new StoredTransform();

        public Transform Source
        {
            get
            {
                if ( _source )
                {
                    return _source;
                }

                _source = source.Reference;

                if ( _source )
                {
                    return _source;
                }

                _source = transform;
                source.Reference = _source;
                return _source;
            }
            set { _source = value; }
        }

        public Transform Target
        {
            get
            {
                if ( _target )
                {
                    return _target;
                }

                _target = target.Reference;

                if ( _target )
                {
                    return _target;
                }

                _target = transform;
                target.Reference = _target;
                return _target;
            }
            set { _target = value; }
        }

        public TransformAnimationCurve()
        {
            animationSetup = new CustomTransformAnimation
            {
                delay = 0,
                endTime = 1,
                startTime = 0,
                timeScale = 1
            };
            _presetDictionary = new Dictionary<string, AnimationPreset>();
            var values = Enum.GetValues ( typeof ( AnimationPreset ) );

            for ( int i = 0; i < values.Length; i++ )
            {
                _presetDictionary.Add ( values.GetValue ( i ).ToString(),
                                        ( AnimationPreset ) values.GetValue ( i ) );
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        void OnEnable()
        {
            if ( playOnEnable )
            {
                PlayAnimation();
            }
        }

        void OnDisable()
        {
            if ( !gameObject.activeInHierarchy )
            { return; }

            if ( revertOnDisable )
            {
                RevertPreviousAnimation();
            }
        }

        public void PlayAnimation()
        {
            PlayAnimation ( Target );
        }

        public void PlayAnimation ( Transform dst )
        {
            PlayAnimation ( Source, dst );
        }

        public void PlayAnimation ( Transform src, Transform dst )
        {
            _PlayAnimation ( src, dst ).Start();
        }

        private IEnumerator _PlayAnimation ( Transform src, Transform dst )
        {
            _initialTransform.ExtractFrom ( src );
            actionBefore.InvokeAll();
            yield return StartCoroutine ( animationSetup.PlayKeyframesCo ( src, dst, useLocal ) );
            actionAfter.InvokeAll();
            _FinalStateAction();
        }

        public void RevertPreviousAnimation()
        {
            _ToInitialTransform().Start();
        }

        private IEnumerator _ToInitialTransform()
        {
            actionBefore.InvokeAll();
            var current = new StoredTransform ( Source );
            yield return StartCoroutine ( animationSetup.PlayKeyframesCo( Source, _initialTransform, useLocal ) );
            _initialTransform = current;
            actionAfter.InvokeAll();
            _FinalStateAction();
        }

        public void AddSetPreset()
        {
            AddPreset ( preset );
        }

        public void TruncateSetPreset()
        {
            if ( animationSetup.operations == null )
            {
                animationSetup.operations = new
                List<CustomTransformAnimation.AnimationPair>();
            }

            animationSetup.operations.Clear();
            AddPreset ( preset );
        }

        public void AddPreset ( AnimationPreset setPreset )
        {
            switch ( setPreset )
            {
                case AnimationPreset.AlignWith:
                    AddLinear ( CustomTransformAnimation.Parameter.Rotation );
                    AddLinear ( CustomTransformAnimation.Parameter.Position );
                    break;

                case AnimationPreset.LookTo:
                    if ( !target.IsReferenceSet )
                    {
                        Debug.Log ( "Target reference needs to be set directly" );
                        return;
                    }

                    var position = target.Reference.position;
                    target.Reference = transform;
                    transform.LookAt ( position );
                    AddLinear ( CustomTransformAnimation.Parameter.Rotation );
                    break;

                case AnimationPreset.MoveTo:
                    AddLinear ( CustomTransformAnimation.Parameter.Position );
                    break;

                case AnimationPreset.RotateTo:
                    AddLinear ( CustomTransformAnimation.Parameter.Rotation );
                    break;

                case AnimationPreset.ScaleTo:
                    AddLinear ( CustomTransformAnimation.Parameter.Scale );
                    break;
            }
        }

        public void AddLinear ( CustomTransformAnimation.Parameter param )
        {
            if ( animationSetup.operations == null )
            {
                animationSetup.operations = new
                List<CustomTransformAnimation.AnimationPair>();
            }

            var pair = new CustomTransformAnimation.AnimationPair
            {
                parameter = param,
                curve = AnimationCurve.Linear ( 0f, 0f, 1f, 1f )
            };
            animationSetup.operations.Add ( pair );
        }

        public void SetSource ( Transform src )
        {
            Source = src;
        }

        public void SetTarget ( Transform dst )
        {
            Target = dst;
        }

        public void SetSourceTarget ( Transform src, Transform dst )
        {
            Source = src;
            Target = dst;
        }

        public void StarTime ( float setup )
        {
            animationSetup.startTime = setup;
        }

        public void EndTime ( float setup )
        {
            animationSetup.endTime = setup;
        }

        public void TimeScale ( float setup )
        {
            animationSetup.timeScale = setup;
        }

        public void AnimationDelay ( float setup )
        {
            animationSetup.delay = setup;
        }

        public void AddPreset ( string presetName )
        {
            if ( !_presetDictionary.ContainsKey ( presetName ) )
            {
                return;
            }

            AddPreset ( _presetDictionary[presetName] );
        }

        public void TruncatePreset ( string presetName )
        {
            if ( !_presetDictionary.ContainsKey ( presetName ) )
            {
                return;
            }

            if ( animationSetup.operations == null )
            {
                return;
            }

            animationSetup.operations.Clear();
            AddPreset ( presetName );
        }

        void OnDestroy()
        {
            animationSetup.Stop = true;
        }
    }
}