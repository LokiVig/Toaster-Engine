using SDL2;

using System;

using DoomNET.Resources;

namespace DoomNET.Rendering;

public class Rasterizer
{
    private IntPtr renderer;

    public Rasterizer( IntPtr renderer )
    {
        this.renderer = renderer;
    }

    /// <summary>
    /// Rasterization-based triangle rendering
    /// </summary>
    public void DrawTriangle( Triangle triangle )
    {
        Camera cam = DoomNET.file?.GetPlayer().camera;

        // Project 3D vertices to 2D screen space
        Vector2 p0 = cam.Project( triangle.v0.position );
        Vector2 p1 = cam.Project( triangle.v1.position );
        Vector2 p2 = cam.Project( triangle.v2.position );

        // Sort the vertices by Y coordinate
        if ( p1.y < p0.y )
        {
            Swap( ref p0, ref p1 );
        }

        if ( p2.y < p1.y )
        {
            Swap( ref p1, ref p2 );
        }

        if ( p1.y < p0.y )
        {
            Swap( ref p0, ref p1 );
        }

        // Draw the triangle in two halves if needed
        if ( p1.y == p2.y )
        {
            DrawBottomTriangle( p0, p1, p2 );
        }
        else if ( p0.y == p1.y )
        {
            DrawUpperTriangle( p0, p1, p2 );
        }
        else
        {
            // Find the intersection point on the longer side
            float alphaSplit = ( p1.y - p0.y ) / ( p2.y - p0.y );
            Vector2 splitVertex = p0 + ( p2 - p0 ) * alphaSplit;
            DrawBottomTriangle( p0, p1, splitVertex );
            DrawUpperTriangle( p1, splitVertex, p2 );
        }
    }

    private void DrawBottomTriangle( Vector2 v0, Vector2 v1, Vector2 v2 )
    {
        float invSlope1 = ( v1.x - v0.x ) / ( v1.y - v0.y );
        float invSlope2 = ( v2.x - v0.x ) / ( v2.y - v0.y );

        float curX1 = v0.x;
        float curX2 = v0.x;

        for ( float y = v0.y; y <= v1.y; y++ )
        {
            DrawHorizontalLine( (int)curX1, (int)curX2, (int)y, new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 } );
            curX1 += invSlope1;
            curX2 += invSlope2;
        }
    }

    private void DrawUpperTriangle( Vector2 v0, Vector2 v1, Vector2 v2 )
    {
        float invSlope1 = ( v2.x - v0.x ) / ( v2.y - v0.y );
        float invSlope2 = ( v2.x - v1.x ) / ( v2.y - v1.y );

        float curX1 = v2.x;
        float curX2 = v2.x;

        for ( float y = v2.y; y >= v0.y; y-- )
        {
            DrawHorizontalLine( (int)curX1, (int)curX2, (int)y, new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 } );
            curX1 -= invSlope1;
            curX2 -= invSlope2;
        }
    }

    private void DrawHorizontalLine( int xStart, int xEnd, int y, SDL.SDL_Color color )
    {
        for ( int x = xStart; x <= xEnd; x++ )
        {
            SDL.SDL_SetRenderDrawColor( renderer, color.r, color.g, color.b, color.a );
            SDL.SDL_RenderDrawPoint( renderer, x, y );
        }
    }

    /// <summary>
    /// Swap two Vector2 values between eachother (<paramref name="source"/> = <paramref name="dest"/> && <paramref name="dest"/> = <paramref name="source"/>)
    /// </summary>
    private void Swap( ref Vector2 source, ref Vector2 dest )
    {
        Vector2 temp = source;
        source = dest;
        dest = temp;
    }
}