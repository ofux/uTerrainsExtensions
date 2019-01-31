using System;
using System.Collections.Generic;
using UltimateTerrains;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[PrettyTypeName("Distance 2D")]
[Serializable]
public class Distance2DPrimitiveSerializable : Primitive2DNodeSerializable
{
    public override string Title {
        get { return "Distance 2D"; }
    }
    
    public override int EditorWidth {
        get { return 100; }
    }

    // Useful properties for the module
    [SerializeField] private Vector2 origin;

    public override void OnEditorGUI(UltimateTerrain uTerrain)
    {
#if UNITY_EDITOR
        EditorGUIUtility.labelWidth = 60;
        origin.x = EditorGUILayout.FloatField("From X", origin.x, GUILayout.Width(100));
        origin.y = EditorGUILayout.FloatField("From Z", origin.y, GUILayout.Width(100));
        EditorGUIUtility.labelWidth = 0;
#endif
    }

    public override IGeneratorNode CreateModule(UltimateTerrain uTerrain, List<CallableNode> inputs)
    {
        return new Distance2DPrimitive(origin.x, origin.y);
    }
}