using System;
using System.Diagnostics;
using System.Threading;

using Toast.Engine.Resources;

namespace Toast.SpacePlacer;

public class SpacePlacer
{
    private WTF file;
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

        // !! DEBUG !! \\
        SaveMap(new WTF("maps/test.wtf"));

        // Start the Update function
        Update();
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    private void Update()
    {
        Stopwatch watch = Stopwatch.StartNew();

        while (active)
        {
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            // Call all the necessary update functions
            OnUpdate?.Invoke();
        }
    }

    private void LoadMap(string path)
    {
        file = WTF.LoadFile(path);
    }

    private void SaveMap()
    {
        WTF.SaveFile(file?.path, file);
    }

    private void SaveMap(WTF inFile)
    {
        WTF.SaveFile(inFile.path, inFile);
    }
}