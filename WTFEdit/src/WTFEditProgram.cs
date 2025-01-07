using System;
using System.Threading;
using System.Diagnostics;

using Toast.Engine.Resources;

namespace Toast.WTFEdit;

public class Program
{
    public static void Main()
    {
        WTFEditProgram wtfe = new WTFEditProgram();
        wtfe.Initialize();
    }
}

public class WTFEditProgram
{
    private WTF currentFile;
    private float deltaTime;
    private bool active;

    public static event Action OnUpdate;
    
    /// <summary>
    /// Initialize the WTFEdit program
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
        currentFile = WTF.LoadFile(path);
    }

    private void SaveMap()
    {
        if (currentFile == null)
        {
            Console.WriteLine("Error: Can't save map, because no map is loaded!");
            return;
        }
        
        WTF.SaveFile(currentFile?.path, currentFile);
    }

    private void SaveMap(WTF inFile)
    {
        WTF.SaveFile(inFile.path, inFile);
    }
}