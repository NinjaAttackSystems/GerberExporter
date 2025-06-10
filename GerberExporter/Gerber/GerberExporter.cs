using GerberExporter.BrokerModels;
using GerberExporter.Common.Models;
using GerberExporter.Gerber.GerberLineStrategy;

namespace GerberExporter.Gerber
{
    public class GerberExporter
    {
        private BrokerDocument BrokerDocument { get; set; }

        public async Task<BrokerDocument> Export(string file)
        {
            var _state = new ExporterState();
            var lineParser = new GerberLineStrategyContext(_state);

            BrokerDocument = new BrokerDocument();

            var gerberFile = await LoadGerberFile(file);

            foreach (var line in gerberFile)
            {
                var parsedLine = lineParser.ParseLine(line);
                ProcessLine(parsedLine);
            }

            return BrokerDocument;
        }

        private async Task<string[]> LoadGerberFile(string file)
        {
            var gerberFile = await File.ReadAllLinesAsync(file);

            return gerberFile;
        }

        private void ProcessLine(IGerberParsedLine line)
        {
            if (line == null) return;

            if (line.Type == GerberParsedLineType.FlashCircle)
            {
                BrokerDocument.Shapes.Add(new Circle(line as FlashCircleLine));
            }
            else if (line.Type == GerberParsedLineType.FlashRectangle)
            {
                if (line is FlashRoundedRectangleLine roundedRectLine)
                {
                    BrokerDocument.Shapes.Add(new RoundedRectangle(roundedRectLine));
                }
                else
                {
                    BrokerDocument.Shapes.Add(new Rectangle(line as FlashRectangleLine));
                }
            }

        }
    }
}