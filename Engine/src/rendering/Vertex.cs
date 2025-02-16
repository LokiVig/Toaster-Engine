using System.Numerics;

namespace Toast.Engine.Rendering;

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

    public override string ToString()
    {
        return $"({position} / {normal} - {uvCoordinate})";
    }
}