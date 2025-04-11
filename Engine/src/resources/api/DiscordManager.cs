using System.IO;

using DiscordRPC;

namespace Toast.Engine.Resources.API;

public static class DiscordManager
{
    public const string PATH_OAUTH = "resources/api/discord/oauth.txt";

    private static DiscordRpcClient client;

    /// <summary>
    /// Initialize the RPC system.
    /// </summary>
    public static void Initialize()
    {
        client = new DiscordRpcClient( File.ReadAllText( PATH_OAUTH ) );

        client.Initialize();

        client.SetPresence( new RichPresence()
        {
            Details = "Last I checked, all toasters toast toast... I don't know about this one though.",
            Buttons =
            [
                new Button() {Label = "Guh", Url = "https://tenor.com/view/kiryu-kiryu-im-gonna-die-yakuza-yakuza-0-yakuza-typing-gif-16502160155422829316"}
            ]
        } );

        Log.Success( "Successfully initialied the Discord RPC system!" );
    }

    /// <summary>
    /// Set the Discord RPC's current presence.
    /// </summary>
    /// <param name="presence">The new presence we should switch to.</param>
    public static void SetPresence( RichPresence presence )
    {
        client.SetPresence( presence );
    }

    /// <summary>
    /// Shut down the RPC system.
    /// </summary>
    public static void Shutdown()
    {
        client.Dispose();
    }
}