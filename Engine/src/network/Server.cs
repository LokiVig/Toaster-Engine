using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using System.Threading.Tasks;
using Steamworks;

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

        // Make sure the IP is actually valid...
        if ( !IPAddress.TryParse( addr, out ipAddr ) )
        {
            // Log the error
            Log.Error( $"{addr} is not a valid IP address!" );

            // Dispose of ourselves
            Dispose();

            // Skip doing anything more
            return;
        }

        this.port = port; // Set our port
        this.family = family; // Set our address family


        connectedClients = new List<Client>(); // Initialize our client list

        // The server should also update!
        EngineManager.OnUpdate += Update;
    }

    /// <summary>
    /// Things to do every frame.<br/>
    /// In this case, we should most likely send and receive packets every frame we can.
    /// </summary>
    private void Update()
    {
        // Make sure we're actively open
        if ( !IsOpen() )
        {
            return;
        }

        // If we have any pending connections...
        if ( listener.Pending() )
        {
            // Accept the pending connection and add the client to our list
            TcpClient tcp = listener.AcceptTcpClient();
            Client client = new Client( tcp );
            connectedClients.Add( client );

            Log.Info( $"Accepted connection from {client.GetAddress()}!", true );
        }

        // For every connected client...
        foreach ( Client client in connectedClients )
        {
            // If the client is still connected...
            if ( client.IsConnected() )
            {
                
            }
            else // Otherwise, the client has disconnected...
            {
                Log.Info( $"{client.GetAddress()} has disconnected!", true );
                client.Dispose();
                connectedClients.Remove( client );
            }
        }
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
        server.Host();

        // Return the newly made and hosted server
        return server;
    }

    /// <summary>
    /// Hosts an online-accessible server.
    /// </summary>
    /// <param name="fetchIP">Determines whether or not we should fetch an IP automatically.</param>
    public void Host( bool fetchIP = false )
    {
        // Make sure we have a valid address family
        if ( family == AddressFamily.Unknown )
        {
            Log.Error( "Can't host a server on an unknown address family!" );
            return;
        }

        // If we should fetch an IP address...
        if ( fetchIP )
        {
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
        }
        else // Otherwise...
        {
            // Try and parse our string address to an IP address
            ipAddr = IPAddress.Parse( addr );
        }

        // If the engine instance's server isn't null...
        if ( EngineManager.server != null )
        {
            // If this engine's instance's server is actively open...
            if ( EngineManager.server.IsOpen() )
            {
                // Close it! We can't have one server host in multiple places
                EngineManager.server.Close();
            }
        }

        try
        {
            // Create our TCP listener
            listener = new TcpListener( ipAddr, port );
            listener.Start();
        }
        catch (Exception exc)
        {
            Log.Error( $"Error hosting on {ipAddr}:{port}! {exc.Message}" );
            return;
        }

        // Try to connect with our local client too
        if ( !EngineManager.client.TryConnectTo( this ) )
        {
            Log.Error( $"Failed connecting to newly hosted server at {addr}:{port}!" );

            Close(); // Close the server

            // Start hosting locally again
            addr = LOCALHOST_ADDR;
            port = LOCALHOST_PORT;
            Host();

            return;
        }

        Log.Info( $"Hosting on {addr}:{port}...", true );
    }

    /// <summary>
    /// Console command letting the user host on this computer's valid IP address and select port.
    /// </summary>
    /// <param name="args">The input arguments from </param>
    [ConsoleCommand( "host", "Hosts a server from either a specified address and port, localhost, or just a specified port (IP chosen automatically)." )]
    public static void Host( List<object> args )
    {
        int argCount = args.Count - 1;

        if ( argCount < 1 || argCount > 2 )
        {
            Log.Error( "Invalid argument count! You need at least 1 argument and at most 2 arguments, one specifying the IP you want to host off, and one for the port of which you want to host from. Typing localhost will host a server on localhost." );
            return;
        }

        // If we have more than one argument...
        if ( argCount > 1 )
        {
            // Host on the specified IP address and port
            EngineManager.server.addr = (string)args[1];
            EngineManager.server.port = ushort.Parse( (string)args[2] );
            EngineManager.server.Host();
        }
        else // Otherwise we just want to host on a port, or localhost
        {
            // If we're specifying localhost...
            if ( args[1].ToString().ToLower() == "localhost" )
            {
                // Start hosting on localhost!
                EngineManager.server.addr = LOCALHOST_ADDR;
                EngineManager.server.port = LOCALHOST_PORT;
                EngineManager.server.Host();
            }
            else // Otherwise, we're specifying a port...
            {
                // Set the port and start hosting from there
                EngineManager.server.port = ushort.Parse( (string)args[1] );
                EngineManager.server.Host( true );
            }
        }
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

    [ConsoleCommand("displayconnections", "Shows all connected clients to the server." )]
    public static void ShowConnections()
    {
        // Header for connected clients
        Log.Info( $"Connected clients:" );

        // For every connected client...
        foreach ( Client client in EngineManager.server.connectedClients )
        {
            // If they're connected...
            if ( client.IsConnected() )
            {
                // Log their existence!
                Log.Info( $"\tAddress: {client.GetAddress()}" );
            }
        }
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

        // Stop subscribing to the update event
        EngineManager.OnUpdate -= Update;
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

        return Equals( lhs, rhs );
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