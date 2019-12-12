#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Debugging;

#endregion

namespace GuildWarsInterface.Networking.Servers.Base
{
        internal abstract class Server : IControllerManager
        {
                private readonly Dictionary<int, Action<List<object>>> _controllers = new Dictionary<int, Action<List<object>>>();
                public Protocol.Protocol Protocol { get; set; }

                protected abstract short Port { get; }
                private TcpServer _server;

                public void RegisterHandler(int messageId, Action<List<object>> handler)
                {
                        _controllers.Add(messageId, handler);
                }

                protected virtual void ReceivedBase(byte[] data)
                {
                        Debug.LogBytes(data, "<-");
                        Received(data);
                }

                protected virtual void Received(byte[] data)
                {
                        try
                        {
                                List<object> packet;
                                while (Protocol.Deserialize(data, out packet, out data))
                                {
                                        Debug.Log(GetType().Name + ": " + packet[0]);
                                        Debug.LogBytes(data, "=>");

                                        if (_controllers.ContainsKey((int)packet[0]))
                                        {
                                                _controllers[(int)packet[0]](packet);
                                        }

                                        if (data.Length == 0) break;
                                }
                        }
                        catch (Exception e)
                        {
                                Debug.LogBytes(data, "ERROR (" + e.GetType().ToString() + ": " + e.Message + "): ->");
                                // TODO: auth 0 seems to cause problems
                        }
                }

                public void Start()
                {
                        _server = new TcpServer(Port);
                        _server.AddReceivedHandler(msg =>
                        {
                                Received(msg);
                        });
                        _server.Start();
                }
                
                protected void Send(int messageId, params object[] parameters)
                {
                        try
                        {
                                using (var stream = new MemoryStream())
                                using (var writer = new BinaryWriter(stream))
                                {
                                        writer.Write((ushort)messageId);

                                        for (int i = 0; i < parameters.Length; i++)
                                        {
                                                dynamic value = parameters[i];

                                                if (value is string) value = value.ToCharArray();

                                                if (value is Array)
                                                {
                                                        switch (Protocol.PrefixSize(messageId, i + 1))
                                                        {
                                                                case 1:
                                                                        writer.Write((byte) value.Length);
                                                                        break;
                                                                case 2:
                                                                        writer.Write((ushort) value.Length);
                                                                        break;
                                                        }

                                                        foreach (dynamic element in value)
                                                        {
                                                                if (element is char)
                                                                {
                                                                        writer.Write((ushort) element);
                                                                }
                                                                else
                                                                {
                                                                        writer.Write(element);
                                                                }
                                                        }
                                                }
                                                else
                                                {
                                                        writer.Write(value);
                                                }
                                        }

                                        Send(stream.ToArray());
                                }
                        }
                        catch (SocketException e)
                        {
                                Debug.Log("Send error: " + e.Message + ": " + e.StackTrace);
                        }
                }

                public void Send(byte[] data)
                {
                        Debug.LogBytes(data, "->");
                        _server.DoBroadcast(data);
                }

                public void Disconnect()
                {
                        _server.Stop();
                }
        }

        public class TcpServer : ServerBase
        {
                public TcpServer(int port)
                {
                        Port = port;
                        // This allows us to safely invoke Broadcast. Since we assume that there's going to be multiple clients, this is a good trade-off.
                        Broadcast += _ => { };
                }

                private event Action<byte[]> Broadcast;

                private event Action<byte[]> Received;

                public void AddReceivedHandler(Action<byte[]> arg)
                {
                        Received += arg;
                }

                public void DoBroadcast(byte[] data)
                {
                        Broadcast(data);
                }

                protected override async Task Accept(TcpClient client)
                {
                        try
                        {
                                var stream = client.GetStream();

                                // Note that local variables are effectively local with respect to any individual connection. Pretty handy - we get a state machine for free.
                                try
                                {
                                        CancellationToken.ThrowIfCancellationRequested();

                                        // Start the main message loop
                                        await HandleMessages(stream);
                                }
                                finally
                                {
                                }
                        }
                        finally
                        {
                                client.Close();
                        }
                }

                async Task HandleMessages(Stream stream)
                {

                        Action<byte[]> broadcastHandler =
                                message =>
                                {
                                        stream.WriteAsync(message, 0, message.Length);
                                };

                        try
                        {
                                Broadcast += broadcastHandler;

                                byte[] recvBuffer = new byte[8192];

                                while (!CancellationToken.IsCancellationRequested)
                                {
                                        var bytesRead = await stream.ReadAsync(recvBuffer, 0, 8192);
                                        if (bytesRead > 0)
                                        {
                                                Received(SubArray(recvBuffer, 0, bytesRead));
                                        }
                                }
                        }
                        finally
                        {
                                // We have to unregister the broadcast message handler. This is quite important - global event handlers are a great way to introduce memory leaks :)
                                Broadcast -= broadcastHandler;
                        }

                }

                private static byte[] SubArray(byte[] array, int start, int count)
                {
                        var result = new byte[count];
                        Array.Copy(array, start, result, 0, count);
                        return result;
                }
        }

        public abstract class ServerBase
        {
                private TcpListener listener;
                private CancellationTokenSource cts;
                private CancellationToken token;

                protected CancellationToken CancellationToken { get { return this.token; } }

                public int Port;

                public void Start()
                {
                        cts = new CancellationTokenSource();
                        token = cts.Token;

                        listener = new TcpListener(IPAddress.Any, Port + Game.PortOffset);
                        listener.Start();

                        // Note that we're not awaiting here - this is going to return almost immediately. 
                        // We're storing the task in a variable to make it explicit that this is not a case of forgotten await :)
                        var _ = Listen();
                }

                async Task Listen()
                {
                        var client = default(TcpClient);

                        while (!token.IsCancellationRequested)
                        {
                                try
                                {
                                        client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                                }
                                catch (ObjectDisposedException)
                                {
                                        // The listener has been stopped.
                                        return;
                                }

                                if (client == null) return;

                                // Again, there's no await - the Accept handler is going to return immediately so that we can handle the next client.
                                var _ = Accept(client);
                        }
                }

                protected abstract Task Accept(TcpClient client);

                public void Stop()
                {
                        cts.Cancel();
                        listener.Stop();
                }
        }
}