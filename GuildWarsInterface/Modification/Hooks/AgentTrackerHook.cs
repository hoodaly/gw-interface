using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using GuildWarsInterface.Debugging;

namespace GuildWarsInterface.Modification.Hooks
{
        internal class AgentTrackerHook
        {
                internal static void Install(HookType hook)
                {
                        // Get start of function
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x56, 0x57, 0x8d, 0x73, 0x78, 0x8d, 0x7d, 0xd0 });
                        Debug.Requires(addrs.Count == 1);
                        int addrStart = addrs[0];

                        // End of function (5f 5e 5b c9 c2 08 00) is at addrStart + 0x01df
                        int addrEnd = addrStart + 0x01df;

                        var hookLocation1 = new IntPtr(addrStart);
                        var hookLocation = new IntPtr(addrEnd);

                        IntPtr codeCave2 = Marshal.AllocHGlobal(4);

                        IntPtr codeCave1 = Marshal.AllocHGlobal(128);

                        byte[] code1 = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave1,
                                        "push esi",
                                        "push edi",
                                        "mov esi, dword[ebx+0x10]",
                                        "mov dword[" + codeCave2 + "],esi",
                                        "lea esi, dword[ebx+0x78]",
                                        "jmp " + (hookLocation1 + 5)
                                });
                        Marshal.Copy(code1, 0, codeCave1, code1.Length);

                        HookHelper.Jump(hookLocation1, codeCave1);


                        IntPtr codeCave = Marshal.AllocHGlobal(128);

                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "push dword [ebp+0x8]",
                                        "push dword [" + codeCave2 + "]",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "pop edi",
                                        "pop esi",
                                        "pop ebx",
                                        "leave",
                                        "retn 8"
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                internal delegate void HookType(uint id, IntPtr data);
        }
}
