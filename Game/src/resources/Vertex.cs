namespace DoomNET.Resources;

public struct Vertex
{
	public Vector3 position;
	public Vector2 textureCoordinate;

	public Vertex(Vector3 position, Vector2 textureCoordinate)
	{
		this.position = position;
		this.textureCoordinate = textureCoordinate;
	}

	public override string ToString()
	{
		return $"({position} - {textureCoordinate})";
	}
}