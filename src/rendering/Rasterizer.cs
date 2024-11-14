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

}