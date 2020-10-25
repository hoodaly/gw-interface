#region

using System;
using System.Collections.Generic;
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

                public static List<IPAddress> LookedupAuthIPs { get; private set; } = new List<IPAddress>();
                public static List<IPAddress> LookedupFileIPs { get; private set; } = new List<IPAddress>();

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
                                return 0;
                        }
                       var ret = _originalDelegate(hWnd, wMsg, name, buf, buflen);
                        if (ret == 0)
                        {
                                return ret;
                        }
                        for (int i = 0; i < 20 && Marshal.ReadByte(buf) == 0; i++)
                        {
                                Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        var addrBase = Marshal.ReadIntPtr(Marshal.ReadIntPtr(buf + 12));
                        var ipb = new byte[4];
                        IPAddress end = new IPAddress(new byte[]{ 0, 0, 0, 0});
                        for (int i = 0; ; i++)
                        {
                                Marshal.Copy(addrBase+i*4, ipb, 0, 4);
                                IPAddress a = new IPAddress(ipb);
                                if (a.Equals(end)) {
                                        break;
                                }
                                if (hostName.StartsWith("File"))
                                {
                                        LookedupFileIPs.Add(a);
                                } else if(hostName.StartsWith("Auth"))
                                {
                                        LookedupAuthIPs.Add(a);
                                }
                        }
                        return ret;
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                private delegate int HookType(IntPtr hWnd, uint wMsg, IntPtr name, IntPtr buf, int buflen);
        }
}
