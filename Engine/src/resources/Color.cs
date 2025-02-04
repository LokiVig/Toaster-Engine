using System.Numerics;

namespace Toast.Engine.Resources;

public struct Color
{
    public byte r, g, b, a;

    public Color()
    {
        // By default, this should just be a white color
        this = White;
    }

    public Color( Vector3 rgb, byte a = 255 )
    {
        // Initializes a color dependant on a Vector3
        // Its x, y, and z values defines the R, G and B values of the color
        r = (byte)rgb.X;
        g = (byte)rgb.Y;
        b = (byte)rgb.Z;
        this.a = a;
    }

    public Color( byte r, byte g, byte b, byte a = 255 )
    {
        // Just sets the color's values to the inputs
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    //
    // Specific, pre-defined colors
    //

    public static readonly Color White  = new Color( 255, 255, 255 ); // White color
    public static readonly Color Red    = new Color( 255,   0,   0 ); // Red color
    public static readonly Color Green  = new Color(   0, 255,   0 ); // Green color
    public static readonly Color Blue   = new Color(   0,   0, 255 ); // Blue color
    public static readonly Color Black  = new Color(   0,   0,   0 ); // Black color
    public static readonly Color Yellow = new Color( 255, 255,   0 ); // Yellow color
    public static readonly Color Cyan   = new Color(   0, 255, 255 ); // Cyan color
    public static readonly Color Purple = new Color( 255,   0, 255 ); // Purple color
}