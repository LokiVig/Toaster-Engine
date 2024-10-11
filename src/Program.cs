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
        using (DoomNET game = new(800, 600, "Doom.NET"))
        {
            game.Initialize();
        }
    }
}
