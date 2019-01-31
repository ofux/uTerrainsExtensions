using System;
using System.Collections.Generic;
using UnityEngine;
using UltimateTerrains;
using LibNoise;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("Fast Noise 3D")]
[Serializable]
public sealed class FastNoise3DPrimitiveSerializable : Primitive3DNodeSerializable
{
    public override string Title {
        get { return "Fast Noise 3D"; }
    }
    
    // Useful properties for the module
    [SerializeField] private float frequency = 0.01f;
    [SerializeField] private float scale = 1f;
    [SerializeField] private int seed = 1337;
    [SerializeField] private int fractalOctaves = 3;
    [SerializeField] private float fractalLacunarity = 2f;
    [SerializeField] private float fractalGain = 0.5f;
    [SerializeField] private float cellularJitter = 0.45f;
    [SerializeField] private float gradientPerturbAmp = 1f;

    [SerializeField] private FastNoise.NoiseType noiseType = FastNoise.NoiseType.Simplex;
    [SerializeField] private FastNoise.Interp interp = FastNoise.Interp.Quintic;
    [SerializeField] private FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
    [SerializeField] private FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;
    [SerializeField] private FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;

    [SerializeField] private FastNoise3DPrimitiveSerializable cellularNoise;

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

    public int FractalOctaves {
        get { return fractalOctaves; }
        set { fractalOctaves = value; }
    }

    public float FractalLacunarity {
        get { return fractalLacunarity; }
        set { fractalLacunarity = value; }
    }

    public float FractalGain {
        get { return fractalGain; }
        set { fractalGain = value; }
    }

    public float CellularJitter {
        get { return cellularJitter; }
        set { cellularJitter = value; }
    }

    public float GradientPerturbAmp {
        get { return gradientPerturbAmp; }
        set { gradientPerturbAmp = value; }
    }

    public FastNoise.NoiseType NoiseType {
        get { return noiseType; }
        set { noiseType = value; }
    }

    public FastNoise.Interp Interp {
        get { return interp; }
        set { interp = value; }
    }

    public FastNoise.FractalType FractalType {
        get { return fractalType; }
        set { fractalType = value; }
    }

    public FastNoise.CellularReturnType CellularReturnType {
        get { return cellularReturnType; }
        set { cellularReturnType = value; }
    }

    public FastNoise.CellularDistanceFunction CellularDistanceFunction {
        get { return cellularDistanceFunction; }
        set { cellularDistanceFunction = value; }
    }

    

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        Seed = EditorGUILayout.IntField("Seed:", Seed);
        Frequency = EditorGUILayout.FloatField("Frequency:", Frequency);
        Scale = EditorGUILayout.FloatField("Scale:", Scale);
        GradientPerturbAmp = EditorGUILayout.FloatField("Gradient Perturb Amplitude:", GradientPerturbAmp);
        NoiseType = (FastNoise.NoiseType) EditorGUILayout.EnumPopup("Noise Type:", NoiseType);
        Interp = (FastNoise.Interp) EditorGUILayout.EnumPopup("Interp:", Interp);

        if (NoiseType == FastNoise.NoiseType.Cellular) {
            CellularJitter = EditorGUILayout.FloatField("Cellular Jitter:", CellularJitter);
            CellularReturnType = (FastNoise.CellularReturnType) EditorGUILayout.EnumPopup("Cellular return Type:", CellularReturnType);
            CellularDistanceFunction = (FastNoise.CellularDistanceFunction) EditorGUILayout.EnumPopup("Cellular distance function:", CellularDistanceFunction);

            if (CellularReturnType == FastNoise.CellularReturnType.NoiseLookup) {
                if (cellularNoise == null) {
                    cellularNoise = CreateInstance<FastNoise3DPrimitiveSerializable>();
                }

                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Noise lookup:");
                cellularNoise.OnEditorGUI(uTerrain);
                EditorGUILayout.EndVertical();
            }
        } else if (NoiseType == FastNoise.NoiseType.CubicFractal ||
                   NoiseType == FastNoise.NoiseType.PerlinFractal ||
                   NoiseType == FastNoise.NoiseType.SimplexFractal ||
                   NoiseType == FastNoise.NoiseType.ValueFractal) {
            FractalType = (FastNoise.FractalType) EditorGUILayout.EnumPopup("Fractal Type:", FractalType);
            FractalOctaves = EditorGUILayout.IntField("Octaves:", FractalOctaves);
            FractalLacunarity = EditorGUILayout.FloatField("Lacunarity:", FractalLacunarity);
            FractalGain = EditorGUILayout.FloatField("Gain:", FractalGain);
        }
#endif
    }

    private FastNoise CreateFastNoise()
    {
        var fastNoise = new FastNoise(seed);
        fastNoise.SetNoiseType(NoiseType);
        fastNoise.SetInterp(Interp);
        fastNoise.SetFrequency(frequency);
        fastNoise.SetFractalType(FractalType);
        fastNoise.SetFractalOctaves(FractalOctaves);
        fastNoise.SetFractalLacunarity(FractalLacunarity);
        fastNoise.SetFractalGain(FractalGain);
        fastNoise.SetGradientPerturbAmp(GradientPerturbAmp);
        fastNoise.SetCellularReturnType(CellularReturnType);
        fastNoise.SetCellularJitter(CellularJitter);
        fastNoise.SetCellularDistanceFunction(CellularDistanceFunction);
        if (cellularNoise != null) {
            fastNoise.SetCellularNoiseLookup(cellularNoise.CreateFastNoise());
        }

        return fastNoise;
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new FastNoise3DPrimitive(CreateFastNoise(), scale);
    }
}