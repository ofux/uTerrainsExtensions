using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Abs")]
[Serializable]
public class AbsFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Abs"; }
    }

    public override void OnEditorGUI(UltimateTerrain uTerrain, IReadOnlyFlowGraph graph)
    {
#if UNITY_EDITOR
        EditorGUIUtility.labelWidth = 60;
        EditorUtils.CenteredBoxedLabelField("out = |in|");
        base.OnEditorGUI(uTerrain, graph);
        EditorGUIUtility.labelWidth = 0;
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new AbsFilter(inputs[0], inputs[1], Intensity);
    }
}