using Toast.Engine.Math;

namespace Toast.Engine.Resources;

public struct Vertex
{
	public Vector3 position { get; set; }
	public Vector3? normal { get; set; }
	public Vector2 uvCoordinate { get; set; }

	public Vertex(Vector3 position, Vector2 uvCoordinate, Vector3? normal = null)
	{
		this.position = position;
		this.uvCoordinate = uvCoordinate;
		this.normal = normal;
	}

	public override string ToString()
	{
		return $"({position} / {normal} - {uvCoordinate})";
	}
}