using System.Net;


public class ClientUDP
{
    private int _id;

    public IPEndPoint EndPoint;


    public ClientUDP(int id)
    {
        _id = id;
    }

    /// <summary>Initializes the newly connected client's UDP-related info.</summary>
    /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
    public void Connect(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    /// <summary>Sends data to the client via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public void SendData(Packet packet)
    {
        _ = ServerManager.Server.ServerUDP.SendUDPDataAsync(EndPoint, packet);
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_packetData">The packet containing the recieved data.</param>
    public void HandleData(Packet packetData)
    {
        int packetLength = packetData.ReadInt();
        byte[] packetBytes = packetData.ReadBytes(packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(packetBytes))
            {
                int packetId = packet.ReadInt();
                PacketRouter.PacketHandlers[packetId](_id, packet); // Call appropriate method to handle the packet
            }
        });
    }

    public void Disconnect()
    {
        EndPoint = null;
    }
}
