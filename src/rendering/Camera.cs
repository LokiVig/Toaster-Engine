using System;

using DoomNET.Entities;
using DoomNET.Resources;

using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace DoomNET.Rendering;

/// <summary>
/// Defines the position and rotation of the player's camera.<br/>
/// This will be used for rendering everything that has to do with <see cref="Game.currentScene"/>.
/// </summary>
public class Camera
{
    public float fieldOfView = (float)Math.PI / 2;
    public float aspectRatio;

    public float Pitch
    {
        get => Quaternion.RadiansToDegrees(pitch);
        set
        {
            // Clamp the value between -89 and 89 to prevent the camera from going upside down
            float angle = Math.Clamp(value, -89f, 89f);
            pitch = Quaternion.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Yaw
    {
        get => Quaternion.RadiansToDegrees(yaw);
        set
        {
            yaw = Quaternion.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public Vector3 Right => right; // X axis
    public Vector3 Forward => forward; // Y axis
    public Vector3 Up => up; // Z axis
    
    private Vector3 right = Vector3.UnitX;
    private Vector3 forward = Vector3.UnitY;
    private Vector3 up = Vector3.UnitZ;
    private Vector3 position;
    private Vector3 positionOffset;
    private Vector3 target = Vector3.Zero;
    private Vector3 direction;
    private Entity parent;
    private float pitch;
    private float yaw = -(float)Math.PI / 2;

    public Camera( Vector3 positionOffset, float aspectRatio, Entity parent)
    {
        this.positionOffset = positionOffset;
        this.aspectRatio = aspectRatio;
        this.parent = parent;

        direction = Vector3.Normalize(position - target);

        Game.OnUpdate += UpdatePosition;
    }

    /// <summary>
    /// We should update the position based off our parent's position and our offset from it.
    /// </summary>
    private void UpdatePosition()
    {
        position = parent.GetPosition() + positionOffset;
    }

    /// <summary>
    /// Get the view matrix using the LookAt function
    /// </summary>
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt((OpenTK.Mathematics.Vector3)position, (OpenTK.Mathematics.Vector3)(position + forward), (OpenTK.Mathematics.Vector3)up);
    }

    /// <summary>
    /// Get the projection matrix using the same method we have used up until this point.
    /// </summary>
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, 0.01f, 1000.0f);
    }
    
    /// <summary>
    /// This method is going to update the direction vertices using some math.
    /// </summary>
    private void UpdateVectors()
    {
        // Calculate the forward matrix
        forward.x = MathF.Cos(pitch) * MathF.Cos(yaw);
        forward.y = MathF.Sin(pitch);
        forward.z = MathF.Cos(pitch) * MathF.Sin(yaw);
        
        // We need to make sure the vectors are all normalized
        forward = Vector3.Normalize(forward);
        
        // Calculate both the right and the up vector using cross product
        right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, forward));
    }
}