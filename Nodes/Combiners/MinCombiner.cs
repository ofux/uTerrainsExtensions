using System;
using UltimateTerrains;

public class MinCombiner : CombinerNode
{
    public MinCombiner(CallableNode right, CallableNode left) : base(right, left)
    {
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return Math.Min(flow.Call(Right,x, y, z), flow.Call(Left, x, y, z));
    }
}