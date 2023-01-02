using GerberExporter.Common;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class FlashCircleLine : IGerberParsedLine
{
    public GerberParsedLineType Type { get; }

    public FlashCircleLine(double x, double y, CircleAperture aperture)
    {
        Type = GerberParsedLineType.FlashCircle;
        Center = new Vertex(x, y, 0);
        Diameter = aperture.Diameter;
    }

    public Vertex Center { get; set; }
    public double Diameter { get; set; }

}