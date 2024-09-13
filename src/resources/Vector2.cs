#pragma warning disable CS0659
#pragma warning disable CS0661

using System;

namespace DoomNET.Resources;

/// <summary>
/// Struct defining a position in 2 dimensions
/// </summary>
public struct Vector2
{
    public float x { get; set; }
    public float y { get; set; }

    public Vector2()
    {
        x = y = 0;
    }

    public Vector2(float xy)
    {
        x = y = xy;
    }

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static float DistanceBetween(Vector2 source, Vector2 dest)
    {
        return (source - dest).Magnitude();
    }

    public float DistanceTo(Vector2 other)
    {
        return (this - other).Magnitude();
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y);
    }

    public Vector2 Normalized()
    {
        float magnitude = Magnitude();

        if (magnitude > 0)
        {
            return new Vector2(x / magnitude, y / magnitude);
        }
        else
        {
            return new Vector2();
        }
    }

    public static Vector2 Normalize(Vector2 vector)
    {
        return vector.Normalized();
    }

    public readonly override string ToString()
    {
        return $"<{x:0.###}, {y:0.###}>";
    }

    #region OPERATORS
    public static Vector2 operator +(Vector2 in1) => in1;
    public static Vector2 operator -(Vector2 in1) => new Vector2(-in1.x, -in1.y);

    public static bool operator ==(Vector2 in1, Vector2 in2)
    {
        return in1.x == in2.x && in1.y == in2.y;
    }

    public static bool operator ==(Vector2 in1, float in2)
    {
        return in1.x == in2 && in1.y == in2;
    }

    public static bool operator ==(float in1, Vector2 in2)
    {
        return in1 == in2.x && in1 == in2.y;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Vector2))
        {
            return this == (Vector2)obj;
        }
        else if (obj.GetType() == typeof(float))
        {
            return this == (float)obj;
        }

        return false;
    }

    public static bool operator !=(Vector2 in1, Vector2 in2)
    {
        return !(in1 == in2);
    }

    public static bool operator !=(Vector2 in1, float in2)
    {
        return !(in1 == in2);
    }

    public static bool operator !=(float in1, Vector2 in2)
    {
        return !(in1 == in2);
    }

    public static bool operator <(Vector2 in1, Vector2 in2)
    {
        return in1.x < in2.x && in1.y < in2.y;
    }

    public static bool operator <(Vector2 in1, float in2)
    {
        return in1.x < in2 && in1.y < in2;
    }

    public static bool operator <(float in1, Vector2 in2)
    {
        return in1 < in2.x && in1 < in2.y;
    }

    public static bool operator >(Vector2 in1, Vector2 in2)
    {
        return in1.x > in2.x && in1.y > in2.y;
    }

    public static bool operator >(Vector2 in1, float in2)
    {
        return in1.x > in2 && in1.y > in2;
    }

    public static bool operator >(float in1, Vector2 in2)
    {
        return in1 > in2.x && in1 > in2.y;
    }

    public static bool operator <=(Vector2 in1, Vector2 in2)
    {
        return in1 < in2 || in1 == in2;
    }

    public static bool operator <=(Vector2 in1, float in2)
    {
        return in1 < in2 || in1 == in2;
    }

    public static bool operator <=(float in1, Vector2 in2)
    {
        return in1 < in2 || in1 == in2;
    }

    public static bool operator >=(Vector2 in1, Vector2 in2)
    {
        return in1 > in2 || in1 == in2;
    }

    public static bool operator >=(Vector2 in1, float in2)
    {
        return in1 > in2 || in1 == in2;
    }

    public static bool operator >=(float in1, Vector2 in2)
    {
        return in1 > in2 || in1 == in2;
    }

    public static Vector2 operator +(Vector2 in1, Vector2 in2)
    {
        return new Vector2(in1.x + in2.x, in1.y + in2.y);
    }

    public static Vector2 operator +(Vector2 in1, float in2)
    {
        return new Vector2(in1.x + in2, in1.y + in2);
    }

    public static Vector2 operator +(float in1, Vector2 in2)
    {
        return new Vector2(in1 + in2.x, in1 + in2.y);
    }

    public static Vector2 operator -(Vector2 in1, Vector2 in2)
    {
        return new Vector2(in1.x - in2.x, in1.y - in2.y);
    }

    public static Vector2 operator -(Vector2 in1, float in2)
    {
        return new Vector2(in1.x - in2, in1.y - in2);
    }
    
    public static Vector2 operator -(float in1, Vector2 in2)
    {
        return new Vector2(in1 - in2.x, in1 - in2.y);
    }

    public static Vector2 operator *(Vector2 in1, Vector2 in2)
    {
        return new Vector2(in1.x * in2.x, in1.y * in2.y);
    }

    public static Vector2 operator *(Vector2 in1, float in2)
    {
        return new Vector2(in1.x * in2, in1.y * in2);
    }

    public static Vector2 operator *(float in1, Vector2 in2)
    {
        return new Vector2(in1 * in2.x, in1 * in2.y);
    }

    public static Vector2 operator /(Vector2 in1, Vector2 in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector2(in1.x / in2.x, in1.y / in2.y);
    }

    public static Vector2 operator /(Vector2 in1, float in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector2(in1.x / in2, in1.y / in2);
    }

    public static Vector2 operator /(float in1, Vector2 in2)
    {
        if (in1 == 0 || in2 == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector2(in1 / in2.x, in1 / in2.y);
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

                default:
                    return 0;
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
            }
        }
    }
    #endregion // OPERATORS
}