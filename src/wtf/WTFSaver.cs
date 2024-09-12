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
    public static void SaveFile(string path = null, WTFFile file = null)
    {
        // A local variable for storing the file
        WTFFile currFile;

        // If we already have a file open, set the path to the current file
        if (DoomNET.file != null && !string.IsNullOrEmpty(DoomNET.file.filepath))
        {
            currFile = DoomNET.file;
            path = currFile.filepath;
        }
        else if (file != null) // If the input file wasn't null, set this file to the input file
        {
            currFile = file;
        }
        else
        {
            throw new NullReferenceException("Error saving file, inFile == null && WTFLoader.file == null!");
        }

        // Call the file's OnSave function
        currFile.OnSave();

        // Set the file's filepath if it wasn't already sourced from the file itself
        if (currFile.filepath != path)
        {
            currFile.filepath = path;
        }

        // Write the WTF file to the path
        File.WriteAllText(path, JsonSerializer.Serialize(currFile, Program.serializerOptions));
    }
}