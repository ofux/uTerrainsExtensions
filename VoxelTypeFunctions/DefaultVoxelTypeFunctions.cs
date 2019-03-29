using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UltimateTerrains
{
    [Serializable]
    public class DefaultVoxelTypeFunctions : AbstractVoxelTypeFunctions
    {
        [SerializeField] private Color32 vertexColor = Color.red;
        [SerializeField] private Vector4 uv2 = Vector4.zero;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return vertexColor;
        }

        public override Vector4 GetVertexUVW2(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return uv2;
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            vertexColor = EditorGUILayout.ColorField("Vertex Color:", vertexColor);
            uv2 = EditorGUILayout.ColorField("UV2 (X = R, Y = G):", uv2);
#endif
        }
    }
}