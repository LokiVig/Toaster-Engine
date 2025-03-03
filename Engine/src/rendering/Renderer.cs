using System.Collections.Generic;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

using ImGuiNET;

using Toast.Engine.Entities;
using Toast.Engine.Resources;
using Toast.Engine.Resources.Console;

namespace Toast.Engine.Rendering;

public class Renderer
{
    // A window should not be able to be less than these two values!
    public const int WINDOW_MIN_WIDTH = 800;
    public const int WINDOW_MIN_HEIGHT = 600;

    public static Sdl2Window window;
    public static GraphicsDevice graphicsDevice;

    private static CommandList commandList;

    private static ImGuiController controller;

    /// <summary>
    /// Initializes a OpenGL rendered window with a specified title.
    /// </summary>
    /// <param name="title">The title of the window we wish to open.</param>
    public static void Initialize( string title )
    {
        // Initialize default renderer console commands
        CreateConsoleCommands();

        SDL_DisplayMode mode;

        // Why did they gotta have it be a pointer :(
        unsafe
        {
            Sdl2Native.SDL_GetCurrentDisplayMode( 0, &mode );
        }

        // Create a window using Veldrid's StartupUtilities
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = title
        };

        windowCI.X = ( windowCI.WindowWidth - mode.w ) / 2;
        windowCI.Y = ( windowCI.WindowHeight - mode.h ) / 2;

        // Create our window
        window = VeldridStartup.CreateWindow( ref windowCI );

        // Create a graphics device using OpenGL
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

        graphicsDevice = VeldridStartup.CreateGraphicsDevice( window, options, GraphicsBackend.OpenGL );

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

        // Create our default resources
        CreateResources();
    }

    /// <summary>
    /// Creates the resources required for rendering.
    /// </summary>
    private static void CreateResources()
    {
        ResourceFactory factory = graphicsDevice.ResourceFactory;

        commandList = factory.CreateCommandList();
    }

    /// <summary>
    /// A quick method to define and initialize console commands related to the renderer.
    /// </summary>
    private static void CreateConsoleCommands()
    {
        // R(enderer) VSync
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "r_vsync",
            description = "Toggles VSync on the renderer.",

            onCall = ToggleVSync
        } );

        // R(enderer) Windowed
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "r_windowed",
            description = "Sets the renderer's window to be windowed.",

            onCall = SetWindowStateToWindowed
        } );

        // R(enderer) Fullscreen
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "r_fullscreen",
            description = "Sets the renderer's window to be fullscreened.",

            onCall = SetWindowStateToFullscreen
        } );

        // R(enderer) Borderless
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "r_borderless",
            description = "Sets the renderer's window to be borderless fullscreened.",

            onCall = SetWindowStateToBorderless
        } );

        // R(enderer) Window Resolution
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "r_resolution",
            description = "Changes the resolution of which the renderer window is displayed at.",

            onArgsCall = ChangeWindowResolution
        } );
    }

    /// <summary>
    /// Sets the renderer's window title to <paramref name="newTitle"/>.
    /// </summary>
    /// <param name="newTitle">The window's new title.</param>
    public static void SetWindowTitle( string newTitle )
    {
        window.Title = $"Toaster Engine ({EngineManager.VERSION}) - {newTitle}";
    }

    /// <summary>
    /// Toggles VSync through the console
    /// </summary>
    private static void ToggleVSync()
    {
        // Toggle the VSync option
        graphicsDevice.MainSwapchain.SyncToVerticalBlank = !graphicsDevice.MainSwapchain.SyncToVerticalBlank;

        // Log its current status
        Log.Info( $"VSync is now {( graphicsDevice.MainSwapchain.SyncToVerticalBlank ? "enabled" : "disabled" )}.", true );
    }

    /// <summary>
    /// Changes the window's current state to windowed.
    /// </summary>
    private static void SetWindowStateToWindowed()
    {
        window.WindowState = WindowState.Normal;
    }

    /// <summary>
    /// Changes the window's current state to fullscreen.
    /// </summary>
    private static void SetWindowStateToFullscreen()
    {
        window.WindowState = WindowState.FullScreen;
    }

    /// <summary>
    /// Changes the window's current state to borderless.
    /// </summary>
    private static void SetWindowStateToBorderless()
    {
        window.WindowState = WindowState.BorderlessFullScreen;
    }

    /// <summary>
    /// Changes the window's current resolution through the console.
    /// </summary>
    private static void ChangeWindowResolution( List<object> args )
    {
        // If we have more than 2 arguments...
        if ( ( args.Count - 1 ) > 2 || ( args.Count - 1 ) < 2 )
        {
            Log.Warning( "Invalid amount of arguments! You need at least, and at most, 2 arguments, one for the height value and one for the width value!" );
            return;
        }

        // Get the width...
        if ( !int.TryParse( (string)args[1], out int width ) )
        {
            Log.Warning( "First argument was an invalid integer value!" );
            return;
        }

        // Get the height...
        if ( !int.TryParse( (string)args[2], out int height ) )
        {
            Log.Warning( "Second argument was an invalid integer value!" );
            return;
        }

        // Make sure width is more than the minimum allowed width
        if ( width < WINDOW_MIN_WIDTH )
        {
            Log.Warning( $"Couldn't set window width to {width} as it's less than the minimum acceptable width ({WINDOW_MIN_WIDTH})!" );
            width = WINDOW_MIN_WIDTH;
        }

        // Make sure height is more than the minimum allowed height
        if ( height < WINDOW_MIN_HEIGHT )
        {
            Log.Warning( $"Couldn't set window height to {height} as it's less than the minimum acceptable height ({WINDOW_MIN_HEIGHT})!" );
            height = WINDOW_MIN_HEIGHT;
        }

        // Set the window's width and height appropriately!
        window.Width = width;
        window.Height = height;
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

        // Render directly to the window
        commandList.SetFramebuffer( graphicsDevice.MainSwapchain.Framebuffer );
        commandList.ClearColorTarget( 0, RgbaFloat.CornflowerBlue );

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

        // Render the ImGui controller
        controller.Render( graphicsDevice, commandList );

        // Update the ImGui controller
        controller.Update( EngineManager.deltaTime, inputSnapshot );

        // End commands
        commandList.End();

        // Submit commands and present the frame
        graphicsDevice.SubmitCommands( commandList );
        graphicsDevice.SwapBuffers( graphicsDevice.MainSwapchain );

        // Handle processing events
        Sdl2Events.ProcessEvents();
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