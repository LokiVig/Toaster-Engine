using System;
using System.Runtime.InteropServices;

using Toast.Engine.Resources;

namespace Toast.Engine;

public class External
{
    //
    // Renderer.dll
    //

    [DllImport("Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer(Scene scene);

    [DllImport("Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer);

    [DllImport("Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShutdownRenderer(IntPtr renderer);
}