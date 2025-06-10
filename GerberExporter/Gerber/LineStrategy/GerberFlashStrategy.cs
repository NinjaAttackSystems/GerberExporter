using System.Text.RegularExpressions;
using GerberExporter.BrokerModels;
using GerberExporter.Common;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberFlashStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("D03");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        if (state.CurrentAperture == null)
            return null;

        // Parse coordinates - handles both positive and negative values
        var regex = new Regex(@"X(-?\d+)Y(-?\d+)D03\*");
        var match = regex.Match(line);

        if (!match.Success)
        {
            // Try alternative format
            var subLine = line.Substring(0, line.Length - 4);
            var splitLine = subLine.Split("Y");
            if (splitLine.Length == 2)
            {
                var xStr = splitLine[0].Replace("X", "");
                var yStr = splitLine[1];

                if (double.TryParse(xStr, out var xAlt) && double.TryParse(yStr, out var yAlt))
                {
                    return CreateFlashLine(xAlt / 1000000.0, yAlt / 1000000.0, state.CurrentAperture);
                }
            }
            return null;
        }

        var x = double.Parse(match.Groups[1].Value) / 1000000.0; // Convert to mm (6 decimal places)
        var y = double.Parse(match.Groups[2].Value) / 1000000.0; // Convert to mm (6 decimal places)

        return CreateFlashLine(x, y, state.CurrentAperture);
    }

    private IGerberParsedLine CreateFlashLine(double x, double y, Aperture aperture)
    {
        var center = new Vertex((float)x, (float)y, 0, false);

        switch (aperture.Type)
        {
            case GerberParsedLineType.CircleAperture:
                var circleAperture = aperture as CircleAperture;
                return new FlashCircleLine
                {
                    Center = center,
                    Diameter = circleAperture?.Diameter ?? 0
                };

            case GerberParsedLineType.RectangleAperture:
                if (aperture is RoundedRectangleAperture roundedRectAperture)
                {
                    return new FlashRoundedRectangleLine
                    {
                        Center = center,
                        Width = roundedRectAperture.Width,
                        Height = roundedRectAperture.Height,
                        CornerRadius = roundedRectAperture.CornerRadius
                    };
                }
                else
                {
                    var rectAperture = aperture as RectangleAperture;
                    return new FlashRectangleLine
                    {
                        Center = center,
                        Width = rectAperture?.Width ?? 0,
                        Height = rectAperture?.Height ?? 0
                    };
                }

            default:
                return null;
        }
    }
}