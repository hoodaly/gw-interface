#region

using System;
using System.Collections.Generic;
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
                        byte[] ip = new byte[4];
                        Marshal.Copy(addr + 4, ip, 0, 4);
                        byte srv_id = Marshal.ReadByte(addr + 4);
                        SendHook.TypeDict[socket] = (uint)port;

                        // change port based on target
                        if (GetHostByNameHook.LookedupAuthIPs.FindAll(a => a.Equals(new IPAddress(ip))).Count > 0)
                        {
                                Marshal.WriteInt16(addr + 2, BigEndian((short)(AuthServer.PORT + Game.PortOffset)));
                        }
                        else if (GetHostByNameHook.LookedupFileIPs.FindAll(a => a.Equals(new IPAddress(ip))).Count > 0)
                        {
                                Marshal.WriteInt16(addr + 2, BigEndian((short)(FileServer.PORT + Game.PortOffset)));
                                SendHook.TypeDict[socket] = 0x9999;
                        }
                        else
                        {
                                Marshal.WriteInt16(addr + 2, BigEndian((short)(GameServer.PORT + Game.PortOffset)));
                                SendHook.TypeDict[socket] = (uint)STANDARD_GAMESERVER_PORT;
                        }
                        Debug.Log("Connecting (" + socket + "): " + srv_id + "(" + BitConverter.ToString(ip) + ":" + port + ")");
                        
                        // Uncomment for original server
                        //return _originalDelegate(socket, addr, addrLen);

                        // Connect to 127.0.0.1
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
