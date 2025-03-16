using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main()
    {
        // Inicia WebSocket
        _ = Task.Run(WebSocketClient);

        // Inicia SignalR
        await SignalRClient();
    }

    static async Task WebSocketClient()
    {
        using var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri("ws://localhost:5062/webteste"), CancellationToken.None);
        Console.WriteLine("Conectado ao WebSocket!");

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
            Console.WriteLine($"[WebSocket] {mensagem}");

            Array.Clear(buffer, 0, buffer.Length);
        }
    }

    static async Task SignalRClient()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5032/chatHub")
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"[SignalR] {user}: {message}");
        });

        await connection.StartAsync();
        Console.WriteLine("Conectado ao SignalR!");

        while (true)
        {
            Console.Write("Digite seu nome: ");
            var user = Console.ReadLine();
            Console.Write("Digite a mensagem: ");
            var message = Console.ReadLine();

            await connection.InvokeAsync("SendMessage", user, message);
        }
    }
}