using System;
using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using UltimateTerrains;

public class TunnelNetwork3DPrimitive : Primitive3DNode
{
    private const double HeightMargin = 32;

    private readonly double frequencyAltitude;
    private readonly double scaleAltitude;
    private readonly double baseAltitude;
    private readonly double tunnelsHeight;
    private readonly SimplexPerlin simplexAltitude;
    private readonly RidgedMultiFractal ridgedMultiFractal;
    private readonly double threshold;
    private readonly SimplexPerlin simplexPerturbation;
    private readonly double frequencyPerturbation;
    private readonly double scalePerturbation;
    private readonly double frequencyMicroPerturbation;
    private readonly double scaleMicroPerturbation;

    public TunnelNetwork3DPrimitive(double frequency, int seed,
                                    double frequencyAltitude, double scaleAltitude, double baseAltitude,
                                    double tunnelsHeight, double threshold,
                                    double frequencyPerturbation, double scalePerturbation,
                                    double frequencyMicroPerturbation, double scaleMicroPerturbation)
    {
        this.frequencyAltitude = frequencyAltitude;
        this.scaleAltitude = scaleAltitude;
        this.baseAltitude = baseAltitude;
        this.tunnelsHeight = tunnelsHeight;
        this.threshold = threshold;
        this.frequencyPerturbation = frequencyPerturbation;
        this.scalePerturbation = scalePerturbation;
        this.frequencyMicroPerturbation = frequencyMicroPerturbation;
        this.scaleMicroPerturbation = scaleMicroPerturbation;
        simplexAltitude = new SimplexPerlin(seed, NoiseQuality.Fast);
        ridgedMultiFractal = new RidgedMultiFractal
        {
            Frequency = frequency,
            OctaveCount = 1,
            Primitive2D = new SimplexPerlin(seed, NoiseQuality.Fast)
        };
        simplexPerturbation = new SimplexPerlin(seed, NoiseQuality.Fast);
    }

    public override double Execute(double x, double y, double z, CallableNode flow)
    {
        var altitude = baseAltitude; // + simplexAltitude.GetValue(x * frequencyAltitude, z * frequencyAltitude) * scaleAltitude;

        if (y < altitude - tunnelsHeight - HeightMargin || y > altitude + tunnelsHeight + HeightMargin) {
            return -HeightMargin;
        }

        var network = Math.Min(ridgedMultiFractal.GetValue(x, z), 1.0 - (1.0 - threshold) * 0.333333);
        var tunnel = network - threshold;
        var sign = tunnel < 0 ? -1.0 : 1.0;
        tunnel = sign * Math.Sqrt(Math.Abs(tunnel));

        return tunnelsHeight * tunnel - Math.Abs(y - altitude)
               + scalePerturbation * simplexPerturbation.GetValue(x * frequencyPerturbation, y * frequencyPerturbation, z * frequencyPerturbation)
               + scaleMicroPerturbation * simplexPerturbation.GetValue(x * frequencyMicroPerturbation, y * frequencyMicroPerturbation, z * frequencyMicroPerturbation);
    }
}