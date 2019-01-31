using LibNoise;
using LibNoise.Primitive;
using UltimateTerrains;

public class Perlin2DPrimitive : Primitive2DNode
{
    private readonly double frequency;
    private readonly double scale;
    private readonly ImprovedPerlin perlin;

    public Perlin2DPrimitive(double frequency, double scale, int seed, NoiseQuality quality)
    {
        this.frequency = frequency;
        this.scale = scale;
        perlin = new ImprovedPerlin(seed, quality);
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return perlin.GetValue(x * frequency, z * frequency) * scale;
    }
}