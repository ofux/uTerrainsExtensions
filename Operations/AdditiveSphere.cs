using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public sealed class AdditiveSphere : IOperation
{
    [JsonProperty("p")] private Vector3i voxelPosition;
    [JsonProperty("s")] private int voxelRadius;
    [JsonProperty("v")] private string voxelTypeName;
    [JsonProperty("d")] private bool dig;
    [JsonProperty("sp")] private double speed;

    private VoxelType voxelType;
    private Vector3i from;
    private Vector3i to;
    private Vector3d unityPosition;
    private float unityRadius;

    // Empty constructor needed by serialization
    public AdditiveSphere()
    {
    }

    public AdditiveSphere(UltimateTerrain uTerrain, bool dig, Vector3d position, double radius, double speed)
    {
        this.speed = speed;
        this.dig = dig;
        this.voxelPosition = position.Rounded;
        this.voxelRadius = Convert.ToInt32(radius);
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    public AdditiveSphere(UltimateTerrain uTerrain, bool dig, Vector3d position, double radius, double speed, VoxelType voxelType) : this(uTerrain, dig, position, radius, speed)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }
    
    public static AdditiveSphere CreateFromUnityWorld(UltimateTerrain uTerrain, bool dig, Vector3 position, float radius, double speed, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new AdditiveSphere(uTerrain, dig, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), speed, voxelType);
    }

    private void Init(UltimateTerrain uTerrain)
    {
        var halfSize = new Vector3i(voxelRadius, voxelRadius, voxelRadius);
        from = voxelPosition - halfSize;
        to = voxelPosition + halfSize;
        unityPosition = uTerrain.Converter.VoxelToUnityPosition(voxelPosition);
        unityRadius = (float) uTerrain.Converter.VoxelToUnityDisance(voxelRadius);
    }

    // Called after this object has been deserialized. You can init some stuff that you would normally do in the constructor.
    public void AfterDeserialize(UltimateTerrain uTerrain)
    {
        Init(uTerrain);
        if (!string.IsNullOrEmpty(voxelTypeName)) {
            voxelType = uTerrain.VoxelTypeSet.GetVoxelType(voxelTypeName);
        }
    }

    public bool CanBeMergedWith(IOperation other)
    {
        return other is AdditiveSphere && from == other.GetAreaOfEffectMin() && to == other.GetAreaOfEffectMax();
    }

    public IOperation Merge(IOperation other)
    {
        //Debug.Log("[AdditiveSphere] Merged " + other + " with " + this);
        var otherAdditiveSphere = other as AdditiveSphere;
        if (otherAdditiveSphere != null) {
            this.speed += otherAdditiveSphere.speed;
        }

        return this;
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
        var voxelDistance = (voxelPosition - new Vector3d(x, y, z)).Magnitude;
        var clampedValue = voxel.GetClampedValue(voxelRadius);

        if (dig) {
            voxel.Value = Math.Max(clampedValue, clampedValue + speed * (voxelRadius - voxelDistance));
        } else {
            voxel.Value = Math.Min(clampedValue, clampedValue + speed * (voxelDistance - voxelRadius));
        }

        if (voxelType != null && voxelDistance <= voxelRadius + 0.1) {
            voxel.VoxelType = voxelType;
            return true;
        }

        return false;
    }

    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        return Physics.OverlapSphereNonAlloc(unityPosition.ToUnityOrigin(), unityRadius, collidersBuffer);
    }

    public void OnOperationDone()
    {
        // do nothing
    }

    public void OnOperationUndone()
    {
        // do nothing
    }

    public OperationColliderDetectionShape GetOperationColliderDetectionShape()
    {
        return OperationColliderDetectionShape.SPHERE;
    }

    public override string ToString()
    {
        return string.Format("[AdditiveSphere] VoxelPosition: {0}, VoxelRadius: {1}, VoxelTypeName: {2}, Dig: {3}, Speed: {4}", voxelPosition, voxelRadius, voxelTypeName, dig, speed);
    }
}