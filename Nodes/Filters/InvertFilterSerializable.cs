using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Invert")]
[Serializable]
public class InvertFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Invert"; }
    }

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField("out = -in");
        base.OnEditorGUI(uTerrain);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new InvertFilter(inputs[0], inputs[1], Intensity);
    }
}