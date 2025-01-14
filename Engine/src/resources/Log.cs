using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Toast.Engine.Resources;

public struct Log
{
    /// <summary>
    /// Logs specific information to the console with a <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message we wish to log to the console with.</param>
    /// <param name="extraInfo">Determines whether or not we should have extra information, such as the line we were called from, caller, and caller method.</param>
    public static void Info( string message, bool extraInfo = false, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // If we should have some extra information about what we're logging...
        if ( extraInfo )
        {
            // Add the line of where it was called, the caller, the method that called us, then the message
            Console.WriteLine( $"(Line {line}) {caller}.{method}: INFO; {message}\n" );
        }
        else // Otherwise...
        {
            // Just write the message
            Console.WriteLine( $"{message}\n" );
        }
    }

    /// <summary>
    /// Log a successful operation, with an optional <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The specific success message used to detail what happened to cause a warning.</param>
    public static void Success( string message = null, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default success sound
        EngineProgram.globalAudioManager.PlaySuccess();

        // Make sure we have a message before we do
        if ( message != null )
        {
            // Get the name of the class that called us
            string caller = Path.GetFileNameWithoutExtension( src );

            // Check if the method is the constructor...
            if ( method == ".ctor" )
            {
                // Make it more obvious that it is such!
                method = "Constructor()";
            }

            // Write to the console what just happened
            Console.WriteLine( $"(Line {line}) {caller}.{method}: SUCCESS; {message}\n" );
        }
    }

    /// <summary>
    /// Do the basic warning functionality, with <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The specific warning message used to detail what happened to cause a warning.</param>
    public static void Warning( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default warning sound
        EngineProgram.globalAudioManager.PlayWarning();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        Console.WriteLine( $"(Line {line}) {caller}.{method}: WARNING; {message}\n" );
    }

    /// <summary>
    /// Do the basic error functionalities, with a <paramref name="message"/> and possible <paramref name="exception"/>.
    /// </summary>
    /// <param name="message">The specific error message used to detail what happened to cause an error.</param>
    /// <param name="exception">The exception we wish to call upon receiving the error.</param>
    public static void Error( string message, Exception exception = null, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default error sound
        EngineProgram.globalAudioManager.PlayError();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        Console.WriteLine( $"(Line {line}) {caller}.{method}: ERROR; {message}\n" );

        // If we have an exception...
        if ( exception != null )
        {
            // Make a new, local exception, with the sourced one as an inner exception
            Exception localException = new Exception( $"(Line {line}) {caller}.{method}: ERROR; {message}", exception );

            // Throw it!
            throw localException;
        }
    }
}