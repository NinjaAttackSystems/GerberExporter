using GerberExporter.BrokerModels;
using GerberExporter.GerberLineStrategy;
using GerberExporter.GerberModels;

namespace GerberExporter
{
    public class GerberExporter
    {
        private BrokerDocument _brokerDocument { get; set; }
        
        public async Task<BrokerDocument> Export(string file)
        {
            var _state = new ExporterState();
            var lineParser = new GerberLineStrategyContext(_state);

            _brokerDocument = new BrokerDocument();

            var gerberFile =  await LoadGerberFile(file);

            foreach (var line in gerberFile)
            {
                var parsedLine =  lineParser.ParseLine(line);
                ProcessLine(parsedLine);
            }

            return _brokerDocument;
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
                _brokerDocument.Shapes.Add(new Circle(line as FlashCircleLine));
            }

        }
    }
}