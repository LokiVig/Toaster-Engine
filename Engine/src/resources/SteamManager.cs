using System;

using Steamworks;

namespace Toast.Engine.Resources
{
    public static class SteamManager
    {
        private static bool initialized;

        /// <summary>
        /// Initializes the Steam manager.<br/>
        /// Connects us to steamworks and does some magic.
        /// </summary>
        public static void Initialize()
        {
            // Make sure we got the right versions...
            if ( !Packsize.Test() )
            {
                Log.Error( "Packsize Test returned false, the wrong version of Steamworks.NET is being used." );
                return;
            }

            if ( !DllCheck.Test() )
            {
                Log.Error( "DllCheck Test returned false, one or more of the Steamworks binaries seems to be the wrong version." );
                return;
            }

            try
            {
                // If we have to, start the app again
                if ( SteamAPI.RestartAppIfNecessary( (AppId_t)480 ) )
                {
                    Environment.Exit( -1 );
                    return;
                }
            }
            catch ( Exception exc )
            {
                // If we didn't find the DLL...
                if ( exc is DllNotFoundException )
                {
                    // Log the error!
                    Log.Error( $"Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. {exc.Message}" );
                    Environment.Exit( -1 );
                }
            }

            // Initialize the Steam API
            initialized = SteamAPI.Init();

            // If we didn't successfully initialize...
            if ( !initialized )
            {
                // Log the error!
                Log.Error( "SteamAPI.Init() failed. Refer to Valve's documentation." );
                return;
            }
        }

        /// <summary>
        /// Determines whether or not the Steam manager is initialized.
        /// </summary>
        /// <returns><see langword="true"/> if it is, <see langword="false"/> otherwise.</returns>
        public static bool IsInitialized()
        {
            return initialized;
        }

        /// <summary>
        /// Shuts down the Steam manager.<br/>
        /// Ends connections to steamworks.
        /// </summary>
        public static void Shutdown()
        {
            // Make sure we're initialized
            if ( !initialized )
            {
                return;
            }

            // Shut down the Steam API
            SteamAPI.Shutdown();
        }
    }
}