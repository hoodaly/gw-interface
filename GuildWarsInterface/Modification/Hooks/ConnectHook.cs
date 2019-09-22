﻿#region

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

                private static readonly IntPtr _hookAddress = (IntPtr) 0x00403DEA;

                public static void Install()
                {
                        _hookDelegate = Hook;

                        IntPtr addr = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("ws2_32.dll"), "connect");

                        _originalDelegate = (HookType) Marshal.GetDelegateForFunctionPointer(addr, typeof (HookType));

                        HookHelper.Jump(_hookAddress, Marshal.GetFunctionPointerForDelegate(_hookDelegate));
                }

                private static int Hook(uint socket, IntPtr addr, uint addrLen)
                {
                        const short STANDARD_GAMESERVER_PORT = 9112;
                        short sin_family = Marshal.ReadInt16(addr, 0);
                        ushort sin_port = (ushort)Marshal.ReadInt16(addr, 2);
                        byte ip0 = Marshal.ReadByte(addr, 4);
                        byte ip1 = Marshal.ReadByte(addr, 5);
                        byte ip2 = Marshal.ReadByte(addr, 6);
                        byte ip3 = Marshal.ReadByte(addr, 7);

                        short port = Marshal.ReadInt16(addr + 2);

                        Debug.Log("Connect " + GetHostByNameHook.LastHostName + " (" + socket + ", " + BigEndian(port) + "): " + sin_family + ", " + sin_port + ", " + ip0 + "." + ip1 + "." + ip2 + "." + ip3);

                        if (BigEndian(port) != STANDARD_GAMESERVER_PORT && !(ip0 == 0x22 && ip1 == 0x22 && ip2 == 0x22 && ip3 == 0x22))
                        {
                                Marshal.WriteInt16(addr + 2, GetHostByNameHook.LastHostName.StartsWith("Auth") ? BigEndian(AuthServer.PORT) : BigEndian(FileServer.PORT));
                        }
                        else
                        {
                                Marshal.WriteInt16(addr + 2, BigEndian(GameServer.PORT));
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