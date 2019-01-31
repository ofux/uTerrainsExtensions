using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class FlattenSphere : IOperation
{
    [JsonProperty("p")] protected Vector3d position;
    [JsonProperty("s")] protected double radius;
    [JsonProperty("h")] protected double desiredHeight;
    [JsonProperty("v")] protected string voxelTypeName;
    [JsonProperty("d")] protected bool upsideDown;

    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected double radiusSquared;
    protected UnitConverter converter;

    // Empty constructor needed by serialization
    public FlattenSphere()
    {
    }

    // 
    public FlattenSphere(UltimateTerrain uTerrain, bool upsideDown, Vector3d position, double radius, double desiredHeight)
    {
        this.upsideDown = upsideDown;
        this.position = position;
        this.radius = radius;
        this.desiredHeight = desiredHeight;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public FlattenSphere(UltimateTerrain uTerrain, bool upsideDown, Vector3d position, double radius, double desiredHeight, VoxelType voxelType) : this(uTerrain, upsideDown, position, radius, desiredHeight)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }

    public static FlattenSphere CreateFromUnityWorld(UltimateTerrain uTerrain, bool upsideDown, Vector3 position, float radius, float desiredHeight, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new FlattenSphere(uTerrain, upsideDown, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), conv.UnityToVoxelDisance(desiredHeight), voxelType);
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
        // Compute distance between the voxel and the center of the sphere.
        var distanceSquared = (position - new Vector3d(x, y, z)).MagnitudeSquared;

        if (distanceSquared < radiusSquared) {
            if (!upsideDown) {
                voxel.Value = y - desiredHeight;
            } else {
                voxel.Value = desiredHeight - y;
            }

            if (voxelType != null) {
                voxel.VoxelType = voxelType;
                return true;
            }
        }

        return false;
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return other is FlattenSphere;
    }

    public IOperation Merge(IOperation other)
    {
        //Debug.Log("[FlattenSphere] Merged " + other + " with " + this);
        return this;
    }

    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        return Physics.OverlapSphereNonAlloc(converter.VoxelToUnityPosition(position), (float) converter.VoxelToUnityDisance(radius), collidersBuffer);
    }

    public virtual void OnOperationDone()
    {
        // do nothing
    }

    public virtual void OnOperationUndone()
    {
        // do nothing
    }

    public override string ToString()
    {
        return string.Format("[FlattenSphere] Position: {0}, Radius: {1}, VoxelTypeName: {2}, UpsideDown: {3}", position, radius, voxelTypeName, upsideDown);
    }

    // Protected methods /////////////////

    protected virtual void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = new Vector3d(radius, radius, radius).Rounded;
        var voxelPos = position.Rounded;
        from = voxelPos - halfSize - Vector3i.one;
        to = voxelPos + halfSize + Vector3i.one;
        radiusSquared = radius * radius;
    }
}