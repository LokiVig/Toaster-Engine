using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

using Toast.Engine.Resources;
using Toast.Engine.Entities;

namespace Toast.Engine.Rendering;

public class Renderer
{
    public static Sdl2Window window;

    private static GraphicsDevice graphicsDevice;

    private static ResourceFactory resourceFactory;
    private static CommandList commandList;
    
    /// <summary>
    /// Initializes a DirectX 11 rendered window with a specified title.
    /// </summary>
    /// <param name="title">The title of the window we wish to open.</param>
    public static void Initialize(string title)
    {
        // Create a window using Veldrid's StartupUtilities
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = title,
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
            SwapchainDepthFormat = PixelFormat.R16_UNorm,
        };

        graphicsDevice = VeldridStartup.CreateGraphicsDevice( window, options, GraphicsBackend.Direct3D11 );

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
    public static void RenderFrame(Scene scene = null)
    {
        // Handle processing events
        Sdl2Events.ProcessEvents();

        // Begin rendering
        commandList.Begin();
        commandList.SetFramebuffer( graphicsDevice.SwapchainFramebuffer );
        commandList.ClearColorTarget( 0, RgbaFloat.CornflowerBlue );
        commandList.End();

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

        // Submit commands and present the frame
        graphicsDevice.SubmitCommands( commandList );
        graphicsDevice.SwapBuffers();
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
        commandList.Dispose();
        graphicsDevice.Dispose();
    }
}