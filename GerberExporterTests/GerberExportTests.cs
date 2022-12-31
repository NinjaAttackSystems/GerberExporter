namespace GerberExporterTests
{
    public class GerberExportTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Can_parse_aperture_line()
        {
            var gerberExporter = new GerberExporter.GerberExporter();
            var gerber = await gerberExporter.Export("Gerber_TopSolderMaskLayer.GTS");
            Assert.IsTrue(gerber.Shapes.Count > 0);
        }
    }
}