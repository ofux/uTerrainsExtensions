using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public sealed class FlattenAxisAlignedCube : IOperation
{
    [JsonProperty("p")] private Vector3d position;
    [JsonProperty("s")] private Vector3d size;
    [JsonProperty("h")] private double desiredHeight;
    [JsonProperty("v")] private string voxelTypeName;
    [JsonProperty("d")] private bool upsideDown;

    private VoxelType voxelType;
    private Vector3i from;
    private Vector3i to;
    private UnitConverter converter;

    // Empty constructor needed by serialization
    public FlattenAxisAlignedCube()
    {
    }

    // 
    public FlattenAxisAlignedCube(UltimateTerrain uTerrain, bool upsideDown, Vector3d position, Vector3d size, double desiredHeight)
    {
        this.upsideDown = upsideDown;
        this.position = position;
        this.size = size;
        this.desiredHeight = desiredHeight;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public FlattenAxisAlignedCube(UltimateTerrain uTerrain, bool upsideDown, Vector3d position, Vector3d size, double desiredHeight, VoxelType voxelType) : this(uTerrain, upsideDown, position, size, desiredHeight)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }

    public static FlattenAxisAlignedCube CreateFromUnityWorld(UltimateTerrain uTerrain, bool upsideDown, Vector3 position, Vector3 size, float desiredHeight, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new FlattenAxisAlignedCube(uTerrain, upsideDown, conv.UnityToVoxelPosition(position), conv.UnityToVoxel(size), conv.UnityToVoxelDisance(desiredHeight), voxelType);
    }

    // Called after this object has been deserialized. You can init some stuff that you would normally do in the constructor.
    public void AfterDeserialize(UltimateTerrain uTerrain)
    {
        Init(uTerrain);
        if (!string.IsNullOrEmpty(voxelTypeName)) {
            voxelType = uTerrain.VoxelTypeSet.GetVoxelType(voxelTypeName);
        }
    }

    public Vector3i GetAreaOfEffectMin()
    {
        return from;
    }

    public Vector3i GetAreaOfEffectMax()
    {
        return to;
    }

    public bool WillAffectChunk(Vector3i chunkDownBackLeft, Vector3i chunkUpFrontRight)
    {
        return true;
    }

    public bool Act(double x, double y, double z, ref Voxel voxel)
    {
        if (!upsideDown) {
            voxel.Value = y - desiredHeight;
        } else {
            voxel.Value = desiredHeight - y;
        }

        if (voxelType != null) {
            voxel.VoxelType = voxelType;
            return true;
        }

        return false;
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return (other is FlattenSphere || other is FlattenAxisAlignedCube);
    }

    public IOperation Merge(IOperation other)
    {
        //Debug.Log("[FlattenAxisAlignedCube] Merged " + other + " with " + this);
        return this;
    }

    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        return Physics.OverlapBoxNonAlloc(converter.VoxelToUnityPosition(position), converter.VoxelToUnity(size * 0.5), collidersBuffer);
    }

    public void OnOperationDone()
    {
        // do nothing
    }

    public void OnOperationUndone()
    {
        // do nothing
    }

    // Protected methods /////////////////

    private void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = (size * 0.5).Rounded;
        var voxelPos = position.Rounded;
        from = voxelPos - halfSize - Vector3i.one;
        to = voxelPos + halfSize + Vector3i.one;
    }
}