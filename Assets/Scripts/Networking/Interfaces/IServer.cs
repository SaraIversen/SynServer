using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Handles the start/stop server lifecycle.
/// </summary>
public interface IServer
{
    bool IsRunning { get; }

    /// <summary>
    /// Starts the server asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Takes a cancellation token to be able to gracefully cancel server operations.</param>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();
}
