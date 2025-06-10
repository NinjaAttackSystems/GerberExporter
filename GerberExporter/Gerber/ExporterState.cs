using GerberExporter.GerberModels;

namespace GerberExporter.Gerber;

public class ExporterState
{
    public ExporterState()
    {
        Apertures = new List<Aperture>();
        Macros = new Dictionary<string, List<string>>();
    }
    public Aperture CurrentAperture { get; set; }
    public IList<Aperture> Apertures { get; set; }
    public Dictionary<string, List<string>> Macros { get; set; }
    public string CurrentMacroName { get; set; }
    public List<string> CurrentMacroDefinition { get; set; }
}