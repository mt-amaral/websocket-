using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120) 
};
app.UseWebSockets(webSocketOptions);

app.Map("/webteste", async (HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var buffer = new byte[1024];

    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var data = Encoding.ASCII.GetBytes($".Net -> {DateTime.Now}");
            await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(1000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro no WebSocket: {ex.Message}");
    }
});

await app.RunAsync();