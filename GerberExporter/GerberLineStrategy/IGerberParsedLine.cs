namespace GerberExporter.GerberLineStrategy;

public interface IGerberParsedLine
{
    public GerberParsedLineType Type { get; }
}