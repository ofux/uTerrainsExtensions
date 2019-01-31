using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class AxisAlignedCube : Parallelepiped
{
    // Empty constructor needed by serialization
    public AxisAlignedCube()
    {
    }

    public AxisAlignedCube(UltimateTerrain uTerrain, bool dig, Vector3d position, Vector3d size)
    {
        var halfSize = size * 0.5;
        this.dig = dig;
        this.corner = position - halfSize;
        this.vL = Vector3d.right * size.x;
        this.vH = Vector3d.up * size.y;
        this.vW = Vector3d.forward * size.z;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    public AxisAlignedCube(UltimateTerrain uTerrain, bool dig, Vector3d position, Vector3d size, VoxelType voxelType) :
        this(uTerrain, dig, position, size)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }
    
    public static AxisAlignedCube CreateFromUnityWorld(UltimateTerrain uTerrain, bool dig, Vector3 position, Vector3 size, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new AxisAlignedCube(uTerrain, dig, conv.UnityToVoxelPosition(position), conv.UnityToVoxel(size), voxelType);
    }

    public override bool CanBeMergedWith(IOperation other)
    {
        return true;
    }

    public override IOperation Merge(IOperation other)
    {
        //Debug.Log("[AxisAlignedCube] Merged " + other + " with " + this);
        return this;
    }

    public override string ToString()
    {
        return string.Format("[AxisAlignedCube] Dig: {0}, Corner: {1}, VL: {2}, VH: {3}, VW: {4}, VoxelTypeName: {5}", dig, corner, vL, vH, vW, voxelTypeName);
    }
}