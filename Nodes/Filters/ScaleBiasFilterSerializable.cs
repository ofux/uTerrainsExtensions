using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Scale & bias")]
[Serializable]
public class ScaleBiasFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Scale & bias"; }
    }

    [SerializeField] private float scale = 1f;
    [SerializeField] private float bias = 0f;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField("out = in * scale\n+ bias");
        EditorGUIUtility.labelWidth = 60;
        
        scale = EditorGUILayout.FloatField("Scale", scale, GUILayout.Width(100));
        bias = EditorGUILayout.FloatField("Bias", bias, GUILayout.Width(100));
        
        base.OnEditorGUI(uTerrain);
        EditorGUIUtility.labelWidth = 0;
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new ScaleBiasFilter(inputs[0], inputs[1], Intensity, scale, bias);
    }
}