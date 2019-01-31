using System;
using UltimateTerrains;

public class Distance2DPrimitive : Primitive2DNode
{
    private readonly double originX;
    private readonly double originZ;

    public Distance2DPrimitive(double originX, double originZ)
    {
        this.originX = originX;
        this.originZ = originZ;
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        return Math.Sqrt((x - originX) * (x - originX) + (z - originZ) * (z - originZ));
    }
}