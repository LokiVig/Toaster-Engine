using System;

using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DoomNET.Rendering;

/// <summary>
/// The thing that will actually render things onto a window.
/// </summary>
public class Renderer : GameWindow
{
    public event Action? OnRender;
    
    // DEBUG: Triangles!
    private float[] vertices =
    { 
         0.5f,  0.5f, 0.0f, // Top-right vertex
         0.5f, -0.5f, 0.0f, // Bottom-right vertex
        -0.5f, -0.5f, 0.0f, // Bottom-left vertex
        -0.5f,  0.5f, 0.0f  // Top-left vertex
    };

    // DEBUG: Indices for the triangles!
    private uint[] indices =
    { // Note that we start from 0!
        0, 1, 3, // First triangle
        1, 2, 3  // Second triangle
    };

    private int vertexBufferObject;
    private int vertexArrayObject;
    private int elementBufferObject;
    private Shader shader;
    
    public Renderer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {Size = (width, height), Title = title}) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        // Initialize the VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw); // Static draw is used because our debug triangle will not be moving
        
        // Initialize the VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        
        // Initialize the EBO
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);
        
        // Set up the vertex shader to interpret the VBO data
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        
        // Enable variable 0 in the shader
        GL.EnableVertexAttribArray(0);
                                                                                                                        
        // Load a default shader and enable it
        shader = new Shader("resources/shaders/shader.vert", "resources/shaders/shader.frag");
        shader.Use();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        // Clear the screen
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Everything subscribed to our OnRender event should happen
        // This should really only be DoomNET.Update()
        OnRender?.Invoke();
        
        // Bind the shader
        shader.Use();
        
        // Bind the VAO
        GL.BindVertexArray(vertexArrayObject);
        
        // DEBUG: Draw the debug triangles
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        
        // Swap the buffers and display our changes
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        
        // Check keyboard inputs
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close(); // FIXME: Should open the pause menu!!!
        }
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        
        // Remove the vertex buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
        
        // Unbind the program
        GL.UseProgram(0);
        
        // Delete the shader program
        GL.DeleteProgram(shader.handle);

        // Dispose of the shader
        shader.Dispose();
    }
}