namespace DoomNET.Resources;

public struct Color
{
    public byte r, g, b, a;

    public Color()
    {
        r = g = b = a = 255;
    }

    public Color( Vector3 rgb, byte a = 255 )
    {
        r = (byte)rgb.x;
        g = (byte)rgb.y;
        b = (byte)rgb.z;
        this.a = a;
    }

    public Color( byte r, byte g, byte b, byte a = 255 )
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}