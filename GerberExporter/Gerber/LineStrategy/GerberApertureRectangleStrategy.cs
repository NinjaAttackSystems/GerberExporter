using System.Text.RegularExpressions;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberApertureRectangleStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("%ADD") && line.Contains("R,");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Example: %ADD10R,0.650X0.650*%
        var regex = new Regex(@"%ADD(\d+)R,([0-9.]+)X([0-9.]+)\*%");
        var match = regex.Match(line);

        if (match.Success)
        {
            var apertureNumber = int.Parse(match.Groups[1].Value);
            var width = double.Parse(match.Groups[2].Value);
            var height = double.Parse(match.Groups[3].Value);

            var aperture = new RectangleAperture
            {
                Index = apertureNumber,
                Type = GerberParsedLineType.RectangleAperture,
                Width = width,
                Height = height
            };

            state.Apertures.Add(aperture);
        }

        return null;
    }
}