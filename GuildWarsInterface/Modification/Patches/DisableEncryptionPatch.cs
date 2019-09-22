#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GuildWarsInterface.Modification.Hooks;
using GuildWarsInterface.Modification.Native;

#endregion

namespace GuildWarsInterface.Modification.Patches
{
        internal static class DisableEncryptionPatch
        {
                public static void Apply()
                {
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x89, 0x55, 0x10, 0x8B, 0x55, 0x08, 0x89, 0x55, 0x08, 0x46, 0x81, 0xE6, 0xFF, 0x00, 0x00, 0x00 });
                        foreach(int addr in addrs)
                        {
                                IntPtr location = (IntPtr)(addr + 0x41);
                                uint dwOldProtection;
                                Kernel32.VirtualProtect(location, 5, 0x40, out dwOldProtection);
                                Marshal.Copy(new byte[] {0x8A, 0x14, 0x18}, 0, location, 3);
                                Kernel32.VirtualProtect(location, 5, dwOldProtection, out dwOldProtection);
                        }
                }
        }
}