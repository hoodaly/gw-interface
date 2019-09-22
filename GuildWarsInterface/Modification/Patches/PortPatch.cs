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
        // loc of 8D533c897e08
        private static readonly IntPtr _location = (IntPtr) 0x0040a950;

        public static void Apply()
        {
                        return;
                List<int> addrs = HookHelper.searchAsm(new byte[] { 0x8D, 0x53, 0x3c, 0x89, 0x7e, 0x08 });
                foreach (int addr in addrs)
                {
                        IntPtr x = (IntPtr) addr + 7;
                        int Fun_offset = Marshal.ReadInt32(x);

                        IntPtr PortFixLoc = x + Fun_offset + 4;
                        PortFixLoc += 10;
                        uint dwOldProtection;
                        Kernel32.VirtualProtect(PortFixLoc, 1, 0x40, out dwOldProtection);
                        Marshal.WriteByte(PortFixLoc, 0x8B);
                        Kernel32.VirtualProtect(PortFixLoc, 1, dwOldProtection, out dwOldProtection);
                }
        }
    }
}