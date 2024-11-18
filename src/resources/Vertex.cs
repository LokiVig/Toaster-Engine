namespace DoomNET.Resources;

/// <summary>
/// A vertice in 3D space, has a position (<see cref="Vector3"/>), normals (facing side) (<see cref="Vector4"/>), color (<see cref="Color"/>).
/// and texture coordinates (<see cref="Vector2"/>)
/// </summary>
public struct Vertex
{
    public Vector3 position;
    public Vector4 normal;
    public Color color;
    public Vector2 texCoords;

    public Vertex( Vector3 position )
    {
        this.position = position;
        normal = Vector4.zero;
    }

    public Vertex( Vector3 position, Vector2 texCoords )
    {
        this.position = position;
        this.texCoords = texCoords;
        normal = Vector4.zero;
    }

    public Vertex( float x, float y, float z )
    {
        position = new Vector3( x, y, z );
        normal = Vector4.zero;
    }

    public Vertex( float x, float y, float z, Vector2 texCoords )
    {
        position = new Vector3( x, y, z );
        this.texCoords = texCoords;
        normal = Vector4.zero;
    }

    public void SetColor( Color newColor )
    {
        color.r = newColor.r;
        color.g = newColor.g;
        color.b = newColor.b;
        color.a = newColor.a;
    }

    public void SetColor( byte r, byte g, byte b, byte a = 255 )
    {
        color.r = r;
        color.g = g;
        color.b = b;
        color.a = a;
    }
}