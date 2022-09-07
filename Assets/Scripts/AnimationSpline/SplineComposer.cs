#if UNITY_EDITOR
using System.Collections;
using AnimationSpline.Runtime;
using UnityEditor;
using UnityEngine;

namespace AnimationSpline
{
    public class SplineComposer : MonoBehaviour
    {
        public SplineAsset SplineAsset;
        public GameObject  Target;
        public float       Duration;
        public Transform   StartPos;
        public Transform   EndPos;


        private const int SPLIT_COUNT = 30;

        private void OnDrawGizmos()
        {
            if (!SplineAsset) return;

            DrawSplines();
            DrawNodePercent();
        }

        private void DrawNodePercent()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            var      c     = Handles.color;
            Handles.color = Color.yellow;
            for (int i = 0; i < SplineAsset.Nodes.Count; i++)
            {
                Handles.Label(SplineAsset.Nodes[i].Position, SplineAsset.GetNodePercent(i).ToString("F2"), style);
            }

            Handles.color = c;
        }

        private void DrawSplines()
        {
            var c = Gizmos.color;
            Gizmos.color = Color.cyan;
            var nodes = SplineAsset.Nodes;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                DrawNodeToNode(nodes[i], nodes[i + 1]);
            }

            Gizmos.color = c;
        }

        private void DrawNodeToNode(SplineNode node1, SplineNode node2)
        {
            var from = node1.Position;
            for (int i = 0; i <= SPLIT_COUNT; i++)
            {
                var to = Bezier.Interpolate((float)i / SPLIT_COUNT,
                                            node1.Position, node1.OutControl,
                                            node2.InControl, node2.Position);
                Gizmos.DrawLine(from, to);
                from = to;
            }
        }

        public void Animate()
        {
            if (SplineAsset == null) return;

            StopAllCoroutines();
            StartCoroutine(AnimateLoop(SplineAsset));
        }

        private IEnumerator AnimateLoop(SplineAsset splineAsset)
        {
            var timePassed = 0f;
            var coord      = splineAsset.GetCoord(0);
            Target.transform.position = coord;

            Matrix4x4 transformation = new Matrix4x4();
            if (StartPos && EndPos)
            {
                var translation = StartPos.position - splineAsset.NormalizedStartPos;
                var rotation = Quaternion.FromToRotation(splineAsset.NormalizedEndPos - splineAsset.NormalizedStartPos,
                                                         EndPos.position - StartPos.position);
                var scale = Vector3.one * (Vector3.Distance(StartPos.position, EndPos.position) /
                                           Vector3.Distance(splineAsset.NormalizedStartPos,
                                                            splineAsset.NormalizedEndPos));
                transformation.SetTRS(translation, rotation, scale);
            }
            else
            {
                var translation = splineAsset.StartPos;
                transformation.SetTRS(translation, Quaternion.identity, Vector3.one);
            }

            while (timePassed < Duration)
            {
                yield return null;
                timePassed = Mathf.Clamp(timePassed + Time.deltaTime, 0, Duration);
                var t = timePassed / Duration;
                coord = transformation.MultiplyPoint3x4(splineAsset.GetCoord(t));

                Target.transform.position = coord;
            }

            Debug.Log("Animation Finish");
        }
    }
}
#endif