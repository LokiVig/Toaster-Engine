using System;
using System.Runtime.InteropServices;

namespace Toast.Engine.Rendering;

public static class Renderer
{
    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern IntPtr Initialize( string pszTitle );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Update( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern bool ShuttingDown( IntPtr pRenderer );

    [DllImport( "Renderer", CallingConvention = CallingConvention.Cdecl )]
    public static extern void Shutdown( IntPtr pRenderer );
}