using DoomNET.Resources;
                                         
namespace DoomNET.Rendering;

public class Camera
{
    public Vector3 position;
    public Quaternion rotation;
    public float fieldOfView = 90;

    public Vector3 right => rotation * new Vector3( 1, 0, 0 );   // X axis
    public Vector3 forward => rotation * new Vector3( 0, 1, 0 ); // Y axis
    public Vector3 up => rotation * new Vector3( 0, 0, 1 );      // Z axis

    public Camera( Vector3 position, Quaternion rotation )
    {
        this.position = position;
        this.rotation = rotation;
    }

    public Vector3 TransformDirection( Vector3 screenDirection )
    {
        // Transform the screen direction into world space using the camera's orientation vectors
        return rotation * screenDirection;
    }
}