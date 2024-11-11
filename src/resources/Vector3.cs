#pragma warning disable CS0659
#pragma warning disable CS0661

using System;

namespace DoomNET.Resources;

/// <summary>
/// Struct defining a point in 3 dimensions (Z -> up)
/// </summary>
public struct Vector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Vector3()
    {
        x = y = z = 0.0f;
    }

    public Vector3(float xyz)
    {
        x = y = z = xyz;
    }

    public Vector3(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }

    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static float DistanceBetween(Vector3 source, Vector3 dest)
    {
        return (source - dest).Magnitude();
    }

    public float DistanceTo(Vector3 other)
    {
        return (this - other).Magnitude();
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y + z * z);
    }

    public Vector3 Normalized()
    {
        float magnitude = Magnitude();

        if (magnitude > 0)
        {
            return new Vector3(x / magnitude, y / magnitude, z / magnitude);
        }
        else
        {
            return new Vector3();
        }
    }

    public static Vector3 Normalize(Vector3 vector)
    {
        return vector.Normalized();
    }

    public readonly override string ToString()
    {
        return $"<{x:0.##}, {y:0.##}, {z:0.##}>";
    }

    #region OPERATORS
    public static Vector3 operator +(Vector3 in1) => in1;
    public static Vector3 operator -(Vector3 in1) => new Vector3(-in1.x, -in1.y, -in1.z);

    public static bool operator ==(Vector3 in1, Vector3 in2)
    {
        return in1.x == in2.x && in1.y == in2.y && in1.z == in2.z;
    }

    public static bool operator ==(Vector3 in1, float in2)
    {
        return in1.x == in2 && in1.y == in2 && in1.z == in2;
    }

    public static bool operator ==(float in1, Vector3 in2)
    {
        return in1 == in2.x && in1 == in2.y && in1 == in2.z;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Vector3))
        {
            return this == (Vector3)obj;
        }
        else if (obj.GetType() == typeof(float))
        {
            return this == (float)obj;
        }

        return false;
    }

    public static bool operator !=(Vector3 in1, Vector3 in2)
    {
        return !(in1 == in2);
    }

    public static bool operator !=(Vector3 in1, float in2)
    {
        return !(in1 == in2);
    }

    public static bool operator !=(float in1, Vector3 in2)
    {
        return !(in1 == in2);
    }

    public static bool operator <(Vector3 in1, Vector3 in2)
    {
        return in1.x - in2.x < 0 && in1.y - in2.y < 0 && in1.z - in2.z < 0;
    }

    public static bool operator <(Vector3 in1, float in2)
    {
        return in1.x - in2 < 0 && in1.y - in2 < 0 && in1.z - in2 < 0;
    }

    public static bool operator <(float in1, Vector3 in2)
    {
        return in1 - in2.x < 0 && in1 - in2.y < 0 && in1 - in2.z < 0;
    }

    public static bool operator >(Vector3 in1, Vector3 in2)
    {
        return in1.x - in2.x > 0 && in1.y - in2.y > 0 && in1.z - in2.z > 0;
    }

    public static bool operator >(Vector3 in1, float in2)
    {
        return in1.x - in2 > 0 && in1.y - in2 > 0 && in1.z - in2 > 0;
    }

    public static bool operator >(float in1, Vector3 in2)
    {
        return in1 - in2.x > 0 && in1 - in2.y > 0 && in1 - in2.z > 0;
    }

    public static bool operator <=(Vector3 in1, Vector3 in2)
    {
        return !(in1 > in2);
    }

    public static bool operator <=(Vector3 in1, float in2)
    {
        return !(in1 > in2);
    }

    public static bool operator <=(float in1, Vector3 in2)
    {
        return !(in1 > in2);
    }

    public static bool operator >=(Vector3 in1, Vector3 in2)
    {
        return !(in1 < in2);
    }

    public static bool operator >=(Vector3 in1, float in2)
    {
        return !(in1 < in2);
    }

    public static bool operator >=(float in1, Vector3 in2)
    {
        return !(in1 < in2);
    }

    public static Vector3 operator +(Vector3 in1, Vector3 in2)
    {
        return new Vector3(in1.x + in2.x, in1.y + in2.y, in1.z + in2.z);
    }

    public static Vector3 operator +(Vector3 in1, float in2)
    {
        return new Vector3(in1.x + in2, in1.y + in2, in1.z + in2);
    }

    public static Vector3 operator +(float in1, Vector3 in2)
    {
        return new Vector3(in1 + in2.x, in1 + in2.y, in1 + in2.z);
    }

    public static Vector3 operator -(Vector3 in1, Vector3 in2)
    {
        return new Vector3(in1.x - in2.x, in1.y - in2.y, in1.z - in2.z);
    }

    public static Vector3 operator -(Vector3 in1, float in2)
    {
        return new Vector3(in1.x - in2, in1.y - in2, in1.z - in2);
    }

    public static Vector3 operator -(float in1, Vector3 in2)
    {
        return new Vector3(in1 - in2.x, in1 - in2.y, in1 - in2.z);
    }

    public static Vector3 operator *(Vector3 in1, Vector3 in2)
    {
        return new Vector3(in1.x * in2.x, in1.y * in2.y, in1.z * in2.z);
    }

    public static Vector3 operator *(Vector3 in1, dynamic in2)
    {
        return new Vector3(in1.x * in2, in1.y * in2, in1.z * in2);
    }

    public static Vector3 operator *(dynamic in1, Vector3 in2)
    {
        return new Vector3(in1 * in2.x, in1 * in2.y, in1 * in2.z);
    }

    public static Vector3 operator /(Vector3 in1, Vector3 in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3(in1.x / in2.x, in1.y / in2.y, in1.z / in2.z);
    }

    public static Vector3 operator /(Vector3 in1, float in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3(in1.x / in2, in1.y / in2, in1.z / in2);
    }

    public static Vector3 operator /(float in1, Vector3 in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3(in1 / in2.x, in1 / in2.y, in1 / in2.z);
    }

    public float this[int i]
    {
        get
        {
            switch (i)
            {
                case 0:
                    return x;

                case 1:
                    return y;

                case 2:
                    return z;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
        set
        {
            switch (i)
            {
                case 0:
                    x = value;
                    break;

                case 1:
                    y = value;
                    break;

                case 2:
                    z = value;
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    #endregion // OPERATORS
}