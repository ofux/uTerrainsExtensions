using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UltimateTerrains
{
    [Serializable]
    public class TopSidesNoLerpColorsVoxelTypeFunctions : AbstractVoxelTypeFunctions
    {
        [SerializeField] private Color32 topColor = Color.clear;
        [SerializeField] private Vector4 topUv2 = Color.clear;
        [SerializeField] private Color32 sidesColor = Color.clear;
        [SerializeField] private Vector4 sidesUv2 = Color.clear;
        [SerializeField] private float slopeLimit = 0f;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return vertexNormal.y < slopeLimit ? sidesColor : topColor;
        }

        public override Vector4 GetVertexUVW2(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return vertexNormal.y < slopeLimit ? sidesUv2 : topUv2;
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            topColor = EditorGUILayout.ColorField("Top Color:", topColor);
            topUv2 = EditorGUILayout.ColorField("Top UV2 (x=r, y=g, z=b, w=a):", topUv2);
            sidesColor = EditorGUILayout.ColorField("Sides Color:", sidesColor);
            sidesUv2 = EditorGUILayout.ColorField("Sides UV2 (x=r, y=g, z=b, w=a):", sidesUv2);
            slopeLimit = EditorGUILayout.Slider("Slope limit:", slopeLimit, -1f, 1f);
#endif
        }
    }
}