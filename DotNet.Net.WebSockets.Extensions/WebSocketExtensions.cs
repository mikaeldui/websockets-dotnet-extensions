using System.Text;
// ReSharper disable UnusedMember.Global

namespace System.Net.WebSockets
{
    public static class WebSocketExtensions
    {
        /// <summary>
        /// Send a string through the <see cref="WebSocket"/>.
        /// </summary>
        public static async Task SendStringAsync(this WebSocket webSocket, string message, Encoding encoding, CancellationToken cancellationToken = default) =>
            await webSocket.SendAsync(new ArraySegment<byte>(encoding.GetBytes(message)), WebSocketMessageType.Text, true, cancellationToken);

        /// <summary>
        /// Send a <see cref="Encoding.UTF8"/> encoded string through the <see cref="WebSocket"/>.
        /// </summary>
        public static async Task SendStringAsync(this WebSocket webSocket, string message, CancellationToken cancellationToken = default) =>
            await SendStringAsync(webSocket, message, Encoding.UTF8, cancellationToken);

        /// <summary>
        /// Receive a string from the <see cref="WebSocket"/>.
        /// </summary>
        public static async Task<string> ReceiveStringAsync(this WebSocket webSocket, Encoding encoding, CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new(new byte[8192]);

            WebSocketReceiveResult result;

            using var ms = new MemoryStream();
            do
            {
                result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        ms.Write(buffer.Array!, buffer.Offset, result.Count);
                        break;
                    case WebSocketMessageType.Close:
                        throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
                    case WebSocketMessageType.Binary:
                    default:
                        throw new WebSocketException(WebSocketError.InvalidMessageType);
                }
            }
            while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(ms, encoding);
            return (await reader.ReadToEndAsync()).Trim('\0');
        }

        /// <summary>
        /// Receive a <see cref="Encoding.UTF8"/> encoded string from the <see cref="WebSocket"/>.
        /// </summary>
        public static async Task<string> ReceiveStringAsync(this WebSocket webSocket, CancellationToken cancellationToken = default) => await ReceiveStringAsync(webSocket, Encoding.UTF8, cancellationToken);
    }
}