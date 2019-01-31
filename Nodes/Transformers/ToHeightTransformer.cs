using UltimateTerrains;

public class ToHeightTransformer : TransformerNode
{
    private readonly double scale;

    public ToHeightTransformer(CallableNode input, double scale) : base(input)
    {
        this.scale = scale;
    }

    public override bool Is2D {
        get { return false; }
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return y - flow.Call(Input, x, y, z) * scale;
    }
}