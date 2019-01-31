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
        [SerializeField] private Color topColor = Color.clear;
        [SerializeField] private Color sidesColor = Color.clear;
        [SerializeField] private float slopeLimit = 0f;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return vertexNormal.y < slopeLimit ? sidesColor : topColor;
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            topColor = EditorGUILayout.ColorField("Top Color:", topColor);
            sidesColor = EditorGUILayout.ColorField("Sides Color:", sidesColor);
            slopeLimit = EditorGUILayout.Slider("Slope limit:", slopeLimit, -1f, 1f);
#endif
        }
    }
}