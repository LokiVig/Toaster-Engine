using System.Numerics;

using Veldrid;

namespace Toast.Engine.Resources;

public struct Vertex
{
    public const uint SIZE_IN_BYTES = 48;

    public Vector3 position { get; set; }
    public Vector3? normal { get; set; }
    public Vector2 uvCoordinate { get; set; }
    public Color? color { get; set; }

    public Vertex( Vector3 position, Vector2 uvCoordinate, Vector3? normal = null, Color? color = null )
    {
        this.position = position;
        this.uvCoordinate = uvCoordinate;
        this.normal = normal;
        this.color = color;
    }

    public static VertexLayoutDescription vertexLayout => new VertexLayoutDescription(
        new VertexElementDescription[]
        {
            new VertexElementDescription("Postion", VertexElementSemantic.Position, VertexElementFormat.Float3),
            new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
            new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
            new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4)
        }
        );

    public override string ToString()
    {
        return $"({position} / {normal} - {uvCoordinate})";
    }
}