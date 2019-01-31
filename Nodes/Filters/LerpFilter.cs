using UltimateTerrains;

public class LerpFilter : FilterNode
{
    private readonly double inMin;
    private readonly double inMax;
    private readonly double outMin;
    private readonly double outMax;

    public LerpFilter(CallableNode input, CallableNode maskInput, double intensity, double inMin, double inMax, double outMin, double outMax) : base(input, maskInput, intensity)
    {
        this.inMin = inMin;
        this.inMax = inMax;
        this.outMin = outMin;
        this.outMax = outMax;
        
        if (inMax < inMin + 0.001)
            this.inMax = inMin + 0.001;
        if (outMax < outMin + 0.001)
            this.outMax = outMin + 0.001;
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        var v01 = UMath.InverseLerp(inMin, inMax, inputValue);
        return UMath.Lerp(outMin, outMax, v01);
    }
}