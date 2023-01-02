namespace GerberExporter.Gerber.GerberLineStrategy;

public class DrawRectangleLine : IGerberParsedLine
{
    public GerberParsedLineType Type { get; }

    public DrawRectangleLine()
    {
        Type = GerberParsedLineType.DrawRectangle;
    }
}