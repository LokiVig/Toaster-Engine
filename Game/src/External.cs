using System;
using System.Runtime.InteropServices;

using DoomNET.Resources;

namespace DoomNET;

public class External
{
    private const string RENDERER_DLL = "Doom.NET.Renderer.dll";
    
    //
    // Global
    //
    
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int pt_x;
        public int pt_y;
    }
    
    //
    // Doom.NET.Renderer.dll
    //

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer(Scene scene);

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer);

    [DllImport(RENDERER_DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShutdownRenderer(IntPtr renderer);
    
    //
    // user32.dll
    //

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax,
        uint wRemoveMsg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool TranslateMessage(ref MSG msg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DispatchMessage(ref MSG msg);
}