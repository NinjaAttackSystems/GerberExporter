using GerberExporter.BrokerModels;
using GerberExporter.Common.Models;
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
            var outputPath = Path.Combine(Path.GetTempPath(), "mobo.stl");
            generator.Create(outputPath, document);
            
            // Clean up the generated file
            if (File.Exists(outputPath))
                File.Delete(outputPath);
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

        [Test]
        public async Task Can_parse_rectangle_shapes()
        {
            // Create a test Gerber file with rectangle apertures
            var testGerberContent = @"G04 Test Gerber with rectangles*
%FSLAX45Y45*%
%MOMM*%
%ADD10R,1.5X2.0*%
%ADD11R,0.5X0.5*%
D10*
X10000Y20000D03*
X30000Y20000D03*
D11*
X50000Y50000D03*
M02*";
            
            var testFileName = "test_rectangles.gbr";
            await File.WriteAllTextAsync(testFileName, testGerberContent);
            
            try
            {
                var gerberExporter = new GerberExporter.Gerber.GerberExporter();
                var document = await gerberExporter.Export(testFileName);
                
                Assert.AreEqual(3, document.Shapes.Count);
                Assert.IsTrue(document.Shapes.All(s => s.Type == ShapeType.Rectangle));
                
                var rectangles = document.Shapes.Cast<Rectangle>().ToList();
                
                // First two rectangles should be 1.5x2.0
                Assert.AreEqual(1.5, rectangles[0].Width);
                Assert.AreEqual(2.0, rectangles[0].Height);
                Assert.AreEqual(1.5, rectangles[1].Width);
                Assert.AreEqual(2.0, rectangles[1].Height);
                
                // Third rectangle should be 0.5x0.5
                Assert.AreEqual(0.5, rectangles[2].Width);
                Assert.AreEqual(0.5, rectangles[2].Height);
            }
            finally
            {
                if (File.Exists(testFileName))
                    File.Delete(testFileName);
            }
        }
    }
}