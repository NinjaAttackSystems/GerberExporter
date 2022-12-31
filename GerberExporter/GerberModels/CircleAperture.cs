using GerberExporter.GerberLineStrategy;

namespace GerberExporter.GerberModels;

public class CircleAperture : Aperture
{
    public CircleAperture()
    {
        Type = GerberParsedLineType.CircleAperture;
    }

    public double Diameter { get; set; }
}