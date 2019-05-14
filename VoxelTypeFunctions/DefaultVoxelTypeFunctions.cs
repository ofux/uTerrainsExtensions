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
        [SerializeField] private Color32 vertexColor = new Color32(255, 0, 0, 0);
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
            var vColLabel = new GUIContent("Vertex Color:", "(RGBA) R <=> 1st texture, G <=> 2nd texture, B <=> 3rd texture, A <=> 4th texture");
            vertexColor = EditorGUILayout.ColorField(vColLabel, vertexColor);
            var vUV2Label = new GUIContent("UV2 (X = R, Y = G):", "(RG) R <=> 5th texture, G <=> 6th texture");
            uv2 = EditorGUILayout.ColorField(vUV2Label, uv2);
#endif
        }
    }
}