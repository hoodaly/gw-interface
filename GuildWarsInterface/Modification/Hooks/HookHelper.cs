#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using GuildWarsInterface.Modification.Native;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal static class HookHelper
        {
                public static void Jump(IntPtr from, IntPtr to)
                {
                        byte[] hook = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + from,
                                        "jmp " + to
                                });

                        uint dwOldProtection;
                        Kernel32.VirtualProtect(from, 5, 0x40, out dwOldProtection);
                        Marshal.Copy(hook, 0, from, 5);
                        Kernel32.VirtualProtect(from, 5, dwOldProtection, out dwOldProtection);
                }

                
                public static List<int> searchAsm(byte[] target)
                {
                        List<int> output = new List<int>();
                        int cur = 0x00401000;
                        int end = 0x00800000;
                        int idx = 0;
                        int chunkstart = cur;
                        while (cur < end)
                        {
                                if (target[idx] == Marshal.ReadByte((IntPtr)cur))
                                {
                                        if (idx == 0)
                                        {
                                                chunkstart = cur;
                                        }
                                        idx += 1;
                                        if (idx == target.Length)
                                        {
                                                // found it
                                                output.Add(chunkstart);
                                                idx = 0;
                                        }
                                        cur += 1;
                                }
                                else
                                {
                                        cur = chunkstart + 1;
                                        chunkstart = cur;
                                        idx = 0;
                                }
                        }
                        return output;
                }
        }
}