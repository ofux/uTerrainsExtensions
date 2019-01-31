using System;
using UltimateTerrains;
using UltimateTerrainsEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class SmoothEditor : IOperationEditor
{
    protected SphericalReticleForEditor Reticle;

    protected float Radius = 12f;
    protected float SamplingAccuracy = 3f;

    public virtual string Name {
        get { return "Smooth"; }
    }

    public virtual void OnInspector(TerrainToolEditor editor)
    {
        Radius = EditorGUILayout.Slider("Radius:", Radius, 2f, 100f);
        
        EditorGUILayout.HelpBox("Smooth operation can quickly drop performance! Try to keep 'Sampling Accuracy' as " +
                                "low as possible.", MessageType.Warning, true);
        SamplingAccuracy = EditorGUILayout.Slider("Sampling Accuracy:", SamplingAccuracy, 1f, 8f);
    }

    public void OnScene(TerrainToolEditor editor, SceneView sceneview)
    {
        if (Reticle == null) {
            Reticle = editor.LoadReticle<SphericalReticleForEditor>("SphericalReticleForEditor");
        }

        var hit = editor.GetIntersectionWithTerrain(false);
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
        return Smooth.CreateFromUnityWorld(editor.Terrain, worldPosition, Radius, Math.Max(1.0, Radius / SamplingAccuracy));
    }
}