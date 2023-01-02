using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GerberExporter.BrokerModels;
using GerberExporter.GerberModels;

namespace GerberExporter.Gerber.GerberLineStrategy
{
    public class GerberLineStrategyContext
    {
        private ExporterState State { get; set; }

        private IList<IGerberLineStrategy> Strategies { get; }

        public GerberLineStrategyContext(ExporterState state)
        {
            State = state;
            Strategies = new List<IGerberLineStrategy>()
            {
                new GerberSelectApertureStrategy(),
                new GerberApertureCircleStrategy(),
                new GerberFlashCircleStrategy(),
                new GerberLineDrawStrategy(),
            };
        }

        public IGerberParsedLine ParseLine(string line)
        {
            var parser = Strategies.FirstOrDefault(x => x.CanHandleLine(line));

            var item = parser?.Handle(line, State);

            return item;
        }
    }
}
