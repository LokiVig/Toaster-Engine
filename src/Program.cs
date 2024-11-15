using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using DoomNET.Entities;

namespace DoomNET;

public class Program
{
    public static JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new EntityConverter() }
    };

    [STAThread]
    public static void Main()
    {
        DoomNET game = new DoomNET();
        game.Initialize();
    }
}
