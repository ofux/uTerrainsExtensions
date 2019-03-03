using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UltimateTerrains
{
    [PrettyTypeName("Comment")]
    [Serializable]
    public class CommentNodeSerializable : UneffectiveNodeSerializable
    {
        [SerializeField] private string comment;

        public override string Title {
            get { return "Comment"; }
        }

        public override int EditorWidth {
            get { return 200; }
        }

        public override void OnEditorGUI(UltimateTerrain uTerrain)
        {
#if UNITY_EDITOR
            comment = EditorGUILayout.TextArea(comment);
#endif
        }
    }
}