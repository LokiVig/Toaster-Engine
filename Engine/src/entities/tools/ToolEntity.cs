using System.Numerics;

namespace Toast.Engine.Entities.Tools;

/// <summary>
/// Wrapper for all tool entities.<br/>
/// Primarily used for filtering and JSON serialization / deserialization
/// </summary>
public class ToolEntity : Entity
{
    public override EntityType type => EntityType.Tool;

    public ToolEntity() : base() { }

    public ToolEntity( Vector3 position ) : base( position ) { }

    public ToolEntity( Entity parent ) : base( parent ) { }

    public ToolEntity( Entity parent, Vector3 position ) : base( parent, position ) { }
}