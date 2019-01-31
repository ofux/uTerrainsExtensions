using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Cylinder : IOperation
{
    [JsonProperty("sp")] protected Vector3d start;
    [JsonProperty("ep")] protected Vector3d end;
    [JsonProperty("s")] protected double radius;
    [JsonProperty("v")] protected string voxelTypeName;
    [JsonProperty("d")] protected bool dig;

    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected Vector3d direction;
    protected double length;
    protected UnitConverter converter;
    protected double voxelSizeInverse;

    // Empty constructor needed by serialization
    public Cylinder()
    {
    }

    // 
    public Cylinder(UltimateTerrain uTerrain, bool dig, Vector3d start, Vector3d end, double radius)
    {
        this.dig = dig;
        this.start = start;
        this.end = end;
        this.radius = radius;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public Cylinder(UltimateTerrain uTerrain, bool dig, Vector3d start, Vector3d end, double radius, VoxelType voxelType) : this(uTerrain, dig, start, end, radius)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }

    public static Cylinder CreateFromUnityWorld(UltimateTerrain uTerrain, bool dig, Vector3 start, Vector3 end, float radius, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new Cylinder(uTerrain, dig, conv.UnityToVoxelPosition(start), conv.UnityToVoxelPosition(end), conv.UnityToVoxelDisance(radius), voxelType);
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
        double distance;
        var pos = new Vector3d(x, y, z);
        var orthogonalOntoLine = UMath.OrthogonalProjectionOntoLineNormalized(pos, start, direction);

        var orthoDistanceToStart = Vector3d.Dot(direction, orthogonalOntoLine - start);
        if (orthoDistanceToStart > 0 && orthoDistanceToStart < length) {
            distance = (orthogonalOntoLine - pos).Magnitude - radius;
        } else if (orthoDistanceToStart < 0) {
            distance = -orthoDistanceToStart;
        } else {
            distance = orthoDistanceToStart - length;
        }

        if (dig) {
            voxel.Value = Math.Max(voxel.Value, -distance * voxelSizeInverse);
        } else {
            voxel.Value = Math.Min(voxel.Value, distance * voxelSizeInverse);
        }

        if (voxelType != null && distance <= radius + 0.1) {
            voxel.VoxelType = voxelType;
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
        return Physics.OverlapCapsuleNonAlloc(converter.VoxelToUnityPosition(start), converter.VoxelToUnityPosition(end), 
                                              (float) converter.VoxelToUnityDisance(radius), collidersBuffer);
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
        return string.Format("Start: {0}, End: {1}, Radius: {2}, VoxelTypeName: {3}, Dig: {4}", start, end, radius, voxelTypeName, dig);
    }


    // Protected methods /////////////////

    protected virtual void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = new Vector3d(radius, radius, radius).Rounded;
        halfSize.x += 1;
        halfSize.y += 1;
        halfSize.z += 1;
        var startVoxelPos = start.Rounded;
        var endVoxelPos = end.Rounded;
        from = UMath.Min(startVoxelPos, endVoxelPos) - halfSize;
        to = UMath.Max(startVoxelPos, endVoxelPos) + halfSize;
        direction = (end - start).Normalized;
        length = (end - start).Magnitude;
        voxelSizeInverse = 1.0 / uTerrain.Params.SizeXVoxel;
    }

}