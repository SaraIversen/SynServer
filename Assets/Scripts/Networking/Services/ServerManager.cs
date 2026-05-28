using System.Threading;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    #region Instance Fields
    private Server _server;

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

        _server = new Server(_tcpPort, _udpPort, _maxClients);
        _ = _server.StartAsync(_serverToken);
    }

    void OnDestroy()
    {
        _cts?.Cancel();
        _server?.Stop();
        _cts?.Dispose();
    }
}
