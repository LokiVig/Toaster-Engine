using System.Numerics;

using Toast.Engine.Entities;

namespace Toast.Engine.Rendering;

/// <summary>
/// A camera with a frustum, offset, parent entity, etc.
/// </summary>
public class Camera : ToolEntity
{
    public Camera( Entity parent, Vector3 offset ) : base( parent )
    {
        transform.localPosition = offset;
        transform.worldRotation = Quaternion.Identity;
    }

    public Camera( Entity parent, Vector3 offset, Quaternion rotation ) : this( parent, offset )
    {
        transform.worldRotation = rotation;
    }

    protected override void Update()
    {
        base.Update();

        transform.localPosition += transform.worldPosition;
    }
}