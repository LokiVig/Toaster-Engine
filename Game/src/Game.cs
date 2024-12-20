using System;
using System.Diagnostics;

using Toast.Engine;
using Toast.Engine.Entities;

namespace Toast.Game;

/// <summary>
/// The game itself.<br/>
/// This class handles initializing and running everything game-wise, meaning it holds the Update function,
/// important variables such as deltaTime, the current file, and the current scene.
/// </summary>
public class Game
{
	private Player mainPlayer;
	
	/// <summary>
	/// Initialize the game
	/// </summary>
	public void Initialize()
	{
		// Initialize the engine
		EngineProgram.Initialize(); // Call the original initialize function
		EngineProgram.OnUpdate += Update;
	}

	/// <summary>
	/// Defines things to do every frame the game is run.
	/// </summary>
	private void Update()
	{
		
	}
}