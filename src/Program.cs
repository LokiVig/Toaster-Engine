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

    public static List<Entity> entities = new();

    public static void Main()
    {
        // Fill the entities list
        // ... Find a better way to do this, PLEASE
        entities.Add(new TestNPC());
        entities.Add(new TriggerBrush());
        entities.Add(new DamageableBrush());

        DoomNET doomNET = new();
        doomNET.Initialize();
    }
}
