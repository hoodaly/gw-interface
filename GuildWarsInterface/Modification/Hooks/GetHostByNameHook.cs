#region

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal static class GetHostByNameHook
        {
                private static HookType _hookDelegate;
                private static HookType _originalDelegate;

                private static IntPtr _hookAddress;

                public static string LastHostName { get; private set; }

                public static void Install()
                {
                        _hookDelegate = Hook;

                        IntPtr addr = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("ws2_32.dll"), "WSAAsyncGetHostByName");
                        _hookAddress = HookHelper.GetThunkLocation("ws2_32.dll", "WSAAsyncGetHostByName");

                        _originalDelegate = (HookType)Marshal.GetDelegateForFunctionPointer(addr, typeof(HookType));

                        HookHelper.Jump(_hookAddress, Marshal.GetFunctionPointerForDelegate(_hookDelegate));
                }

                private static int Hook(IntPtr hWnd, uint wMsg, IntPtr name, IntPtr buf, int buflen)
                {
                        string hostName = Marshal.PtrToStringAnsi(name);
                        if (hostName.StartsWith("File") && !hostName.StartsWith("File1"))
                        {
                                return 0; //WSAHOST_NOT_FOUND
                        }
                        LastHostName = hostName;
                        var ret = _originalDelegate(hWnd, wMsg, name, buf, buflen);
                        if (ret == 0)
                        {
                                return ret;
                        }
                        for (int i = 0; i < 20 && Marshal.ReadByte(buf) == 0; i++)
                        {
                                Thread.Sleep(100);
                        }
                        Thread.Sleep(100);
                        var addrBase = Marshal.ReadIntPtr(Marshal.ReadIntPtr(buf + 12));
                        var ipb = new byte[4];
                        Marshal.Copy(addrBase, ipb, 0, 4);
                        IPAddress ip = new IPAddress(ipb);
                        if (hostName.StartsWith("Auth"))
                        {
                                Marshal.Copy(new byte[] { 0x12, 0x12, 0x12, 0x12 }, 0, addrBase, 4);
                                Marshal.Copy(new byte[] { 0x12, 0x12, 0x12, 0x12 }, 0, addrBase+4, 4);
                                Debug.Log("Lookup " + hostName + " (Auth)");
                        }
                        else
                        if (hostName.StartsWith("File1."))
                        {
                                Marshal.Copy(new byte[] { 0x13, 0x13, 0x13, 0x13 }, 0, addrBase, 4);
                                Marshal.Copy(new byte[] { 0x13, 0x13, 0x13, 0x13 }, 0, addrBase+4, 4);
                                Debug.Log("Lookup " + hostName + " (F1)");
                        }
                        else
                        if (hostName.StartsWith("File"))
                        {
                                Marshal.Copy(new byte[] { 0x14, 0x14, 0x14, 0x14 }, 0, addrBase, 4);
                                Marshal.Copy(new byte[] { 0x14, 0x14, 0x14, 0x14 }, 0, addrBase+4, 4);
                                Debug.Log("Lookup " + hostName + " (F2)");
                        } else
                        {
                                // Gameserver
                                Marshal.Copy(new byte[] { 0x15, 0x15, 0x15, 0x15 }, 0, addrBase, 4);
                                Marshal.Copy(new byte[] { 0x15, 0x15, 0x15, 0x15 }, 0, addrBase+4, 4);
                                Debug.Log("Lookup " + hostName + " (Game)");
                        }
                        // null terminate the list
                        Marshal.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, addrBase + 8, 4);
                        return ret;
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                private delegate int HookType(IntPtr hWnd, uint wMsg, IntPtr name, IntPtr buf, int buflen);
        }
}
