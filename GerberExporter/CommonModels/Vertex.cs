namespace GerberExporter.CommonModels;

public class Vertex
{
    public Vertex(double x, double y, double z)
    {
        X = x / 100000;
        Y = y / 100000;
        Z = z / 100000;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }
}