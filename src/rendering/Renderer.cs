using System;

using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DoomNET.Rendering;

public class Renderer : GameWindow
{
    public event Action? OnRender;

    private int vertexBufferObject;
    private int vertexArrayObject;
    private Shader shader;
    
    // Triangle vertices
    private float[] vertices =
    {
        -0.5f, -0.5f, 0.0f, // Bottom-left vertex
         0.5f, -0.5f, 0.0f, // Bottom-right vertex
         0.0f,  0.5f, 0.0f  // Top vertex
    };
    
    public Renderer(int width, int height, string title) :
        base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        // The color of which we should clear with
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        // Create the VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
        
        // Create and the VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        // Initialize the shader
        shader = new Shader("resources/shaders/shader.vert", "resources/shaders/shader.frag");
        shader.Use();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Clear the buffer
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Whenever we render, OnRender should be invoked
        OnRender?.Invoke();
        
        // Use the shader
        shader.Use();
        
        // Bind and draw the vertices
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        // Swap the buffers!
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // Close the renderer if ESC is pressed
        // This is a debug thing! We should actually pause the game, later...
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        
        // Clean-up!
        // Clear and remove the VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferObject);
        
        // Dispose of the shader system
        shader.Dispose();
    }
}