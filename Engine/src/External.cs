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
    public static extern IntPtr CreateRenderer(string pszTitle = null);

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void SetScene( IntPtr pRenderer, Scene pScene );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void RenderFrame( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void RenderText( IntPtr pRenderer, string pszText, int x, int y );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void ShutdownRenderer( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern bool RendererShuttingDown( IntPtr pRenderer );
}