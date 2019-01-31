using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class ParallelepipedEditor : IOperationEditor
{
    protected ReticleLinesForEditor Reticle;

    protected bool Dig;
    protected float Width = 4f;
    protected float Height = 4f;
    protected int VoxelTypeIndex;
    protected Vector3? startPosition;

    public virtual string Name {
        get { return "Parallelepiped"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        Dig = EditorGUILayout.ToggleLeft("Dig", Dig);
        Width = EditorGUILayout.Slider("Width:", Width, 1f, 100f);
        Height = EditorGUILayout.Slider("Height:", Height, 1f, 100f);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }

    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<ReticleLinesForEditor>("ReticleLinesForEditor");
        }

        if (Event.current.control || Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            startPosition = null;
        }

        var hit = editor.GetIntersectionWithTerrain(true);
        if (hit.HasValue) {
            var p = hit.Value.point - new Vector3(0, Height * 0.5f, Width * 0.5f);
            Vector3 vL, vH, vW;
            if (startPosition.HasValue) {
                UMath.ComputeEdgeVectors(startPosition.Value, p, Width, Height, out vL, out vH, out vW);
                Reticle.SetCornerAndEdges(startPosition.Value, vL, vH, vW);
            } else {
                UMath.ComputeEdgeVectors(p, p + Vector3.forward, Width, Height, out vL, out vH, out vW);
                Reticle.SetCornerAndEdges(p, vL, vH, vW);
            }

            if (editor.Clicking) {
                Event.current.Use();
                if (!startPosition.HasValue) {
                    startPosition = p;
                } else {
                    editor.PerformOperation(CreateOperationFromEditor(startPosition.Value, p, editor), sceneview);
                    startPosition = null;
                }
            }
        }
    }

    public void DestroyReticle()
    {
        if (Reticle != null) {
            Object.DestroyImmediate(Reticle.gameObject);
            Reticle = null;
        }
    }

    protected virtual IOperation CreateOperationFromEditor(Vector3 start, Vector3 end, TerrainToolEditor editor)
    {
        var voxelType = EditorUtils.GetVoxelTypeFromIndex(VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
        Vector3 vL, vH, vW;
        UMath.ComputeEdgeVectors(start, end, Width, Height, out vL, out vH, out vW);
        return Parallelepiped.CreateFromUnityWorld(editor.Terrain, Dig, start, vL, vH, vW, voxelType);
    }
}