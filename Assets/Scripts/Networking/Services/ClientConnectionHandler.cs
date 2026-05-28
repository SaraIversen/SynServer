using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Accepts and handles incoming client connections.
/// </summary>
public class ClientConnectionHandler
{
    public static async Task AcceptClientsAsync(TcpListener tcpListener, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

            Debug.Log($"SERVER: Incoming connection from {tcpClient.Client.RemoteEndPoint}...");

            if (ClientManager.IsFull)
            {
                Console.WriteLine($"{tcpClient.Client.RemoteEndPoint} failed to connect: Server full!");
                ClientManager.DenyClientConnection(tcpClient);
                continue;
            }

            ClientConnection clientConnection = ClientManager.RegisterClient(tcpClient);
            HandleClientConnected(clientConnection, cancellationToken);
        }
    }

    private static void HandleClientConnected(ClientConnection clientConnection, CancellationToken cancellationToken)
    {
        Debug.Log($"SERVER: New client connection from {clientConnection.TCP.Socket.Client.RemoteEndPoint}");

        _ = clientConnection.TCP.TCPReceiveLoop(cancellationToken); // Begin TCP receive loop.

        ServerSend.Welcome(clientConnection.Id, "Welcome to the server!");
    }
}
