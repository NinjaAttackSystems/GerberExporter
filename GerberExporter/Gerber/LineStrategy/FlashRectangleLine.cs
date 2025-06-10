using GerberExporter.BrokerModels;
using GerberExporter.Common;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class FlashRectangleLine : IGerberParsedLine
{
    public GerberParsedLineType Type { get; }
    public Vertex Center { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public FlashRectangleLine()
    {
        Type = GerberParsedLineType.FlashRectangle;
    }
}