using System.Text.Json;
using System.Collections.Generic;

using DoomNET.Entities;

namespace DoomNET;

public class Program
{
    public static JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };

    public static void Main()
    {
        DoomNET doomNET = new();
        doomNET.Initialize();
    }
}
