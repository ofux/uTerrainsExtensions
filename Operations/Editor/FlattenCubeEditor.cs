using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class FlattenCubeEditor : IOperationEditor
{
    protected ReticleForEditor Reticle;
    
    protected float DesiredHeight;
    protected Vector3 Size = new Vector3(4f, 30f, 4f);
    protected bool UpsideDown;
    protected int VoxelTypeIndex;

    public string Name {
        get { return "Flatten-Cube"; }
    }

    public void OnInspector(TerrainToolEditor editor)
    {
        DesiredHeight = EditorGUILayout.FloatField("Height to flatten to:", DesiredHeight);
        Size = editor.SizeField("Size:", Size, true);
        UpsideDown = EditorGUILayout.Toggle("Upside-down:", UpsideDown);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }
    
    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<ReticleForEditor>("ReticleForEditor");
        }

        var hit = editor.GetIntersectionWithTerrain(true);
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

    protected IOperation CreateOperationFromEditor(Vector3 worldPosition, TerrainToolEditor editor)
    {
        var voxelType = EditorUtils.GetVoxelTypeFromIndex(VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
        return FlattenAxisAlignedCube.CreateFromUnityWorld(editor.Terrain, UpsideDown, worldPosition, Size, DesiredHeight, voxelType);
    }
}