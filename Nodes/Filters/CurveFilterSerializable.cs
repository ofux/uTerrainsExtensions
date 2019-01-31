using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Curve")]
[Serializable]
public class CurveFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Curve"; }
    }

    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private Vector2 inBounds = new Vector2(-1, 1);
    [SerializeField] private Vector2 outBounds = new Vector2(-1, 1);

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR

        inBounds = EditorUtils.MinMaxField("Input min/max", inBounds);
        if (inBounds.y < inBounds.x + 0.001f)
            inBounds.y = inBounds.x + 0.001f;

        outBounds = EditorUtils.MinMaxField("Output min/max", outBounds);
        if (outBounds.y < outBounds.x + 0.001f)
            outBounds.y = outBounds.x + 0.001f;
        
        curve = EditorGUILayout.CurveField(curve, GUILayout.Width(100), GUILayout.Height(80));
        base.OnEditorGUI(uTerrain);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new CurveFilter(inputs[0], inputs[1], Intensity, curve, inBounds.x, inBounds.y, outBounds.x, outBounds.y);
    }
}