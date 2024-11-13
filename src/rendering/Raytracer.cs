using SDL2;

using System;

using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET.Rendering;

public class Raytracer
{
    private IntPtr renderer;
    private Camera camera = DoomNET.file?.GetPlayer().camera;

    public Raytracer( IntPtr renderer )
    {
        this.renderer = renderer;
    }

    /// <summary>
    /// The final result, render everything we've successfully traced
    /// </summary>
    public void Render()
    {
        for (int y = 0; y < DoomNET.windowHeight; y++)
        {
            for (int x = 0; x < DoomNET.windowWidth; x++)
            {
                // Convert screenspace (x, y) to world space ray direction
                Vector3 rayDirection = Vector3.ScreenToWorldDirection( x, y, camera );

                // Trace the ray
                if (TraceRay( camera.position, rayDirection, out float distance ))
                {
                    // Calculate color based on distance (e.g., closer objects are brighter)
                    byte colorIntensity = (byte)( 255 - Math.Min( distance * 10, 255 ) );
                    SDL.SDL_SetRenderDrawColor( renderer, colorIntensity, colorIntensity, colorIntensity, 255 );
                }
                else
                {
                    SDL.SDL_SetRenderDrawColor( renderer, 135, 206, 235, 255 );
                }

                // Draw the pixel
                SDL.SDL_RenderDrawPoint( renderer, x, y );
            }
        }

        SDL.SDL_RenderPresent( renderer );
    }

    private bool TraceRay( Vector3 origin, Vector3 direction, out float distance )
    {
        distance = float.MaxValue;
        bool hit = false;

        foreach (Entity entity in DoomNET.file.GetEntities())
        {
            // Skip over invisible entities
            if (!entity.visible)
            {
                continue;
            }

            if (entity.bbox.IntersectingWith( origin, direction, out float entityDistance ) && entityDistance < distance)
            {
                hit = true;
                distance = entityDistance;
            }
        }

        return hit;
    }
}