#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Modification.Native;
using PeNet;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        public static class HookHelper
        {
                public static IntPtr BaseAddress = (IntPtr) 0;
                public static IntPtr EndAddress = (IntPtr) 0;
                public static new Dictionary<string, IntPtr> ThunkDict = new Dictionary<string, IntPtr>();



                public static void Initialize()
                {
                        var gw_module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                        BaseAddress = gw_module.BaseAddress;
                        EndAddress = gw_module.BaseAddress + gw_module.ModuleMemorySize;

                        // Get Import Table
                        byte[] proc = new byte[gw_module.ModuleMemorySize];
                        Marshal.Copy(BaseAddress, proc, 0, gw_module.ModuleMemorySize);
                        var pe = new PeFile(gw_module.FileName);
                        var impf = pe.ImportedFunctions;
                        uint IAT = pe.ImageNtHeaders?.OptionalHeader?.DataDirectory[(int)Constants.DataDirectoryIndex.IAT].VirtualAddress ?? 0;
                        var impHash = new PeNet.ImpHash.ImportHash(pe.ImportedFunctions);
                        MethodInfo formatFunctionName = impHash.GetType().GetMethod("FormatFunctionName", BindingFlags.NonPublic | BindingFlags.Instance);

                        foreach (ImportFunction f in pe.ImportedFunctions)
                        {
                                var name = (string)formatFunctionName.Invoke(impHash, new object[] { f });
                                ThunkDict.Add(f.DLL.ToLower() + ";;" + name.ToLower(), BaseAddress + (int)IAT + (int)f.IATOffset);
                        }
                }

                internal static IntPtr MakeCodeCave(string[] v)
                {
                        IntPtr codeCave = Marshal.AllocHGlobal(128);
                        var preamble = new string[] {
                                "use32",
                                "org " + codeCave
                        };
                        var asm = preamble.Concat(v).ToArray();
                        byte[] code = FasmNet.Assemble(asm);
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        // Make executable
                        uint dwOldProtection;
                        Kernel32.VirtualProtect(codeCave, (uint)code.Length, 0x40, out dwOldProtection);
                        return codeCave;
                }

                public static IntPtr GetThunkLocation(string dllName, string funName)
                {
                        string idx = dllName.ToLower() + ";;" + funName.ToLower();
                        Debug.Requires(ThunkDict.ContainsKey(idx));
                        int addr = (int) ThunkDict[idx];

                        // search calling function
                        var loc = searchAsm(new byte[] { 0xff, 0x25, (byte)(addr & 0xff), (byte)((addr >> 8) & 0xff), (byte)((addr >> 16) & 0xff), (byte)((addr >> 24) & 0xff) });
                        Debug.Requires(loc.Count == 1);
                        return (IntPtr) loc[0];
                }

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

                
                public static List<int> searchAsm(byte[] target, string mask = "")
                {
                        List<int> output = new List<int>();
                        int cur = (int) HookHelper.BaseAddress;
                        int end = (int) HookHelper.EndAddress;
                        int idx = 0;
                        int chunkstart = cur;
                        while (cur < end)
                        {
                                if (target[idx] == Marshal.ReadByte((IntPtr)cur) || (mask.Length == target.Length && mask[idx] == '?'))
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

                public static IntPtr reloc(IntPtr addr)
                {
                        return (IntPtr)((int)addr + (int)HookHelper.BaseAddress - 0x00400000);
                }
        }
}
