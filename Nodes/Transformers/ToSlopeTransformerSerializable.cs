using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
#if UNITY_EDITOR

#endif


[PrettyTypeName("To Slope")]
[Serializable]
public class ToSlopeTransformerSerializable : TransformerNodeSerializable
{
    public override string Title {
        get { return "To Slope"; }
    }

    public override NodeLayer Layer {
        get { return NodeLayer.Layer2D; }
    }

    public override void OnEditorGUI(UltimateTerrain uTerrain, IReadOnlyFlowGraph graph)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField("Uses Sobel Filter.\n" +
                                            "CAUTION: This is costly as it needs to compute the input 4 times at different positions.");
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new ToSlopeTransformer(inputs[0]);
    }
}