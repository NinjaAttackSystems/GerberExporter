using System.Text.RegularExpressions;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberApertureRoundRectStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        return line.Contains("%ADD") && line.Contains("RoundRect,");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Example: %ADD15RoundRect,0.250000X-0.425000X-0.250000X0.425000X-0.250000X0.425000X0.250000X-0.425000X0.250000X0*%
        // The RoundRect has corner radius followed by 8 corner coordinates
        // For simplicity, we'll treat it as a rectangle using the bounding box
        
        var match = Regex.Match(line, @"%ADD(\d+)RoundRect,([0-9.-]+)X(.+)\*%");
        
        if (match.Success)
        {
            var apertureNumber = int.Parse(match.Groups[1].Value);
            var cornerRadius = double.Parse(match.Groups[2].Value);
            var coords = match.Groups[3].Value.Split('X');
            
            // Extract the corner coordinates to find width and height
            // RoundRect format has 8 values after corner radius (4 corner X,Y pairs)
            if (coords.Length >= 8)
            {
                // Find min/max to get dimensions
                double minX = double.MaxValue, maxX = double.MinValue;
                double minY = double.MaxValue, maxY = double.MinValue;
                
                for (int i = 0; i < coords.Length; i += 2)
                {
                    if (i + 1 < coords.Length)
                    {
                        var x = double.Parse(coords[i]);
                        var y = double.Parse(coords[i + 1]);
                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);
                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);
                    }
                }
                
                var width = Math.Abs(maxX - minX);
                var height = Math.Abs(maxY - minY);
                
                // Check if it's essentially a circle
                var isSquare = Math.Abs(width - height) < 0.001;
                var radiusRatio = cornerRadius / (Math.Min(width, height) / 2);
                
                if (isSquare && radiusRatio >= 0.9) // 90% or more means it's essentially a circle
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
                    // Create a RoundedRectangleAperture to preserve corner radius
                    var aperture = new RoundedRectangleAperture
                    {
                        Index = apertureNumber,
                        Type = GerberParsedLineType.RectangleAperture,
                        Width = width,
                        Height = height,
                        CornerRadius = cornerRadius
                    };
                    state.Apertures.Add(aperture);
                }
            }
        }

        return null;
    }
}