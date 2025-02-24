using Toast.Engine.Resources;
using Toast.Engine;
using Toast.Engine.Rendering;

namespace Toast.WTFEdit;

public class Program
{
    public static void Main()
    {
        HTFManager htf = new HTFManager();
        htf.Initialize();
    }
}

public class HTFManager
{
    private WTF currentFile;
    private bool isDirty;

    /// <summary>
    /// Initialize the WTFEdit program
    /// </summary>
    public void Initialize()
    {
        // Initialize the engine
        EngineManager.Initialize("HTF");
        EngineManager.OnUpdate += Update;

        // !! DEBUG !! \\
        SaveMap(new WTF("maps/test.wtf"));

        // Start the engine's update function
        EngineManager.Update();

        // After having run the update for however many times, shut down the engine
        EngineManager.Shutdown();
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

        Renderer.SetWindowTitle( $"HTF - \"{currentFile.path}\"{(isDirty ? "*" : "")}" );
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