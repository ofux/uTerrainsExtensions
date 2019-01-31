using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Blend")]
[Serializable]
public class GenericCombinerSerializable : CombinerNodeSerializable
{
    public override string Title {
        get { return "Blend"; }
    }
    
    public enum CombinerType
    {
        Add, Sub, Mul, Min, Max
    }

    [SerializeField] private CombinerType combinerType;
    [SerializeField] private float inWeight = 0.5f;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        var text = "";
        switch (combinerType) {
            case CombinerType.Add:
                text = "out = in1 + in2";
                break;
            case CombinerType.Sub:
                text = "out = in1 - in2";
                break;
            case CombinerType.Mul:
                text = "out = in1 * in2";
                break;
            case CombinerType.Min:
                text = "out = min(in1, in2)";
                break;
            case CombinerType.Max:
                text = "out = max(in1, in2)";
                break;
        }
        EditorUtils.CenteredBoxedLabelField(text);
        combinerType = (CombinerType) EditorGUILayout.EnumPopup(combinerType);
        if (combinerType == CombinerType.Add || combinerType == CombinerType.Sub) {
            EditorUtils.CenteredLabelField("Weight of inputs\nin1 <-> in2");
            inWeight = EditorGUILayout.Slider(inWeight, 0f, 1f, GUILayout.MinWidth(200));
        }
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        switch (combinerType) {
            case CombinerType.Add:
                return new AddCombiner(inputs[0], inputs[1], inWeight);
            case CombinerType.Sub:
                return new SubCombiner(inputs[0], inputs[1], inWeight);
            case CombinerType.Mul:
                return new MulCombiner(inputs[0], inputs[1]);
            case CombinerType.Min:
                return new MinCombiner(inputs[0], inputs[1]);
            case CombinerType.Max:
                return new MaxCombiner(inputs[0], inputs[1]);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}