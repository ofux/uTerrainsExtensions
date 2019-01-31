using System;
using Newtonsoft.Json;
using UltimateTerrains;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Smooth : IOperation
{
    [JsonProperty("p")] protected Vector3d position;
    [JsonProperty("s")] protected double step;
    [JsonProperty("r")] protected double radius;
    [JsonProperty("vox")] protected double[] voxelValues;

    protected Vector3i from;
    protected Vector3i to;
    protected UnitConverter converter;
    protected double radiusSquared;
    protected double radiusSquaredInverse;
    protected double stepInverse;
    protected int size;

    // Empty constructor needed by serialization
    public Smooth()
    {
    }

    // 
    public Smooth(UltimateTerrain uTerrain, Vector3d position, double radius, double step)
    {
        this.position = position;
        this.radius = radius;
        this.step = step;
        Init(uTerrain);
        PickupVoxels(uTerrain);
    }

    public static Smooth CreateFromUnityWorld(UltimateTerrain uTerrain, Vector3 position, float radius, double step)
    {
        var conv = uTerrain.Converter;
        return new Smooth(uTerrain, conv.UnityToVoxelPosition(position), conv.UnityToVoxelDisance(radius), step);
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
        var vpos = new Vector3d(x, y, z);
        var distanceSquared = position.DistanceSquared(vpos);
        if (distanceSquared < radiusSquared) {
            var px = (int) ((x - (position.x - radius)) * stepInverse);
            var py = (int) ((y - (position.y - radius)) * stepInverse);
            var pz = (int) ((z - (position.z - radius)) * stepInverse);
            if (px >= 0 && px < size - 1 && py >= 0 && py < size - 1 && pz >= 0 && pz < size - 1) {
                var pointInCell = vpos - new Vector3d(px * step + (position.x - radius),
                                                      py * step + (position.y - radius),
                                                      pz * step + (position.z - radius));
                pointInCell *= stepInverse;

                var interpolated = UMath.TrilinearInterpolate(
                    GetVoxelValue(voxelValues, px, py, pz),
                    GetVoxelValue(voxelValues, px, py, pz + 1),
                    GetVoxelValue(voxelValues, px, py + 1, pz),
                    GetVoxelValue(voxelValues, px, py + 1, pz + 1),
                    GetVoxelValue(voxelValues, px + 1, py, pz),
                    GetVoxelValue(voxelValues, px + 1, py, pz + 1),
                    GetVoxelValue(voxelValues, px + 1, py + 1, pz),
                    GetVoxelValue(voxelValues, px + 1, py + 1, pz + 1),
                    pointInCell
                );

                voxel.Value = UMath.Lerp(interpolated, voxel.Value, distanceSquared * radiusSquaredInverse);
            }
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
        return string.Format("[Smooth] at " + position + " of radius " + radius);
    }

    // Protected methods /////////////////

    private void Init(UltimateTerrain uTerrain)
    {
        converter = uTerrain.Converter;
        var halfSize = new Vector3d(radius, radius, radius).Rounded;
        halfSize.x += 1;
        halfSize.y += 1;
        halfSize.z += 1;
        var voxelPos = position.Rounded;
        from = voxelPos - halfSize;
        to = voxelPos + halfSize;
        radiusSquared = radius * radius;
        radiusSquaredInverse = 1.0 / radiusSquared;
        stepInverse = 1.0 / step;
        size = (int) (radius * 2.0 / step) + 1;
    }

    private void PickupVoxels(UltimateTerrain uTerrain)
    {
        var values = new double[size * size * size];

        for (var x = 0; x < size; ++x) {
            var px = x * step + position.x - radius;
            for (var y = 0; y < size; ++y) {
                var py = y * step + position.y - radius;
                for (var z = 0; z < size; ++z) {
                    var pz = z * step + position.z - radius;
                    values[x + y * size + z * size * size] = uTerrain.GetVoxelAt(px, py, pz).Value;
                }
            }
        }

        var averagedValues = new double[size * size * size];
        for (var x = 0; x < size; ++x) {
            for (var y = 0; y < size; ++y) {
                for (var z = 0; z < size; ++z) {
                    if (x >= 1 && x < size - 1 && y >= 1 && y < size - 1 && z >= 1 && z < size - 1) {
                        var average = 0.0;
                        for (var kx = -1; kx <= 1; ++kx) {
                            for (var ky = -1; ky <= 1; ++ky) {
                                for (var kz = -1; kz <= 1; ++kz) {
                                    average += GetVoxelValue(values, x + kx, y + ky, z + kz);
                                }
                            }
                        }

                        average /= 27.0;
                        averagedValues[x + y * size + z * size * size] = average;
                    } else {
                        averagedValues[x + y * size + z * size * size] = GetVoxelValue(values, x, y, z);
                    }
                }
            }
        }

        voxelValues = averagedValues;
    }


    private double GetVoxelValue(double[] values, int x, int y, int z)
    {
        return values[x + y * size + z * size * size];
    }
}