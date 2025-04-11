using System;
using System.IO;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Toast.Engine.Rendering;
using Toast.Engine.Extensions;
using Toast.Engine.Resources.Audio;

namespace Toast.Engine.Resources;

public static class Log
{
    // The path to the engine's logging file
    // This should just be our root dir, saved in a file called "Toaster.Engine.log"
    private const string PATH_LOG = "Toaster.Engine.log";

    // The different colors that logs can have in the console
    private static Vector4 colSuc = new Vector4( 0, 1, 0, 1 ); // Log.Success
    private static Vector4 colWrn = new Vector4( 1, 0.25f, 0.2f, 1 ); // Log.Warning
    private static Vector4 colErr = new Vector4( 1, 0, 0, 1 ); // Log.Error

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

        // Notify that this is is the start of the logging file
        logWriter.WriteLine( "Start of engine log file." );

        // Then write the date
        logWriter.WriteLine( $"{DateTime.Now:yyyy/MM/dd} @ {DateTime.Now:HH:mm:ss}" );

        // Separator
        logWriter.WriteSeparator();

        // Show information about the user's computer
        logWriter.WriteLine( "System Information" );
        logWriter.WriteLine( $"OS: {Environment.OSVersion} - \"{Environment.UserName}\" (\"{Environment.MachineName}\")" );
        logWriter.WriteLine( $"C# Version: {Environment.Version}" );

        // Separator
        logWriter.WriteSeparator();

        // Start our runtime clock
        runtime = Stopwatch.StartNew();

        // Log our success!
        Success( "Successfully initialized logging system!" );
    }

    /// <summary>
    /// Ends log file writing with some extra, interesting info. Should be called upon engine shutdown.
    /// </summary>
    public static void CloseLogFile()
    {
        // Stop our runtime clock
        runtime.Stop();

        // Write infomatically filled messages to show that this is the end of the file
        logWriter.WriteSeparator();
        logWriter.WriteLine( "End of engine log file." );
        logWriter.WriteLine( $"Total engine runtime was: {runtime.Elapsed}" );

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
            ConsoleUI.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) :  (Line {line}) {caller}.{method}; {message}" );

            // Write to the log file
            logWriter.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );

            // Write to the debug output
            Debug.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );
        }
        else // Otherwise...
        {
            // Just write the message
            ConsoleUI.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) : {message}" );

            // Write to the log file
            logWriter.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) : {message}" );

            // Write to the debug output
            Debug.WriteLine( $"[INFO] ({DateTime.Now.ToLongTimeString()}) : {message}" );
        }
    }

    /// <summary>
    /// Log a successful operation, with an optional <paramref name="message"/>.<br/>
    /// Features a success sound effect.
    /// </summary>
    /// <param name="message">The specific success message used to detail what happened to cause a success.</param>
    public static void Success( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
    {
        // Play the engine's default success sound
        //AudioManager.PlaySuccess();

        // Get the name of the class that called us
        string caller = Path.GetFileNameWithoutExtension( src );

        // Check if the method is the constructor...
        if ( method == ".ctor" )
        {
            // Make it more obvious that it is such!
            method = "Constructor()";
        }

        // Write to the console what just happened
        ConsoleUI.WriteLine( $"[SUCCESS] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}", colSuc );

        // Write to the log file
        logWriter.WriteLine( $"[SUCCESS] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );

        // Write to the debug output
        Debug.WriteLine( $"[SUCCESS] ({DateTime.Now.ToLongTimeString()}) :  (Line {line}) {caller}.{method}; {message}" );
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
        ConsoleUI.WriteLine( $"[WARNING] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}", colWrn );

        // Write to the log file
        logWriter.WriteLine( $"[WARNING] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );

        // Write to the debug output
        Debug.WriteLine( $"[WARNING] ({DateTime.Now.ToLongTimeString()}) :  (Line {line}) {caller}.{method}; {message}" );
    }

    /// <summary>
    /// Log an error to the console, with a <paramref name="message"/>.<br/>
    /// Features an error sound effect.
    /// </summary>
    /// <param name="message">The specific error message used to detail what happened to cause an error.</param>
    public static void Error<T>( string message, [CallerLineNumber] int line = 0, [CallerFilePath] string src = "", [CallerMemberName] string method = "" )
        where T : Exception, new()
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
        ConsoleUI.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}", colErr );

        // Write to the log file
        logWriter.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );

        // Write to the debug output
        Debug.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) :  (Line {line}) {caller}.{method}; {message}" );

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
        ConsoleUI.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}", colErr );

        // Write to the log file
        logWriter.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) : (Line {line}) {caller}.{method}; {message}" );

        // Write to the debug output
        Debug.WriteLine( $"[ERROR] ({DateTime.Now.ToLongTimeString()}) :  (Line {line}) {caller}.{method}; {message}" );

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