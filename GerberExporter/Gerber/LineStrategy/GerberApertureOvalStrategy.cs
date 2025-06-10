using System;
using System.Text.RegularExpressions;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberApertureOvalStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("%ADD") && line.Contains("O,");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Example: %ADD38O,2.000000X1.700000*%
        var regex = new Regex(@"%ADD(\d+)O,([0-9.]+)X([0-9.]+)\*%");
        var match = regex.Match(line);

        if (match.Success)
        {
            var apertureNumber = int.Parse(match.Groups[1].Value);
            var width = double.Parse(match.Groups[2].Value);
            var height = double.Parse(match.Groups[3].Value);

            // Check if it's actually a circle (equal dimensions)
            if (Math.Abs(width - height) < 0.001)
            {
                var aperture = new CircleAperture
                {
                    Index = apertureNumber,
                    Type = GerberParsedLineType.CircleAperture,
                    Diameter = width
                };
                state.Apertures.Add(aperture);
            }
            else
            {
                // Treat as rectangle for true ovals
                var aperture = new RectangleAperture
                {
                    Index = apertureNumber,
                    Type = GerberParsedLineType.RectangleAperture,
                    Width = width,
                    Height = height
                };
                state.Apertures.Add(aperture);
            }
        }

        return null;
    }
}