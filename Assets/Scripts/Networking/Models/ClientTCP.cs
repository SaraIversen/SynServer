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
    public void SendData(Packet _packet)
    {
        try
        {
            if (Socket != null)
            {
                Stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to server
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to server via TCP: {_ex}");
        }
    }


    /// <summary>Reads incoming data from the stream.</summary>
    public async Task TCPReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await Stream.ReadAsync(ReceiveBuffer, 0, ReceiveBuffer.Length, cancellationToken);
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
        catch
        {
            //Debug.Log($"Error receiving TCP data: {_ex}");
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

        if (ReceivedData.UnreadLength() >= 4)
        {
            // If client's received data contains a packet
            _packetLength = ReceivedData.ReadInt();
            if (_packetLength <= 0)
            {
                // If packet contains no data
                return true; // Reset receivedData instance to allow it to be reused
            }
        }

        while (_packetLength > 0 && _packetLength <= ReceivedData.UnreadLength())
        {
            // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
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
            if (ReceivedData.UnreadLength() >= 4)
            {
                // If client's received data contains another packet
                _packetLength = ReceivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }
        }

        if (_packetLength <= 1)
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


