using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
#if UNITY_EDITOR

#endif


[PrettyTypeName("Invert")]
[Serializable]
public class InvertFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Invert"; }
    }

    public override void OnEditorGUI(UltimateTerrain uTerrain, IReadOnlyFlowGraph graph)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField("out = -in");
        base.OnEditorGUI(uTerrain, graph);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new InvertFilter(inputs[0], inputs[1], Intensity);
    }
}