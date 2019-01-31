using UltimateTerrains;

public class SubCombiner : CombinerNode
{
    private readonly double in1Weight;
    private readonly double in2Weight;
    
    public SubCombiner(CallableNode right, CallableNode left, double inWeight) : base(right, left)
    {
        this.in1Weight = 1 - inWeight;
        this.in2Weight = inWeight;
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return flow.Call(Right, x, y, z) * in1Weight - flow.Call(Left, x, y, z) * in2Weight;
    }
}