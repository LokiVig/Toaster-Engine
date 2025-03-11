using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Toast.Engine.Resources;
using Toast.Engine.Attributes;

namespace Toast.Engine.Network;

/// <summary>
/// A network server. Holds a list of connected clients and manages their connections and etc.
/// </summary>
public class Server : IDisposable
{
    public const string LOCALHOST_ADDR = "127.0.0.1"; // Localhost IP address
    public const ushort LOCALHOST_PORT = 3001; // Localhost port

    public static readonly Server LocalHost = new Server( LOCALHOST_ADDR, LOCALHOST_PORT );

    public string addr; // This server's address
    public ushort port; // This server's port

    private IPAddress ipAddr; // The actual IP address of this server
    private TcpListener listener; // The listener of this server
    private AddressFamily family; // Which address family this server is a part of
    private List<Client> connectedClients; // The list of clients that are connected to this server

    public Server( string addr, ushort port, AddressFamily family = AddressFamily.Unspecified )
    {
        this.addr = addr; // Set our address
        this.port = port; // Set our port
        this.family = family; // Set our address family

        connectedClients = new List<Client>(); // Initialize our client list
    }

    /// <summary>
    /// Creates a new server that's hosting locally.
    /// </summary>
    /// <returns>The locally hosted server.</returns>
    public static Server CreateLocalServer()
    {
        // Create a new server using localhost
        Server server = new Server( LOCALHOST_ADDR, LOCALHOST_PORT );

        // Tell the server to start hosting locally
        server.HostLocal();

        // Return the newly made and hosted server
        return server;
    }

    /// <summary>
    /// Hosts an online-accessible server.
    /// </summary>
    public void Host()
    {
        // Make sure we have a valid address family
        if ( family == AddressFamily.Unknown )
        {
            Log.Error( "Can't host a server on an unknown address family!" );
            return;
        }

        // Gets the system's public IP address
        // Source:
        //  - https://stackoverflow.com/a/6803109
        IPHostEntry host = Dns.GetHostEntry( Dns.GetHostName() );

        foreach ( IPAddress ip in host.AddressList )
        {
            if ( ip.AddressFamily == AddressFamily.InterNetwork )
            {
                ipAddr = ip;
                addr = ip.ToString();
            }
        }

        // If this engine's instance's server is actively open...
        if ( EngineManager.server.IsOpen() )
        {
            // Close it! We can't have one server host in multiple places
            EngineManager.server.Close();
        }

        // Create our TCP listener
        listener = new TcpListener( ipAddr, port );
        listener.Start();

        Log.Info( $"Hosting on {addr}:{port}...", true );
    }

    /// <summary>
    /// Hosts this server locally.
    /// </summary>
    public void HostLocal()
    {
        // If we're actively open...
        if ( IsOpen() )
        {
            // Close ourselvess! We can't have one server host in multiple places
            Close();
        }

        // Set the address and port to localhost
        addr = LOCALHOST_ADDR;
        port = LOCALHOST_PORT;

        // Get the actual IP address
        if ( !IPAddress.TryParse( addr, out ipAddr ) )
        {
            Log.Error( "Error parsing address to a valid IP address!" );
            return;
        }

        // Initialize our listener and start accepting connections
        listener = new TcpListener( ipAddr, port );
        listener.Start();

        Log.Info( $"Started hosting on localhost ({addr}:{port})...", true );
    }

    /// <summary>
    /// Console command letting the user host on this computer's valid IP address and select port.
    /// </summary>
    /// <param name="args">The input arguments from </param>
    [ConsoleCommand( "host", "Hosts a server from this computer's IPv4 address with a specified port." )]
    public static void Host_Command( List<object> args )
    {
        int argCount = args.Count - 1;

        if ( argCount < 1 || argCount > 1 )
        {
            Log.Error( "Invalid argument count! You need at least, and at most, 1 argument specifying the port of which you want to host from." );
            return;
        }

        // Set the EngineManager's server's port
        EngineManager.server.port = ushort.Parse( (string)args[1] );

        // Start hosting!
        EngineManager.server.Host();
    }

    [ConsoleCommand( "host_local", "Hosts a local server." )]
    public static void HostLocal_Command()
    {
        EngineManager.server.HostLocal();
    }

    /// <summary>
    /// Determines whether or not this server is actively open.
    /// </summary>
    /// <returns><see langword="true"/> if it is, <see langword="false"/> otherwise.</returns>
    public bool IsOpen()
    {
        // Make sure we have a listener in the first place
        if ( listener == null )
        {
            return false;
        }

        return listener.Server.IsBound;
    }

    /// <summary>
    /// Closes this server.
    /// </summary>
    public void Close()
    {
        // Disconnect all clients
        foreach ( Client client in connectedClients )
        {
            client.Disconnect();
        }

        // Clear the list of connected clients
        connectedClients.Clear();

        // Stop listening
        listener.Stop();
    }

    /// <summary>
    /// Disposes of this server.
    /// </summary>
    public void Dispose()
    {
        // Close the server and dispose the listener
        Close();
        listener.Dispose();
    }

    /// <summary>
    /// Checks if two servers share the same values, meaning they're the same server.
    /// </summary>
    /// <param name="lhs">The left hand side <see cref="Server"/> variable.</param>
    /// <param name="rhs">The right hand side <see cref="Server"/> variable.</param>
    /// <returns><see langword="true"/> if both servers have matching values, <see langword="false"/> otherwise.</returns>
    public static bool operator ==( Server lhs, Server rhs )
    {
        if ( lhs is null )
        {
            return rhs is null;
        }

        return Equals( lhs, rhs ); // Check port against port
    }

    /// <summary>
    /// Checks if two servers don't share the same values, meaning they're different servers.
    /// </summary>
    /// <param name="lhs">The left hand side <see cref="Server"/> variable.</param>
    /// <param name="rhs">The right hand side <see cref="Server"/> variable.</param>
    /// <returns><see langword="true"/> if either side has differing values from the other, <see langword="false"/> otherwise.</returns>
    public static bool operator !=( Server lhs, Server rhs )
    {
        return !( lhs == rhs );
    }

    public override bool Equals( object obj )
    {
        if ( ReferenceEquals( this, obj ) )
        {
            return true;
        }

        if ( ReferenceEquals( obj, null ) )
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}