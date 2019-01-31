using UltimateTerrains;
using UnityEngine;

public class Heightmap2DPrimitive : Primitive2DNode
{
    private readonly int fromX;
    private readonly int fromZ;
    private readonly int width;
    private readonly int height;
    private readonly double voxel2PixelPosX;
    private readonly double voxel2PixelPosZ;
    private readonly double[] heights;

    public Heightmap2DPrimitive(int fromX, int fromZ, int toX, int toZ, Texture2D heightmap, double heightScale)
    {
        this.fromX = fromX;
        this.fromZ = fromZ;
        voxel2PixelPosX = heightmap.width / (double) (toX - fromX);
        voxel2PixelPosZ = heightmap.height / (double) (toZ - fromZ);
        width = heightmap.width;
        height = heightmap.height;

        heights = new double[width * height];

        for (var x = 0; x < width; ++x) {
            for (var y = 0; y < height; ++y) {
                heights[x + width * y] = heightmap.GetPixel(x, y).r * heightScale;
            }
        }
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        var ix = (x - fromX) * voxel2PixelPosX;
        var iz = (z - fromZ) * voxel2PixelPosZ;
        var hx = Mathf.Clamp((int) ix, 0, width - 1);
        var hz = Mathf.Clamp((int) iz, 0, height - 1);
        
        if (hx < 0 || hz < 0 || hx >= width - 1 || hz >= height - 1) {
            return 1.0;
        }

        var h00 = heights[hx + width * hz];
        var h01 = heights[hx + width * (hz + 1)];
        var h10 = heights[(hx + 1) + width * hz];
        var h11 = heights[(hx + 1) + width * (hz + 1)];
        return UMath.BilinearInterpolate(h00, h01, h10, h11, ix - hx, iz - hz);
    }
}