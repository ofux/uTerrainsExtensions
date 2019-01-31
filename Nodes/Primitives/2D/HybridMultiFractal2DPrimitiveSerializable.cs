using System;
using System.Collections.Generic;
using LibNoise;
using UltimateTerrains;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("LibNoise Hybrid-Multifractal 2D")]
[Serializable]
public class HybridMultiFractal2DPrimitiveSerializable : Primitive2DNodeSerializable
{

    public override string Title {
        get { return "LibNoise Hybrid-Multifractal 2D"; }
    }
    
    // Useful properties for the module
    [SerializeField] private float frequency = 1f / 90f;
    [SerializeField] private float scale = 1f;
    [SerializeField] private int seed = PrimitiveModule.DefaultSeed;
    [SerializeField] private NoiseQuality quality = NoiseQuality.Standard;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        frequency = EditorGUILayout.FloatField("Frequency:", frequency);
        scale = EditorGUILayout.FloatField("Scale:", scale);
        seed = EditorGUILayout.IntField("Seed:", seed);
        quality = (NoiseQuality) EditorGUILayout.EnumPopup("Quality:", quality);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new HybridMultiFractal2DPrimitive(frequency, scale, seed, quality);
    }
}