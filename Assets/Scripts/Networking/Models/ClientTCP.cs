using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ClientTCP
{
    public static int DataBufferSize = 4096;

    private readonly int _id;


    public TcpClient Socket { get; set; }
    public NetworkStream Stream { get; set; }
    public Packet ReceivedData { get; set; }
    public byte[] ReceiveBuffer { get; set; }


    public ClientTCP(int id, TcpClient tcpClient)
    {
        _id = id;

        Socket = tcpClient;
        Socket.ReceiveBufferSize = DataBufferSize;
        Socket.SendBufferSize = DataBufferSize;

        Stream = Socket.GetStream();
        ReceivedData = new Packet();
        ReceiveBuffer = new byte[DataBufferSize];
    }


    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public async Task SendDataAsync(Packet packet)
    {
        try
        {
            if (Socket != null)
            {
                await Stream.WriteAsync(packet.ToArray(), 0, packet.Length()).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to server via TCP: {ex}");
        }
    }


    /// <summary>Reads incoming data from the stream.</summary>
    public async Task TCPReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await Stream.ReadAsync(ReceiveBuffer, 0, ReceiveBuffer.Length, cancellationToken).ConfigureAwait(false);
                if (bytesRead <= 0)
                {
                    if (ClientManager.GetClientConnection(_id, out ClientConnection clientConnection))
                    {
                        ClientManager.Disconnect(clientConnection);
                    }
                    return;
                }

                byte[] _data = new byte[bytesRead];
                Array.Copy(ReceiveBuffer, _data, bytesRead);

                ReceivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Client {_id} TCP Receive Loop Error: {ex}");

            if (ClientManager.GetClientConnection(_id, out ClientConnection clientConnection))
            {
                ClientManager.Disconnect(clientConnection);
            }
        }
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_data">The recieved data.</param>
    private bool HandleData(byte[] _data)
    {
        int _packetLength = 0;

        ReceivedData.ReadBytes(_data);

        if (ReceivedData.UnreadLength() >= 4) // Make sure there is enough bytes to read an int (packet length)
        {
            _packetLength = ReceivedData.ReadInt();
            if (_packetLength <= 0)
            {
                // If packet contains no data
                return true; // Reset receivedData instance to allow it to be reused
            }
        }

        // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
        while (_packetLength > 0 && _packetLength <= ReceivedData.UnreadLength())
        {
            byte[] _packetBytes = ReceivedData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    PacketRouter.PacketHandlers[_packetId](_id, _packet); // Call appropriate method to handle the packet
                }
            });

            _packetLength = 0; // Reset packet length
            if (ReceivedData.UnreadLength() >= 4) // If client's received data contains data from another packet
            {
                _packetLength = ReceivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }
        }

        if (_packetLength <= 1) // There are reamining bytes but they can not be processed since we need at least 4 bytes to read the next packet's length
        {
            return true; // Reset receivedData instance to allow it to be reused
        }

        return false;
    }

    public void Disconnect()
    {
        Socket?.Dispose();
        Stream?.Dispose();

        Stream = null;
        ReceivedData = null;
        ReceiveBuffer = null;
        Socket = null;
    }
}


