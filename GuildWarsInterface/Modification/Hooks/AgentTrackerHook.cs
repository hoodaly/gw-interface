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
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x89, 0x4d, 0xe8, 0x8b, 0x46, 0x78 });
                        Debug.Requires(addrs.Count == 1);
                        int addrStart = addrs[0];

                        // End of function (5e 8b e5 5d c2 08 00) is at addrStart + 0x0267
                        int addrEnd = addrStart + 0x0267;

                        var hookLocation1 = new IntPtr(addrStart);
                        var hookLocation = new IntPtr(addrEnd);

                        IntPtr codeCave2 = Marshal.AllocHGlobal(4);

                        IntPtr codeCave1 = Marshal.AllocHGlobal(128);

                        byte[] code1 = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave1,
                                        "mov eax, dword[esi+0x10]", //eax will be overwritten soon
                                        "mov dword[" + codeCave2 + "],eax", //store value in codeCave
                                        "mov dword[ebp-0x18],ecx", //instruction from original (89 4d e8)
                                        "mov eax,dword[esi+0x78]", //instruction from original (8b 46 78)
                                        "jmp " + (hookLocation1 + 6)
                                });
                        Marshal.Copy(code1, 0, codeCave1, code1.Length);

                        HookHelper.Jump(hookLocation1, codeCave1);


                        IntPtr codeCave = Marshal.AllocHGlobal(128);

                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "push dword [ebp+0x8]", //id parameter
                                        "push dword [" + codeCave2 + "]",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "pop esi",
                                        "mov esp,ebp",
                                        "pop ebp",
                                        "retn 8"
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                internal delegate void HookType(uint id, IntPtr data);
        }
}
