using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Paint : IOperation
{
    [JsonProperty("p")] protected Vector3i voxelPosition;
    [JsonProperty("s")] protected int voxelRadius;
    [JsonProperty("v")] protected string voxelTypeName;

    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected double radiusSquared;

    // Empty constructor needed by serialization
    public Paint()
    {
    }

    // 
    public Paint(UltimateTerrain uTerrain, Vector3d position, double radius, VoxelType voxelType)
    {
        this.voxelPosition = position.Rounded;
        this.voxelRadius = Convert.ToInt32(radius);
        this.voxelType = voxelType;
        voxelTypeName = voxelType.Name;
        Init(uTerrain);
    }
    
    public static Paint CreateFromUnityWorld(UltimateTerrain uTerrain, Vector3 position, float radius, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new Paint(uTerrain, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), voxelType);
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

    public virtual bool Act(double x, double y, double z, ref Voxel voxel)
    {
        if ((voxelPosition - new Vector3d(x, y, z)).MagnitudeSquared <= radiusSquared) {
            voxel.VoxelType = voxelType;
            return true;
        }

        return false;
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return other is Paint &&
               (other.GetAreaOfEffectMax() == GetAreaOfEffectMax() && other.GetAreaOfEffectMin() == GetAreaOfEffectMin()
                || ((Paint) other).voxelType == voxelType);
    }

    public IOperation Merge(IOperation other)
    {
        //Debug.Log("[Paint] Merged " + other + " with " + this);
        return this;
    }

    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        return 0;
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
        return string.Format("[Paint] VoxelPosition: {0}, VoxelRadius: {1}, VoxelTypeName: {2}", voxelPosition, voxelRadius, voxelTypeName);
    }

    // Protected methods /////////////////

    protected void Init(UltimateTerrain uTerrain)
    {
        var halfSize = new Vector3i(voxelRadius, voxelRadius, voxelRadius);
        from = voxelPosition - halfSize;
        to = voxelPosition + halfSize;
        radiusSquared = voxelRadius * voxelRadius;
    }
}