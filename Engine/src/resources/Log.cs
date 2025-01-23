using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Toast.Engine.Rendering;

namespace Toast.Engine.Resources;

public struct Log
{
    // The path to the engine's logging file
    // This should just be our root dir, saved in a file called "engine.log"
    private const string PATH_LOG = "engine.log";

    // The filestream that'll let us actually write our loggings to the log file
    private static StreamWriter logWriter;

    // Since we should initialize logging to our file at the same time as the engine itself,
    // we can at least keep time of how long we've run the engine
    private static Stopwatch runtime;

    /// <summary>
    /// Initializes log file writing.
    /// </summary>
    public static void OpenLogFile()
    {
        // This will automatically make a logging file for us if we don't already have one
        FileStream logFile = File.Open( PATH_LOG, FileMode.Create );
        logFile.Close(); // Close the file stream

        // Write to the log file
        logWriter = File.AppendText( PATH_LOG );
        logWriter.Write( "Start of engine log file.\n" );
        logWriter.WriteLine( "-----------------------------------------------\n" );

        // Start our runtime clock
        runtime = Stopwatch.StartNew();
    }

    /// <summary>
    /// Ends log file writing with some extra, interesting info. Should be called upon engine shutdown.
    /// </summary>
    public static void CloseLogFile()
    {
        // Stop our runtime clock
        runtime.Stop();

        // Write infomatically filled messages to show that this is the end of the file
        logWriter.WriteLine( "\n-----------------------------------------------" );
        logWriter.WriteLine( "End of engine log file." );
        logWriter.WriteLine( $"Total engine runtime was: {runtime.Elapsed.ToString()}" );

        // Clean the log writer of its resources
        logWriter.Dispose();

        // Close the file
        logWriter.Close();
    }

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
            ConsoleUI.WriteLine( $"(Line {line}) {caller}.{method}: INFO; {message}" );

            // Write to the log file
            logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : (Line {line}) {caller}.{method}: INFO; {message}" );
        }
        else // Otherwise...
        {
            // Just write the message
            ConsoleUI.WriteLine( $"{message}" );

            // Write to the log file
            logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : {message}" );
        }
    }

    /// <summary>
    /// Log a successful operation, with an optional <paramref name="message"/>.<br/>
    /// Features a success sound effect.
    /// </summary>
    /// <param name="message">The specific success message used to detail what happened to cause a warning.</param>
    public static void Success( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default success sound
        AudioManager.PlaySuccess();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        ConsoleUI.WriteLine( $"(Line {line}) {caller}.{method}: SUCCESS; {message}" );

        // Write to the log file
        logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : (Line {line}) {caller}.{method}: SUCCESS; {message}" );
    }

    /// <summary>
    /// Log a warning to the console, with <paramref name="message"/>.<br/>
    /// Features a warning sound effect.
    /// </summary>
    /// <param name="message">The specific warning message used to detail what happened to cause a warning.</param>
    public static void Warning( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default warning sound
        AudioManager.PlayWarning();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        ConsoleUI.WriteLine( $"(Line {line}) {caller}.{method}: WARNING; {message}" );

        // Write to the log file
        logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : (Line {line}) {caller}.{method}: WARNING; {message}" );
    }

    /// <summary>
    /// Log an error to the console, with a <paramref name="message"/>.<br/>
    /// Features an error sound effect.
    /// </summary>
    /// <param name="message">The specific error message used to detail what happened to cause an error.</param>
    public static void Error<T>( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" ) where T : Exception, new()
    {
        // Play the engine's default error sound
        AudioManager.PlayError();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        ConsoleUI.WriteLine( $"(Line {line}) {caller}.{method}: ERROR; {message}" );

        // Write to the log file
        logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : (Line {line}) {caller}.{method}: ERROR; {message}" );

        // Create an exception from the type designated by the caller
        T exception = new T();

        // Make a new, local exception, with the sourced one as an inner exception
        Exception localException = new Exception( $"(Line {line}) {caller}.{method}; {message}", exception );

        // Close the log file, we don't want to keep it running after an exception has been caught
        CloseLogFile();

        // Throw it!
        throw localException;
    }

    /// <summary>
    /// Log an error to the console, with a <paramref name="message"/> and an optional <paramref name="exception"/>.<br/>
    /// Features an error sound effect.
    /// </summary>
    /// <param name="message">The specific error message used to detail what happened to cause an error.</param>
    /// <param name="exception">The optional exception to derive from.</param>
    public static void Error( string message, Exception exception = null, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default error sound
        AudioManager.PlayError();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        ConsoleUI.WriteLine( $"(Line {line}) {caller}.{method}: ERROR; {message}" );

        // Write to the log file
        logWriter.WriteLine( $"{DateTime.Now.ToLongTimeString()} : (Line {line}) {caller}.{method}: ERROR; {message}" );

        // If we have an exception...
        if ( exception != null )
        {
            // Make a new, local exception, with the sourced one as an inner exception
            Exception localException = new Exception( $"(Line {line}) {caller}.{method}; {message}", exception );

            // Close the log file, we don't want to keep it running after an exception has been caught
            CloseLogFile();

            // Throw it!
            throw localException;
        }
    }
}