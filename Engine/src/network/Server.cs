using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using Steamworks;
using Steamworks.Data;

namespace Toast.Engine.Network;

/// <summary>
/// A network server. Holds a list of connected clients and manages their connections and etc.
/// </summary>
public class Server : ISocketManager
{
    public void OnConnecting( Connection connection, ConnectionInfo data )
    {
        connection.Accept();
        Log.Info( $"{data.Identity} is connecting..." );
    }

    public void OnConnected( Connection connection, ConnectionInfo data )
    {
        Log.Info( $"{data.Identity} has connected!" );
    }

    public void OnDisconnected( Connection connection, ConnectionInfo data )
    {
        Log.Info( $"{data.Identity} has disconnected!" );
    }
    
    public void OnMessage( Connection conn, NetIdentity ident, IntPtr data, int size, long msgNum, long recvTime, int channel )
    {
        Log.Info( $"We got a message from {ident}!" );

        // Send it right back
        conn.SendMessage( data, size, SendType.Reliable );
    }
}