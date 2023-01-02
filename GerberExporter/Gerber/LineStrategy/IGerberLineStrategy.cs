namespace GerberExporter.Gerber.GerberLineStrategy;

public interface IGerberLineStrategy
{
    public bool CanHandleLine(string line);
    public IGerberParsedLine Handle(string line, ExporterState state);
}