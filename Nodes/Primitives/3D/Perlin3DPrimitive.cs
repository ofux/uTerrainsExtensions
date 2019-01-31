using LibNoise;
using LibNoise.Primitive;
using UltimateTerrains;

public class Perlin3DPrimitive : Primitive3DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly ImprovedPerlin perlin;

    public Perlin3DPrimitive(double frequency, double scale, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        perlin = new ImprovedPerlin(seed, quality);
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return perlin.GetValue(x * frequency, y * frequency, z * frequency) * scale;
    }
}