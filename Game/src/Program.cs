using System;
using System.Text.Json;

namespace Toast.Game;

public class Program
{
    [STAThread]
    public static void Main()
    {
        Game game = new Game();
        game.Initialize();
    }
}
