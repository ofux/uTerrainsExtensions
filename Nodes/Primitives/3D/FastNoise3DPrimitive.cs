using UltimateTerrains;

public sealed class FastNoise3DPrimitive : Primitive3DNode
{
    private readonly double scale;
    private readonly FastNoise fastNoise;

    public FastNoise3DPrimitive(FastNoise fastNoise, double scale)
    {
        this.scale = scale;
        this.fastNoise = fastNoise;
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return fastNoise.GetNoise(x, y, z) * scale;
    }
}