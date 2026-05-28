using System;

public class ClientConnection 
{
    public int Id { get; }
    public Player Player { get; private set; }

    public ClientTCP TCP { get; }
    public ClientUDP UDP { get; }


    public ClientConnection(int id, ClientTCP tcp, ClientUDP udp)
    {
        Id = id;

        TCP = tcp;
        UDP = udp;
    }

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="_playerName">The username of the new player.</param>
    public void SendIntoGame(string playerName)
    {
        Player = PlayerManager.Instance.SpawnPlayer(Id, playerName);

        // Send the new player to all players (including himself)
        foreach (ClientConnection client in ClientManager.GetAllClients())
        {
            if (client.Player != null)
            {
                ServerSend.SpawnPlayer(client.Id, Player);
            }
        }

        // Send all players to the new player
        foreach (ClientConnection client in ClientManager.GetAllClients())
        {
            if (client.Player != null)
            {
                if (client.Id != Id)
                { 
                    ServerSend.SpawnPlayer(Id, client.Player);
                }
            }
        }

        foreach (Enemy enemy in EnemyManager.Instance.GetAllEnemies())
        {
            if (!enemy.IsSpawned) return;

            ServerSend.SpawnEnemy(Id, enemy);
        }
    }

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
        Console.WriteLine($"{TCP.Socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(Player.gameObject);
            Player = null;
        });

        TCP.Disconnect();
        UDP.Disconnect();

        ServerSend.PlayerDisconnected(Id);
    }
}
