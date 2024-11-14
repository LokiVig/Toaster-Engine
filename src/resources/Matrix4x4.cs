using System;

namespace DoomNET.Resources;

public struct Matrix4x4
{
    public float[,] matrix = new float[ 4, 4 ];

    public Matrix4x4()
    {
        SetIdentity();
    }

    public Matrix4x4( Vector4 vec4 )
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i == j)
                {
                    matrix[ i, j ] = vec4[ i ];
                }
            }
        }
    }

    public Matrix4x4( Matrix4x4 other )
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrix[ i, j ] = other.matrix[ i, j ];
            }
        }
    }

    public Matrix4x4 CreatePerspective( float fov, float aspectRatio, float zNear, float zFar )
    {
        float tangent = (float)Math.Tan( fov / 2 * ( Math.PI / 180 ) );
        float height = zNear * tangent;
        float width = height * aspectRatio;

        return CreateFrustrum( -width, width, -height, height, zNear, zFar );
    }

    public Matrix4x4 CreateFrustrum( float left, float right, float bottom, float top, float zNear, float zFar )
    {
        float maxView = 2.0f * zNear;
        float width = right - left;
        float height = top - bottom;
        float zRange = zFar - zNear;
        Matrix4x4 frustrum = Identity;

        frustrum.matrix[ 0, 0 ] = maxView / width;
        frustrum.matrix[ 1, 1 ] = maxView / height;
        frustrum.matrix[ 2, 0 ] = ( right + left ) / width;
        frustrum.matrix[ 2, 1 ] = ( top + bottom ) / height;
        frustrum.matrix[ 2, 2 ] = ( -zFar - zNear ) / zRange;
        frustrum.matrix[ 2, 3 ] = -1.0f;
        frustrum.matrix[ 3, 2 ] = ( -maxView * zFar ) / zRange;
        frustrum.matrix[ 3, 3 ] = 0.0f;

        return frustrum;
    }

    public Matrix4x4 CreateInverse()
    {
        float cof0 = GetMinor( matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 1, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof1 = GetMinor( matrix[ 0, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof2 = GetMinor( matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 3, 3 ] );
        float cof3 = GetMinor( matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 2, 3 ] );

        float det = matrix[ 0, 0 ] * cof0 - matrix[ 1, 0 ] * cof1 + matrix[ 2, 0 ] * cof2 - matrix[ 3, 0 ] * cof3;

        if (Math.Abs( det ) <= 0.00001f)
        {
            return Identity;
        }

        float cof4 = GetMinor( matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 1, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof5 = GetMinor( matrix[ 0, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 0, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof6 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 3, 0 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 3, 3 ] );
        float cof7 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 2, 3 ] );

        float cof8 = GetMinor( matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 1, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof9 = GetMinor( matrix[ 0, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 0, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 0, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] );
        float cof10 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 3, 0 ], matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 3, 1 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 3, 3 ] );
        float cof11 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 2, 3 ] );

        float cof12 = GetMinor( matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ] );
        float cof13 = GetMinor( matrix[ 0, 0 ], matrix[ 2, 0 ], matrix[ 3, 0 ], matrix[ 0, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ] );
        float cof14 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 3, 0 ], matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 3, 2 ] );
        float cof15 = GetMinor( matrix[ 0, 0 ], matrix[ 1, 0 ], matrix[ 2, 0 ], matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 2, 2 ] );

        float detInv = 1.0f / det;
        Matrix4x4 inverse = Identity;

        inverse.matrix[ 0, 0 ] = detInv * cof0;
        inverse.matrix[ 1, 0 ] = -detInv * cof4;
        inverse.matrix[ 2, 0 ] = detInv * cof8;
        inverse.matrix[ 3, 0 ] = -detInv * cof12;
        inverse.matrix[ 0, 1 ] = -detInv * cof1;
        inverse.matrix[ 1, 1 ] = detInv * cof5;
        inverse.matrix[ 2, 1 ] = -detInv * cof9;
        inverse.matrix[ 3, 1 ] = detInv * cof13;
        inverse.matrix[ 0, 2 ] = detInv * cof2;
        inverse.matrix[ 1, 2 ] = -detInv * cof6;
        inverse.matrix[ 2, 2 ] = detInv * cof10;
        inverse.matrix[ 3, 2 ] = -detInv * cof14;
        inverse.matrix[ 0, 3 ] = -detInv * cof3;
        inverse.matrix[ 1, 3 ] = detInv * cof7;
        inverse.matrix[ 2, 3 ] = -detInv * cof11;
        inverse.matrix[ 3, 3 ] = detInv * cof15;

        return inverse;
    }

    public Matrix4x4 CreateTranspose()
    {
        Matrix4x4 transpose = Identity;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                transpose.matrix[ i, j ] = matrix[ i, j ];
            }
        }

        return transpose;
    }

    public Matrix4x4 CreateTranslation( Vector3 vec )
    {
        return CreateTranslation( vec.x, vec.y, vec.z );
    }

    public Matrix4x4 CreateTranslation( float x, float y, float z )
    {
        Matrix4x4 translate = Identity;
        translate.matrix[ 0, 3 ] = x;
        translate.matrix[ 1, 3 ] = y;
        translate.matrix[ 2, 3 ] = z;

        return translate;
    }

    public Matrix4x4 CreateView( float eyeX, float eyeY, float eyeZ, float lookX, float lookY, float lookZ, float upX, float upY, float upZ )
    {
        Vector3 eye = new Vector3( eyeX, eyeY, eyeZ );
        Vector3 look = new Vector3( lookX, lookY, lookZ );
        Vector3 up = new Vector3( upX, upY, upZ );
        Vector3 eyeLook = eye - look;
        Vector3 n = eyeLook.Normalized();
        Vector3 upXn = up.Cross( n );
        Vector3 u = upXn / upXn.Magnitude();
        Vector3 v = n.Cross( u );
        Matrix4x4 view = Identity;

        view.matrix[ 0, 0 ] = u.x;
        view.matrix[ 0, 1 ] = u.y;
        view.matrix[ 0, 2 ] = u.z;
        view.matrix[ 0, 3 ] = -eye.DistanceTo( u );
        view.matrix[ 1, 0 ] = v.x;
        view.matrix[ 1, 1 ] = v.y;
        view.matrix[ 1, 2 ] = v.z;
        view.matrix[ 1, 3 ] = -eye.DistanceTo( v );
        view.matrix[ 2, 0 ] = n.x;
        view.matrix[ 2, 1 ] = n.y;
        view.matrix[ 2, 2 ] = n.z;
        view.matrix[ 2, 3 ] = -eye.DistanceTo( n );

        return view;
    }

    public Matrix4x4 CreateRotation( float xAngle, float yAngle, float zAngle )
    {
        Matrix4x4 xRot = Identity, yRot = Identity, zRot = Identity;
        float xRad = (float)( xAngle * Math.PI / 180.0f );
        float yRad = (float)( yAngle * Math.PI / 180.0f );
        float zRad = (float)( zAngle * Math.PI / 180.0f );

        xRot.matrix[ 1, 1 ] = (float)Math.Cos( xRad );
        xRot.matrix[ 1, 2 ] = -(float)Math.Sin( xRad );
        xRot.matrix[ 2, 1 ] = (float)Math.Sin( xRad );
        xRot.matrix[ 2, 2 ] = (float)Math.Cos( xRad );

        yRot.matrix[ 0, 0 ] = (float)Math.Cos( yRad );
        yRot.matrix[ 0, 2 ] = -(float)Math.Sin( yRad );
        yRot.matrix[ 2, 0 ] = (float)Math.Sin( yRad );
        yRot.matrix[ 2, 2 ] = (float)Math.Cos( yRad );

        zRot.matrix[ 0, 0 ] = (float)Math.Cos( zRad );
        zRot.matrix[ 0, 1 ] = -(float)Math.Sin( zRad );
        zRot.matrix[ 1, 0 ] = (float)Math.Sin( zRad );
        zRot.matrix[ 1, 1 ] = (float)Math.Cos( zRad );

        return xRot * yRot * zRot;
    }

    public Matrix4x4 CreateScale( float xScale, float yScale, float zScale )
    {
        Matrix4x4 scale = Identity;
        scale.matrix[ 0, 0 ] = xScale;
        scale.matrix[ 1, 1 ] = yScale;
        scale.matrix[ 2, 2 ] = zScale;

        return scale;
    }

    public Vector3 ConvertToScreen( Vector3 vector, float width, float height )
    {
        float widthHalf = width / 2.0f;
        float heightHalf = height / 2.0f;

        return new Vector3( ( ( vector.x / 5.0f ) + 1 ) * widthHalf, height - ( ( vector.y / 5.0f ) + 1 ) * heightHalf, vector.z );
    }

    public Vector3 ScreenToView( Vector3 vector, float width, float height )
    {
        float widthHalf = width / 2.0f;
        float heightHalf = height / 2.0f;

        return new Vector3( ( ( vector.x / widthHalf ) - 1 ) * 5.0f, ( ( ( ( vector.y - height ) * -1 ) / heightHalf ) - 1 ) * 5.0f, vector.z );
    }

    public void SetNull()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                matrix[ row, col ] = 0;
            }
        }
    }

    public static Matrix4x4 Identity => new Matrix4x4();

    public void SetIdentity()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrix[ i, j ] = i == j ? 1 : 0;
            }
        }
    }

    public float Determinant()
    {
        return matrix[ 0, 0 ] * GetMinor( matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 1, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] )
             - matrix[ 1, 0 ] * GetMinor( matrix[ 0, 1 ], matrix[ 2, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 2, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 2, 3 ], matrix[ 3, 3 ] )
             + matrix[ 2, 0 ] * GetMinor( matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 3, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 3, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 3, 3 ] )
             - matrix[ 3, 0 ] * GetMinor( matrix[ 0, 1 ], matrix[ 1, 1 ], matrix[ 2, 1 ], matrix[ 0, 2 ], matrix[ 1, 2 ], matrix[ 2, 2 ], matrix[ 0, 3 ], matrix[ 1, 3 ], matrix[ 2, 3 ] );
    }

    public float GetMinor( float m0, float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8 )
    {
        return m0 * ( m4 * m8 - m5 * m7 ) - m1 * ( m3 * m8 - m5 * m6 ) + m2 * ( m3 * m7 - m4 * m6 );
    }

    #region OPERATORS
    public static Matrix4x4 operator *( Matrix4x4 lhs, Matrix4x4 rhs )
    {
        Matrix4x4 ret = new Matrix4x4();

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                float value = 0;

                for (int i = 0; i < 4; i++)
                {
                    value += lhs.matrix[ row, i ] * rhs.matrix[ i, col ];
                }

                ret.matrix[ row, col ] = value;
            }
        }

        return ret;
    }

    public static Vector4 operator *( Matrix4x4 lhs, Vector4 rhs )
    {
        float multVec = 0;
        Vector4 multiply = Vector4.Zero;

        for (int i = 0; i < 4; i++)
        {
            multVec += lhs.matrix[ i, 0 ] * rhs.x;
            multVec += lhs.matrix[ i, 1 ] * rhs.y;
            multVec += lhs.matrix[ i, 2 ] * rhs.z;
            multVec += lhs.matrix[ i, 3 ] * rhs.w;

            multiply[ i ] = multVec;
            multVec = 0;
        }

        return multiply;
    }
    #endregion // OPERATORS
}