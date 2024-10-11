using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System;
using System.Windows;
using System.Threading;
using System.Diagnostics;

using DoomNET.WTF;
using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET;

public class DoomNET : GameWindow
{
    public static event Action OnUpdate;
    public static WTFFile file;

    public static float deltaTime;

    private float[] vertices =
    {
         0.5f,  0.5f, 0.0f,  // Top right
         0.5f, -0.5f, 0.0f,  // Bottom right
        -0.5f, -0.5f, 0.0f,  // Bottom left
        -0.5f,  0.5f, 0.0f   // Top left
    };

    private uint[] indices =
    {
        0, 1, 3, // First triangle
        1, 2, 3, // Second triangle
    };

    private Shader shader;

    private int elementBufferObject;
    private int vertexBufferObject;
    private int vertexArrayObject;

    public DoomNET(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // DEBUG! Load a test map
        //file = WTFLoader.LoadFile("maps/test.wtf");

        file = new();

        TestNPC npc = new(new Vector3(-50.3f, 23.9f, 7.6f), new BBox(new Vector3(16.0f, 16.0f, 64.0f), new Vector3(-16.0f, -16.0f, 0.0f)));
        npc.SetVelocity(new Vector3(-0.5f, -25.0f, 0f));

        TriggerBrush trigger = new();
        trigger.SetBBox(new BBox(new Vector3(-15.0f, -15.0f, -15.0f), new Vector3(15.0f, 15.0f, 15.0f)));
        trigger.triggerType = TriggerType.Once;
        trigger.triggerBy = TriggerBy.Players;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.targetEvent = EntityEvent.TakeDamage;
        trigger.targetEntity = npc;
        trigger.fValue = 500.0f;

        Player player = new(new Vector3(50.0f, 50.0f, 50.0f), new BBox(new Vector3(32.0f, 32.0f, 64.0f), new Vector3(-32.0f, -32.0f, 0.0f)));
        player.SetVelocity(new Vector3(1.5f, 0.25f, 0f));

        file.AddEntity(player);
        file.AddEntity(npc);
        file.AddEntity(trigger);

        WTFSaver.SaveFile("maps/test.wtf", file);

        Ray.Trace(player, npc, out object hitObject, RayIgnore.None, trigger);

        // Initialize a window to render upon
        Run();
    }

    /// <summary>
    /// Effectively the same as <see cref="Initialize"/>
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        shader = new Shader("../../../src/shaders/shader.vert", "../../../src/shaders/shader.frag");
        shader.Use();
    }

    /// <summary>
    /// Things to do every frame, game-wise<br/>
    /// This includes calling every other update function that updates every frame
    /// </summary>
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        Stopwatch watch = Stopwatch.StartNew();

        // Calculate deltaTime
        deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
        watch.Restart();

        // Call the OnUpdate event, so everything else that should update also updates with us
        OnUpdate?.Invoke();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        shader.Use();
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);

        GL.DeleteProgram(shader.handle);

        shader.Dispose();
    }
}