using System;
using UltimateTerrains;

public class ToSlopeTransformer : TransformerNode
{
    private const double step = 1;

    public override bool Is2D {
        get { return true; }
    }

    public ToSlopeTransformer(CallableNode input) : base(input)
    {
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        var dx = flow.Call(Input, x + step, y, z) - flow.Call(Input, x - step, y, z);
        var dz = flow.Call(Input, x, y, z + step) - flow.Call(Input, x, y, z - step);
        return Math.Abs(dx) + Math.Abs(dz);
    }
}