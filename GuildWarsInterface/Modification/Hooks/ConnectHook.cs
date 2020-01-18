#region

using System;
using System.Net;
using System.Runtime.InteropServices;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;
using GuildWarsInterface.Networking.Servers;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal static class ConnectHook
        {
                private static HookType _hookDelegate;
                private static HookType _originalDelegate;

                private static IntPtr _hookAddress;

                public static void Install()
                {
                        _hookDelegate = Hook;

                        IntPtr addr = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("ws2_32.dll"), "connect");
                        _hookAddress = HookHelper.GetThunkLocation("ws2_32.dll", "connect");

                        _originalDelegate = (HookType) Marshal.GetDelegateForFunctionPointer(addr, typeof (HookType));

                        HookHelper.Jump(_hookAddress, Marshal.GetFunctionPointerForDelegate(_hookDelegate));
                }

                private static int Hook(uint socket, IntPtr addr, uint addrLen)
                {
                        const short STANDARD_GAMESERVER_PORT = 9112;

                        short port = Marshal.ReadInt16(addr + 2);

                        if (BigEndian(port) != STANDARD_GAMESERVER_PORT)
                        {
                                Marshal.WriteInt16(addr + 2, GetHostByNameHook.LastHostName.StartsWith("Auth") ? BigEndian((short)(AuthServer.PORT + Game.PortOffset)) : BigEndian((short)(FileServer.PORT + Game.PortOffset)));
                        }
                        else
                        {
                                Marshal.WriteInt16(addr + 2, BigEndian((short)(GameServer.PORT + Game.PortOffset)));
                        }

                        Marshal.WriteInt32(addr + 4, BitConverter.ToInt32(IPAddress.Loopback.GetAddressBytes(), 0));


                        return _originalDelegate(socket, addr, addrLen);
                }

                private static short BigEndian(short input)
                {
                        byte[] bytes = BitConverter.GetBytes(input);

                        return BitConverter.ToInt16(new[] {bytes[1], bytes[0]}, 0);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                private delegate int HookType(uint socket, IntPtr addr, uint addrLen);
        }
}
