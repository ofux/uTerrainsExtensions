using UltimateTerrains;

public class ScaleBiasFilter : FilterNode
{
    private readonly double scale;
    private readonly double bias;

    public ScaleBiasFilter(CallableNode input, CallableNode maskInput, double intensity, double scale, double bias) : base(input, maskInput, intensity)
    {
        this.scale = scale;
        this.bias = bias;
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return inputValue * scale + bias;
    }
}