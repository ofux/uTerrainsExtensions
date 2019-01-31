using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using UltimateTerrains;

public sealed class HybridMultiFractal3DPrimitive : Primitive3DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly HybridMultiFractal noise;

    public HybridMultiFractal3DPrimitive(double frequency, double scale, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        noise = new HybridMultiFractal {Primitive3D = new SimplexPerlin(seed, quality)};
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return noise.GetValue(x * frequency, y * frequency, z * frequency) * scale;
    }
}