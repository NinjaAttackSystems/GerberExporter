using GerberExporter.Gerber.GerberLineStrategy;

namespace GerberExporter.GerberModels;

public class Aperture : IGerberParsedLine
{
    public GerberParsedLineType Type { get; set; }
    public int Index { get; set; }
}