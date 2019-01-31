using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class SphereEditor : IOperationEditor
{
    protected SphericalReticleForEditor Reticle;

    protected bool Dig;
    protected bool FollowGrid;
    protected float Radius = 4f;
    protected int VoxelTypeIndex;

    public virtual string Name {
        get { return "Sphere"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        FollowGrid = EditorGUILayout.ToggleLeft(new GUIContent("Follow Grid", "Check this to force the brush to be positionned on voxels."), FollowGrid);
        Dig = EditorGUILayout.ToggleLeft("Dig", Dig);
        Radius = EditorGUILayout.Slider("Radius:", Radius, 2f, 100f);
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
        return Sphere.CreateFromUnityWorld(editor.Terrain, Dig, worldPosition, Radius, voxelType);
    }
}