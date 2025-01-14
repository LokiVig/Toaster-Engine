using System;
using System.Threading;
using System.Diagnostics;

using Toast.Engine.Resources;
using Toast.Engine;

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

    /// <summary>
    /// Initialize the WTFEdit program
    /// </summary>
    public void Initialize()
    {
        // Initialize the engine
        EngineProgram.Initialize("WTFEdit");
        EngineProgram.OnUpdate += Update;

        // !! DEBUG !! \\
        SaveMap(new WTF("maps/test.wtf"));

        // Start the engine's update function
        EngineProgram.Update();

        // After having run the update for however many times, shut down the engine
        EngineProgram.Shutdown();
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    private void Update()
    {

    }

    private void LoadMap(string path)
    {
        currentFile = WTF.LoadFile(path);
    }

    private void SaveMap()
    {
        if (currentFile == null)
        {
            Log.Error("Can't save map, because no map is loaded!");
            return;
        }
        
        WTF.SaveFile(currentFile?.path, currentFile);
    }

    private void SaveMap(WTF inFile)
    {
        WTF.SaveFile(inFile.path, inFile);
    }
}