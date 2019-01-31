using System;
using UltimateTerrains;
using UnityEngine;

// This operation is doing the exact same thing than DigCube, except that the value must be modified in the opposite way.
[Obsolete]
public class AddCube : DigCube
{
    // Empty constructor needed by serialization
    public AddCube()
    {
    }

    // Inherit from DigCube constructor
    public AddCube(UltimateTerrain uTerrain, Vector3d position, Vector3 size) : base(uTerrain, position, size)
    {
    }

    // Inherit from DigCube constructor
    public AddCube(UltimateTerrain uTerrain, Vector3d position, Vector3 size, VoxelType voxelType) : base(uTerrain, position, size, voxelType)
    {
    }

    public override bool Act(double x, double y, double z, ref Voxel voxel)
    {
        if (x >= from.x && x <= to.x &&
            y >= from.y && y <= to.y &&
            z >= from.z && z <= to.z) {
            voxel.Value = Math.Min(voxel.Value, Vector3Utils.DistanceTo(bounds, converter.VoxelToUnityPosition(x, y, z).ToUnityOrigin()));
            if (voxelType != null) {
                voxel.VoxelType = voxelType;
            }

            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return string.Format("[AddCube] at " + position + " of size " + size);
    }
}