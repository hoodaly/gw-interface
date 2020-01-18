#region

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;
using GuildWarsInterface.Networking.Servers;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal static class SendHook
        {
                private static HookType _hookDelegate;
                private static HookType _originalDelegate;

                private static IntPtr _hookAddress;

                public static void Install()
                {
                        _hookDelegate = Hook;

                        IntPtr addr = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("ws2_32.dll"), "send");
                        _hookAddress = HookHelper.GetThunkLocation("ws2_32.dll", "send");

                        _originalDelegate = (HookType) Marshal.GetDelegateForFunctionPointer(addr, typeof (HookType));

                        HookHelper.Jump(_hookAddress, Marshal.GetFunctionPointerForDelegate(_hookDelegate));
                }

                private static int Hook(uint socket, IntPtr buf, int len, int flags)
                {
                        byte[] bs = new byte[len];
                        for (int i = 0; i < len; i++)
                        {
                                bs[i] = Marshal.ReadByte(buf, i);
                        }
                        //byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(str);
                        if (!(bs[0] == 0 && (bs[1] == 0 || bs[1] == 0x80))) {
                                Debug.LogBytes(bs, "Send (" + socket.ToString() + "): ");
                        }
                        
                        return _originalDelegate(socket, buf, len, flags);
                }

                private static short BigEndian(short input)
                {
                        byte[] bytes = BitConverter.GetBytes(input);

                        return BitConverter.ToInt16(new[] {bytes[1], bytes[0]}, 0);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                private delegate int HookType(uint socket, IntPtr buf, int len, int flags);
        }
}
