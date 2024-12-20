using System;
using System.Text.Json;
using System.Diagnostics;

using Toast.Engine.Entities;
using Toast.Engine.Resources;

namespace Toast.Engine;

public class EngineProgram
{
	public static readonly JsonSerializerOptions serializerOptions = new()
	{
		WriteIndented = true,
		AllowTrailingCommas = true
	};

	public static event Action OnUpdate; // Whenever we should update things, this event gets called

	public static WTF currentFile; // The currently loaded WTF file / map
	public static Scene currentScene; // The currently running scene, initialized from the current file

	public static GameState currentState = GameState.Active; // The state the game currently is in

	public static float deltaTime; // Helps stopping you from using FPS-dependant calculations

	public static IntPtr renderer;

	public static void Initialize()
	{
		// Initialize the renderer
		renderer = External.CreateRenderer(currentScene);
	}

	private static Stopwatch watch = Stopwatch.StartNew();
	
	public static void Update()
	{
		while (renderer != IntPtr.Zero)
		{
			// Calculate deltaTime
			deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
			watch.Restart();

			// Things to do when there is a loaded scene
			if (currentScene != null)
			{
			}

			if ((currentState & GameState.Active) != 0)
			{
				// Call the OnUpdate event, so everything else that should update also updates with us
				OnUpdate?.Invoke();
			}

			// Call the rendering function
			External.RenderFrame(renderer);
		}
	}

	public static void Shutdown()
	{
		External.ShutdownRenderer(renderer);
	}
}

/// <summary>
/// This enum defines the different states the game can be in.
/// </summary>
[Flags]
public enum GameState
{
	Menu = 1, // Used when accessing any sort of menu
	Active = 2, // Rendering, updating, doing everything it should
	Paused = 4, // Paused for any reason, possibly maskable with menu?
}