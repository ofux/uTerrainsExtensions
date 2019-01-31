using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using UltimateTerrains;

public sealed class MultiFractal2DPrimitive : Primitive2DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly MultiFractal noise;

    public MultiFractal2DPrimitive(double frequency, double scale, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        noise = new MultiFractal {Primitive2D = new SimplexPerlin(seed, quality)};
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return noise.GetValue(x * frequency, z * frequency) * scale;
    }
}