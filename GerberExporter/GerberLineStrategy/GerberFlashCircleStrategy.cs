using GerberExporter.GerberModels;

namespace GerberExporter.GerberLineStrategy;

public class GerberFlashCircleStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        if (!line.Contains("D03"))
            return false;

        return true;
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        var subLine = line.Substring(0, line.Length - 4);
        var splitLine = subLine.Split("Y-");
        var xStr = splitLine[0].Replace("X", "");
        var yStr = splitLine[1];

        var x_parsed = double.TryParse(xStr, out var x);
        var y_parsed = double.TryParse(yStr, out var y);

        if (!x_parsed || !y_parsed)
        {
            throw new Exception($"Unable to parse Vertex for {line}");
        }

        if (state.CurrentAperture == null) return null;

        var result = new FlashCircleLine(x, y, state.CurrentAperture as CircleAperture);

        return result;
    }

}