using System.Net;
using System.Net.Sockets;
using Garnet;
using Garnet.server;

namespace ResponseCaching.Test.WebHost;

public static class GarnetServerUtil
{
    #region Public 方法

    public static GarnetServerDescriptor Create()
    {
        var garnetServerOptions = new GarnetServerOptions()
        {
            EndPoints = [new IPEndPoint(IPAddress.Loopback, 0)],
        };

        var garnetServer = new GarnetServer(garnetServerOptions);
        garnetServer.Start();
        var garnetEndpoint = GetGarnetServerEndPoint(garnetServer);

        return new(garnetServer, garnetEndpoint);
    }

    #endregion Public 方法

    #region Private 方法

    private static EndPoint GetGarnetServerEndPoint(GarnetServer garnetServer)
    {
        var serversField = garnetServer.GetType().GetField("servers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var servers = (object[])serversField.GetValue(garnetServer)!;
        var server = servers[0];
        var listenSocketField = server.GetType().GetField("listenSocket", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var listenSocket = (Socket)listenSocketField.GetValue(server)!;
        return listenSocket.LocalEndPoint!;
    }

    #endregion Private 方法
}

public sealed record GarnetServerDescriptor(GarnetServer GarnetServer, EndPoint GarnetServerEndpoint) : IDisposable
{
    public string RedisConfigureString { get; } = $"{GarnetServerEndpoint},allowAdmin=true";

    public void Dispose() => GarnetServer.Dispose();
}
