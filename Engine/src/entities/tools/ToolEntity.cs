using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

/// <summary>
/// Wrapper for all tool entities.<br/>
/// Primarily used for filtering and JSON serialization / deserialization
/// </summary>
public class ToolEntity : Entity
{
	public override EntityType type => EntityType.Tool;
	
	public ToolEntity() {}
	public ToolEntity(Vector3 position) : base(position) {}
}