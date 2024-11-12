using DoomNET.Resources;

namespace DoomNET.Rendering;

public class Camera
{
    public Vector3 position;
    public Vector3 rotation;
    public float fieldOfView = 90;

    public Camera(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}