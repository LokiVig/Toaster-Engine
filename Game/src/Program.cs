using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DoomNET.Entities;

namespace DoomNET;

public class Program
{
    public static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };

    [STAThread]
    public static void Main()
    {
        Game game = new Game();
        game.Initialize();
    }
}
