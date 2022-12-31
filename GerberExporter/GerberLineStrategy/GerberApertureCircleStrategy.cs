using GerberExporter.GerberModels;

namespace GerberExporter.GerberLineStrategy;

public class GerberApertureCircleStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("%ADD") && !line.Contains("MACRO") && line.Contains("C");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        var splitString = line.Split(",");

        var indexStr = splitString[0].Replace("%ADD", "").Replace("C", "");

        var indexParsed = int.TryParse(indexStr, out var index);

        if (!indexParsed)
        {
            throw new Exception($"Unable to parse index of {line}");
        }

        var diameterStr = splitString[1].Replace("*%", "");

        var parsed = double.TryParse(diameterStr, out var diameter);

        if (!parsed)
        {
            throw new Exception($"Unable to parse diameter of {line}");
        }

        var result =  new CircleAperture(){Diameter = diameter, Index = index};
        
        state.Apertures.Add(result);
        
        return null;
    }

}