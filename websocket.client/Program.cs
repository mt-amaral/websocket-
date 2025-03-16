using System.Net.WebSockets;
using System.Text;

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5062/webteste"), CancellationToken.None);

var buffer = new byte[1024];

while (ws.State == WebSocketState.Open)
{
    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    if (result.MessageType == WebSocketMessageType.Close)
    {
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexão encerrada", CancellationToken.None);
        break;
    }

    string mensagem = Encoding.ASCII.GetString(buffer, 0, result.Count);
    Console.WriteLine($"Mensagem do servidor: {mensagem}");

    // Limpa o buffer
    Array.Clear(buffer, 0, buffer.Length);
}