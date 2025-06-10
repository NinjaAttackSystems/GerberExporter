using System.Text;
using GerberExporter.BrokerModels;
using GerberExporter.Gerber.GerberLineStrategy;
using QuantumConcepts.Formats.StereoLithography;
using Facet = GerberExporter.BrokerModels.Facet;

namespace GerberExporter.Common.Models;

public class Rectangle : IShape
{
    public Rectangle(FlashRectangleLine line)
    {
        Center = line.Center;
        Type = ShapeType.Rectangle;
        Width = line.Width;
        Height = line.Height;
    }

    public double Width { get; set; }
    public double Height { get; set; }
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }

    public void ToSvg(double transformX, double transformY, StringBuilder sb)
    {
        var x = Center.X - transformX - (Width / 2);
        var y = Center.Y - transformY - (Height / 2);
        sb.AppendLine($"<rect x=\"{x}mm\" y=\"{y}mm\" width=\"{Width}mm\" height=\"{Height}mm\" stroke=\"black\" stroke-width=\"0.1mm\" fill=\"black\" />");
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
        var halfWidth = Width / 2;
        var halfHeight = Height / 2;

        // Top face
        var topVertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false)
        };
        facets.Add(new Facet { Normal = GetNormal(topVertices), Vertices = topVertices });

        topVertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0.2, false)
        };
        facets.Add(new Facet { Normal = GetNormal(topVertices), Vertices = topVertices });

        // Bottom face
        var bottomVertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0, false)
        };
        facets.Add(new Facet { Normal = GetNormal(bottomVertices), Vertices = bottomVertices });

        bottomVertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0, false)
        };
        facets.Add(new Facet { Normal = GetNormal(bottomVertices), Vertices = bottomVertices });

        // Side faces
        facets.AddRange(GetSideFacets(halfWidth, halfHeight));

        return facets;
    }

    private List<Facet> GetSideFacets(double halfWidth, double halfHeight)
    {
        var sides = new List<Facet>();

        // Front face
        var vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0.2, false),
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        // Back face
        vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        // Left face
        vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0.2, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        vertices = new List<Vertex>
        {
            new Vertex(Center.X - halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0.2, false),
            new Vertex(Center.X - halfWidth, Center.Y + halfHeight, 0, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        // Right face
        vertices = new List<Vertex>
        {
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false),
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        vertices = new List<Vertex>
        {
            new Vertex(Center.X + halfWidth, Center.Y - halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0, false),
            new Vertex(Center.X + halfWidth, Center.Y + halfHeight, 0.2, false)
        };
        sides.Add(new Facet { Normal = GetNormal(vertices), Vertices = vertices });

        return sides;
    }

    private static Vertex GetNormal(IList<Vertex> vertices)
    {
        var normal = new Vertex(0, 0, 0, false);

        for (var i = 0; i < vertices.Count; i++)
        {
            var j = (i + 1) % vertices.Count;
            normal.X += (vertices[i].Y - vertices[j].Y) * (vertices[i].Z + vertices[j].Z);
            normal.Y += (vertices[i].Z - vertices[j].Z) * (vertices[i].X + vertices[j].X);
            normal.Z += (vertices[i].X - vertices[j].X) * (vertices[i].Y + vertices[j].Y);
        }
        return normal;
    }
}