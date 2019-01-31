using System;
using System.Collections.Generic;
using UnityEngine;
using UltimateTerrains;
using LibNoise;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("Simplex 3D")]
[Serializable]
public sealed class Simplex3DPrimitiveSerializable : Primitive3DNodeSerializable
{

    public override string Title {
        get { return "Simplex 3D"; }
    }
    
    // Useful properties for the module
    [SerializeField] private float frequency = 0.005f;
    [SerializeField] private float scale = 1f;
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
        Seed = EditorGUILayout.IntField("Seed:", Seed);
        Quality = (NoiseQuality) EditorGUILayout.EnumPopup("Quality:", Quality);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new Simplex3DPrimitive(Frequency, Scale, Seed, Quality);
    }
}