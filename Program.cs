using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSockets();
builder.Services.AddSingleton<WebSocketHandler>();
// builder.Services.AddSingleton<RedisStore>(new RedisStore("localhost:6379"));

var app = builder.Build();

app.UseWebSockets();

app.Use(async (context, next) =>
{
  if (context.Request.Path == "/ws")
  {
    if (context.WebSockets.IsWebSocketRequest)
    {
      using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

      var handler = app.Services.GetRequiredService<WebSocketHandler>();

      var connectionId = handler.CreateConnection(webSocket);

      while (webSocket.State == WebSocketState.Open)
      {
        var buffer = new byte[4096];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        await handler.ReceiveMessage(connectionId, result, buffer);
      }
    }
    else
    {
      context.Response.StatusCode = 400;
    }
  }
  else
  {
    await next();
  }
});

app.Run("http://localhost:3000");
