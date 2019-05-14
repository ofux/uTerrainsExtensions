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
        [SerializeField] private Color32 topColor = new Color32(255, 0, 0, 0);
        [SerializeField] private Vector4 topUv2 = Vector4.zero;
        [SerializeField] private Color32 sidesColor = new Color32(0, 255, 0, 0);
        [SerializeField] private Vector4 sidesUv2 = Vector4.zero;
        [SerializeField] private float slopeModificator = 0.7f;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return Color.Lerp(sidesColor, topColor, vertexNormal.y + slopeModificator);
        }

        public override Vector4 GetVertexUVW2(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return Vector4.Lerp(sidesUv2, topUv2, vertexNormal.y + slopeModificator);
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            var vColLabel = new GUIContent("Top Color:", "(RGBA) R <=> 1st texture, G <=> 2nd texture, B <=> 3rd texture, A <=> 4th texture");
            topColor = EditorGUILayout.ColorField(vColLabel, topColor);
            var vUV2Label = new GUIContent("Top UV2:", "(RG) R <=> 5th texture, G <=> 6th texture");
            topUv2 = EditorGUILayout.ColorField(vUV2Label, topUv2);

            vColLabel = new GUIContent("Sides Color:", "(RGBA) R <=> 1st texture, G <=> 2nd texture, B <=> 3rd texture, A <=> 4th texture");
            sidesColor = EditorGUILayout.ColorField(vColLabel, sidesColor);
            vUV2Label = new GUIContent("Sides UV2:", "(RG) R <=> 5th texture, G <=> 6th texture");
            sidesUv2 = EditorGUILayout.ColorField(vUV2Label, sidesUv2);

            var slopeLabel = new GUIContent("Top min normal.y:", "Min normal.y of ground to be considered as Top. If normal.y is lower than this, it will be considered as Sides.");
            slopeModificator = EditorGUILayout.Slider(slopeLabel, slopeModificator, -1f, 1f);
#endif
        }
    }
}