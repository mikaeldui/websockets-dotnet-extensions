using System.Text;

namespace System.Net.WebSockets
{
    public static class WebSocketExtensions
    {
        public static async Task SendStringAsync(this WebSocket webSocket, string message, CancellationToken cancellationToken = default) =>
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, cancellationToken);

        public static async Task<string> ReceiveStringAsync(this WebSocket webSocket, CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new();
            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
            return result.MessageType switch
            {
                WebSocketMessageType.Text => Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count).Trim('\0'),
                WebSocketMessageType.Close => throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely),
                _ => throw new WebSocketException(WebSocketError.InvalidMessageType),
            };
        }
    }
}