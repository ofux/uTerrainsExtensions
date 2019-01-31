using System;
using System.Collections.Generic;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


[PrettyTypeName("Terrace")]
[Serializable]
public class TerraceFilterSerializable : FilterNodeSerializable
{
    public override string Title {
        get { return "Terrace"; }
    }

    public override int InputCount {
        get { return 3; }
    }
    
    [SerializeField] private float width = 0.1f;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorGUIUtility.labelWidth = 60;
        width = EditorGUILayout.FloatField("Width", width);
        base.OnEditorGUI(uTerrain);
        EditorGUIUtility.labelWidth = 0;
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new TerraceFilter(inputs[0], inputs[2], Intensity, inputs[1], width);
    }
}