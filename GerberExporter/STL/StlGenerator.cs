using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GerberExporter.BrokerModels;
using GerberExporter.Common.Models;
using QuantumConcepts.Formats.StereoLithography;

namespace GerberExporter.STL
{
    public class StlGenerator
    {
        public void Create(string fileName, BrokerDocument document)
        {
            var stl = new STLDocument();
            //find minX and minY to bring close to 0,0 
            var minx = document.Shapes.MinBy(x => x.Center.X)?.Center.X;
            var miny = document.Shapes.MinBy(x => x.Center.Y)?.Center.Y;
            
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

            var index = 0;

            foreach (var shape in document.Shapes)
            {
                index++;
                shape.ToStl(transformX, transformY, stl, index);
                
            }
            stl.Name = fileName;
            // stl.SaveAsText($@"C:\Users\jbeam\Documents\StlExport\{fileName}");
            stl.SaveAsBinary(fileName);

        }

    }
}