using GerberExporter.Gerber;
using GerberExporter.GerberModels;
using GerberExporter.Gerber.GerberLineStrategy;
using GerberExporter.STL;
using GerberExporter.Common.Models;
using GerberExporter.BrokerModels;

// Debug: Parse the file manually first to check for issues
var testFilePath = Path.Combine("..", "GerberExporterTests", "mobo.GTS");
var state = new ExporterState();
var lineParser = new GerberLineStrategyContext(state);
var gerberLines = await File.ReadAllLinesAsync(testFilePath);

Console.WriteLine("Parsing Gerber file...");

// Parse all lines
int flashCount = 0;
int selectCount = 0;
int apertureDefCount = 0;

foreach (var line in gerberLines)
{
    var parsedLine = lineParser.ParseLine(line);
    
    if (line.Contains("%ADD"))
        apertureDefCount++;
    if (line.Contains("D03"))
        flashCount++;
    if (line.Contains("D") && line.EndsWith("*") && !line.Contains("D03") && !line.Contains("%"))
        selectCount++;
}

Console.WriteLine($"Found {apertureDefCount} aperture definitions");
Console.WriteLine($"Found {flashCount} flash commands");
Console.WriteLine($"Found {selectCount} select commands");
Console.WriteLine($"Total apertures parsed: {state.Apertures.Count}");

// Run the full GerberExporter
var outputFilePath = Path.Combine(".", "mobo_output.stl");

var gerberExporter = new GerberExporter.Gerber.GerberExporter();
var document = await gerberExporter.Export(testFilePath);

Console.WriteLine($"\nExported {document.Shapes.Count} shapes from Gerber file");

if (document.Shapes.Count > 0)
{
    // Generate STL
    var stlGenerator = new StlGenerator();
    stlGenerator.Create(outputFilePath, document);

    Console.WriteLine($"STL file generated: {outputFilePath}");

    // Analyze the shapes
    var shapesByType = document.Shapes.GroupBy(s => s.Type)
        .Select(g => new { Type = g.Key, Count = g.Count() })
        .OrderBy(x => x.Type.ToString());

    Console.WriteLine("\nShapes by type:");
    foreach (var group in shapesByType)
    {
        Console.WriteLine($"  {group.Type}: {group.Count}");
    }

    // Check for any square shapes that might actually be circles
    var allRectangles = document.Shapes.Where(s => s.Type == ShapeType.Rectangle).ToList();
    
    Console.WriteLine($"\nTotal rectangles (including rounded): {allRectangles.Count}");
    
    var regularRectangles = allRectangles.OfType<Rectangle>().ToList();
    var roundedRectangles = allRectangles.OfType<RoundedRectangle>().ToList();
    
    Console.WriteLine($"Regular rectangles: {regularRectangles.Count}");
    Console.WriteLine($"Rounded rectangles: {roundedRectangles.Count}");
    
    var squares = regularRectangles.Where(r => Math.Abs(r.Width - r.Height) < 0.001).ToList();
    Console.WriteLine($"\nSquare rectangles: {squares.Count}");
    foreach (var square in squares.Take(5)) // Show first 5
    {
        Console.WriteLine($"  Size: {square.Width:F3}x{square.Height:F3}mm at ({square.Center.X:F3}, {square.Center.Y:F3})");
    }
}