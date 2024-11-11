using SDL2;

using System;
using System.Diagnostics;

using DoomNET.WTF;
using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET;

public class DoomNET
{
    public static event Action OnUpdate;
    public static WTFFile file;

    public bool active;

    public static float deltaTime;

    private IntPtr window;
    private IntPtr renderer;

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

        // Initialize an SDL window
        SetupSDL();

        // Now we can run the game every frame
        Update();

        // Clean up everything SDL-wise
        CleanUpSDL();
    }

    /// <summary>
    /// Setup an SDL window and renderer
    /// </summary>
    private void SetupSDL()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initializing SDL.\n" +
                                    $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // Create a new window given a title, size, and passes it a flag indicating it should be shown
        window = SDL.SDL_CreateWindow("Doom.NET", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 1280, 720, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window.\n" +
                                    $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // Creates a new SDL hardware renderer using the default graphics device with vsync enabled
        renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (renderer == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the renderer.\n" +
                                    $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // Initializes SDL_image for use with png files
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
        {
            Console.WriteLine($"There was an issue initializing SDL2_Image.\n" +
                                    $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // Initializes SDL_ttf for use with text rendering
        if (SDL_ttf.TTF_Init() != 0)
        {
            Console.WriteLine($"There was an issue initializing SDL_TTF.\n" +
                                    $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    /// <summary>
    /// Things to do every frame, game-wise<br/>
    /// This includes calling every other update function that updates every frame
    /// </summary>
    private void Update()
    {
        // Start the loop
        active = true;

        // Stopwatch to calculate delta time with
        Stopwatch watch = Stopwatch.StartNew();

        while (active)
        {
            // Calculate deltaTime
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            // Call the OnUpdate event, so everything else that should update also updates with us
            OnUpdate?.Invoke();

            // Poll SDL events and render everything necessary on the window
            PollEventsSDL();
            RenderSDL();
        }
    }

    /// <summary>
    /// Handle polling events
    /// </summary>
    private void PollEventsSDL()
    {
        // Check to see if there are any events and continue to do so until the queue is empty
        while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    active = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Things to do that will allow the rendering of SDL
    /// </summary>
    private void RenderSDL()
    {
        // Sets the color that the screen will be cleared with
        if (SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255) < 0)
        {
            Console.WriteLine($"There was an issue with setting the render draw color.\n" +
                                $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // Clears the current render surface
        if (SDL.SDL_RenderClear(renderer) < 0)
        {
            Console.WriteLine($"There was an issue with clearing the render surface.\n" +
                                $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        // DEBUG: Draw the deltatime
        DisplayText($"Deltatime: {deltaTime}", 200, 25);

        // Switches out the currently presented render surface with the one we just did work on
        SDL.SDL_RenderPresent(renderer);
    }

    /// <summary>
    /// Draw text onto the SDL rendered window.
    /// </summary>
    public void DisplayText(string text, int width, int height, int x = 0, int y = 0)
    {
        IntPtr sans = SDL_ttf.TTF_OpenFont($"{Environment.CurrentDirectory}\\resources\\fonts\\COUR.TTF", 17);

        if (sans == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue with getting font.\n" +
                                $"\t{SDL.SDL_GetError()}\n");
            return;
        }

        SDL.SDL_Color black = new SDL.SDL_Color();
        black.r = black.g = black.b = 0; black.a = 255;

        IntPtr surfaceMessage = SDL_ttf.TTF_RenderText_Solid(sans, text, black);

        IntPtr message = SDL.SDL_CreateTextureFromSurface(renderer, surfaceMessage);

        SDL.SDL_Rect messageRect;
        messageRect.x = 0;
        messageRect.y = 0;
        messageRect.w = width;
        messageRect.h = height;

        SDL.SDL_RenderCopy(renderer, message, (IntPtr)null, ref messageRect);

        SDL.SDL_FreeSurface(surfaceMessage);
        SDL.SDL_DestroyTexture(message);
        SDL_ttf.TTF_CloseFont(sans);
    }

    /// <summary>
    /// Appropriately clean up everything with SDL
    /// </summary>
    private void CleanUpSDL()
    {
        // Clean up the resources that were created
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}