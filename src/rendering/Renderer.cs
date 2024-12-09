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
    private int elementBufferObject;
    private Shader shader;
    private Texture texture;
    
    // Triangle vertices
    private float[] vertices = 
    { 
         //   Position - Texture coordinates
         0.5f,  0.5f, 0.0f, 2.0f, 2.0f,  // Top-right
         0.5f, -0.5f, 0.0f, 2.0f, 0.0f,  // Bottom-right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,  // Bottom-left
        -0.5f,  0.5f, 0.0f, 0.0f, 2.0f   // Top-left
    };

    // Triangle indices
    private uint[] indices =
    {
        0, 1, 3, // First triangle
        1, 2, 3  // Second triangle
    };
    
    public Renderer(int width, int height, string title) :
        base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        // The color of which we should clear with
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        // Create the VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        
        // Create the VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
        
        // Create the EBO
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);
        
        // Initialize the shader
        shader = new Shader("resources/shaders/shader.vert", "resources/shaders/shader.frag");
        shader.Use();
        
        // Set up the vertex locations
        int vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray((uint)vertexLocation);
        GL.VertexAttribPointer((uint)vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        
        // Set up the texture coordinates
        int texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray((uint)texCoordLocation);
        GL.VertexAttribPointer((uint)texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        
        // Initialize the missing texture
        texture = Texture.LoadFromFile("resources/textures/dev/missing_texture.png");
        texture.Use(TextureUnit.Texture0);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        
        // Whenever we render, OnRender should be invoked
        OnRender?.Invoke();

        // Clear the buffer
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Bind the vertices
        GL.BindVertexArray(vertexArrayObject);
        
        // Use the texture
        texture.Use(TextureUnit.Texture0);
        
        // Use the shader
        shader.Use();
        
        // Draw the triangles
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        
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