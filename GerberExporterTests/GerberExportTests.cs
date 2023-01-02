using GerberExporter.STL;
using GerberExporter.SVG;

namespace GerberExporterTests
{
    public class SvgExporterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Can_export_svg()
        {
            var gerberExporter = new GerberExporter.Gerber.GerberExporter();
            var document = await gerberExporter.Export("mobo.GTS");
            var generator = new SvgGenerator();
            generator.Create("mobo.svg", document);
        }
    }

    public class StlExportTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Can_export_stl()
        {
            var gerberExporter = new GerberExporter.Gerber.GerberExporter();
            var document = await gerberExporter.Export("mobo.GTS");
            var generator = new StlGenerator();
            generator.Create($@"C:\Users\jbeam\Documents\StlExport\mobo.stl", document);
        }
    }

    public class GerberExportTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Can_parse_aperture_line()
        {
            var gerberExporter = new GerberExporter.Gerber.GerberExporter();
            var gerber = await gerberExporter.Export("Gerber_TopSolderMaskLayer.GTS");
            Assert.IsTrue(gerber.Shapes.Count > 0);
        }
    }
}