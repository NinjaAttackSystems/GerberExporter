namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberLineDrawStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return false;
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        throw new NotImplementedException();
    }
}