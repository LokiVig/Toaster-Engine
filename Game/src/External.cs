using System;
using System.Runtime.InteropServices;

using DoomNET.Resources;

namespace DoomNET;

public class External
{
    //
    // Doom.NET.Renderer.dll
    //

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer(Scene scene);

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer);

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShutdownRenderer(IntPtr renderer);
}