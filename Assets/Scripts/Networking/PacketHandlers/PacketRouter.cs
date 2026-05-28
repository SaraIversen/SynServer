using System.Collections.Concurrent;
using UnityEngine;

public static class PacketRouter
{
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static ConcurrentDictionary<int, PacketHandler> PacketHandlers;


    /// <summary>Initializes all necessary server data.</summary>
    public static void InitializeServerData()
    {
        PacketHandlers = new ConcurrentDictionary<int, PacketHandler>()
        {
            [(int)PacketId.welcomeReceived] = ServerHandle.WelcomeReceived,
            [(int)PacketId.ping] = ServerHandle.Ping,
            [(int)PacketId.playerMovement] = ServerHandle.PlayerMovement,
            [(int)PacketId.playerShoot] = ServerHandle.PlayerShoot,
        };
        Debug.Log("Initialized server packets.");
    }
}
