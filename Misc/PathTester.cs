using System.Collections.Generic;
using System.Diagnostics;
using UltimateTerrains;
using UltimateTerrains.Pathfinder;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// A simple behaviour to test path finding. Press 'I' to set start point, move where you want, and then press 'I' again to
/// find path between the start point and your position.
/// </summary>
public class PathTester
{
    private readonly UltimateTerrain terrain;
    private readonly List<GameObject> cubes = new List<GameObject>();

    private Vector3d start;
    private Vector3d end;

    public PathTester(UltimateTerrain terrain)
    {
        this.terrain = terrain;
    }

    public Vector3d Start {
        get { return Start; }
        set { start = value; }
    }

    public Vector3d End {
        get { return end; }
        set { end = value; }
    }

    public void DebugPath(double step = 1.0, double maxSlope = 0.7, bool aboveGroundOnly = true)
    {
        if (aboveGroundOnly) {
            while (end.y > 0) {
                end.y--;
                var voxel = terrain.GetVoxelAt(end);
                if (voxel.IsInside) {
                    end.y++;
                    break;
                }
            }
        }

        Debug.Log(string.Format("Searching path from {0} to {1}...", start, end));
        var pathFinder = new PathFinder(terrain);

        var watch = Stopwatch.StartNew();
        PathFinder.Result path;
        if (aboveGroundOnly) {
            path = pathFinder.FindPath(start, end, step, maxSlope);
        } else {
            path = pathFinder.FindPathInAir(start, end, step);
        }

        watch.Stop();

        if (!path.Found) {
            Debug.Log(string.Format("PATH NOT FOUND (in {0}ms)", watch.ElapsedMilliseconds));
            return;
        }

        Debug.Log(string.Format("PATH FOUND (in {0}ms)", watch.ElapsedMilliseconds));

        var current = path.FirstNode;
        while (current != null) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = terrain.Converter.VoxelToUnityPosition(current.Position);
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            cube.GetComponent<Collider>().enabled = false;
            cube.GetComponent<Renderer>().material.color = Color.yellow;
            cube.layer = 2;
            cubes.Add(cube);
            current = current.Next;
        }
    }


    public void ClearCubes()
    {
        foreach (var cube in cubes) {
            Object.Destroy(cube);
        }

        cubes.Clear();
    }
}