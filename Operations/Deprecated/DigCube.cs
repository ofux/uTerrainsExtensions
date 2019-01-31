using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[Obsolete]
[JsonObject(MemberSerialization.OptIn)]
public class DigCube : IOperation
{
    [JsonProperty("p")] protected Vector3d position;
    [JsonProperty("s")] protected Vector3 size;
    [JsonProperty("v")] protected string voxelTypeName;

    protected Bounds bounds;
    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected UnitConverter converter;

    // Empty constructor needed by serialization
    public DigCube()
    {
    }

    // 
    public DigCube(UltimateTerrain uTerrain, Vector3d position, Vector3 size)
    {
        this.position = position;
        this.size = size;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public DigCube(UltimateTerrain uTerrain, Vector3d position, Vector3 size, VoxelType voxelType) : this(uTerrain, position, size)
    {
        this.voxelType = voxelType;
        voxelTypeName = voxelType.Name;
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
        if (x >= from.x && x <= to.x &&
            y >= from.y && y <= to.y &&
            z >= from.z && z <= to.z) {
            voxel.Value = Math.Max(voxel.Value, -Vector3Utils.DistanceTo(bounds, converter.VoxelToUnityPosition(x, y, z).ToUnityOrigin()));
            if (voxelType != null) {
                voxel.VoxelType = voxelType;
            }

            return true;
        }

        return false;
    }
    
    public bool CanBeMergedWith(IOperation other)
    {
        return false;
    }

    public IOperation Merge(IOperation other)
    {
        return null;
    }
    
    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        return Physics.OverlapBoxNonAlloc(position.ToUnityOrigin(), size * 0.5f, collidersBuffer);
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
        return string.Format("[DigCube] at " + position + " of size " + size);
    }

    // Protected methods /////////////////

    protected void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = converter.UnityToVoxelPositionRound(size * 0.5f);
        var voxelPos = converter.UnityToVoxelPositionRound(position);
        from = voxelPos - halfSize - Vector3i.one;
        to = voxelPos + halfSize + Vector3i.one;
        bounds = new Bounds((Vector3) position, size);
    }

    public OperationColliderDetectionShape GetOperationColliderDetectionShape()
    {
        return OperationColliderDetectionShape.BOX;
    }
}