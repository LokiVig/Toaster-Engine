using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DoomNET.WTF;

/// <summary>
/// Handler for saving WTF files to paths and etc
/// </summary>
public class WTFSaver
{
    /// <summary>
    /// Save a WTF file to a specified path
    /// </summary>
    /// <param name="path">The path of the WTF file, already specified if WTFLoader.file and/or its filepath isn't null</param>
    public static void SaveFile(string path, WTFFile inFile)
    {
        // A local variable for storing the file
        WTFFile file;

        // If we already have a file open, set the path to the current file
        if (DoomNET.file != null && !string.IsNullOrEmpty(DoomNET.file.directory))
        {
            file = DoomNET.file;
            path = file.directory;
        }
        else if (inFile != null) // If the input file wasn't null, set it accordingly
        {
            file = inFile;
        }
        else // We couldn't find a file to save, error!
        {
            throw new NullReferenceException("Error saving file, SaveFile().inFile == null && DoomNET.file == null!");
        }

        // Call the file's OnSave function
        file.OnSave();

        // Set the file's filepath if it wasn't already sourced from the file itself
        if (file.directory != path)
        {
            file.directory = path;
        }

        // Write the WTF file to the path
        File.WriteAllText(path, JsonSerializer.Serialize(file, Program.serializerOptions));
    }
}