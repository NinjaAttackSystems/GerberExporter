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
            
            // Calculate actual bounds including shape sizes
            double? actualMinX = null;
            double? actualMaxX = null;
            double? actualMinY = null;
            double? actualMaxY = null;
            
            foreach (var shape in document.Shapes)
            {
                double shapeMinX, shapeMaxX, shapeMinY, shapeMaxY;
                
                if (shape is Rectangle rect)
                {
                    shapeMinX = rect.Center.X - rect.Width / 2;
                    shapeMaxX = rect.Center.X + rect.Width / 2;
                    shapeMinY = rect.Center.Y - rect.Height / 2;
                    shapeMaxY = rect.Center.Y + rect.Height / 2;
                }
                else if (shape is Circle circle)
                {
                    shapeMinX = circle.Center.X - circle.Radius;
                    shapeMaxX = circle.Center.X + circle.Radius;
                    shapeMinY = circle.Center.Y - circle.Radius;
                    shapeMaxY = circle.Center.Y + circle.Radius;
                }
                else
                {
                    continue;
                }
                
                actualMinX = actualMinX == null ? shapeMinX : Math.Min(actualMinX.Value, shapeMinX);
                actualMaxX = actualMaxX == null ? shapeMaxX : Math.Max(actualMaxX.Value, shapeMaxX);
                actualMinY = actualMinY == null ? shapeMinY : Math.Min(actualMinY.Value, shapeMinY);
                actualMaxY = actualMaxY == null ? shapeMaxY : Math.Max(actualMaxY.Value, shapeMaxY);
            }
            
            // Transform to put shapes near origin with small margin
            double transformX = (actualMinX ?? 0) - 1; // 1mm margin
            double transformY = (actualMinY ?? 0) - 1; // 1mm margin

            var index = 0;

            foreach (var shape in document.Shapes)
            {
                index++;
                shape.ToStl(transformX, transformY, stl, index);
            }
            
            stl.Name = fileName;
            stl.SaveAsBinary(fileName);
        }

    }
}