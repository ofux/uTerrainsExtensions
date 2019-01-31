using UltimateTerrains;

public class ClampFilter : FilterNode
{
    private readonly double min;
    private readonly double max;

    public ClampFilter(CallableNode input, CallableNode maskInput, double intensity, double min, double max) : base(input, maskInput, intensity)
    {
        this.min = min;
        this.max = max;
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return UMath.Clamp(min, max, inputValue);
    }
}