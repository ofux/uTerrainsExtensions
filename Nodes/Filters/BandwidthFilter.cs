using UltimateTerrains;

public class BandwidthFilter : FilterNode
{
    private readonly double min;
    private readonly double max;

    public BandwidthFilter(CallableNode input, CallableNode maskInput, double intensity, double min, double max) : base(input, maskInput, intensity)
    {
        this.min = min;
        this.max = max;
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return min < inputValue && inputValue < max ? 1 : 0;
    }
}