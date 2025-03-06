using System.Numerics;

namespace Toast.Engine.Rendering;

/// <summary>
/// Defines a point in space with a UV coordinate and normals.
/// </summary>
public struct Vertex
{
    public const uint SIZE_IN_BYTES = 48;

    public Vector3 position; // The position in 3D space this vertex finds itself in
    public Vector2 uvCoordinate; // The UV coordinate of this vertex, used for textures
    public Vector3? normal; // The normal of this vertex

    public Vertex( Vector3 position, Vector2 uvCoordinate, Vector3? normal = null )
    {
        this.position = position;
        this.uvCoordinate = uvCoordinate;
        this.normal = normal ?? Vector3.Zero;
    }

    public override string ToString()
    {
        return $"({position} - {normal} - {uvCoordinate})";
    }
}