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
        normal = Vector4.Zero;
    }

    public Vertex( Vector3 position, Vector2 texCoords )
    {
        this.position = position;
        this.texCoords = texCoords;
        normal = Vector4.Zero;
    }

    public Vertex( float x, float y, float z )
    {
        position = new Vector3( x, y, z );
        normal = Vector4.Zero;
    }

    public Vertex( float x, float y, float z, Vector2 texCoords )
    {
        position = new Vector3( x, y, z );
        this.texCoords = texCoords;
        normal = Vector4.Zero;
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

    public Vertex NormalTransform( Matrix4x4 transform )
    {
        normal = ( transform * normal );
        normal.Normalized();
        return this;
    }

    public Vertex VertexTransform( Matrix4x4 transform )
    {
        Vector4 vec4 = transform * new Vector4( position );
        vec4.Homogenize();
        position.x = vec4.x;
        position.y = vec4.y;
        position.z = vec4.z;
        return this;
    }

    public Vertex FirstTransform( Matrix4x4 transform, Matrix4x4 normalTransform )
    {
        Vector4 vec4 = transform * new Vector4( position );
        Vertex temp = new Vertex( new Vector3( vec4.x, vec4.y, vec4.z ) );

        temp.normal = normalTransform * normal;
        temp.normal.Normalized();
        return temp;
    }
}