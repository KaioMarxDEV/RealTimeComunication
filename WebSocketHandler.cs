using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;


public class WebSocketHandler
{
  private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
  private readonly ILogger<WebSocketHandler> _logger;

  public WebSocketHandler(ILogger<WebSocketHandler> logger)
  {
    _logger = logger;
  }

  public string CreateConnection(WebSocket socket)
  {
    var conId = Guid.NewGuid().ToString();
    _sockets.TryAdd(conId, socket);

    _logger.LogInformation($"Connection with ID {conId} opened.");

    return conId;
  }

  public async Task ReceiveMessage(string connectionId, WebSocketReceiveResult result, byte[] buffer)
  {
    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
    _logger.LogInformation($"Message received on connection {connectionId}: {message}");

    // Implement your logic to handle the received message here
  }
}
