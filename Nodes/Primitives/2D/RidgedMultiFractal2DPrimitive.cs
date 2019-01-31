using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using UltimateTerrains;

public sealed class RidgedMultiFractal2DPrimitive : Primitive2DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly RidgedMultiFractal noise;
    private readonly double offsetX;
    private readonly double offsetZ;

    public RidgedMultiFractal2DPrimitive(double frequency, double scale, double offsetX, double offsetZ, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        this.offsetX = offsetX;
        this.offsetZ = offsetZ;
        noise = new RidgedMultiFractal {Primitive2D = new SimplexPerlin(seed, quality)};
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return noise.GetValue(x * frequency + offsetX, z * frequency + offsetZ) * scale;
    }
}