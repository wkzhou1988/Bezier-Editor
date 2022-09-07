using AnimationSpline.Runtime;
using UnityEditor;
using UnityEngine;

namespace AnimationSpline.Editor
{
    [CustomEditor(typeof(SplineAsset))]
    public class SplineAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var obj = (SplineAsset)target;
            if (GUILayout.Button("创建新控制点"))
            {
                var node = new SplineNode();
                if (obj.Nodes.Count > 0)
                {
                    node.Position = obj.Nodes[obj.Nodes.Count - 1].Position + new Vector3(50, 0);
                }

                node.InControl  = node.Position + new Vector3(-50, 0);
                node.OutControl = node.Position + new Vector3(50, 0);
                obj.Nodes.Add(node);
            }

            if (GUILayout.Button("Bake"))
            {
                obj.Bake();
            }
        }
    }
}