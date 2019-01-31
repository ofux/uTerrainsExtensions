using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Lerp")]
[Serializable]
public class LerpFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Lerp"; }
    }

    [SerializeField] private Vector2 inBounds = new Vector2(-1, 1);
    [SerializeField] private Vector2 outBounds = new Vector2(-1, 1);

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField(string.Format("in ∈ [{0},{1}]\n=> out ∈ [{2},{3}]",
                                                          inBounds.x, inBounds.y, outBounds.x, outBounds.y), 
                                            GUILayout.MaxWidth(100));
        
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(100));

        inBounds = EditorUtils.MinMaxField("Input min/max", inBounds);
        if (inBounds.y < inBounds.x + 0.001f)
            inBounds.y = inBounds.x + 0.001f;

        outBounds = EditorUtils.MinMaxField("Output min/max", outBounds);
        if (outBounds.y < outBounds.x + 0.001f)
            outBounds.y = outBounds.x + 0.001f;
        
        EditorGUILayout.EndVertical();
        base.OnEditorGUI(uTerrain);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new LerpFilter(inputs[0], inputs[1], Intensity, inBounds.x, inBounds.y, outBounds.x, outBounds.y);
    }
}