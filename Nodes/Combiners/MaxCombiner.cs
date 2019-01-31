using System;
using UltimateTerrains;

public class MaxCombiner : CombinerNode
{
    public MaxCombiner(CallableNode right, CallableNode left) : base(right, left)
    {
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return Math.Max(flow.Call(Right,x, y, z), flow.Call(Left, x, y, z));
    }
}