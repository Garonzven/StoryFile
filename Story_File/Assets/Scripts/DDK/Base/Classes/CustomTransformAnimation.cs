using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using DDK.Base.Extensions;

using UnityEngine;

namespace DDK.Base.Classes
{
    [Serializable]
    public class CustomTransformAnimation
    {
        /// <summary>
        /// Indicates which <see cref="Transform"/>
        /// components the animation will work on
        /// </summary>
        public enum Parameter
        {
            Position,
            Rotation,
            Scale
        }
        /// <summary>
        /// Describes which operation will be used to
        /// composite multiple curves working on the
        /// same <see cref="Transform"/> component
        /// </summary>
        public enum OnSameParameter
        {
            Multiply,
            Divide,
            Addition,
            Substract
        }


        [Serializable]
        public class AnimationPair
        {
            public Parameter parameter;
            public OnSameParameter onSameParameter;
            public AnimationCurve curve;
        }

        public float delay;
        public float startTime;
        public float endTime;
        public float timeScale;
        public List<AnimationPair> operations;

        public bool Stop { get; set; }

        /// <summary>
        /// Evaluates the composite of multiple curves operating on the same
        /// <see cref="Transform"/> component at a specified <see cref="time"/> mark
        /// </summary>
        /// <param name="curvesPairs">The curves pairs.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private static float _AnimationCurvesValueAt( IList<Pair<AnimationCurve, OnSameParameter>> curvesPairs, float time )
        {
            if ( curvesPairs == null || curvesPairs.Count == 0 )
            {
                return 0f;
            }

            var curve = curvesPairs[0].first;
            var op = curvesPairs[0].second;
            var firstT = curve.Evaluate ( time );

            for ( int i = 1; i < curvesPairs.Count; i++ )
            {
                curve = curvesPairs[i].first;
                var secondT = curve.Evaluate ( time );

                switch ( op )
                {
                    case OnSameParameter.Addition:
                        firstT = firstT + secondT;
                        break;

                    case OnSameParameter.Divide:
                        firstT = firstT / secondT;
                        break;

                    case OnSameParameter.Substract:
                        firstT = firstT - secondT;
                        break;

                    case OnSameParameter.Multiply:
                        firstT = firstT * secondT;
                        break;
                }
            }

            return firstT;
        }

        /// <summary>
        /// Extracts the animation curves per <see cref="Transform"/>
        /// component
        /// </summary>
        /// <param name="positionCurves">The position curves.</param>
        /// <param name="rotationCurves">The rotation curves.</param>
        /// <param name="scaleCurves">The scale curves.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void _ExtractTransformAnimCurves (
            ICollection<Pair<AnimationCurve, OnSameParameter>> positionCurves,
            ICollection<Pair<AnimationCurve, OnSameParameter>> rotationCurves,
            ICollection<Pair<AnimationCurve, OnSameParameter>> scaleCurves )
        {
            for ( int i = 0; i < operations.Count; i++ )
            {
                switch ( operations[i].parameter )
                {
                    case Parameter.Position:
                        positionCurves.Add (
                            new Pair<AnimationCurve, OnSameParameter> (
                                operations[i].curve,
                                operations[i].onSameParameter
                            )
                        );
                        break;

                    case Parameter.Rotation:
                        rotationCurves.Add (
                            new Pair<AnimationCurve,
                            OnSameParameter> (
                                operations[i].curve,
                                operations[i].onSameParameter
                            )
                        );
                        break;

                    case Parameter.Scale:
                        scaleCurves.Add (
                            new Pair<AnimationCurve, OnSameParameter>
                            (
                                operations[i].curve,
                                operations[i].onSameParameter
                            )
                        );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IEnumerator PlayKeyframesCo ( Transform source, Transform target, bool local = false )
        {
            var sTarget = new StoredTransform ( target );
            yield return PlayKeyframesCo ( source, sTarget ).Start() ;
        }

        /// <summary>
        /// Plays the transform animation
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="local">Local mode</param>
        /// <returns></returns>
        public IEnumerator PlayKeyframesCo ( Transform source, StoredTransform target, bool local = false )
        {
            if ( operations == null || operations.Count == 0 )
            {
                yield break;
            }

            Stop = false;
            yield return new WaitForSeconds ( delay );
            var time = startTime;
            var rotationCurves = new List<Pair<AnimationCurve, OnSameParameter>>();
            var positionCurves = new List<Pair<AnimationCurve, OnSameParameter>>();
            var scaleCurves = new List<Pair<AnimationCurve, OnSameParameter>>();
            _ExtractTransformAnimCurves ( positionCurves, rotationCurves, scaleCurves );
            var rota = local ? source.localRotation : source.rotation;
            var posi = local ? source.localPosition : source.position;
            var trota = local ? target.localRotation : target.rotation;
            var tposi = local ? target.localPosition : target.position;
            var scal = source.localScale;
            var tScale = target.localScale;

            if ( timeScale < Mathf.Epsilon )
            {
                timeScale = Mathf.Epsilon;
            }

            var targetTime = endTime / timeScale;

            while ( time <= targetTime && !Stop )
            {
                time += Time.deltaTime;
                var t = time * timeScale;
                var pMark = _AnimationCurvesValueAt ( positionCurves, t );
                var rMark = _AnimationCurvesValueAt ( rotationCurves, t );
                var sMark = _AnimationCurvesValueAt ( scaleCurves, t );
                var p = Vector3.SlerpUnclamped ( posi, tposi, pMark );
                var r = Quaternion.SlerpUnclamped ( rota, trota, rMark );
                var s = Vector3.SlerpUnclamped ( scal, tScale, sMark );
                source.position = p;
                source.rotation = r;
                source.localScale = s;
                yield return null;
            }
        }
    }

}