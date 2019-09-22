#region

using System;
using System.Runtime.InteropServices;
using Binarysharp.Assemblers.Fasm;

#endregion

namespace GuildWarsInterface.Modification.Hooks
{
        internal class GameProtocolHook
        {
                internal static void Install(HookType hook)
                {

                        var hookLocation = new IntPtr(0x00595ec0);

                        IntPtr codeCave = Marshal.AllocHGlobal(128);

                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "call 0x00596570",
                                        "pushad",
                                        "push edi",
                                        "push eax",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "popad",
                                        "jmp " + (hookLocation + 5)
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }
                internal static void Install2(HookType hook)
                {
                        

                        var hookLocation = new IntPtr(0x005953a5);

                        IntPtr codeCave = Marshal.AllocHGlobal(128);
                        
                        byte[] code = FasmNet.Assemble(new[]
                                {
                                        "use32",
                                        "org " + codeCave,
                                        "pushad",
                                        "push edx",
                                        "push edx",
                                        "call " + Marshal.GetFunctionPointerForDelegate(hook),
                                        "popad",
                                        "call 0x595e10",
                                        "jmp " + (hookLocation + 5)
                                });
                        Marshal.Copy(code, 0, codeCave, code.Length);

                        HookHelper.Jump(hookLocation, codeCave);
                }

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                internal delegate void HookType(IntPtr gameProtocol, uint protocolType);
        }
}