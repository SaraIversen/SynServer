using UnityEngine;

public static class ServerSend
{
    #region Send Methods
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="toClient">The client to send the packet the packet to.</param>
    /// <param name="packet">The packet to send to the client.</param>
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        ClientManager.GetClientConnection(toClient, out ClientConnection clientConnection);
        _ = clientConnection.TCP.SendDataAsync(packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="toClient">The client to send the packet the packet to.</param>
    /// <param name="packet">The packet to send to the client.</param>
    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        ClientManager.GetClientConnection(toClient, out ClientConnection clientConnection);
        clientConnection.UDP.SendData(packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();

        foreach (ClientConnection clientConnection in ClientManager.GetAllClients())
        {
            _ = clientConnection.TCP.SendDataAsync(packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="exceptClient">The client to NOT send the data to.</param>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();

        foreach (ClientConnection clientConnection in ClientManager.GetAllClients())
        {
            if (clientConnection.Id != exceptClient)
            {
                _ = clientConnection.TCP.SendDataAsync(packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();

        foreach (ClientConnection clientConnection in ClientManager.GetAllClients())
        {
            clientConnection.UDP.SendData(packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();

        foreach (ClientConnection clientConnection in ClientManager.GetAllClients())
        {
            if (clientConnection.Id != exceptClient)
            {
                clientConnection.UDP.SendData(packet);
            }
        }
    }
    #endregion

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="toClient">The client to send the packet to.</param>
    /// <param name="msg">The message to send.</param>
    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)PacketId.welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }

    public static void Pong(int toClient)
    {
        using (Packet packet = new Packet((int)PacketId.pong))
        {
            SendUDPData(toClient, packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="toClient">The client that should spawn the player.</param>
    /// <param name="player">The player to spawn.</param>
    public static void SpawnPlayer(int toClient, Player player)
    {
        using (Packet packet = new Packet((int)PacketId.spawnPlayer))
        {
            packet.Write(player.Id);
            packet.Write(player.Username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            SendTCPData(toClient, packet);
        }
    }

    public static void PlayerDisconnected(int playerId)
    {
        using (Packet packet = new Packet((int)PacketId.playerDisconnected))
        {
            packet.Write(playerId);

            SendTCPDataToAll(packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="player">The player whose position to update.</param>
    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)PacketId.playerPosition))
        {
            packet.Write(player.Id);
            packet.Write(player.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)PacketId.playerRotation))
        {
            packet.Write(player.Id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.Id, packet);
        }
    }

    public static void PlayerHealth(Player player)
    {
        using (Packet packet = new Packet((int)PacketId.playerHealth))
        {
            packet.Write(player.Id);
            packet.Write(player.CurrentHealth);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerRespawned(Player player)
    {
        using (Packet packet = new Packet((int)PacketId.playerRespawned))
        {
            packet.Write(player.Id);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation.eulerAngles);

            SendTCPDataToAll(packet);
        }
    }

    public static void SpawnProjectile(Projectile projectile, Vector3 initialMovementDirection, Vector3 initialForce, int thrownByPlayer)
    {
        using (Packet packet = new Packet((int)PacketId.spawnProjectile))
        {
            packet.Write(projectile.Id);
            packet.Write(projectile.transform.position);
            packet.Write(initialMovementDirection);
            packet.Write(initialForce);
            packet.Write(thrownByPlayer);

            SendTCPDataToAll(packet);
        }
    }

    public static void ProjectilePosition(Projectile projectile)
    {
        using (Packet packet = new Packet((int)PacketId.projectilePosition))
        {
            packet.Write(projectile.Id);
            packet.Write(projectile.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void ProjectileExploded(Projectile projectile)
    {
        using (Packet packet = new Packet((int)PacketId.projectileExploded))
        {
            packet.Write(projectile.Id);
            packet.Write(projectile.transform.position);

            SendTCPDataToAll(packet);
        }
    }

    public static void SpawnEnemy(Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.spawnEnemy))
        {
            SendTCPDataToAll(SpawnEnemy_Data(enemy, packet));
        }
    }
    public static void SpawnEnemy(int toClient, Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.spawnEnemy))
        {
            SendTCPData(toClient, SpawnEnemy_Data(enemy, packet));
        }
    }

    private static Packet SpawnEnemy_Data(Enemy enemy, Packet packet)
    {
        packet.Write(enemy.Id);
        packet.Write(enemy.transform.position);
        return packet;
    }

    public static void EnemyPosition(Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.enemyPosition))
        {
            packet.Write(enemy.Id);
            packet.Write(enemy.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void EnemyRotation(Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.enemyRotation))
        {
            packet.Write(enemy.Id);
            packet.Write(enemy.transform.rotation);

            SendTCPDataToAll(packet);
        }
    }

    public static void EnemyHealth(Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.enemyHealth))
        {
            packet.Write(enemy.Id);
            packet.Write(enemy.CurrentHealth);

            SendTCPDataToAll(packet);
        }
    }

    public static void EnemyRespawned(Enemy enemy)
    {
        using (Packet packet = new Packet((int)PacketId.enemyRespawned))
        {
            packet.Write(enemy.Id);
            packet.Write(enemy.transform.position);
            packet.Write(enemy.transform.rotation);

            SendTCPDataToAll(packet);
        }
    }
    #endregion
}
