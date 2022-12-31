using GerberExporter.GerberModels;

namespace GerberExporter.BrokerModels;

public class BrokerDocument
{
    public BrokerDocument()
    {
        Shapes = new List<IShape>();
    }
    public IList<IShape> Shapes { get; set; }
}