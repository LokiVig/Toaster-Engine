using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using DoomNET.Entities;
using DoomNET.Resources;

using MouseState = OpenTK.Windowing.GraphicsLibraryFramework.MouseState;

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
        //Position      Texture coordinates
         0.5f,  0.5f, 0.0f, 2.0f, 2.0f, // Top-right
         0.5f, -0.5f, 0.0f, 2.0f, 0.0f, // Bottom-right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // Bottom-left
        -0.5f,  0.5f, 0.0f, 0.0f, 2.0f  // Top-left
    };

    // DEBUG: Indices for the triangles!
    private readonly uint[] indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private int elementBufferObject;
    private int vertexBufferObject;
    private int vertexArrayObject;
    private Shader shader;
    private Texture texture;
    private bool firstMove = true;
    private Vector2 lastPos;
    
    public Renderer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {Size = (width, height), Title = title}) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        
        // Initialize the VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        
        // Initialize the VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw); // Static draw is used because our debug triangle will not be moving
        
        // Initialize the EBO
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsage.StaticDraw);
        
        // Load a default shader and enable it
        shader = new Shader("resources/shaders/shader.vert", "resources/shaders/shader.frag");
        shader.Use();
        
        // Set up the vertex location
        int vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray((uint)vertexLocation);
        GL.VertexAttribPointer((uint)vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        
        // Set up the texture coordinate location
        int texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray((uint)texCoordLocation);
        GL.VertexAttribPointer((uint)texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        
        // Enable variable 0 in the shader
        GL.EnableVertexAttribArray(0);

        texture = Texture.LoadFromFile("resources/textures/dev/missing_texture.png");
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
        
        // Bind the VAO
        GL.BindVertexArray(vertexArrayObject);
        
        // Bind the texture
        texture.Use(TextureUnit.Texture0);
        
        // Bind the shader
        shader.Use();
        
        // DEBUG: Draw the debug triangles
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        
        // Swap the buffers and display our changes
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
        {
            return;
        }
        
        KeyboardState input = KeyboardState;
        Player player = DoomNET.currentScene.GetPlayer();
        Camera camera = player.camera;
        float dt = DoomNET.deltaTime;
        float moveSpeed = 15.0f;
        
        // Check keyboard inputs
        if (input.IsKeyDown(Keys.W))
        {
            player.SetVelocity(new Vector3(15, player.GetVelocity().y, player.GetVelocity().z));
        }

        if (input.IsKeyDown(Keys.S))
        {
            player.SetVelocity(new Vector3(-15, player.GetVelocity().y, player.GetVelocity().z));
        }

        if (input.IsKeyDown(Keys.A))
        {
            player.SetVelocity(new Vector3(player.GetVelocity().x, -15, player.GetVelocity().z));
        }

        if (input.IsKeyDown(Keys.D))
        {
            player.SetVelocity(new Vector3(player.GetVelocity().x, 15, player.GetVelocity().z));
        }

        if (input.IsKeyDown(Keys.Space))
        {
            player.SetVelocity(new Vector3(player.GetVelocity().x, player.GetVelocity().y, 15));
        }

        if (input.IsKeyDown(Keys.LeftControl))
        {
            player.SetVelocity(new Vector3(player.GetVelocity().x, player.GetVelocity().y, -15));
        }
        
        Console.WriteLine($"Player velocity: {player.GetVelocity()}\nPlayer position: {player.position}");
        
        MouseState mouse = MouseState;

        if (firstMove)
        {
            lastPos = new Vector2(mouse.X, mouse.Y);
            firstMove = false;
        }
        else
        {
            float deltaX = mouse.X - lastPos.x;
            float deltaY = mouse.Y - lastPos.y;
            lastPos = new Vector2(mouse.X, mouse.Y);

            camera.pitch += deltaX * 0.05f;
            camera.yaw -= deltaY * 0.05f;
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