using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace UltimateTerrains
{
    [Serializable]
    public class MegaSplatVoxelTypeFunctions : AbstractVoxelTypeFunctions
    {
        [SerializeField] private float wetness = 0.0f;
        //[SerializeField] private float flow = 0.0f;
        //[SerializeField] private float direction = 0.0f;
        [SerializeField] private float layerBlend = 0.0f;
        [SerializeField] private float displacement = 0.0f;
        [SerializeField] private float puddleAmount = 0.0f;
        [SerializeField] private int topLayer = 0;
        [SerializeField] private int bottomLayer = 0;


        private static readonly List<TextureArrayPreviewCache> PreviewCache = new List<TextureArrayPreviewCache>(20);


        public override Color32 GetVertexColor(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return new Color32(0, 0, 0, (byte) bottomLayer);
        }

        public override Vector4 GetVertexUVW2(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return new Vector4
            {
                //z = Flow;
                //w = Direction; //not supported at the moment
                w = wetness //don't believe Megasplat documentation...
            };
        }

        public override Vector4 GetVertexUVW3(Vector3d meshWorldPosition, Vector3 vertexPosition, Vector3 vertexNormal)
        {
            return new Vector4
            {
                x = layerBlend,
                y = puddleAmount,
                z = 1 - displacement, //invert, so 0 is no displacement, 1 is full
                w = topLayer / 255f
            };
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain, VoxelType voxelType)
        {
#if UNITY_EDITOR

            var material = uTerrain.VoxelTypeSet.Materials[voxelType.MaterialIndex];
            var twoLayer = material.IsKeywordEnabled("_TWOLAYER");
            var txArray = material.GetTexture("_Diffuse") as Texture2DArray;
            if (txArray == null) {
                EditorGUILayout.HelpBox("_Diffuse is null", MessageType.Error, true);
                return;
            }

            var txBottom = GetCachedTexture(txArray, bottomLayer);
            var txTop = GetCachedTexture(txArray, topLayer);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Bottom Layer");
            EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(GUILayout.Width(96), GUILayout.Height(96)), txBottom);
            bottomLayer = EditorGUILayout.IntSlider(bottomLayer, 0, txArray.depth - 1, GUILayout.Width(120));
            EditorGUILayout.EndVertical();
            if (twoLayer) {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Top Layer");
                EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(GUILayout.Width(96), GUILayout.Height(96)), txTop);
                topLayer = EditorGUILayout.IntSlider(topLayer, 0, txArray.depth - 1, GUILayout.Width(120));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                layerBlend = EditorGUILayout.Slider("Layer Blend", layerBlend, 0.0f, 1.0f);
            } else {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("TwoLayer disabled in material settings.", MessageType.Info, true);
            }


            if (topLayer > 0 && layerBlend < 0.01f) {
                EditorGUILayout.HelpBox("Info: As LayerBlend is 0 only bottom layer will be drawn", MessageType.Info, true);
            }

            if (material.IsKeywordEnabled("_WETNESS")) {
                wetness = EditorGUILayout.Slider("Wetness", wetness, 0.0f, 1.0f);
            }

            //Flow = EditorGUILayout.Slider("Flow", Flow, 0.0f, 1.0f);   //not supported yet
            //Direction = EditorGUILayout.Slider("Direction", Direction, 0.0f, 1.0f);
            if (material.IsKeywordEnabled("_TESSDISTANCE") || material.IsKeywordEnabled("_TESSEDGE")) {
                displacement = EditorGUILayout.Slider("Displacement", displacement, 0.0f, 1.0f);
            }

            if (material.IsKeywordEnabled("_PUDDLES") || material.IsKeywordEnabled("_PUDDLEFLOW") || material.IsKeywordEnabled("_PUDDLEREFRACT")) {
                puddleAmount = EditorGUILayout.Slider("Puddle Amount", puddleAmount, 0.0f, 1.0f);
            }

            EditorGUILayout.LabelField("Active Array: " + material.GetTexture("_Diffuse").name);

#endif
        }


        private static Texture2D GetCachedTexture(Texture2DArray ta, int textureIndex)
        {
            var hash = ta.GetHashCode() * (textureIndex + 7);
            var hashed = FindInPreviewCache(hash);
            if (hashed != null)
                return hashed;

            hashed = new Texture2D(ta.width, ta.height, ta.format, false);
            Graphics.CopyTexture(ta, textureIndex, 0, hashed, 0, 0);
            hashed.Apply(false, false);

            var hd = new TextureArrayPreviewCache
            {
                Hash = hash,
                Texture = hashed
            };
            PreviewCache.Add(hd);
            if (PreviewCache.Count > 100) {
                hd = PreviewCache[0];
                PreviewCache.RemoveAt(0);
                if (hd.Texture != null) {
                    DestroyImmediate(hd.Texture);
                }
            }

            return hashed;
        }

        private static Texture2D FindInPreviewCache(int hash)
        {
            for (var i = 0; i < PreviewCache.Count; ++i) {
                if (PreviewCache[i].Hash == hash)
                    return PreviewCache[i].Texture;
            }

            return null;
        }

        private class TextureArrayPreviewCache
        {
            public int Hash;
            public Texture2D Texture;
        }
    }
}