using GerberExporter.Common;

namespace GerberExporter.BrokerModels;

public class Facet
{
    public Vertex Normal { get; set; }
    public IList<Vertex> Vertices { get; set; }
}