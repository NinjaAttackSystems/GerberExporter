using System.Text;
using GerberExporter.BrokerModels;
using QuantumConcepts.Formats.StereoLithography;

namespace GerberExporter.Common.Models;

public interface IShape
{
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }
    public void ToSvg(double transformX, double transformY, StringBuilder sb);
    public void ToStl(double transformX, double transformY, STLDocument sb, int index);

}