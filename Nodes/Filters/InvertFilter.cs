using System;
using UltimateTerrains;

public class InvertFilter : FilterNode
{
    public InvertFilter(CallableNode input, CallableNode maskInput, double intensity) : base(input, maskInput, intensity)
    {
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return -inputValue;
    }
}