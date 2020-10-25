#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Servers;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal static class SendHook
        {
                private static HookType _hookDelegateSend;
                private static HookType _hookDelegateRecv;
                private static HookType _originalDelegate;
                public static Dictionary<uint, uint> TypeDict = new Dictionary<uint, uint>();

                private static IntPtr _hookAddress;

                public static void InstallSend()
                {
                        _hookDelegateSend = HookSend;
                        var addrs = HookHelper.searchAsm(new byte[] { 0x8d, 0x8e, 0x7c, 0x01, 0x00, 0x00, 0x89, 0x45, 0xf8 });
                        Debug.Requires(addrs.Count == 1);
                        _hookAddress = (IntPtr)addrs[0];

                        IntPtr codeCave = HookHelper.MakeCodeCave(new[]
                        {
                                "pushad",
                                "lea ecx, [esi + 0x17c]",
                                "mov dword[ebp-0x8], eax",
                                "push edi",
                                "push ebx",
                                "mov eax, [ebp + 8]", //param1
                                "mov eax, [eax]",
                                "push dword[eax+0x40]", //type2 - placeholder
                                "call " + Marshal.GetFunctionPointerForDelegate(_hookDelegateSend),
                                "popad",
                                "lea ecx, [esi + 0x17c]",
                                "jmp " + (_hookAddress+9)
                        });

                        HookHelper.Jump(_hookAddress, codeCave);
                }

                public static void InstallRecv()
                {
                        _hookDelegateRecv = HookRecv;
                        var addrs = HookHelper.searchAsm(new byte[] { 0xff, 0x75, 0x14, 0x8d, 0x04, 0x1f });
                        Debug.Requires(addrs.Count == 1);
                        var _hookAddressPre = (IntPtr)addrs[0] - 14;
                        var _hookAddressPost = (IntPtr)addrs[0];

                        IntPtr codeCavePre = HookHelper.MakeCodeCave(new[]
                        {
                                "push ebx", //len
                                "push edi", //dest
                                "mov eax, [ebp + 8]", //param1
                                "mov eax, [eax]",
                                "push dword[eax+0x40]", //type2 - placeholder
                                "push edi",
                                "push esi",
                                "push ebx",
                                "mov esi, [ebp + 8]",
                                "lea ecx, [esi + 0x74]",
                                //"mov dword[esp + 0xc], ecx", //store key in placeholder
                                "jmp " + (_hookAddressPre+9) // calls Rc4 and takes 3 arguments off the stack
                        });

                        IntPtr codeCavePost = HookHelper.MakeCodeCave(new[]
                        {
                                "mov eax, esp",
                                "pushad",
                                "mov ebx, [eax+8]", //len
                                "push ebx",
                                "mov ebx, [eax+4]", //dest
                                "push ebx",
                                "mov ebx, [eax]", //type
                                "push ebx",
                                "call " + Marshal.GetFunctionPointerForDelegate(_hookDelegateRecv),
                                "popad",
                                "push dword[ebp + 0x14]",
                                "lea eax, [edi + ebx]",
                                "jmp " + (_hookAddressPost+6)
                        });

                        HookHelper.Jump(_hookAddressPre, codeCavePre);
                        HookHelper.Jump(_hookAddressPost, codeCavePost);
                }

                private static void HookSend(uint socket, IntPtr buf, int len)
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
                }

                private static void HookRecv(uint socket, IntPtr buf, int len)
                {
                        Debug.Log("Recv: " + socket + " " + TypeDict[socket]);

                        byte[] bs = new byte[len];
                        for (int i = 0; i < len; i++)
                        {
                                bs[i] = Marshal.ReadByte(buf, i);
                        }
                        //byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(str);
                        
                        if (!(bs[0] == 0 && (bs[1] == 0 || bs[1] == 0x80)))
                        {
                                Debug.LogBytes(bs, "Recv (" + socket.ToString() + "): ");
                        }
                }

                private static short BigEndian(short input)
                {
                        byte[] bytes = BitConverter.GetBytes(input);

                        return BitConverter.ToInt16(new[] {bytes[1], bytes[0]}, 0);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                private delegate void HookType(uint socket, IntPtr buf, int len);
        }
}
