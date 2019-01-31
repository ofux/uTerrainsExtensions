using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class AxisAlignedCubeEditor : IOperationEditor
{
    protected ReticleForEditor Reticle;

    protected bool Dig;
    protected Vector3 Size = new Vector3(1f, 1f, 1f);
    protected int VoxelTypeIndex;

    public virtual string Name {
        get { return "Axis-Aligned Cube"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        Dig = EditorGUILayout.ToggleLeft("Dig", Dig);
        Size = editor.SizeField("Size:", Size, true);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }

    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<ReticleForEditor>("ReticleForEditor");
        }

        var hit = editor.GetIntersectionWithTerrain(true, !Dig);
        if (hit.HasValue) {
            Reticle.SetPositionAndSize(hit.Value.point, Size);
        }

        if (editor.Clicking) {
            Event.current.Use();
            editor.PerformOperation(CreateOperationFromEditor(Reticle.CenterPosition, editor), sceneview);
        }
    }

    public void DestroyReticle()
    {
        if (Reticle != null) {
            Object.DestroyImmediate(Reticle.gameObject);
            Reticle = null;
        }
    }

    protected virtual IOperation CreateOperationFromEditor(Vector3 worldPosition, TerrainToolEditor editor)
    {
        var voxelType = EditorUtils.GetVoxelTypeFromIndex(VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
        return AxisAlignedCube.CreateFromUnityWorld(editor.Terrain, Dig, worldPosition, Size, voxelType);
    }
}