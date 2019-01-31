using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("To Height")]
[Serializable]
public class ToHeightTransformerSerializable : TransformerNodeSerializable
{
    public override string Title {
        get { return "To Height"; }
    }
    
    public override NodeLayer Layer {
        get { return NodeLayer.Layer3D; }
    }
    
    [SerializeField] private float scale = 1f;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorUtils.CenteredBoxedLabelField("out = y - in * scale");
        scale = EditorGUILayout.FloatField("Scale", scale);
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new ToHeightTransformer(inputs[0], scale);
    }
}