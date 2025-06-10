using System;
using System.Linq;
using System.Text.RegularExpressions;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy;

public class GerberApertureMacroInstanceStrategy : IGerberLineStrategy
{
    public bool CanHandleLine(string line)
    {
        // Matches aperture definitions that use macros
        // Example: %ADD10MACRO1,0.1016X-1X1X1X1*%
        return line.Contains("%ADD") && !line.Contains("C,") && !line.Contains("R,") && 
               !line.Contains("O,") && !line.Contains("RoundRect,") && line.Contains("*%");
    }

    public IGerberParsedLine Handle(string line, ExporterState state)
    {
        // Parse macro-based aperture
        // Example: %ADD10MACRO1,0.1016X-1X1X1X1*%
        var match = Regex.Match(line, @"%ADD(\d+)([A-Za-z0-9]+),(.+)\*%");
        
        if (match.Success)
        {
            var apertureNumber = int.Parse(match.Groups[1].Value);
            var macroName = match.Groups[2].Value;
            var parameters = match.Groups[3].Value.Split('X').Select(p => double.Parse(p)).ToArray();
            
            // Check if this is the RoundRect macro
            if (macroName == "RoundRect" || macroName == "MACRO1")
            {
                // For RoundRect macro, parameters are usually:
                // [0] = corner radius
                // [1-8] = corner coordinates (4 x,y pairs)
                
                if (parameters.Length >= 9)
                {
                    var cornerRadius = Math.Abs(parameters[0]);
                    
                    // Calculate bounds from corner coordinates
                    double minX = double.MaxValue, maxX = double.MinValue;
                    double minY = double.MaxValue, maxY = double.MinValue;
                    
                    for (int i = 1; i < parameters.Length; i += 2)
                    {
                        if (i + 1 < parameters.Length)
                        {
                            minX = Math.Min(minX, parameters[i]);
                            maxX = Math.Max(maxX, parameters[i]);
                            minY = Math.Min(minY, parameters[i + 1]);
                            maxY = Math.Max(maxY, parameters[i + 1]);
                        }
                    }
                    
                    var width = Math.Abs(maxX - minX);
                    var height = Math.Abs(maxY - minY);
                    
                    // Check if it's essentially a circle
                    var isSquare = Math.Abs(width - height) < 0.001;
                    var radiusRatio = cornerRadius / (Math.Min(width, height) / 2);
                    
                    if (isSquare && radiusRatio >= 0.9)
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
        }
        
        return null;
    }
}