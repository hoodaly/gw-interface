#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GuildWarsInterface.Modification.Hooks;
using GuildWarsInterface.Modification.Native;

#endregion

namespace GuildWarsInterface.Modification.Patches
{
    internal static class PortPatch
    {
        public static void Apply()
        {
                List<int> addrs = HookHelper.searchAsm(new byte[] { 0x66, 0x89, 0x46, 0x02, 0x5e, 0xc3, 0xcc });
                foreach (int addr in addrs)
                {
                        IntPtr PortFixLoc = (IntPtr)addr + 1;
                        uint dwOldProtection;
                        Kernel32.VirtualProtect(PortFixLoc, 1, 0x40, out dwOldProtection);
                        Marshal.WriteByte(PortFixLoc, 0x8B);
                        Kernel32.VirtualProtect(PortFixLoc, 1, dwOldProtection, out dwOldProtection);
                }
        }
    }
}
