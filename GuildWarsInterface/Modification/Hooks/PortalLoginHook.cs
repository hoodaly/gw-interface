#region

using System;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal class PortalLoginHook
        {
                internal static void Install(HookType hook)
                {
                        var hookLocation = new IntPtr(0x0081c8a0);
                        IntPtr codeCave = Marshal.AllocHGlobal(128);
                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "int 3",
                                        "push ebp",
                                        "mov ebp, esi",
                                        "push 0",
                                        "pushad",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "mov [ebp+0x4], eax",
                                        "popad",
                                        "pop eax",
                                        "pop ebp",
                                        "jmp " + (hookLocation + 5)
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                internal delegate int HookType(IntPtr gameProtocol, uint protocolType);
        }
}