using System.Text.RegularExpressions;
using GerberExporter.BrokerModels;
using GerberExporter.Common;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberFlashRectangleStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("D03");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        if (state.CurrentAperture?.Type != GerberParsedLineType.RectangleAperture)
            return null;

        var rectangleAperture = state.CurrentAperture as RectangleAperture;
        if (rectangleAperture == null)
            return null;

        // Example: X1234Y5678D03*
        var regex = new Regex(@"X(-?\d+)Y(-?\d+)D03\*");
        var match = regex.Match(line);

        if (match.Success)
        {
            var x = double.Parse(match.Groups[1].Value) / 10000.0; // Convert to mm
            var y = double.Parse(match.Groups[2].Value) / 10000.0; // Convert to mm

            return new FlashRectangleLine
            {
                Center = new Vertex((float)x, (float)y, 0, false),
                Width = rectangleAperture.Width,
                Height = rectangleAperture.Height
            };
        }

        return null;
    }
}