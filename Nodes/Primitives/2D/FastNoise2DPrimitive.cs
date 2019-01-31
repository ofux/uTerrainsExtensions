using UltimateTerrains;

public sealed class FastNoise2DPrimitive : Primitive2DNode
{
    private readonly double scale;
    private readonly FastNoise fastNoise;

    public FastNoise2DPrimitive(FastNoise fastNoise, double scale)
    {
        this.scale = scale;
        this.fastNoise = fastNoise;
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return fastNoise.GetNoise(x, z) * scale;
    }
}