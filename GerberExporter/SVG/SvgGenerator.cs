using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GerberExporter.BrokerModels;
using GerberExporter.Common.Models;

namespace GerberExporter.SVG
{
    public class SvgGenerator
    {
        public void Create(string fileName, BrokerDocument document)
        {
            var sb = new StringBuilder();
            var maxX = document.Shapes.MaxBy(x => x.Center.X)?.Center.X;
            var maxY = document.Shapes.MaxBy(x => x.Center.Y)?.Center.Y;
            //find minX and minY to bring close to 0,0 
            var minx = document.Shapes.MinBy(x => x.Center.X)?.Center.X;
            var miny = document.Shapes.MinBy(x => x.Center.Y)?.Center.Y;

            var width = maxX - minx +20;
            var height = maxY - miny + 20;


            sb.AppendLine($"<svg width=\"{width}mm\" height=\"{height}mm\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink= \"http://www.w3.org/1999/xlink\">");

            double transformX = 0;
            double transformY = 0;
            
            if (minx != null && miny != null)
            {
                if (minx > 10)
                {
                    transformX = (double)(minx - 10);

                }

                if (minx > 10)
                {
                    transformY = (double)(miny - 10);
                }
            }
            foreach (var shape in document.Shapes)
            {
                shape.ToSvg(transformX, transformY, sb);
            }

            sb.AppendLine("</svg>");

            File.WriteAllText(fileName, sb.ToString());
        }

    }
}
