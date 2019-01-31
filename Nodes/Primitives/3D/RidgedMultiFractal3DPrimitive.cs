using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using UltimateTerrains;

public sealed class RidgedMultiFractal3DPrimitive : Primitive3DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly RidgedMultiFractal noise;
    private readonly double offsetX;
    private readonly double offsetY;
    private readonly double offsetZ;

    public RidgedMultiFractal3DPrimitive(double frequency, double scale, Vector3d offset, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        this.offsetX = offset.x;
        this.offsetY = offset.y;
        this.offsetZ = offset.z;
        noise = new RidgedMultiFractal {Primitive3D = new SimplexPerlin(seed, quality)};
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return noise.GetValue(x * frequency + offsetX, y * frequency + offsetY, z * frequency + offsetZ) * scale;
    }
}