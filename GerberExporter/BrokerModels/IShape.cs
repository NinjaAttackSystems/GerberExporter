using GerberExporter.CommonModels;

namespace GerberExporter.BrokerModels;

public interface IShape
{
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }
}