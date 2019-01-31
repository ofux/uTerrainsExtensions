using UltimateTerrains;
using UnityEngine;

public class CurveFilter : FilterNode
{
    private readonly AnimationCurve curve;
    private readonly double inMin;
    private readonly double inMax;
    private readonly double outMin;
    private readonly double outMax;

    public CurveFilter(CallableNode input, CallableNode maskInput, double intensity, AnimationCurve curve, double inMin, double inMax, double outMin, double outMax) : base(input, maskInput, intensity)
    {
        // Create a copy of the animation curve to avoid any concurrency issue
        this.curve = new AnimationCurve(curve.keys);
        
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
        var vC = curve.Evaluate((float) v01);
        return UMath.Lerp(outMin, outMax, vC);
    }
}