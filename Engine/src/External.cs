﻿using System;
using System.Runtime.InteropServices;

using Toast.Engine.Resources;

namespace Toast.Engine;

public class External
{
    //
    // Renderer.dll
    //
    
    [DllImport("Renderer", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer();

    [DllImport("Renderer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer);

    [DllImport("Renderer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderText(IntPtr renderer, string text, int x, int y);
    
    [DllImport("Renderer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShutdownRenderer(IntPtr renderer);
    
    [DllImport("Renderer", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool RendererShuttingDown(IntPtr renderer);
}