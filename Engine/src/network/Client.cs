using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

using Toast.Engine.Resources;
using Toast.Engine.Attributes;

namespace Toast.Engine.Network;

/// <summary>
/// A network client. Can connect to servers through IPs, etc.
/// </summary>
public class Client : IDisposable
{
    public TcpClient tcpClient; // The actual TCP client
    public IPAddress connectedAddr; // The string representation of the IP address we're currently connected to
    public ushort connectedPort; // The port of the server we're currently connected to

    private IPEndPoint localEndpoint;
    private IPEndPoint remoteEndpoint;

    /// <summary>
    /// Creates a new network client object.
    /// </summary>
    public Client()
    {
        // Initialize our TCP client
        tcpClient = new TcpClient();

        // Set our endpoints
        localEndpoint = tcpClient.Client.LocalEndPoint as IPEndPoint;
        remoteEndpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
    }

    /// <summary>
    /// Creates a new network client object with a specified TCP client.
    /// </summary>
    /// <param name="client">The specified TCP client.</param>
    public Client( TcpClient client )
    {
        tcpClient = client;

        // Set our endpoints
        localEndpoint = tcpClient.Client.LocalEndPoint as IPEndPoint;
        remoteEndpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
    }

    /// <summary>
    /// Connects this client to a specified IP address and port.<br/>
    /// Should only be used when you're certain the connection will be successful!
    /// </summary>
    /// <param name="addr">The IP address of the server we wish to connect to.</param>
    /// <param name="port">The port of the server we weish to connect to.</param>
    public void ConnectTo( IPAddress addr, ushort port )
    {
        // We want to disconnect before trying to connect anywhere, so if connecting to the new server fails,
        // we should return to the last server we were connected to
        // If even that fails however, we should just connect back to localhost
        IPAddress prevAddr = connectedAddr;
        ushort prevPort = connectedPort;

        // Disconnect from the current server
        Disconnect();

        // Try to connect to the specified address and port...
        try
        {
            tcpClient.Connect( addr, port );
        }
        catch ( Exception exc ) // If we got an exception...
        {
            // Log our failure!
            Log.Error( $"Error connecting to {addr}:{port}! {exc.Message}" );

            // If we can't manage to connect to a previous address...
            if ( !TryConnectTo( prevAddr, prevPort ) )
            {
                // Host a local server and connect to it!
                if ( EngineManager.server != Server.LocalHost )
                {
                    EngineManager.server = Server.CreateLocalServer();
                }

                ConnectTo( IPAddress.Parse( Server.LOCALHOST_ADDR ), Server.LOCALHOST_PORT );
            }

            // We shouldn't do anything more
            return;
        }

        // Make sure we *actually* connected properly
        if ( tcpClient.Connected )
        {
            Log.Info( $"Successfully connected to {addr}:{port}!", true );
            connectedAddr = addr;
            connectedPort = port;
        }
        else // Otherwise...
        {
            // We got a seemingly unknown error!
            Log.Error( $"Error connecting to {addr}:{port}!" );
        }
    }

    /// <inheritdoc cref="ConnectTo(IPAddress, ushort)"/>
    public void ConnectTo( string addr, ushort port )
    {
        try
        {
            ConnectTo( IPAddress.Parse( addr ), port );
        }
        catch ( Exception exc )
        {
            Log.Error( $"Error caught when trying to connect to {addr}:{port}! {exc.Message}" );
        }
    }

    /// <summary>
    /// Connects this client to a specific server.
    /// </summary>
    /// <param name="server">The server we wish to connect to.</param>
    public void ConnectTo( Server server )
    {
        ConnectTo( server.addr, server.port );
    }

    /// <summary>
    /// Tries to connect to a server through a console command.
    /// </summary>
    /// <param name="args">The arguments given to the console command.</param>
    [ConsoleCommand( "connect", "Connects the engine's local client to a specified server (IP - port)." )]
    public static void ConnectTo( List<object> args )
    {
        // Get the amount of arguments
        int argCount = args.Count - 1;

        // If we have an invalid amount of arguments...
        if ( argCount < 1 || argCount > 2 )
        {
            Log.Error( "Invalid amount of arguments! You need to specify at least 1 argument and at most 2 arguments, one for the IP address of the server you wish to connect to, and the other for the port, or just one specifying localhost." );
            return;
        }

        // If we have more than one argument...
        if ( argCount > 1 )
        {
            // Try to call the default TryConnectTo command with the input arguments...
            if ( !EngineManager.client.TryConnectTo( (string)args[1], ushort.Parse( (string)args[2] ) ) )
            {
                Log.Error( $"Failed to connect to {args[1]}:{args[2]}!" );
            }
        }
        else
        {
            // If we've typed localhost...
            if ( args[1].ToString().ToLower() == "localhost" )
            {
                // Try to connect to localhost!
                if ( !EngineManager.client.TryConnectTo( Server.LocalHost ) )
                {
                    Log.Error( "Failed to connect to localhost!" );
                    return;
                }
            }
            else
            {
                Log.Error( $"Failed to connect to \"{args[1]}\"!" );
            }
        }
    }

    /// <summary>
    /// Asynchronously try to connect to a specified IP address and port.
    /// </summary>
    /// <param name="addr">The IP address of the server we wish to connect to.</param>
    /// <param name="port">The port of the server we wish to connect to.</param>
    public async void ConnectToAsync( IPAddress addr, ushort port )
    {
        // We want to disconnect before trying to connect anywhere, so if connecting to the new server fails,
        // we should return to the last server we were connected to
        // If even that fails however, we should just connect back to localhost
        IPAddress prevAddr = connectedAddr;
        ushort prevPort = connectedPort;

        // Disconnect from the current server
        Disconnect();

        // Try to connect to the specified address and port...
        try
        {
            await tcpClient.ConnectAsync( addr, port );
        }
        catch ( Exception exc ) // If we got an exception...
        {
            // Log our failure!
            Log.Error( $"Error connecting to {addr}:{port}! {exc.Message}" );

            // If we can't manage to connect to a previous address...
            if ( !TryConnectTo( prevAddr, prevPort ) )
            {
                // Host a local server and connect to it!
                EngineManager.server = Server.CreateLocalServer();
                ConnectTo( Server.LocalHost );
            }

            // We shouldn't do anything more
            return;
        }

        // Make sure we *actually* connected properly
        if ( tcpClient.Connected )
        {
            Log.Info( $"Successfully connected to {addr}:{port}!", true );
            connectedAddr = addr;
            connectedPort = port;
        }
        else // Otherwise...
        {
            // We got a seemingly unknown error!
            Log.Error( $"Error connecting to {addr}:{port}!" );
        }
    }

    /// <summary>
    /// Tries to connect this client to a specified IP address and port.
    /// </summary>
    /// <param name="addr">The IP address of the server we wish to connect to.</param>
    /// <param name="port">The port of the server we wish to connect to.</param>
    /// <returns><see langword="true"/> if we successfully connect to the desired server, <see langword="false"/> otherwise.</returns>
    public bool TryConnectTo( IPAddress addr, ushort port )
    {
        // Disconnect from the current server
        Disconnect();

        // Try to connect to the specified address and port...
        try
        {
            tcpClient.Connect( addr, port );
        }
        catch ( Exception exc ) // If we got an exception...
        {
            // Log our failure!
            Log.Error( $"Error connecting to {addr}:{port}! {exc.Message}" );

            // We shouldn't do anything more
            return false;
        }

        // Make sure we *actually* connected properly
        if ( tcpClient.Connected )
        {
            Log.Info( $"Successfully connected to {addr}:{port}!", true );
            connectedAddr = addr;
            connectedPort = port;

            return true;
        }
        else // Otherwise...
        {
            // We got a seemingly unknown error!
            Log.Error( $"Error connecting to {addr}:{port}!" );
            return false;
        }
    }

    /// <inheritdoc cref="TryConnectTo(IPAddress, ushort)"/>
    public bool TryConnectTo( string addr, ushort port )
    {
        return TryConnectTo( IPAddress.Parse( addr ), port );
    }

    /// <inheritdoc cref="TryConnectTo(IPAddress, ushort)"/>
    /// <param name="server">The specific server we wish to try to connect to.</param>
    public bool TryConnectTo( Server server )
    {
        return TryConnectTo( server.addr, server.port );
    }

    /// <summary>
    /// Determines whether or not this client is actively connected to something.
    /// </summary>
    /// <returns><see langword="true"/> if we're connected to anything, <see langword="false"/> otherwise.</returns>
    public bool IsConnected()
    {
        return tcpClient.Connected;
    }

    /// <summary>
    /// Disconnects the client from the server it's actively connected with.
    /// </summary>
    public void Disconnect()
    {
        // Make sure we're connected...
        if ( !IsConnected() )
        {
            return;
        }

        // Log the fact that we're disconnecting
        Log.Info( $"Disconnecting from {connectedAddr}:{connectedPort}...", true );

        tcpClient.Close(); // Close the TCP client
        connectedAddr = null; // Nullify the connected address
        connectedPort = 0; // Zero the connected port

        // Re-initialize the TCP client
        tcpClient = new TcpClient();
    }

    /// <summary>
    /// Console command equivalent of <see cref="Disconnect"/>.
    /// </summary>
    [ConsoleCommand( "disconnect", "Disconnects the current local client from the server they're connected to." )]
    public static void DisconnectCmd()
    {
        // Simply calls the engine instance's client's disconnect method
        EngineManager.client.Disconnect();
    }

    /// <summary>
    /// Gets the address of this client.
    /// </summary>
    /// <returns>The address of this client.</returns>
    public string GetAddress()
    {
        return $"{localEndpoint.Address}:{localEndpoint.Port}";
    }

    /// <summary>
    /// Disposes of this network client.
    /// </summary>
    public void Dispose()
    {
        // Dispose the TCP client
        tcpClient.Dispose();

        // Nullify values
        connectedAddr = null;
        connectedPort = 0;
    }
}