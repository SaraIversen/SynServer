using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages client registration, Id's, max player check, and client disconnection.
/// </summary>
public static class ClientManager
{
    #region Instance Fields
    private static int _maxPlayers;

    private static int _nextClientId = 0;
    private static ConcurrentDictionary<int, ClientConnection> _clients = new();
    #endregion


    public static bool IsFull => _clients.Count >= _maxPlayers;
    private static int GetClientId() => Interlocked.Increment(ref _nextClientId);



    public static void Initalize(int maxPlayers)
    {
        _maxPlayers = maxPlayers;
    }

    public static IEnumerable<ClientConnection> GetAllClients()
    {
        return _clients.Values;
    }
        
    public static bool GetClientConnection(int clientId, out ClientConnection clientConnection)
    {
        return _clients.TryGetValue(clientId, out clientConnection);
    }

    public static ClientConnection RegisterClient(TcpClient tcpClient/*, CancellationToken cancellationToken*/)
    {
        int clientId = GetClientId();

        ClientConnection clientConnection = new ClientConnection(
            clientId,
            new ClientTCP(clientId, tcpClient),
            new ClientUDP(clientId)
        );

        if (!_clients.TryAdd(clientId, clientConnection))
        {
            CloseClientConnection(clientConnection);
            throw new Exception($"Failed to register client. Id collision.");
        }

        Debug.Log($"SERVER: Client registered with id: {clientId}");
        return clientConnection;
    }

    public static bool UnregisterClient(int clientId)
    {
        return _clients.TryRemove(clientId, out ClientConnection clientConnection);
    }

    private static void CloseClientConnection(ClientConnection clientConnection)
    {
        if (UnregisterClient(clientConnection.Id))
        {
            Debug.Log("SERVER: Disconnecting Client...");
            clientConnection.Disconnect();
            Debug.Log("SERVER: Client connection closed.");
        }
    }

    public static void DenyClientConnection(TcpClient client)
    {
        try
        {
            Debug.Log("SERVER: Disconnecting Client...");
            client?.Dispose();
            Debug.Log($"SERVER: Client connection closed");
        }
        catch (Exception ex)
        {
            Debug.Log($"SERVER: Exception while denying client: {ex.Message}");
        }
    }

    public static void Disconnect(ClientConnection clientConnection)
    {
        CloseClientConnection(clientConnection);
    }

    public static int DisconnectAll()
    {
        int disconnectedClients = 0;

        foreach (ClientConnection clientConnection in _clients.Values)
        {
            CloseClientConnection(clientConnection);
            disconnectedClients++;
        }
        _clients.Clear();

        return disconnectedClients;
    }
}
