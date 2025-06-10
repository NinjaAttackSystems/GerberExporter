namespace GerberExporter.GerberModels;

public class RoundedRectangleAperture : Aperture
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double CornerRadius { get; set; }
}