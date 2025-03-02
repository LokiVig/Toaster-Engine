using System.IO;

namespace Toast.Engine.Resources;

public static class StreamWriterExtensions
{
    public static void WriteSeparator( this StreamWriter sw, bool newlinePrefix = true )
    {
        sw.WriteLine($"{(newlinePrefix ? "\n" : "")}-----------------------------------------------\n" );
    }
}