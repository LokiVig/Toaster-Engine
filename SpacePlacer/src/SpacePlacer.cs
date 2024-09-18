using System;

using DoomNET.WTF;

namespace DoomNET.SpacePlacer;

public class SpacePlacer
{
    private WTFFile openFile;
    private float deltaTime;
    private bool active;

    public static event Action OnUpdate;
    
    /// <summary>
    /// Initialize the SpacePlacer program
    /// </summary>
    public void Initialize()
    {
        // The program is now active
        active = true;

        // Start the Update function
        Update();
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    private void Update()
    {
        while (active)
        {
            // Call all the necessary update functions
            OnUpdate?.Invoke();
        }
    }
}