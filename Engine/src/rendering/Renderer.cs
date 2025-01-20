using System.Numerics;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

using ImGuiNET;

using Toast.Engine.Resources;
using Toast.Engine.Entities;

namespace Toast.Engine.Rendering;

public class Renderer
{
    public static Sdl2Window window;
    public static GraphicsDevice graphicsDevice;

    private static ResourceFactory resourceFactory;
    private static DeviceBuffer vertexBuffer;
    private static DeviceBuffer indexBuffer;
    private static Shader[] shaders;
    private static Pipeline pipeline;
    private static CommandList commandList;

    private static ImGuiController controller;

    /// <summary>
    /// Initializes a DirectX 11 rendered window with a specified title.
    /// </summary>
    /// <param name="title">The title of the window we wish to open.</param>
    public static void Initialize( string title )
    {
        // Create a window using Veldrid's StartupUtilities
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = title
        };

        window = VeldridStartup.CreateWindow( ref windowCI );

        // Create a graphics device using Direct3D 11
        GraphicsDeviceOptions options = new GraphicsDeviceOptions()
        {
#if RELEASE
            Debug = false,
#else
            Debug = true,
#endif // RELEASE
            PreferDepthRangeZeroToOne = true,
            SwapchainDepthFormat = PixelFormat.R16_UNorm,
        };

        graphicsDevice = VeldridStartup.CreateGraphicsDevice( window, options, GraphicsBackend.Direct3D11 );

        // The things to do when the window is resized
        window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize( (uint)window.Width, (uint)window.Height );
            controller.WindowResized( window.Width, window.Height );
        };

        // Create an ImGui context
        ImGui.CreateContext();

        // Initializes a new ImGui controller
        controller = new ImGuiController( graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height );

        // Create our other, default resources
        CreateResources();
    }

    /// <summary>
    /// Initializes all of our resources needed to render things.
    /// </summary>
    private static void CreateResources()
    {
        // Define our resource factory variable
        // This is to shorten down the later variable definitions
        resourceFactory = graphicsDevice.ResourceFactory;

        // Define our command list variable
        commandList = resourceFactory.CreateCommandList();
    }

    /// <summary>
    /// Renders a frame to the window.
    /// </summary>
    public static void RenderFrame( Scene scene = null )
    {
        // Pump window events
        InputSnapshot inputSnapshot = window.PumpEvents();

        // Begin commands
        commandList.Begin();

        // If we have a scene to render from
        if ( scene != null )
        {
            foreach ( Entity entity in scene.GetEntities() )
            {
                DrawEntity( entity );
            }

            foreach ( Brush brush in scene.GetBrushes() )
            {
                DrawBrush( brush );
            }
        }

        // Render directly to the window
        commandList.SetFramebuffer( graphicsDevice.MainSwapchain.Framebuffer );
        commandList.ClearColorTarget( 0, RgbaFloat.CornflowerBlue );

        // Render the ImGui controller
        controller.Render(graphicsDevice, commandList);

        // Update the ImGui controller
        controller.Update( EngineManager.deltaTime, inputSnapshot );

        // End commands
        commandList.End();

        // Submit commands and present the frame
        graphicsDevice.SubmitCommands( commandList );
        graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);

        // Handle processing events
        //Sdl2Events.ProcessEvents();
    }

    /// <summary>
    /// Draws 2D text on the screen.
    /// </summary>
    /// <param name="text">The text we wish to draw.</param>
    /// <param name="x">The X position on the screen of which where we want the text to render.</param>
    /// <param name="y">The Y position on the screen of which where we want the text to render.</param>
    public static void DrawText( string text, float x, float y )
    {

    }

    /// <summary>
    /// Renders a specified brush.
    /// </summary>
    /// <param name="brush">The brush we wish to render.</param>
    private static void DrawBrush( Brush brush )
    {

    }

    /// <summary>
    /// Renders a specified entity.
    /// </summary>
    /// <param name="entity">The entity we wish to render.</param>
    private static void DrawEntity( Entity entity )
    {

    }

    /// <summary>
    /// Checks if the window is shutting down.
    /// </summary>
    /// <returns><see langword="true"/> if the window doesn't exist, aka, is shutting down.</returns>
    public static bool ShuttingDown()
    {
        return !window.Exists;
    }

    /// <summary>
    /// Shuts down the window and its rendering variable stuffs.
    /// </summary>
    public static void Shutdown()
    {
        // TODO: Remove all '?' after having actually written functionality for the variables

        pipeline?.Dispose();

        if ( shaders != null )
        {
            foreach ( Shader shader in shaders )
            {
                shader.Dispose();
            }
        }

        commandList?.Dispose();
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
        graphicsDevice?.Dispose();
    }
}