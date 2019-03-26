using System.Collections;
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

    public IEnumerator DebugPathCoroutine(double step = 1.0, double maxSlope = 0.7, bool aboveGroundOnly = true)
    {
        // Just make sure start point and end point are above the ground
        if (aboveGroundOnly) {
            EnsureStartEndPointsAreAboveTheGround(step, maxSlope);
        }

        Debug.Log(string.Format("Searching path from {0} to {1}...", start, end));

        var watch = Stopwatch.StartNew();

        PathFinder.Result path;
        if (aboveGroundOnly) {
            path = terrain.FindPathAsync(start, end, step, maxSlope);
        } else {
            path = terrain.FindPathInAirAsync(start, end, step);
        }

        // Wait for the result
        while (!path.Done) {
            yield return null;
        }

        watch.Stop();

        if (path.Found) {
            Debug.Log(string.Format("PATH FOUND (in less than {0}ms)", watch.ElapsedMilliseconds));

            var currentNode = path.FirstNode;
            while (currentNode != null) {
                var cube = CreateCubeAtNode(currentNode);
                cubes.Add(cube);
                currentNode = currentNode.Next;
            }
        } else {
            Debug.Log(string.Format("PATH NOT FOUND (in less than {0}ms)", watch.ElapsedMilliseconds));
        }
    }

    private GameObject CreateCubeAtNode(SearchNode currentNode)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = terrain.Converter.VoxelToUnityPosition(currentNode.Position);
        cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        cube.GetComponent<Collider>().enabled = false;
        cube.GetComponent<Renderer>().material.color = Color.yellow;
        cube.layer = 2;
        return cube;
    }

    private void EnsureStartEndPointsAreAboveTheGround(double step, double maxSlope)
    {
        while (start.y > -200 && start.y < 200) {
            start.y -= step * maxSlope;
            var voxel = terrain.GetVoxelAt(start);
            if (voxel.IsInside) {
                start.y += step * maxSlope;
                break;
            }
        }

        while (end.y > -200 && end.y < 200) {
            end.y -= step * maxSlope;
            var voxel = terrain.GetVoxelAt(end);
            if (voxel.IsInside) {
                end.y += step * maxSlope;
                break;
            }
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