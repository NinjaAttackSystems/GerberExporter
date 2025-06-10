namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberSelectApertureStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        // D10* to D999* are aperture selections
        // D01, D02, D03 are draw/move/flash commands
        return line.StartsWith("D") && line.EndsWith("*") && 
               !line.StartsWith("D01") && !line.StartsWith("D02") && !line.StartsWith("D03");
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