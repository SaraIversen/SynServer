using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles the start/stop server lifecycle.
/// </summary>
public class Server : IServer
{
    public static int MaxPlayers { get; private set; }
    public static int TcpPort { get; private set; }
    public static int UdpPort { get; private set; }

    public static TcpListener TcpListener { get; private set; }
    public static UdpClient UdpListener { get; private set; }


    public bool IsRunning { get; private set; }


    /// <summary>
    /// Initializes the server so it is ready to be started.
    /// </summary>
    /// <param name="tcpPort">The tcp port to listen for clients on.</param>
    /// <param name="udpPort">The udp port to receive udp packets on.</param>
    /// <param name="maxPlayers">The maximum players that can be connected simultaneously.</param>
    public Server(int tcpPort, int udpPort, int maxPlayers)
    {
        TcpPort = tcpPort;
        UdpPort = udpPort;
        MaxPlayers = maxPlayers;

        TcpListener = new TcpListener(IPAddress.Any, TcpPort);
        UdpListener = new UdpClient(UdpPort);

        PacketRouter.InitializeServerData();
        ClientManager.Initalize(MaxPlayers);
    }

    /// <summary>Starts the server.</summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Debug.Log("SERVER: Starting server...");

        TcpListener.Start(); // Begins to listen for incoming TCP connection requests.
        _ = ServerUDP.UDPReceiveLoop(cancellationToken); // Starts UDP receive loop.

        IsRunning = true;

        Console.WriteLine($"SERVER: Server started on port {TcpPort}(tcp) and {UdpPort}(udp)");

        await ClientConnectionHandler.AcceptClientsAsync(TcpListener, cancellationToken);
    }

    public void Stop()
    {
        int disconnectedClients = ClientManager.DisconnectAll();
        Debug.Log($"Disconnected {disconnectedClients} clients");

        TcpListener?.Stop();
        UdpListener?.Dispose();

        IsRunning = false;
    }
}

