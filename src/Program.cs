using System;
using System.Text.Json;

namespace DoomNET;

public class Program
{
    public static JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };

    [STAThread]
    public static void Main()
    {
        DoomNET game = new DoomNET();
        game.Initialize();
    }
}
