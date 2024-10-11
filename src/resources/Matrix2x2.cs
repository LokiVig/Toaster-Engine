using System.Collections.Generic;

namespace DoomNET.Resources;

public class Matrix2x2
{
    public int[,] values = new int[2, 2];

    public Matrix2x2()
    {
        values = new int[2, 2];
    }

    public Matrix2x2(int[,] values)
    {
        this.values = values;
    }

    public Matrix2x2(int[] values1, int[] values2)
    {
        values = new int[,] { { values1[0], values1[1] }, { values2[0], values2[1] } };
    }

    public Matrix2x2(int value1, int value2, int value3, int value4)
    {
        values = new int[,] { { value1, value2 }, { value3, value4 } };
    }

    public static Matrix2x2 operator +(Matrix2x2 lhs, Matrix2x2 rhs)
    {
        return new Matrix2x2(lhs.values[0, 0] + rhs.values[0, 0], lhs.values[0, 1] + rhs.values[0, 1], lhs.values[1, 0] + rhs.values[1, 0], lhs.values[1, 1] + rhs.values[1, 1]);
    }

    public static Matrix2x2 operator +(Matrix2x2 lhs, int rhs)
    {
        return new Matrix2x2(lhs.values[0, 0] + rhs, lhs.values[0, 1] + rhs, lhs.values[1, 0] + rhs, lhs.values[1, 1] + rhs);
    }
}