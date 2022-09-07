using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationSpline.Runtime
{
    [CreateAssetMenu]
    public class SplineAsset : ScriptableObject
    {
        private const int COORD_PER_SPLINE = 30;

        public List<SplineNode> Nodes = new List<SplineNode>();
        public AnimationCurve   Curve;

        [SerializeField]
        private float[] _splineLengths;

        [SerializeField]
        private SplineNode[] _normalizedNodes;


        public float   TotalLength        => _splineLengths[_splineLengths.Length - 1];
        public Vector3 StartPos           => Nodes[0].Position;
        public Vector3 EndPos             => Nodes[Nodes.Count - 1].Position;
        public Vector3 NormalizedStartPos => _normalizedNodes[0].Position;
        public Vector3 NormalizedEndPos   => _normalizedNodes[_normalizedNodes.Length - 1].Position;

        public float GetNodePercent(int index)
        {
            if (index == 0) return 0;
            if (index == _splineLengths.Length - 1) return 1;
            return _splineLengths[index] / TotalLength;
        }

        public List<SplineNode> NormalizedNodes
        {
            get
            {
                var offset = Nodes[0].Position;
                return Nodes.Select(v => new SplineNode()
                {
                    Position   = v.Position - offset, InControl = v.InControl - offset,
                    OutControl = v.OutControl - offset
                }).ToList();
            }
        }

        public void Bake()
        {
            if (Nodes == null || Nodes.Count == 0) return;


            _splineLengths   = new float[Nodes.Count];
            _normalizedNodes = NormalizedNodes.ToArray(); // translate all nodes so that startPos is (0, 0)
            var length = 0f;
            for (int i = 0; i < _normalizedNodes.Length - 1; i++)
            {
                length                += GetSplineLength(_normalizedNodes[i], _normalizedNodes[i + 1]);
                _splineLengths[i + 1] =  length;
            }

            Debug.Log($"Baking {name} finished, TotalLength: {length:F2}");
        }

        private float GetSplineLength(SplineNode nodeFrom, SplineNode nodeTo)
        {
            var start  = nodeFrom.Position;
            var length = 0f;
            for (int i = 0; i <= COORD_PER_SPLINE; i++)
            {
                var end = Bezier.Interpolate((float)i / COORD_PER_SPLINE,
                                             nodeFrom.Position,
                                             nodeFrom.OutControl,
                                             nodeTo.InControl,
                                             nodeTo.Position);
                length += Vector3.Distance(start, end);
                start  =  end;
            }

            return length;
        }

        public Vector3 GetCoord(float t)
        {
            if (Curve != null)
            {
                t = Curve.Evaluate(t);
            }

            t = Mathf.Clamp01(t);

            if (t >= 1f) return _normalizedNodes[_normalizedNodes.Length - 1].Position;

            t *= TotalLength; // current length
            var index = GetIndex(t, _splineLengths);
            Debug.Log($"length {t},  index {index}");
            t = (t - _splineLengths[index]) /
                (_splineLengths[index + 1] - _splineLengths[index]); // ratio in the current spline split  
            var ret = Bezier.Interpolate(t,
                                         _normalizedNodes[index].Position,
                                         _normalizedNodes[index].OutControl,
                                         _normalizedNodes[index + 1].InControl,
                                         _normalizedNodes[index + 1].Position);
            return ret;
        }

        private int GetIndex(float dist, float[] lengths)
        {
            int left = 0, right = lengths.Length - 1;
            while (left < right)
            {
                var mid    = (left + right + 1) / 2;
                var midVal = lengths[mid];
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (dist == midVal) return mid;
                if (dist > midVal)
                    left = mid;
                else
                    right = mid - 1;
            }

            return left;
        }
    }
}