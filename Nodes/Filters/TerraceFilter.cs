using System;
using UltimateTerrains;

public class TerraceFilter : FilterNode
{
    private readonly double width;
    private readonly CallableNode widthInput;

    public TerraceFilter(CallableNode input, CallableNode maskInput, double intensity, CallableNode widthInput, double width) : base(input, maskInput, intensity)
    {
        this.width = width;
        this.widthInput = widthInput;
    }

    protected override double ExecuteFilter(double x, double y, double z, CallableNode flow, double inputValue)
    {
        return Terrace(inputValue, width * flow.Call(widthInput, x, y, z));
    }

    private double Terrace(double h, double width)
    {
        var k = Math.Floor(h / width);
        var f = (h - k * width) / width;
        var s = Math.Min(2.0 * f, 1.0);
        return (k + s) * width;
    }

    private double Terrace2(double h, double terraceHeightVariation, double width)
    {
        var h1 = 1.0;
        var h2 = 2.0;
        var damp = .01;

        var hm = terraceHeightVariation;
        double th;
        if (h1 + hm <= h && h <= h2 + hm) {
            th = h * damp;
        } else if (h2 + hm < h) {
            th = h - (h2 - h1) * damp;
        } else {
            th = h;
        }

        return th;
    }
}