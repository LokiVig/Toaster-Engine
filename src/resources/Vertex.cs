namespace DoomNET.Resources;

/// <summary>
/// A vertice in 3D space, has a position, normals (facing side), and texture coordinates
/// </summary>
public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 texCoords;

    public Vertex(Vector3 position)
    {
        this.position = position;
    }

    public Vertex( float x, float y, float z )
    {
        position = new Vector3( x, y, z );
    }
}