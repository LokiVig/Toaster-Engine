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
    /// <param name="directory">The specified path to the WTF file</param>
    /// <param name="outFile">An output file, for external uses</param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void LoadFile( string directory, out WTFFile outFile )
    {
        // Couldn't find file from the input path! Throw an exception
        if (!File.Exists( directory ))
        {
            throw new FileNotFoundException( $"Couldn't find WTF file at \"{directory}\"." );
        }

        // Deserialize the file through JSON
        outFile = JsonSerializer.Deserialize<WTFFile>( File.OpenRead( directory ), Program.serializerOptions );
    }

    /// <summary>
    /// Find a WTF file by a specific path
    /// </summary>
    /// <param name="directory">The specified path to the WTF file</param>
    /// <returns>The desired file as a variable</returns>
    public static WTFFile LoadFile( string directory )
    {
        WTFFile file;
        LoadFile( directory, out file );
        return file;
    }
}