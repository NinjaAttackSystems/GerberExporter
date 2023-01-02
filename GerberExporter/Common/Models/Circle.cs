using System.Text;
using GerberExporter.BrokerModels;
using GerberExporter.Gerber.GerberLineStrategy;
using QuantumConcepts.Formats.StereoLithography;
using Facet = GerberExporter.BrokerModels.Facet;
using STLDocument = QuantumConcepts.Formats.StereoLithography.STLDocument;

namespace GerberExporter.Common.Models;

public class Circle : IShape
{
    public Circle(FlashCircleLine line)
    {
        Center = line.Center;
        Type = ShapeType.Circle;
        Diameter = line.Diameter;
    }
    public int Id { get; set; }
    public double Diameter { get; set; }
    public double Radius => Diameter / 2;
    public ShapeType Type { get; set; }
    public Vertex Center { get; set; }

    public void ToSvg(double transformX, double transformY, StringBuilder sb)
    {
        sb.AppendLine($"<circle cx=\"{Center.X - transformX}mm\" cy=\"{Center.Y - transformY}mm\" r=\"{Radius}mm\" stroke=\"black\" stroke-width=\"1mm\" fill=\"black\" />");
    }

    public void ToStl(double transformX, double transformY, STLDocument stl, int index)
    {
        var facets = GetFacets();

        foreach (var facet in facets)
        {
            var model = new QuantumConcepts.Formats.StereoLithography.Facet(new QuantumConcepts.Formats.StereoLithography.Normal((float)facet.Normal.X, (float)facet.Normal.Y, (float)facet.Normal.Z),
                new List<QuantumConcepts.Formats.StereoLithography.Vertex>(), 0
                );

            foreach (var vertex in facet.Vertices)
            {
                var vert = new QuantumConcepts.Formats.StereoLithography.Vertex((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
               model.Vertices.Add(vert);
            }

            stl.Facets.Add(model);
        }

    }


    private List<Facet> GetFacets()
    {
        var facets = new List<Facet>();

        for (var startTheta = 0; startTheta < 360; startTheta += 10)
        {
            var vertices =
                GetCircleFacet(
                    Radius,
                    Center.X,
                    Center.Y,
                    0,
                    startTheta,
                    startTheta + 10
                );

            var normal = GetNormal(vertices);

            facets.Add(new Facet(){Normal = normal, Vertices = vertices});

            vertices =
                GetCircleFacet(
                    Radius,
                    Center.X,
                    Center.Y,
                    0.2,
                    startTheta,
                    startTheta + 10
                );

            normal = GetNormal(vertices);

            facets.Add(new Facet() { Normal = normal, Vertices = vertices });

            var sides = GetSide(Radius,
                Center.X,
                Center.Y,
                0.2,
                startTheta,
                startTheta + 10);

            facets.AddRange(sides);
        }

        return facets;
    }

    private List<Facet> GetSide(double radius, double centerX, double centerY, double z, int startTheta, int endTheta)
    {
        var sides = new List<Facet>();
        var side1Vertices = new List<Vertex>()
        {
            GetCircleVertex(centerX, centerY, 0, radius, startTheta),
            GetCircleVertex(centerX, centerY, 0, radius, endTheta),
            GetCircleVertex(centerX, centerY, 0.2, radius, startTheta),
        };

        var side1Normal = GetNormal(side1Vertices);
        sides.Add( new Facet(){ Normal = side1Normal, Vertices = side1Vertices});

        var side2Vertices = new List<Vertex>()
        {
            GetCircleVertex(centerX, centerY, 0.2, radius, startTheta),
            GetCircleVertex(centerX, centerY, 0.2, radius, endTheta),
            GetCircleVertex(centerX, centerY, 0, radius, endTheta),
        };

        var side2Normal = GetNormal(side2Vertices);
        sides.Add(new Facet() { Normal = side2Normal, Vertices = side2Vertices });
        return sides;
    }

    private IList<Vertex> GetCircleFacet(double radius, double centerX, double centerY, double z, int startTheta, int endTheta)
    {
        var vertices = new List<Vertex>()
        {
            new Vertex(centerX, centerY, z, false),
            GetCircleVertex(centerX, centerY, z,radius, startTheta),
            GetCircleVertex(centerX, centerY, z, radius, endTheta)
        };

        return vertices;
    }
    private static Vertex GetCircleVertex(double centerX, double centerY, double z, double radius, int theta)
    {
        var x = centerX + radius / 2 * Math.Cos(theta * Math.PI / 180);
        var y = centerY + radius / 2 * Math.Sin(theta * Math.PI / 180);
        return new Vertex((float)x, (float)y, z, false);
    }


    private static Vertex GetNormal(IList<Vertex> vertices)
    {
        var normal = new Vertex(0, 0, 0, false);

        for (var i = 0; i < vertices.Count; i++)
        {
            var j = (i + 1) % (vertices.Count);
            normal.X += (vertices[i].Y - vertices[j].Y) * (vertices[i].Z + vertices[j].Z);
            normal.Y += (vertices[i].Z - vertices[j].Z) * (vertices[i].X + vertices[j].X);
            normal.Z += (vertices[i].X - vertices[j].X) * (vertices[i].Y + vertices[j].Y);
        }
        return normal;
    }

}