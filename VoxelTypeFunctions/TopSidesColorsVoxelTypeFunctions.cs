using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UltimateTerrains
{
    [Serializable]
    public class TopSidesColorsVoxelTypeFunctions : AbstractVoxelTypeFunctions
    {
        [SerializeField] private Color topColor = Color.clear;
        [SerializeField] private Color sidesColor = Color.clear;
        [SerializeField] private float slopeModificator;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return Color.Lerp(sidesColor, topColor, vertexNormal.y + slopeModificator);
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            topColor = EditorGUILayout.ColorField("Top Color:", topColor);
            sidesColor = EditorGUILayout.ColorField("Sides Color:", sidesColor);
            slopeModificator = EditorGUILayout.Slider("Slope modificator:", slopeModificator, -1f, 1f);
#endif
        }
    }
}