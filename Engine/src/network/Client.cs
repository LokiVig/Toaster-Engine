using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using System.Text;

namespace Toast.Engine.Network;

/// <summary>
/// A network client. Can connect to servers through IPs, etc.
/// </summary>
public class Client : IDisposable
{
    public TcpClient tcpClient; // The actual TCP client
    public IPAddress connectedAddr; // The string representation of the IP address we're currently connected to
    public ushort connectedPort; // The port of the server we're currently connected to

    /// <summary>
    /// Creates a new network client object.
    /// </summary>
    public Client()
    {
        // Initialize our TCP client
        tcpClient = new TcpClient();
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

            // Do certain things upon connecting
            OnConnect();
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
    public static void ConnectTo_Command( List<object> args )
    {
        // Get the amount of arguments
        int argCount = args.Count - 1;

        // If we have an invalid amount of arguments...
        if ( argCount < 2 || argCount > 2 )
        {
            Log.Error( "Invalid amount of arguments! You need to specify at least, and at most, 2 arguments, one for the IP address of the server you wish to connect to, and the other for the port." );
            return;
        }

        // Try to call the default ConnectTo command with the input arguments...
        try
        {
            EngineManager.client.ConnectTo( (string)args[1], ushort.Parse( (string)args[2] ) );
        }
        catch ( Exception exc ) // If an exception is caught...
        {
            // Log it!
            Log.Error( $"Error caught when trying to connect to {args[1]}:{args[2]}! {exc.Message}" );
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

            OnConnect();
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
            Log.Error( $"Error connecting to {addr}:{port}!\n{exc.Message}" );

            // We shouldn't do anything more
            return false;
        }

        // Make sure we *actually* connected properly
        if ( tcpClient.Connected )
        {
            Log.Info( $"Successfully connected to {addr}:{port}!", true );
            connectedAddr = addr;
            connectedPort = port;

            OnConnect();

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
    /// <param name="server">The specific server we wish to try to connect to</param>
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

    [ConsoleCommand( "disconnect", "Disconnects the current local client from the server they're connected to." )]
    public static void Disconnect_Command()
    {
        Client cl = EngineManager.client;
        cl.Disconnect();
    }

    /// <summary>
    /// Things to do when this client's connected
    /// </summary>
    private void OnConnect()
    {
        tcpClient.GetStream().Write(Encoding.ASCII.GetBytes("Client Connected"));
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