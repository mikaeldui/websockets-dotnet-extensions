namespace System.Net.WebSockets
{
    public static class ClientWebSocketExtensions
    {
        public static async Task ConnectAsync(this ClientWebSocket client, string uri, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));

            await client.ConnectAsync(new Uri(uri), cancellationToken);
        }
    }
}