using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServerUDP
{
    public UdpClient UdpListener { get; private set; }

    public ServerUDP(UdpClient udpListener)
    {
        UdpListener = udpListener;
    }

    public async Task UDPReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await UdpListener.ReceiveAsync().ConfigureAwait(false);

                byte[] data = result.Buffer;
                IPEndPoint clientEndPoint = result.RemoteEndPoint;

                if (data.Length < 4)
                {
                    continue;
                }

                using (Packet packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();

                    if (clientId == 0)
                    {
                        continue;
                    }

                    if (!ClientManager.GetClientConnection(clientId, out ClientConnection client)) continue;

                    if (client.UDP.EndPoint == null)
                    {
                        client.UDP.Connect(clientEndPoint);
                        continue;
                    }

                    if (client.UDP.EndPoint.Equals(clientEndPoint))
                    {
                        client.UDP.HandleData(packet);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving UDP data: {ex}");
        }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public async Task SendUDPDataAsync(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                await UdpListener.SendAsync(packet.ToArray(), packet.Length(), clientEndPoint).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
        }
    }
}
