using System;
using System.Threading;
using System.Diagnostics;

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

        // !! DEBUG !! \\
        SaveMap(new WTFFile("maps/test.wtf"));

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

            // Handle FPS locking - to 60FPS max
            float elapsedTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            float timeToWait = (1.0f / 60.0f) - elapsedTime;

            if (timeToWait > 0)
            {
                Thread.Sleep((int)(timeToWait * 1000));
            }
        }
    }

    private void LoadMap(string directory)
    {
        DoomNET.file = openFile = WTFLoader.LoadFile(directory);
    }

    private void SaveMap()
    {
        WTFSaver.SaveFile(openFile?.directory, openFile);
    }

    private void SaveMap(WTFFile file)
    {
        WTFSaver.SaveFile(file.directory, file);
    }
}