using UltimateTerrains;

public class MulCombiner : CombinerNode
{
    public MulCombiner(CallableNode right, CallableNode left) : base(right, left)
    {
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return flow.Call(Right,x, y, z) * flow.Call(Left, x, y, z);
    }
}