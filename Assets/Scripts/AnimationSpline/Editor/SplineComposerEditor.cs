using AnimationSpline.Runtime;
using UnityEditor;
using UnityEngine;

namespace AnimationSpline.Editor
{
    [CustomEditor(typeof(SplineComposer))]
    public class SplineComposerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Animate"))
            {
                if (!Application.isPlaying)
                {
                    EditorUtility.DisplayDialog("Notice", "Animation only works in Play Mode!", "OK");
                    return;
                }

                ((SplineComposer)target).Animate();
            }
        }

        private void OnSceneGUI()
        {
            var obj = (SplineComposer)target;
            if (!obj.SplineAsset) return;

            for (var index = 0; index < obj.SplineAsset.Nodes.Count; index++)
            {
                var node = obj.SplineAsset.Nodes[index];
                DrawNode(node);
            }

            DrawNodeGizmos(obj.SplineAsset);
            
        }

        private void DrawNodeGizmos(SplineAsset splineAsset)
        {
            var c = Handles.color;
            Handles.color = Color.white;
            foreach (var node in splineAsset.Nodes)
            {
                Handles.DrawLine(node.Position, node.InControl);
                Handles.DrawLine(node.Position, node.OutControl);
            }

            Handles.color = c;
        }


        private void DrawNode(SplineNode n)
        {
            EditorGUI.BeginChangeCheck();
            var c = Handles.color;
            Handles.color = Color.green;
            var coord = Vector3.zero;
            coord = Handles.FreeMoveHandle(n.InControl,
                                           Quaternion.identity,
                                           10f,
                                           Vector3.zero,
                                           Handles.CubeHandleCap);
            n.SetInControl(coord);

            coord = Handles.FreeMoveHandle(n.OutControl,
                                           Quaternion.identity,
                                           10f,
                                           Vector3.zero,
                                           Handles.CubeHandleCap);
            n.SetOutControl(coord);

            Handles.color = Color.red;
            coord = Handles.FreeMoveHandle(n.Position,
                                           Quaternion.identity,
                                           10f,
                                           Vector3.zero,
                                           Handles.SphereHandleCap);
            n.SetPosition(coord);
            
            Handles.color = Color.yellow;
            
            Handles.color = c;
            if (EditorGUI.EndChangeCheck())
            {
                var obj = (SplineComposer)target;
                if (obj.SplineAsset)
                {
                    obj.SplineAsset.Bake();
                }
            }
        }
    }
}