namespace GerberExporter.GerberLineStrategy;

public class GerberSelectApertureStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.StartsWith("D");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        var indexStr = line.Replace("D", "").Replace("*", "");

        var indexParsed = int.TryParse(indexStr, out var index);

        if (!indexParsed)
        {
            throw new Exception($"Unable to parse index of {line}");
        }

        state.CurrentAperture = state.Apertures.FirstOrDefault(x => x.Index == index);

        return null;
    }

}