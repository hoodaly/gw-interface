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
                        List<int> addrs = HookHelper.searchAsm(new byte[] { 0x32, 0x44, 0x0a, 0xff });
                        foreach(int addr in addrs)
                        {
                                IntPtr location = (IntPtr) addr;
                                uint dwOldProtection;
                                Kernel32.VirtualProtect(location, 1, 0x40, out dwOldProtection);
                                Marshal.WriteByte(location, 0x8a);
                                Kernel32.VirtualProtect(location, 1, dwOldProtection, out dwOldProtection);
                        }
                }
        }
}
