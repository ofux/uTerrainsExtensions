using System;
using System.Collections.Generic;
using LibNoise;
using UltimateTerrains;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("Tunnel Network 3D")]
[Serializable]
public sealed class TunnelNetwork3DPrimitiveSerializable : Primitive3DNodeSerializable
{
    public override string Title {
        get { return "Tunnel Network 3D"; }
    }

    // Useful properties for the module
    [SerializeField] private float frequency = 0.005f;
    [SerializeField] private int seed = PrimitiveModule.DefaultSeed;
    [SerializeField] private float frequencyAltitude = 0.008f;
    [SerializeField] private float scaleAltitude = 40f;
    [SerializeField] private float baseAltitude = 0f;
    [SerializeField] private float tunnelsHeight = 20f;
    [SerializeField] private float threshold = 0.8f;

    [SerializeField] private float frequencyPerturbation = 0.03f;
    [SerializeField] private float scalePerturbation = 5f;
    [SerializeField] private float frequencyMicroPerturbation = 0.1f;
    [SerializeField] private float scaleMicroPerturbation = 1f;

    public override void OnEditorGUI(UltimateTerrain uTerrain, IReadOnlyFlowGraph graph)
    {
#if UNITY_EDITOR
        frequency = EditorGUILayout.FloatField("Frequency:", frequency);
        seed = EditorGUILayout.IntField("Seed:", seed);
        frequencyAltitude = EditorGUILayout.FloatField("Frequency Altitude:", frequencyAltitude);
        scaleAltitude = EditorGUILayout.FloatField("Scale altitude:", scaleAltitude);
        baseAltitude = EditorGUILayout.FloatField("Base altitude:", baseAltitude);
        tunnelsHeight = EditorGUILayout.FloatField("Tunnels height:", tunnelsHeight);
        threshold = EditorGUILayout.FloatField("Threshold:", threshold);
        frequencyPerturbation = EditorGUILayout.FloatField("Perturbation freq.:", frequencyPerturbation);
        scalePerturbation = EditorGUILayout.FloatField("Perturbation scale:", scalePerturbation);
        frequencyMicroPerturbation = EditorGUILayout.FloatField("Micro Perturbation freq.:", frequencyMicroPerturbation);
        scaleMicroPerturbation = EditorGUILayout.FloatField("Micro Perturbation scale:", scaleMicroPerturbation);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new TunnelNetwork3DPrimitive(frequency, seed,
                                            frequencyAltitude, scaleAltitude, baseAltitude,
                                            tunnelsHeight, threshold,
                                            frequencyPerturbation, scalePerturbation,
                                            frequencyMicroPerturbation, scaleMicroPerturbation);
    }
}