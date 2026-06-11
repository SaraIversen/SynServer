using System.Threading;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    #region Instance Fields
    public static Server Server;

    [SerializeField] private int _tcpPort = 5000;
    [SerializeField] private int _udpPort = 5001;
    [SerializeField] private int _maxClients = 20;

    private CancellationTokenSource _cts;
    private CancellationToken _serverToken;
    #endregion

    void Start()
    {
        _cts = new CancellationTokenSource();
        _serverToken = _cts.Token;

        Server = new Server(_tcpPort, _udpPort, _maxClients);
        Server.Start(_serverToken);
    }

    void OnDestroy()
    {
        _cts?.Cancel();
        Server?.Stop();
        _cts?.Dispose();
    }
}
