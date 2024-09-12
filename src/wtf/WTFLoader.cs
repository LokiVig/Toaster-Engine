using System.IO;
using System.Text.Json;

namespace DoomNET.WTF;

/// <summary>
/// Handler for loading WTF files
/// </summary>
public class WTFLoader
{
    /// <summary>
    /// Find a WTF file by a specific path
    /// </summary>
    /// <param name="filepath">The specified path to the WTF file</param>
    /// <param name="outFile">An output file, for external uses</param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void LoadFile(string filepath, out WTFFile outFile)
    {
        // Couldn't find file from the input path! Throw an exception
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException($"Couldn't find WTF file at \"{filepath}\".");
        }

        // Deserialize the file through JSON
        outFile = JsonSerializer.Deserialize<WTFFile>(File.OpenRead(filepath), Program.serializerOptions);
    }

    /// <summary>
    /// Find a WTF file by a specific path
    /// </summary>
    /// <param name="filepath">The specified path to the WTF file</param>
    /// <returns>The desired file as a variable</returns>
    public static WTFFile LoadFile(string filepath)
    {
        WTFFile file;
        LoadFile(filepath, out file);
        return file;
    }
}