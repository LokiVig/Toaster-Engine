using System;
using System.Runtime.InteropServices;

using Toast.Engine.Resources;

namespace Toast.Engine;

public class External
{
    //
    // Renderer.dll
    //

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern IntPtr Renderer_Initialize(string pszTitle = null);

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Renderer_SetScene( IntPtr pRenderer, Scene pScene );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Renderer_RenderFrame( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Renderer_DrawText( IntPtr pRenderer, string pszText, float x, float y, float scale, float r = 1.0f, float g = 1.0f, float b = 1.0f );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Renderer_Shutdown( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern bool Renderer_ShuttingDown( IntPtr pRenderer );
}