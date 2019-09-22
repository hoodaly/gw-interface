#region

using GuildWarsInterface.Networking.Servers.Base;
using System;

#endregion

namespace GuildWarsInterface.Networking.Servers
{
        internal class FileServer : Server
        {
                internal const short PORT = 5112;

                protected override short Port
                {
                        get { return PORT; }
                }

                protected override void Received(byte[] data)
                {
                        if (data.Length <= 0)
                        {
                                return;
                        }
                        if (BitConverter.ToUInt16(data, 0) == 0x10f1)
                        {
                                return;
                        }
                        Send(new byte[] {
                                0xf1, 0x02, 0x20, 0x00, 0x61, 0xcb, 0x05, 0x00, 0x62, 0xcb, 0x05, 0x00, 0x9c, 0x43, 0x05, 0x00,
                                0x88, 0xcb, 0x05, 0x00, 0x89, 0xcb, 0x05, 0x00, 0x8a, 0xcb, 0x05, 0x00, 0x87, 0xcb, 0x05, 0x00
                        });
                }
        }
}