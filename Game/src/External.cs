using System;
using System.Runtime.InteropServices;

using DoomNET.Resources;

namespace DoomNET;

public class External
{
    //
    // Local
    //
    
    private const string RENDERER_DLL = "DoomNET.Renderer.dll";
    
    //
    // DoomNET.Renderer.dll
    //

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer(Scene scene);

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer);

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShutdownRenderer(IntPtr renderer);
}