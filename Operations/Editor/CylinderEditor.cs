using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;

public class CylinderEditor : IOperationEditor
{
    protected CylinderReticleForEditor Reticle;

    protected bool Dig;
    protected float Radius = 4f;
    protected int VoxelTypeIndex;
    protected Vector3? startPosition;

    public virtual string Name {
        get { return "Cylinder"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        Dig = EditorGUILayout.ToggleLeft("Dig", Dig);
        Radius = EditorGUILayout.Slider("Radius:", Radius, 1f, 100f);
        VoxelTypeIndex = EditorUtils.VoxelTypeField("Voxel type:", VoxelTypeIndex, editor.Terrain.VoxelTypeSet);
    }

    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<CylinderReticleForEditor>("CylinderReticleForEditor");
        }

        if (Event.current.control || Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            startPosition = null;
        }

        var hit = editor.GetIntersectionWithTerrain(true);
        if (hit.HasValue) {
            var p = hit.Value.point;
            if (startPosition.HasValue) {
                Reticle.SetPositionsAndRadius(startPosition.Value, hit.Value.point, Radius);
            } else {
                Reticle.SetPositionsAndRadius(hit.Value.point, hit.Value.point + Vector3.right, Radius);
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
        return Cylinder.CreateFromUnityWorld(editor.Terrain, Dig, start, end, Radius, voxelType);
    }
}