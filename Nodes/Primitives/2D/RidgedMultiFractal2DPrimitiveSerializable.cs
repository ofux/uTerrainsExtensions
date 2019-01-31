using System;
using System.Collections.Generic;
using LibNoise;
using UltimateTerrains;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("LibNoise Ridged-Multifractal 2D")]
[Serializable]
public class RidgedMultiFractal2DPrimitiveSerializable : Primitive2DNodeSerializable
{

    public override string Title {
        get { return "LibNoise Ridged-Multifractal 2D"; }
    }
    
    // Useful properties for the module
    [SerializeField] private float frequency = 0.001f;
    [SerializeField] private float scale = 1f;
    [SerializeField] private float offsetX = 200f;
    [SerializeField] private float offsetZ = 200f;
    [SerializeField] private int seed = PrimitiveModule.DefaultSeed;
    [SerializeField] private NoiseQuality quality = NoiseQuality.Standard;

    public float Frequency {
        get { return frequency; }
        set { frequency = value; }
    }

    public float Scale {
        get { return scale; }
        set { scale = value; }
    }

    public float OffsetX {
        get { return offsetX; }
        set { offsetX = value; }
    }

    public float OffsetZ {
        get { return offsetZ; }
        set { offsetZ = value; }
    }

    public int Seed {
        get { return seed; }
        set { seed = value; }
    }

    public NoiseQuality Quality {
        get { return quality; }
        set { quality = value; }
    }

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        Frequency = EditorGUILayout.FloatField("Frequency:", Frequency);
        Scale = EditorGUILayout.FloatField("Scale:", Scale);
        OffsetX = EditorGUILayout.FloatField("Offset X:", OffsetX);
        OffsetZ = EditorGUILayout.FloatField("Offset Z:", OffsetZ);
        Seed = EditorGUILayout.IntField("Seed:", Seed);
        Quality = (NoiseQuality) EditorGUILayout.EnumPopup("Quality:", Quality);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new RidgedMultiFractal2DPrimitive(Frequency, Scale, OffsetX, OffsetZ, Seed, Quality);
    }
}