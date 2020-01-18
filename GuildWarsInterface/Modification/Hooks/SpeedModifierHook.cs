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
                        // 8b 34 b0 85 f6 75 14 68 2d 04 00 00
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x8b, 0x34, 0xb0, 0x85, 0xf6, 0x75, 0x14, 0x68, 0x2d, 0x04, 0x00, 0x00 });
                        Debug.Requires(addrs.Count == 1);
                        var hookLocation = new IntPtr(addrs[0]);

                        _speedModifierLocation = Marshal.AllocHGlobal(4);
                        _db8Location = Marshal.AllocHGlobal(4);

                        IntPtr codeCave = HookHelper.MakeCodeCave(new[] {
                                        "mov esi, dword[esi*0x4+eax]", //instruction from original
                                        "mov dword[" + _speedModifierLocation + "], esi",
                                        "mov dword[" + _db8Location + "], ebx",
                                        "test esi, esi", //instruction from original
                                        "jmp " + (hookLocation + 5)
                                });
                        HookHelper.Jump(hookLocation, codeCave);
                }
        }
}
