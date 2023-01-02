namespace GerberExporter.Common;

public class Vertex
{
    public Vertex(double x, double y, double z, bool fromGbr = true)
    {
        if (fromGbr)
        {
            X = x / 1000000;
            Y = y / 1000000;
            Z = z / 1000000;
        }
        else
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}