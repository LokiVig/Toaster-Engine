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

	public static float deltaTime; // Helps stopping you from using FPS-dependant calculations

	public static IntPtr renderer;

	public static void Initialize()
	{
		// Initialize the renderer
		// renderer = External.CreateRenderer(currentScene);
	}

	private static Stopwatch watch = Stopwatch.StartNew();
	
	public static void Update()
	{
		while (true /*renderer != IntPtr.Zero*/)
		{
			// Calculate deltaTime
			deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
			watch.Restart();

			// Call the rendering function
			// External.RenderFrame(renderer);
			
			// Call the OnUpdate event
			// This makes it so everything subscribed to the event will call their own,
			// subsidiary update method
			OnUpdate?.Invoke();
		}
	}

	public static void Shutdown()
	{
		External.ShutdownRenderer(renderer);
	}
}