using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Clamp")]
[Serializable]
public class ClampFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Clamp"; }
    }

    [SerializeField] private Vector2 outBounds = new Vector2(-1,1);

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorGUIUtility.labelWidth = 60;
        EditorUtils.CenteredBoxedLabelField(string.Format("out ∈ [{0},{1}]", outBounds.x, outBounds.y));
        outBounds = EditorGUILayout.Vector2Field("Min/max", outBounds);
        base.OnEditorGUI(uTerrain);
        EditorGUIUtility.labelWidth = 0;
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new ClampFilter(inputs[0], inputs[1], Intensity, outBounds.x, outBounds.y);
    }
}