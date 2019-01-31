using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Perturb : IOperation
{
    [JsonProperty("p")] protected Vector3d position;
    [JsonProperty("s")] protected double radius;
    [JsonProperty("w")] protected double weight;
    [JsonProperty("o")] protected int octave;

    protected Vector3i from;
    protected Vector3i to;
    protected UnitConverter converter;
    protected double radiusPlusOneSquared;

    // Empty constructor needed by serialization
    public Perturb()
    {
    }

    // 
    public Perturb(UltimateTerrain uTerrain, Vector3d position, double radius, double weight)
    {
        this.position = position;
        this.radius = radius;
        this.weight = weight;
        this.octave = 1;
        Init(uTerrain);
    }

    public static Perturb CreateFromUnityWorld(UltimateTerrain uTerrain, Vector3 position, float radius, double step)
    {
        var conv = uTerrain.Converter;
        return new Perturb(uTerrain, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), step);
    }

    // Called after this object has been deserialized. You can init some stuff that you would normally do in the constructor.
    public void AfterDeserialize(UltimateTerrain uTerrain)
    {
        Init(uTerrain);
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

    public virtual bool Act(double x, double y, double z, ref Voxel voxel)
    {
        var distanceSquared = position.DistanceSquared(new Vector3d(x, position.y, z));
        if (!(distanceSquared <= radiusPlusOneSquared)) return false;

        for (var i = 0; i < octave; ++i) {
            voxel.Value -= Math.Sign(voxel.Value) * weight;
        }

        return false;
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return other is Perturb && other.GetAreaOfEffectMax() == this.GetAreaOfEffectMax()
                                && other.GetAreaOfEffectMin() == this.GetAreaOfEffectMin()
                                && UMath.Approximately(((Perturb) other).weight, weight);
    }

    public IOperation Merge(IOperation other)
    {
        ++octave;
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
        return string.Format("[Perturb] at " + position + " of radius " + radius);
    }

    // Protected methods /////////////////

    protected void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = new Vector3d(radius, radius, radius).Rounded;
        halfSize.x += 2;
        halfSize.y += 2;
        halfSize.z += 2;
        var voxelPos = position.Rounded;
        from = voxelPos - halfSize;
        to = voxelPos + halfSize;
        radiusPlusOneSquared = (radius + uTerrain.Params.SizeXVoxel) * (radius + uTerrain.Params.SizeXVoxel);
    }
}