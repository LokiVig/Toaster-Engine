using System.Numerics;

namespace Toast.Engine.Rendering;

public struct Vertex
{
    public const uint SIZE_IN_BYTES = 48;

    public Vector3 position;
    public Vector3? normal;
    public Vector2 uvCoordinate;
    public Color? color;

    public Vertex( Vector3 position, Vector2 uvCoordinate, Vector3? normal = null, Color? color = null )
    {
        this.position = position;
        this.uvCoordinate = uvCoordinate;
        this.normal = normal ?? Vector3.Zero;
        this.color = color ?? Color.White;
    }

    public override string ToString()
    {
        return $"({position} / {normal} - {uvCoordinate})";
    }
}