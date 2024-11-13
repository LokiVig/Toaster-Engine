using DoomNET.Resources;

using SDL2;

using System;

namespace DoomNET.Rendering;

public class Renderer
{
    public static IntPtr[] fonts;

    private Rasterizer rasterizer;
    private IntPtr window;
    private IntPtr renderer;

    /// <summary>
    /// The constructor should setup the renderer
    /// </summary>
    public Renderer()
    {
        Setup();
    }

    /// <summary>
    /// Setup an SDL window and renderer
    /// </summary>
    public void Setup()
    {
        if ( SDL.SDL_Init( SDL.SDL_INIT_VIDEO ) < 0 )
        {
            Console.WriteLine( $"There was an issue initializing SDL.\n" +
                                    $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Create a new window given a title, size, and pass it a flag indicating it should be shown
        window = SDL.SDL_CreateWindow( "Doom.NET", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, DoomNET.windowWidth, DoomNET.windowHeight, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN );

        if ( window == IntPtr.Zero )
        {
            Console.WriteLine( $"There was an issue creating the window.\n" +
                                    $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Creates a new SDL hardware renderer using the default graphics device with vsync enabled
        renderer = SDL.SDL_CreateRenderer( window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC );

        if ( renderer == IntPtr.Zero )
        {
            Console.WriteLine( $"There was an issue creating the renderer.\n" +
                                    $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Initializes SDL_image for use with png files
        if ( SDL_image.IMG_Init( SDL_image.IMG_InitFlags.IMG_INIT_PNG ) == 0 )
        {
            Console.WriteLine( $"There was an issue initializing SDL2_Image.\n" +
                                    $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Initializes SDL_ttf for use with text rendering
        if ( SDL_ttf.TTF_Init() != 0 )
        {
            Console.WriteLine( $"There was an issue initializing SDL_TTF.\n" +
                                    $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Initialize the rasterizer
        rasterizer = new Rasterizer( renderer );

        // Fill the font array
        fonts =
        [
            SDL_ttf.TTF_OpenFont($"{Environment.CurrentDirectory}\\resources\\fonts\\COUR.TTF", 17),
            // Add more fonts...
        ];

        SDL.SDL_SetRenderDrawBlendMode( renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND );
    }

    /// <summary>
    /// Handle polling events
    /// </summary>
    public void PollEvents()
    {
        // Check to see if there are any events and continue to do so until the queue is empty
        while ( SDL.SDL_PollEvent( out SDL.SDL_Event e ) == 1 )
        {
            switch ( e.type )
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    DoomNET.active = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Things to do that will allow the rendering of SDL
    /// </summary>
    public void Render()
    {
        if ( SDL.SDL_GetError() != "" )
        {
            Console.WriteLine( "Error rendering scene.\n" +
                                $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Sets the color that the screen will be cleared with
        if ( SDL.SDL_SetRenderDrawColor( renderer, 135, 206, 235, 255 ) < 0 )
        {
            Console.WriteLine( $"There was an issue with setting the render draw color.\n" +
                                $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        // Clears the current render surface
        if ( SDL.SDL_RenderClear( renderer ) < 0 )
        {
            Console.WriteLine( $"There was an issue with clearing the render surface.\n" +
                                $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        rasterizer.DrawTriangle( new Triangle( new Vertex( 65, 65, 45 ), new Vertex( 45, 65, 45 ), new Vertex( 45, 65, 65 ) ) );

        // DEBUG: Show the deltatime
        DisplayText( $"Deltatime: {DoomNET.deltaTime:0.###}", 0, 200, 25 );

        // DEBUG: Display the player's position & velocity
        DisplayText( $"Player pos: {DoomNET.file.entities[ 0 ].position}", 0, 300, 25, y: 720 - 25 );
        DisplayText( $"Player vel: {DoomNET.file.entities[ 0 ].GetVelocity()}", 0, 300, 25, y: 720 - 50 );

        // Switches out the currently presented render surface with the one we just did work on
        SDL.SDL_RenderPresent( renderer );

        SDL.SDL_Delay( 16 ); // Roughly a 60FPS cap, to not 100% the CPU usage
    }

    /// <summary>
    /// Draw text onto the SDL rendered window.
    /// </summary>
    public void DisplayText( string text, int fontIndex, int width, int height, int pt = 17, int x = 0, int y = 0 )
    {
        IntPtr font = fonts[ fontIndex ];

        if ( font == IntPtr.Zero )
        {
            Console.WriteLine( $"There was an issue with getting a font.\n" +
                                $"\t{SDL.SDL_GetError()}\n" );
            return;
        }

        SDL.SDL_Color black = new SDL.SDL_Color();
        black.r = black.g = black.b = 255; black.a = 255;

        IntPtr surfaceMessage = SDL_ttf.TTF_RenderText_Solid( font, text, black );

        IntPtr message = SDL.SDL_CreateTextureFromSurface( renderer, surfaceMessage );

        SDL.SDL_Rect messageRect;
        messageRect.x = x;
        messageRect.y = y;
        messageRect.w = width;
        messageRect.h = height;

        SDL.SDL_RenderCopy( renderer, message, (IntPtr)null, ref messageRect );

        SDL.SDL_FreeSurface( surfaceMessage );
        SDL.SDL_DestroyTexture( message );
    }

    /// <summary>
    /// Appropriately clean up everything with SDL
    /// </summary>
    public void CleanUp()
    {
        // Clean up the resources that were created
        SDL.SDL_DestroyRenderer( renderer );
        SDL.SDL_DestroyWindow( window );

        // Cleanup all the fonts we've called
        foreach ( IntPtr font in fonts )
        {
            SDL_ttf.TTF_CloseFont( font );
        }

        SDL.SDL_Quit();
    }
}