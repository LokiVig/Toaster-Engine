using System;

namespace Toast.Engine.Resources;

/// <summary>
/// Struct defining a point in 2 dimensions
/// </summary>
public struct Vector2 : IEquatable<Vector2>
{
	public float x { get; set; }
	public float y { get; set; }

	public Vector2()
	{
		this = Zero;
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

	public static readonly Vector2 One = new Vector2(1, 1);
	public static readonly Vector2 Zero = new Vector2(0, 0);

	public static readonly Vector2 UnitX = new Vector2(1, 0);
	public static readonly Vector2 UnitY = new Vector2(0, 1);

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
		return $"<{x:0.##}, {y:0.##}>";
	}

	#region OPERATORS

	public static Vector2 operator +(Vector2 lhs) => lhs;
	public static Vector2 operator -(Vector2 lhs) => new Vector2(-lhs.x, -lhs.y);

	public static bool operator ==(Vector2 lhs, Vector2 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator ==(Vector2 lhs, float rhs)
	{
		return lhs.x == rhs && lhs.y == rhs;
	}

	public static bool operator ==(float lhs, Vector2 rhs)
	{
		return lhs == rhs.x && lhs == rhs.y;
	}

	public static bool operator !=(Vector2 lhs, Vector2 rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(Vector2 lhs, float rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(float lhs, Vector2 rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator <(Vector2 lhs, Vector2 rhs)
	{
		return lhs.Magnitude() - rhs.Magnitude() < 0;
	}

	public static bool operator <(Vector2 lhs, float rhs)
	{
		return lhs.Magnitude() - rhs < 0;
	}

	public static bool operator <(float lhs, Vector2 rhs)
	{
		return lhs - rhs.Magnitude() < 0;
	}

	public static bool operator >(Vector2 lhs, Vector2 rhs)
	{
		return lhs.Magnitude() - rhs.Magnitude() > 0;
	}

	public static bool operator >(Vector2 lhs, float rhs)
	{
		return lhs.Magnitude() - rhs > 0;
	}

	public static bool operator >(float lhs, Vector2 rhs)
	{
		return lhs - rhs.Magnitude() < 0;
	}

	public static bool operator <=(Vector2 lhs, Vector2 rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(Vector2 lhs, float rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(float lhs, Vector2 rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator >=(Vector2 lhs, Vector2 rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(Vector2 lhs, float rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(float lhs, Vector2 rhs)
	{
		return !(lhs < rhs);
	}

	public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
	{
		return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
	}

	public static Vector2 operator +(Vector2 lhs, float rhs)
	{
		return new Vector2(lhs.x + rhs, lhs.y + rhs);
	}

	public static Vector2 operator +(float lhs, Vector2 rhs)
	{
		return new Vector2(lhs + rhs.x, lhs + rhs.y);
	}

	public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
	{
		return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
	}

	public static Vector2 operator -(Vector2 lhs, float rhs)
	{
		return new Vector2(lhs.x - rhs, lhs.y - rhs);
	}

	public static Vector2 operator -(float lhs, Vector2 rhs)
	{
		return new Vector2(lhs - rhs.x, lhs - rhs.y);
	}

	public static Vector2 operator *(Vector2 lhs, Vector2 rhs)
	{
		return new Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
	}

	public static Vector2 operator *(Vector2 lhs, float rhs)
	{
		return new Vector2(lhs.x * rhs, lhs.y * rhs);
	}

	public static Vector2 operator *(float lhs, Vector2 rhs)
	{
		return new Vector2(lhs * rhs.x, lhs * rhs.y);
	}

	public static Vector2 operator /(Vector2 lhs, Vector2 rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector2(lhs.x / rhs.x, lhs.y / rhs.y);
	}

	public static Vector2 operator /(Vector2 lhs, float rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector2(lhs.x / rhs, lhs.y / rhs);
	}

	public static Vector2 operator /(float lhs, Vector2 rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector2(lhs / rhs.x, lhs / rhs.y);
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

				default:
					throw new IndexOutOfRangeException();
			}
		}
	}

	#endregion // OPERATORS

	public bool Equals(Vector2 other)
	{
		return x.Equals(other.x) && y.Equals(other.y);
	}

	public override bool Equals(object obj)
	{
		return obj is Vector2 other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(x, y);
	}
}