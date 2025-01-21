using System.Collections.Generic;

using Veldrid;

namespace Toast.Engine.Resources;

public static class InputManager
{
    private static List<Key> keysDown = new();

    public static void OnKeyDown( KeyEvent ev )
    {
        if ( ev.Down )
        {
            if ( !keysDown.Contains( ev.Key ) )
            {
                keysDown.Add( ev.Key );
            }
        }
    }

    public static void OnKeyUp( KeyEvent ev )
    {
        if ( keysDown.Contains( ev.Key ) )
        {
            keysDown.Remove( ev.Key );
        }
    }

    public static bool IsKeyDown( Key key )
    {
        return keysDown.Contains( key );
    }
}