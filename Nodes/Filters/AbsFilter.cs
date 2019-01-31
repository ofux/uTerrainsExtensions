using System;
using UltimateTerrains;

public class AbsFilter : FilterNode
{
    public AbsFilter(CallableNode input, CallableNode maskInput, double intensity) : base(input, maskInput, intensity)
    {
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return Math.Abs(inputValue);
    }
}