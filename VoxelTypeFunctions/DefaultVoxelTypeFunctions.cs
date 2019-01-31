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
        [SerializeField] private Color32
            vertexColor = Color.clear;

        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return vertexColor;
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR
            vertexColor = EditorGUILayout.ColorField("Vertex Color:", vertexColor);
#endif
        }
    }
}