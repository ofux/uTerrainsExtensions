using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class FlattenSphereEditor : IOperationEditor
{
    protected SphericalReticleForEditor Reticle;
    
    protected float DesiredHeight;
    protected float Radius = 4f;
    protected bool UpsideDown;
    protected int VoxelTypeIndex;

    public string Name {
        get { return "Flatten-Sphere"; }
    }

    public void OnInspector(TerrainToolEditor editor)
    {
        DesiredHeight = EditorGUILayout.FloatField("Height to flatten to:", DesiredHeight);
        Radius = EditorGUILayout.Slider("Radius:", Radius, 2f, 100f);
        UpsideDown = EditorGUILayout.Toggle("Upside-down:", UpsideDown);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }
    
    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<SphericalReticleForEditor>("SphericalReticleForEditor");
        }

        var hit = editor.GetIntersectionWithTerrain(true);
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

    protected IOperation CreateOperationFromEditor(Vector3 worldPosition, TerrainToolEditor editor)
    {
        var voxelType = EditorUtils.GetVoxelTypeFromIndex(VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
        return FlattenSphere.CreateFromUnityWorld(editor.Terrain, UpsideDown, worldPosition, Radius, DesiredHeight, voxelType);
    }
}