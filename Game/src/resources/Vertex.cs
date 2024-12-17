namespace DoomNET.Resources;

public struct Vertex
{
	public Vector3 position { get; set; }
	public Vector2 uvCoordinate { get; set; }

	public Vertex(Vector3 position, Vector2 uvCoordinate)
	{
		this.position = position;
		this.uvCoordinate = uvCoordinate;
	}

	public override string ToString()
	{
		return $"({position} - {uvCoordinate})";
	}
}