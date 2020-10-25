#region

using GuildWarsInterface.Modification.Hooks;
using GuildWarsInterface.Modification.Patches;
using GuildWarsInterface.Networking.Servers;
using System;
using System.Runtime.InteropServices;

#endregion

namespace GuildWarsInterface.Networking
{
        internal static class Network
        {
                private static readonly FileServer _fileServer = new FileServer();
                public static readonly AuthServer AuthServer = new AuthServer();
                public static readonly GameServer GameServer = new GameServer();

                public static void Initialize()
                {
                        GetHostByNameHook.Install();
                        ConnectHook.Install();
                        SendHook.InstallSend();
                        SendHook.InstallRecv();
                        PortPatch.Apply();
                        DisableEncryptionPatch.Apply();
                        GameProtocolHook.Install2((msgid, msg) =>
                        {
                                Debugging.Debug.Log("msg: " + msgid.ToString());
                        });

                        GameProtocolHook.Install((gameProtocol, type) =>
                        {
                                Debugging.Debug.Log(gameProtocol.ToString() + ", " + type.ToString());
                                if (type == 0)
                                {
                                        GameServer.Protocol = new Protocol.Protocol(gameProtocol);
                                }
                                else if (type == 3)
                                {
                                        AuthServer.Protocol = new Protocol.Protocol(gameProtocol);
                                } else
                                {
                                        Debugging.Debug.Log("Unknown protocol type: " + gameProtocol.ToString() + ", " + type.ToString());
                                }
                        });

                        _fileServer.Start();
                        AuthServer.Start();
                        GameServer.Start();
                }
        }
}
