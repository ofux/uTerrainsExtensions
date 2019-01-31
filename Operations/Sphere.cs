using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Sphere : IOperation
{
    [JsonProperty("p")] protected Vector3d position;
    [JsonProperty("s")] protected double radius;
    [JsonProperty("v")] protected string voxelTypeName;
    [JsonProperty("d")] protected bool dig;

    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected double radiusSquared;
    protected UnitConverter converter;

    // Empty constructor needed by serialization
    public Sphere()
    {
    }

    // 
    public Sphere(UltimateTerrain uTerrain, bool dig, Vector3d position, double radius)
    {
        this.dig = dig;
        this.position = position;
        this.radius = radius;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public Sphere(UltimateTerrain uTerrain, bool dig, Vector3d position, double radius, VoxelType voxelType) : this(uTerrain, dig, position, radius)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }

    public static Sphere CreateFromUnityWorld(UltimateTerrain uTerrain, bool dig, Vector3 position, float radius, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new Sphere(uTerrain, dig, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), voxelType);
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
        var distance = (position - new Vector3d(x, y, z)).Magnitude;

        if (dig) {
            voxel.Value = Math.Max(voxel.Value, (radius - distance));
        } else {
            voxel.Value = Math.Min(voxel.Value, (distance - radius));
        }

        if (voxelType != null && distance < radius + 0.1) {
            voxel.VoxelType = voxelType;
            return true;
        }

        return false;
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return (position - other.GetAreaOfEffectMin()).MagnitudeSquared <= radiusSquared
               &&
               (position - other.GetAreaOfEffectMax()).MagnitudeSquared <= radiusSquared;
    }

    public IOperation Merge(IOperation other)
    {
        //Debug.Log("[Sphere] Merged " + other + " with " + this);
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
        return string.Format("Position: {0}, Radius: {1}, VoxelTypeName: {2}, Dig: {3}", position, radius, voxelTypeName, dig);
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