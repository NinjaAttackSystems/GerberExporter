using GerberExporter.CommonModels;
using GerberExporter.GerberLineStrategy;

namespace GerberExporter.BrokerModels;

public class Circle : IShape
{
    public Circle(FlashCircleLine line)
    {
        Center = line.Center;
        Type = ShapeType.Circle;
        Diameter = line.Diameter;
    }

    public double Diameter { get; set; }
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }
}