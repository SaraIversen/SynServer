using UnityEngine;

public static class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        if (!ClientManager.GetClientConnection(fromClient, out ClientConnection clientConnection)) return;

        Debug.Log($"{clientConnection.TCP.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");

        clientConnection.SendIntoGame(username);
    }

    public static void Ping(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        ServerSend.Pong(clientId);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();

        if (!ClientManager.GetClientConnection(fromClient, out ClientConnection clientConnection)) return;
        clientConnection.Player.SetInput(inputs, rotation);
    }

    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 shootDirection = packet.ReadVector3();

        if (!ClientManager.GetClientConnection(fromClient, out ClientConnection clientConnection)) return;
        clientConnection.Player.Shoot(shootDirection);
    }
}
