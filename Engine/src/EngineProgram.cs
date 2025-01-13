using System;
using System.Text.Json;
using System.Diagnostics;

using Toast.Engine.Resources;
using System.Runtime.CompilerServices;
using System.IO;

namespace Toast.Engine;

public class EngineProgram
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    public static readonly JsonSerializerOptions serializerOptions = new()
	{
		WriteIndented = true,
		AllowTrailingCommas = true
	};

    private const string ENGINE_VERSION = "0.0.1";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    public static event Action OnUpdate; // Whenever we should update things, this event gets called

	public static WTF currentFile; // The currently loaded WTF file / map
	public static Scene currentScene; // The currently running scene, initialized from the current file

	public static AudioManager globalAudioManager; // Global audio manager

	public static float deltaTime; // Helps stopping you from using FPS-dependant calculations

	public static IntPtr renderer; // The C++ initialized renderer as a whole

    //---------------------------------------//
    //				 Privates                //
    //---------------------------------------//

    private static Stopwatch watch = Stopwatch.StartNew(); // Used to calculate delta time

    //---------------------------------------//
    //				Functions				 //
    //---------------------------------------//

    public static void Initialize(string title = null )
	{
		// Initialize the global audio manager
		globalAudioManager = new AudioManager();

		// Initialize the renderer
		try
		{
			renderer = External.CreateRenderer( $"Toaster Engine (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
			globalAudioManager.PlaySuccess();
		}
		catch ( Exception exc )
		{
			DoError($"Exception caught while initializing renderer!\n{exc.Message}\n", exc );
		}
	}

	public static void Update()
	{
		while (renderer != IntPtr.Zero)
		{
			// Calculate deltaTime
			deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
			watch.Restart();

			// Call the rendering function
			External.RenderFrame(renderer);

			// The renderer is shutting down!
			// Null the renderer value so we don't continue running without it
			if (External.RendererShuttingDown(renderer))
			{
				renderer = IntPtr.Zero;
			}

			// Update the global audio manager
			globalAudioManager.UpdateAllPlayingFiles();
			
			//External.RenderText(renderer, "FUCK", 1280/2, 720/2, 25.0f);
			
			// Call the OnUpdate event
			// This makes it so everything subscribed to the event will call their own,
			// subsidiary update method
			OnUpdate?.Invoke();
		}
	}

	/// <summary>
	/// Do the basic error functionalities, with a <paramref name="message"/> and possible <paramref name="exception"/>.
	/// </summary>
	/// <param name="message">The specific error message used to detail what exactly happened to cause an error.</param>
	/// <param name="exception">The exception we wish to call upon receiving the error.</param>
	public static void DoError(string message, Exception exception = null, [CallerLineNumber] int line = 0, [CallerMemberName] string method = "", [CallerFilePath] string src = "" )
	{
		// Get the name of the class that called us
		string caller = Path.GetFileNameWithoutExtension(src);

		// Check if the method is the constructor...
		if ( method == ".ctor" )
		{
			// Make it more obvious that it is such!
			method = "Constructor()";
		}

		// Play the engine's default error sound
		globalAudioManager.PlayError();

		// Write to the console what just happened
		Console.WriteLine( $"(Line {line}) {caller}.{method}: ERROR; {message}\n" );

		// If we have an exception...
		if ( exception != null )
		{
			// Make a new, local exception, with the sourced one as an inner exception
			Exception localException = new Exception($"(Line {line}) {caller}.{method}: ERROR; {message}", exception);

			// Throw it!
			throw localException;
		}
	}

	public static void Shutdown()
	{
		External.ShutdownRenderer(renderer);
	}
}