namespace GerberExporter.Gerber.GerberLineStrategy;

public interface IGerberParsedLine
{
    public GerberParsedLineType Type { get; }
}