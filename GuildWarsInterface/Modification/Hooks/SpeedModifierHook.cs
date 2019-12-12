using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using GuildWarsInterface.Debugging;

namespace GuildWarsInterface.Modification.Hooks
{
        internal class SpeedModifierHook
        {
                private static IntPtr _speedModifierLocation;
                private static IntPtr _db8Location;

                [HandleProcessCorruptedStateExceptions]
                internal static float SpeedModifier()
                {
                        try
                        {
                                var data = Marshal.ReadIntPtr(_speedModifierLocation);
                                return BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(data + 0x60)), 0);
                        }
                        catch (AccessViolationException)
                        {
                                return 0;
                        }
                }

                [HandleProcessCorruptedStateExceptions]
                internal static IntPtr DB8Location()
                {
                        return _db8Location;
                }



                internal static void Install()
                {
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x8b, 0x1c, 0x88, 0x85, 0xdb, 0x75, 0x14, 0x68, 0x2d, 0x04, 0x00, 0x00 });
                        Debug.Requires(addrs.Count == 1);
                        var hookLocation = new IntPtr(addrs[0]);

                        IntPtr codeCave = Marshal.AllocHGlobal(128);
                        _speedModifierLocation = Marshal.AllocHGlobal(4);
                        _db8Location = Marshal.AllocHGlobal(4);

                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "mov ebx, dword[ecx*0x4+eax]",
                                        "mov dword[" + _speedModifierLocation + "], ebx",
                                        "mov dword[" + _db8Location + "], esi",
                                        "test ebx, ebx",
                                        "jmp " + (hookLocation + 5)
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }
        }
}
