using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class PaintEditor : IOperationEditor
{
    protected SphericalReticleForEditor Reticle;

    protected bool FollowGrid;
    protected float Radius = 4f;
    protected int VoxelTypeIndex;

    public virtual string Name {
        get { return "Paint"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        FollowGrid = EditorGUILayout.ToggleLeft(new GUIContent("Follow Grid", "Check this to force the brush to be positionned on voxels."), FollowGrid);
        Radius = EditorGUILayout.Slider("Radius:", Radius, 1f, 100f);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }

    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<SphericalReticleForEditor>("SphericalReticleForEditor");
        }

        var hit = editor.GetIntersectionWithTerrain(FollowGrid);
        if (hit.HasValue) {
            Reticle.SetPositionAndSize(hit.Value.point, Radius);
        }

        if (editor.Clicking) {
            Event.current.Use();
            editor.PerformOperation(CreateOperationFromEditor(Reticle.transform.position, editor), sceneview);
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
        return Paint.CreateFromUnityWorld(editor.Terrain, worldPosition, Radius, voxelType);
    }
}