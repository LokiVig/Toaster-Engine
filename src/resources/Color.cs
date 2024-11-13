namespace DoomNET.Resources;

public struct Color
{
    public Vector3 rgb;
    public float alpha;

    public Color()
    {
        rgb = new Vector3( 255, 255, 255 );
        alpha = 255;
    }

    public Color( Vector3 rgb, float alpha = 255 )
    {
        this.rgb = rgb;
        this.alpha = alpha;
    }

    public Color( float r, float g, float b, float alpha = 255 )
    {
        rgb = new Vector3( r, g, b );
        this.alpha = alpha;
    }
}