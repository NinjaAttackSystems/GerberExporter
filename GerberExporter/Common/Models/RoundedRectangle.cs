using System;
using System.Collections.Generic;
using System.Text;
using GerberExporter.BrokerModels;
using GerberExporter.Gerber.GerberLineStrategy;
using QuantumConcepts.Formats.StereoLithography;
using Facet = GerberExporter.BrokerModels.Facet;
using Vertex = GerberExporter.Common.Vertex;

namespace GerberExporter.Common.Models;

public class RoundedRectangle : IShape
{
    public RoundedRectangle(FlashRoundedRectangleLine line)
    {
        Center = line.Center;
        Type = ShapeType.Rectangle; // Using Rectangle type for now
        Width = line.Width;
        Height = line.Height;
        CornerRadius = line.CornerRadius;
    }

    public double Width { get; set; }
    public double Height { get; set; }
    public double CornerRadius { get; set; }
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }

    public void ToSvg(double transformX, double transformY, StringBuilder sb)
    {
        var x = Center.X - transformX - (Width / 2);
        var y = Center.Y - transformY - (Height / 2);
        
        // SVG rounded rectangle
        sb.AppendLine($"<rect x=\"{x}mm\" y=\"{y}mm\" width=\"{Width}mm\" height=\"{Height}mm\" rx=\"{CornerRadius}mm\" ry=\"{CornerRadius}mm\" stroke=\"black\" stroke-width=\"0.1mm\" fill=\"black\" />");
    }

    public void ToStl(double transformX, double transformY, STLDocument stl, int index)
    {
        var facets = GetFacets();

        foreach (var facet in facets)
        {
            var model = new QuantumConcepts.Formats.StereoLithography.Facet(
                new QuantumConcepts.Formats.StereoLithography.Normal((float)facet.Normal.X, (float)facet.Normal.Y, (float)facet.Normal.Z),
                new List<QuantumConcepts.Formats.StereoLithography.Vertex>(), 0);

            foreach (var vertex in facet.Vertices)
            {
                var vert = new QuantumConcepts.Formats.StereoLithography.Vertex(
                    (float)(vertex.X - transformX), 
                    (float)(vertex.Y - transformY), 
                    (float)vertex.Z);
                model.Vertices.Add(vert);
            }

            stl.Facets.Add(model);
        }
    }

    private List<Facet> GetFacets()
    {
        var facets = new List<Facet>();
        var thickness = 0.2;
        
        // Create vertices for the rounded rectangle outline (without center point)
        var bottomVertices = GetRoundedRectangleOutline(Center.X, Center.Y, Width, Height, CornerRadius, 0);
        var topVertices = GetRoundedRectangleOutline(Center.X, Center.Y, Width, Height, CornerRadius, thickness);
        
        // Create top face using ear clipping triangulation
        var topFacets = TriangulatePolygon(topVertices);
        facets.AddRange(topFacets);
        
        // Create bottom face
        var bottomFacets = TriangulatePolygon(bottomVertices);
        // Reverse winding for bottom face
        foreach (var facet in bottomFacets)
        {
            facets.Add(new Facet 
            { 
                Normal = new Vertex(-facet.Normal.X, -facet.Normal.Y, -facet.Normal.Z, false),
                Vertices = new List<Vertex> { facet.Vertices[2], facet.Vertices[1], facet.Vertices[0] }
            });
        }
        
        // Create side faces
        for (int i = 0; i < bottomVertices.Count; i++)
        {
            int next = (i + 1) % bottomVertices.Count;
            
            facets.Add(CreateTriangleFacet(
                bottomVertices[i].X, bottomVertices[i].Y, bottomVertices[i].Z,
                bottomVertices[next].X, bottomVertices[next].Y, bottomVertices[next].Z,
                topVertices[next].X, topVertices[next].Y, topVertices[next].Z));
                
            facets.Add(CreateTriangleFacet(
                bottomVertices[i].X, bottomVertices[i].Y, bottomVertices[i].Z,
                topVertices[next].X, topVertices[next].Y, topVertices[next].Z,
                topVertices[i].X, topVertices[i].Y, topVertices[i].Z));
        }
        
        return facets;
    }
    
    private List<Vertex> GetRoundedRectangleOutline(double centerX, double centerY, double width, double height, double radius, double z)
    {
        var vertices = new List<Vertex>();
        var halfWidth = width / 2;
        var halfHeight = height / 2;
        
        // Limit corner radius to half of the smaller dimension
        radius = Math.Min(radius, Math.Min(halfWidth, halfHeight));
        
        // Start from right edge, middle point and go counter-clockwise
        // Right edge to top-right corner
        vertices.Add(new Vertex(centerX + halfWidth, centerY + halfHeight - radius, z, false));
        
        // Top-right corner arc
        for (int angle = 0; angle <= 90; angle += 10)
        {
            var rad = angle * Math.PI / 180;
            var x = centerX + halfWidth - radius + radius * Math.Cos(rad);
            var y = centerY + halfHeight - radius + radius * Math.Sin(rad);
            vertices.Add(new Vertex(x, y, z, false));
        }
        
        // Top edge
        vertices.Add(new Vertex(centerX - halfWidth + radius, centerY + halfHeight, z, false));
        
        // Top-left corner arc
        for (int angle = 90; angle <= 180; angle += 10)
        {
            var rad = angle * Math.PI / 180;
            var x = centerX - halfWidth + radius + radius * Math.Cos(rad);
            var y = centerY + halfHeight - radius + radius * Math.Sin(rad);
            vertices.Add(new Vertex(x, y, z, false));
        }
        
        // Left edge
        vertices.Add(new Vertex(centerX - halfWidth, centerY - halfHeight + radius, z, false));
        
        // Bottom-left corner arc
        for (int angle = 180; angle <= 270; angle += 10)
        {
            var rad = angle * Math.PI / 180;
            var x = centerX - halfWidth + radius + radius * Math.Cos(rad);
            var y = centerY - halfHeight + radius + radius * Math.Sin(rad);
            vertices.Add(new Vertex(x, y, z, false));
        }
        
        // Bottom edge
        vertices.Add(new Vertex(centerX + halfWidth - radius, centerY - halfHeight, z, false));
        
        // Bottom-right corner arc
        for (int angle = 270; angle < 360; angle += 10)
        {
            var rad = angle * Math.PI / 180;
            var x = centerX + halfWidth - radius + radius * Math.Cos(rad);
            var y = centerY - halfHeight + radius + radius * Math.Sin(rad);
            vertices.Add(new Vertex(x, y, z, false));
        }
        
        return vertices;
    }
    
    // Simple triangulation for convex polygon (rounded rectangle is convex)
    private List<Facet> TriangulatePolygon(List<Vertex> vertices)
    {
        var facets = new List<Facet>();
        
        // For a convex polygon, we can use fan triangulation from the first vertex
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            facets.Add(CreateTriangleFacet(
                vertices[0].X, vertices[0].Y, vertices[0].Z,
                vertices[i].X, vertices[i].Y, vertices[i].Z,
                vertices[i + 1].X, vertices[i + 1].Y, vertices[i + 1].Z));
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