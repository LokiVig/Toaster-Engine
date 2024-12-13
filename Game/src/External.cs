using System;
using System.Runtime.InteropServices;

using DoomNET.Resources;

namespace DoomNET;

public class External
{
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

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateRenderer();

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void StartRenderer(IntPtr renderer, IntPtr hInstance, int nCmdShow);

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RenderFrame(IntPtr renderer, Scene scene);

    [DllImport("Doom.NET.Renderer.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyRenderer(IntPtr renderer);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax,
        uint wRemoveMsg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool TranslateMessage(ref MSG msg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DispatchMessage(ref MSG msg);
}