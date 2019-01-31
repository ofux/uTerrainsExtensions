using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Parallelepiped : IOperation
{
    [JsonProperty("d")] protected bool dig;
    [JsonProperty("p")] protected Vector3d corner;
    [JsonProperty("l")] protected Vector3d vL; // Length
    [JsonProperty("h")] protected Vector3d vH; // Height
    [JsonProperty("w")] protected Vector3d vW; // Width
    [JsonProperty("v")] protected string voxelTypeName;

    protected VoxelType voxelType;
    protected Vector3i from;
    protected Vector3i to;
    protected Vector3d oppositeCorner, nLH, nHW, nWL;
    protected UnitConverter converter;
    protected Vector3d halfExtents;

    // Empty constructor needed by serialization
    public Parallelepiped()
    {
    }

    // 
    public Parallelepiped(UltimateTerrain uTerrain, bool dig, Vector3d corner, Vector3d vL, Vector3d vH,
                          Vector3d vW)
    {
        this.dig = dig;
        this.corner = corner;
        this.vL = vL;
        this.vH = vH;
        this.vW = vW;
        voxelType = null;
        voxelTypeName = string.Empty;
        Init(uTerrain);
    }

    // 
    public Parallelepiped(UltimateTerrain uTerrain, bool dig, Vector3d corner, Vector3d vL, Vector3d vH,
                          Vector3d vW, VoxelType voxelType) : this(
        uTerrain, dig, corner, vL, vH, vW)
    {
        if (voxelType != null) {
            this.voxelType = voxelType;
            voxelTypeName = voxelType.Name;
        }
    }
    
    public static Parallelepiped CreateFromUnityWorld(UltimateTerrain uTerrain, bool dig, Vector3 corner, Vector3 vL, Vector3 vH,
                                              Vector3 vW, VoxelType voxelType)
    {
        var conv = uTerrain.Converter;
        return new Parallelepiped(uTerrain, dig, conv.UnityToVoxelPosition(corner), 
                                  conv.UnityToVoxel(vL), conv.UnityToVoxel(vH), conv.UnityToVoxel(vW), voxelType);
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
        return DistanceToParallelepiped(chunkDownBackLeft) <= Param.ChunkDiagonalSize+1 &&
               DistanceToParallelepiped(chunkUpFrontRight) <= Param.ChunkDiagonalSize+1;
    }

    public bool Act(double x, double y, double z, ref Voxel voxel)
    {
        var d = DistanceToParallelepiped(new Vector3d(x, y, z));
        voxel.Value = dig ? Math.Max(voxel.Value, -d) : Math.Min(voxel.Value, d);
        if (voxelType != null && d < 0.1) {
            voxel.VoxelType = voxelType;
            return true;
        }

        return false;
    }

    public virtual bool CanBeMergedWith(IOperation other)
    {
        return false;
    }

    public virtual IOperation Merge(IOperation other)
    {
        return null;
    }
    
    public int FindAffectedColliders(Collider[] collidersBuffer)
    {
        var p = converter.VoxelToUnityPosition((oppositeCorner + corner) * 0.5);
        var he = converter.VoxelToUnityPosition(halfExtents);
        // Uncomment for debug
        //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.localScale = he * 2;
        //cube.transform.position = p;
        //cube.transform.rotation = Quaternion.LookRotation((Vector3) vL);
        return Physics.OverlapBoxNonAlloc(p, he, collidersBuffer, Quaternion.LookRotation((Vector3)vL));
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
        return string.Format("[Parallelepiped] Dig: {0}, Corner: {1}, VL: {2}, VH: {3}, VW: {4}, VoxelTypeName: {5}", dig, corner, vL, vH, vW, voxelTypeName);
    }

    // Protected methods /////////////////

    protected double DistanceToParallelepiped(Vector3d p)
    {
        var d1 = DistanceToHalfParallelepiped(p, corner, -nLH, -nHW, -nWL);
        var d2 = DistanceToHalfParallelepiped(p, oppositeCorner, nLH, nHW, nWL);
        return Math.Max(d1, d2);
    }

    protected double DistanceToHalfParallelepiped(Vector3d p, Vector3d halfPCorner,
                                                  Vector3d normLH, Vector3d normHW, Vector3d normWL)
    {
        var dW = UMath.DistanceToPlaneNormalized(p, halfPCorner, normLH);

        var dL = UMath.DistanceToPlaneNormalized(p, halfPCorner, normHW);

        var dH = UMath.DistanceToPlaneNormalized(p, halfPCorner, normWL);

        return Math.Max(dW, Math.Max(dL, dH));
    }

    protected void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        Vector3d min, max;

        min.x = corner.x;
        max.x = min.x;
        if (vL.x < 0)
            min.x += vL.x;
        else
            max.x += vL.x;
        if (vH.x < 0)
            min.x += vH.x;
        else
            max.x += vH.x;
        if (vW.x < 0)
            min.x += vW.x;
        else
            max.x += vW.x;

        min.y = corner.y;
        max.y = min.y;
        if (vL.y < 0)
            min.y += vL.y;
        else
            max.y += vL.y;
        if (vH.y < 0)
            min.y += vH.y;
        else
            max.y += vH.y;
        if (vW.y < 0)
            min.y += vW.y;
        else
            max.y += vW.y;

        min.z = corner.z;
        max.z = min.z;
        if (vL.z < 0)
            min.z += vL.z;
        else
            max.z += vL.z;
        if (vH.z < 0)
            min.z += vH.z;
        else
            max.z += vH.z;
        if (vW.z < 0)
            min.z += vW.z;
        else
            max.z += vW.z;
        
        from = new Vector3i(min) - Vector3i.one;
        to = new Vector3i(max) + Vector3i.one;

        oppositeCorner = corner + vL + vH + vW;
        nLH = Vector3d.Cross(vL, vH).Normalized;
        nLH *= Math.Sign(UMath.DistanceToPlane(oppositeCorner, corner, nLH));
        nHW = Vector3d.Cross(vH, vW).Normalized;
        nHW *= Math.Sign(UMath.DistanceToPlane(oppositeCorner, corner, nHW));
        nWL = Vector3d.Cross(vW, vL).Normalized;
        nWL *= Math.Sign(UMath.DistanceToPlane(oppositeCorner, corner, nWL));
        
        halfExtents = new Vector3d(vW.Magnitude, vH.Magnitude, vL.Magnitude) * 0.5;
    }
}