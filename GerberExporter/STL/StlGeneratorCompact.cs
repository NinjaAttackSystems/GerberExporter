using System;
using System.Collections.Generic;
using System.Linq;
using GerberExporter.BrokerModels;
using GerberExporter.Common;
using GerberExporter.Common.Models;
using QuantumConcepts.Formats.StereoLithography;
using Facet = GerberExporter.BrokerModels.Facet;
using Vertex = GerberExporter.Common.Vertex;

namespace GerberExporter.STL
{
    /// <summary>
    /// Generates a compact STL by arranging shapes in a grid pattern
    /// instead of maintaining their original positions
    /// </summary>
    public class StlGeneratorCompact
    {
        public void Create(string fileName, BrokerDocument document)
        {
            var stl = new STLDocument();
            
            // Sort shapes by size (larger first) for better packing
            var sortedShapes = document.Shapes.OrderByDescending(s => 
            {
                if (s is Rectangle rect)
                    return rect.Width * rect.Height;
                else if (s is Circle circle)
                    return Math.PI * circle.Radius * circle.Radius;
                return 0;
            }).ToList();

            // Arrange shapes in a grid
            double currentX = 0;
            double currentY = 0;
            double rowHeight = 0;
            double maxWidth = 100; // Maximum width before wrapping to next row
            double spacing = 2; // 2mm spacing between shapes
            
            var index = 0;

            foreach (var shape in sortedShapes)
            {
                index++;
                
                double shapeWidth = 0;
                double shapeHeight = 0;
                
                if (shape is Rectangle rect)
                {
                    shapeWidth = rect.Width;
                    shapeHeight = rect.Height;
                }
                else if (shape is Circle circle)
                {
                    shapeWidth = circle.Diameter;
                    shapeHeight = circle.Diameter;
                }
                
                // Check if we need to wrap to next row
                if (currentX + shapeWidth > maxWidth && currentX > 0)
                {
                    currentX = 0;
                    currentY += rowHeight + spacing;
                    rowHeight = 0;
                }
                
                // Position shape at current location
                double shapeCenterX = currentX + shapeWidth / 2;
                double shapeCenterY = currentY + shapeHeight / 2;
                
                // Generate STL at the new position
                if (shape is Rectangle rectangle)
                {
                    var facets = GetRectangleFacets(shapeCenterX, shapeCenterY, rectangle.Width, rectangle.Height);
                    AddFacetsToStl(facets, stl);
                }
                else if (shape is Circle circle)
                {
                    var facets = GetCircleFacets(shapeCenterX, shapeCenterY, circle.Radius);
                    AddFacetsToStl(facets, stl);
                }
                
                // Update position for next shape
                currentX += shapeWidth + spacing;
                rowHeight = Math.Max(rowHeight, shapeHeight);
            }
            
            stl.Name = fileName;
            stl.SaveAsBinary(fileName);
        }
        
        private void AddFacetsToStl(List<Facet> facets, STLDocument stl)
        {
            foreach (var facet in facets)
            {
                var model = new QuantumConcepts.Formats.StereoLithography.Facet(
                    new QuantumConcepts.Formats.StereoLithography.Normal((float)facet.Normal.X, (float)facet.Normal.Y, (float)facet.Normal.Z),
                    new List<QuantumConcepts.Formats.StereoLithography.Vertex>(), 0);

                foreach (var vertex in facet.Vertices)
                {
                    var vert = new QuantumConcepts.Formats.StereoLithography.Vertex(
                        (float)vertex.X, 
                        (float)vertex.Y, 
                        (float)vertex.Z);
                    model.Vertices.Add(vert);
                }

                stl.Facets.Add(model);
            }
        }
        
        private List<Facet> GetRectangleFacets(double centerX, double centerY, double width, double height)
        {
            var facets = new List<Facet>();
            var halfWidth = width / 2;
            var halfHeight = height / 2;
            var thickness = 0.2;

            // Top face
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, thickness,
                centerX + halfWidth, centerY - halfHeight, thickness,
                centerX + halfWidth, centerY + halfHeight, thickness));
            
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, thickness,
                centerX + halfWidth, centerY + halfHeight, thickness,
                centerX - halfWidth, centerY + halfHeight, thickness));

            // Bottom face
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, 0,
                centerX + halfWidth, centerY - halfHeight, 0));
            
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX - halfWidth, centerY + halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, 0));

            // Side faces
            // Front
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY - halfHeight, thickness));
            
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY - halfHeight, thickness,
                centerX - halfWidth, centerY - halfHeight, thickness));

            // Back
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY + halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, thickness,
                centerX + halfWidth, centerY + halfHeight, 0));
            
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY + halfHeight, 0,
                centerX - halfWidth, centerY + halfHeight, thickness,
                centerX + halfWidth, centerY + halfHeight, thickness));

            // Left
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX - halfWidth, centerY - halfHeight, thickness,
                centerX - halfWidth, centerY + halfHeight, thickness));
            
            facets.Add(CreateTriangleFacet(
                centerX - halfWidth, centerY - halfHeight, 0,
                centerX - halfWidth, centerY + halfHeight, thickness,
                centerX - halfWidth, centerY + halfHeight, 0));

            // Right
            facets.Add(CreateTriangleFacet(
                centerX + halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, thickness,
                centerX + halfWidth, centerY - halfHeight, thickness));
            
            facets.Add(CreateTriangleFacet(
                centerX + halfWidth, centerY - halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, 0,
                centerX + halfWidth, centerY + halfHeight, thickness));

            return facets;
        }
        
        private List<Facet> GetCircleFacets(double centerX, double centerY, double radius)
        {
            var facets = new List<Facet>();
            var thickness = 0.2;
            
            for (var angle = 0; angle < 360; angle += 10)
            {
                var angle1Rad = angle * Math.PI / 180;
                var angle2Rad = (angle + 10) * Math.PI / 180;
                
                var x1 = centerX + radius * Math.Cos(angle1Rad);
                var y1 = centerY + radius * Math.Sin(angle1Rad);
                var x2 = centerX + radius * Math.Cos(angle2Rad);
                var y2 = centerY + radius * Math.Sin(angle2Rad);
                
                // Top face
                facets.Add(CreateTriangleFacet(centerX, centerY, thickness, x1, y1, thickness, x2, y2, thickness));
                
                // Bottom face
                facets.Add(CreateTriangleFacet(centerX, centerY, 0, x2, y2, 0, x1, y1, 0));
                
                // Side faces
                facets.Add(CreateTriangleFacet(x1, y1, 0, x2, y2, 0, x2, y2, thickness));
                facets.Add(CreateTriangleFacet(x1, y1, 0, x2, y2, thickness, x1, y1, thickness));
            }
            
            return facets;
        }
        
        private Facet CreateTriangleFacet(double x1, double y1, double z1,
                                         double x2, double y2, double z2,
                                         double x3, double y3, double z3)
        {
            var vertices = new List<Vertex>
            {
                new Vertex(x1, y1, z1, false),
                new Vertex(x2, y2, z2, false),
                new Vertex(x3, y3, z3, false)
            };
            
            var normal = CalculateNormal(vertices);
            
            return new Facet { Normal = normal, Vertices = vertices };
        }
        
        private Vertex CalculateNormal(IList<Vertex> vertices)
        {
            var v1 = vertices[1];
            var v0 = vertices[0];
            var v2 = vertices[2];
            
            var u = new Vertex(v1.X - v0.X, v1.Y - v0.Y, v1.Z - v0.Z, false);
            var v = new Vertex(v2.X - v0.X, v2.Y - v0.Y, v2.Z - v0.Z, false);
            
            var normal = new Vertex(
                u.Y * v.Z - u.Z * v.Y,
                u.Z * v.X - u.X * v.Z,
                u.X * v.Y - u.Y * v.X,
                false);
            
            var length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
            if (length > 0)
            {
                normal.X /= length;
                normal.Y /= length;
                normal.Z /= length;
            }
            
            return normal;
        }
    }
}