using System;
using System.Collections.Generic;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

using ImGuiNET;

using Toast.Engine.Entities;
using Toast.Engine.Resources;
using Toast.Engine.Resources.Console;
using Toast.Engine.Attributes;

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
        // The window info
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
            WindowTitle = title,
            WindowInitialState = WindowState.Normal
        };

        // Places the window in the middle of the screen, dependant on width and height
        // Clamped to 0 for top-left corner, just incase
        windowCI.X = Math.Max( 0, ( windowCI.WindowWidth - mode.w ) / 2 );
        windowCI.Y = Math.Max( 0, ( windowCI.WindowHeight - mode.h ) / 2 );

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

        // Create our graphics device
        graphicsDevice = VeldridStartup.CreateGraphicsDevice( window, options, GraphicsBackend.OpenGL );

        // The things to do when the window is resized
        window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize( (uint)window.Width, (uint)window.Height );
            controller.WindowResized( window.Width, window.Height );

            if ( window.Width < WINDOW_MIN_WIDTH )
            {
                window.Width = WINDOW_MIN_WIDTH;
            }

            if ( window.Height < WINDOW_MIN_HEIGHT )
            {
                window.Height = WINDOW_MIN_HEIGHT;
            }
        };

        // Create an ImGui context
        ImGui.CreateContext();

        // Initializes a new ImGui controller
        controller = new ImGuiController( graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height );

        // Create our default resources
        CreateResources();

        // Load settings
        SetWindowState( EngineManager.settings.WindowState );
        SetVSync( EngineManager.settings.VSyncEnabled );
        SetWindowWidth( EngineManager.settings.WindowResolution.width );
        SetWindowHeight( EngineManager.settings.WindowResolution.height );
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
    /// Sets the renderer's window title to <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The window's new title.</param>
    [ConsoleCommand( "r_setwindowtitle", "Sets the renderer's window's title to the argument string. For spaces, place underscores.", CommandConditions.Cheats )]
    public static void SetWindowTitle( List<object> args )
    {
        // Get the count of arguments
        int argCount = args.Count - 1;

        // Make sure we have the right amount of arguments...
        if ( argCount < 1 || argCount > 2 )
        {
            Log.Error( "Invalid amount of arguments! You need at least 1, and at most 2 arguments specifying the title you wish to switch to, and if it should suffix the standard engine title. If you're trying to use spaces, replace those spaces with underscores!" );
            return;
        }

        // Set the window's title to the argument
        window.Title = $"{EngineManager.EngineTitle} - {args[1].ToString().Replace( "_", " " )}";
        Log.Info( $"Set the window's title to \"{args[1].ToString().Replace( "_", " " )}\"!", true );
    }

    /// <summary>
    /// Sets the renderer's window title to <paramref name="newTitle"/>.
    /// </summary>
    /// <param name="newTitle">The window's new title.</param>
    public static void SetWindowTitle( string newTitle )
    {
        window.Title = $"{EngineManager.EngineTitle} - {newTitle}";
    }

    /// <summary>
    /// Toggles VSync through the console
    /// </summary>
    [ConsoleCommand( "r_vsync", "Toggles VSync on the renderer." )]
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
    [ConsoleCommand( "r_windowed", "Sets the renderer's window to be windowed." )]
    private static void SetWindowStateToWindowed()
    {
        window.WindowState = WindowState.Normal;
    }

    /// <summary>
    /// Changes the window's current state to fullscreen.
    /// </summary>
    [ConsoleCommand( "r_fullscreen", "Sets the renderer's window to be fullscreened." )]
    private static void SetWindowStateToFullscreen()
    {
        window.WindowState = WindowState.FullScreen;
    }

    /// <summary>
    /// Changes the window's current state to borderless.
    /// </summary>
    [ConsoleCommand( "r_borderless", "Sets the renderer's window to be borderless fullscreened." )]
    private static void SetWindowStateToBorderless()
    {
        window.WindowState = WindowState.BorderlessFullScreen;
    }

    /// <summary>
    /// Changes the window's current resolution through the console.
    /// </summary>
    [ConsoleCommand( "r_resolution", "Changes the resolution of which the renderer window is displayed at." )]
    private static void ChangeWindowResolution( List<object> args )
    {
        // The amount of arguments
        int argCount = args.Count - 1;

        // If we have more than 2 arguments...
        if ( argCount > 2 || argCount < 2 )
        {
            Log.Error( "Invalid amount of arguments! You need at least, and at most, 2 arguments, one for the height value and one for the width value!" );
            return;
        }

        // Get the width...
        if ( !int.TryParse( (string)args[1], out int width ) )
        {
            Log.Error( "First argument was an invalid integer value!" );
            return;
        }

        // Get the height...
        if ( !int.TryParse( (string)args[2], out int height ) )
        {
            Log.Error( "Second argument was an invalid integer value!" );
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
    /// Get the renderer's current window state.
    /// </summary>
    /// <returns>The state of the window as <see cref="WindowState"/></returns>
    public static WindowState GetWindowState()
    {
        return window.WindowState;
    }

    /// <summary>
    /// Is VSync enabled on the renderer?
    /// </summary>
    /// <returns><see langword="true"/> if it is, <see langword="false"/> otherwise.</returns>
    public static bool IsVSyncEnabled()
    {
        return graphicsDevice.MainSwapchain.SyncToVerticalBlank;
    }

    /// <summary>
    /// Gets the width of the renderer's window.
    /// </summary>
    /// <returns>The width of the active window.</returns>
    public static int GetWindowWidth()
    {
        return window.Width;
    }

    /// <summary>
    /// Gets the height of the renderer's window.
    /// </summary>
    /// <returns>The height of the active window.</returns>
    public static int GetWindowHeight()
    {
        return window.Height;
    }

    /// <summary>
    /// Sets the value of the window's window state.
    /// </summary>
    /// <param name="windowState">The window state we should switch to.</param>
    public static void SetWindowState( WindowState windowState)
    {
        window.WindowState = windowState;
    }

    /// <summary>
    /// Sets the value of VSync to the parameter.
    /// </summary>
    /// <param name="vSync">Determines whether or not VSync should be enabled.</param>
    public static void SetVSync( bool vSync )
    {
        graphicsDevice.MainSwapchain.SyncToVerticalBlank = vSync;
    }

    /// <summary>
    /// Sets the window's width.
    /// </summary>
    /// <param name="width">The new width of the window.</param>
    public static void SetWindowWidth( int width )
    {
        window.Width = width;
    }

    /// <summary>
    /// Sets the window's height.
    /// </summary>
    /// <param name="height">The new height of the window.</param>
    public static void SetWindowHeight( int height )
    {
        window.Height = height;
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