using GerberExporter.GerberModels;

namespace GerberExporter;

public class ExporterState
{
    public ExporterState()
    {
        Apertures = new List<Aperture>();
    }
    public Aperture CurrentAperture { get; set; }
    public IList<Aperture> Apertures { get; set; }
}