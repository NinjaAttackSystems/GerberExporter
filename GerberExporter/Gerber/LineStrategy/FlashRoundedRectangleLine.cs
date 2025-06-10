using GerberExporter.BrokerModels;
using GerberExporter.Common;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class FlashRoundedRectangleLine : IGerberParsedLine
{
    public GerberParsedLineType Type { get; }
    public Vertex Center { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double CornerRadius { get; set; }

    public FlashRoundedRectangleLine()
    {
        Type = GerberParsedLineType.FlashRectangle; // Reusing the same type for now
    }
}