#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal class GameProtocolHook
        {
                internal static void Install(HookType hook)
                {
                        // 8b f0 83 c4 08 8b 45 14
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x8b, 0xf0, 0x83, 0xc4, 0x08, 0x8b, 0x45, 0x14 });
                        Debug.Requires(addrs.Count == 1);
                        int addrStart = addrs[0];
                        var hookLocation = new IntPtr(addrStart);

                        IntPtr codeCave = HookHelper.MakeCodeCave(new[] {
                                        "pushad",
                                        "push dword[ebp+8]",
                                        "push eax",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "popad",
                                        "mov esi,eax",  //instruction from original
                                        "add esp,8", //instruction from original
                                        "jmp " + (hookLocation + 5)
                                });

                        HookHelper.Jump(hookLocation, codeCave);
                }
                internal static void Install2(HookType hook)
                {
                        // 83 c4 08 89 46 18
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x83, 0xc4, 0x08, 0x89, 0x46, 0x18 });
                        Debug.Requires(addrs.Count == 1);
                        int addrStart = addrs[0];
                        var hookLocation = new IntPtr(addrStart);

                        IntPtr codeCave = HookHelper.MakeCodeCave(new[] {
                                        "pushad",
                                        "push dword[ebp+8]",
                                        "push dword[ebp+8]",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "popad",
                                        "add esp,8",  //instruction from original
                                        "mov dword[esi+0x18],eax", //instruction from original
                                        "jmp " + (hookLocation + 6)
                                });
                        HookHelper.Jump(hookLocation, codeCave);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                internal delegate void HookType(IntPtr gameProtocol, uint protocolType);
        }
}
