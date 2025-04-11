using System.Numerics;

using Toast.Engine.Entities;
using Toast.Engine.Entities.Tools;

namespace Toast.Engine.Rendering;

/// <summary>
/// A camera with a frustum, offset, parent entity, etc.
/// </summary>
public class Camera : ToolEntity
{
    public Camera( Entity parent, Vector3 offset ) : base( parent )
    {
        // Set our local position to be our offset
        transform.localPosition = offset;

        // Add ourselves to the list of entities in the current scene
        EngineManager.currentScene?.AddEntity( this );
    }

    public Camera( Entity parent, Vector3 offset, Quaternion rotation ) : this( parent, offset )
    {
        transform.worldRotation = rotation;
    }
}