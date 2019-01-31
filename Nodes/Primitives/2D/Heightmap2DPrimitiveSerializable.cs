using System;
using System.Collections.Generic;
using UltimateTerrains;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("Heightmap")]
[Serializable]
public class Heightmap2DPrimitiveSerializable : Primitive2DNodeSerializable
{
    public override string Title {
        get { return "Heightmap"; }
    }

    // Useful properties for the module
    [SerializeField] private int fromX, fromZ, toX, toZ;
    [SerializeField] private Texture2D heightmap;
    [SerializeField] private float heightScale;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        fromX = EditorGUILayout.IntField("min X:", fromX);
        fromZ = EditorGUILayout.IntField("min Z:", fromZ);
        toX = EditorGUILayout.IntField("max X:", toX);
        toZ = EditorGUILayout.IntField("max Z:", toZ);
        heightmap = (Texture2D) EditorGUILayout.ObjectField("Heightmap:", heightmap, typeof(Texture2D), false);
        heightScale = EditorGUILayout.FloatField("Height scale", heightScale);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new Heightmap2DPrimitive(fromX, fromZ, toX, toZ, heightmap, heightScale);
    }
}