#region

using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Modification.Hooks;

#endregion

namespace GuildWarsInterface.Datastructures.Agents.Components
{
        public sealed class AgentClientMemory
        {
                private readonly Agent _agent;
                private readonly IntPtr _db8location = SpeedModifierHook.DB8Location();

                public AgentClientMemory(Agent agent)
                {
                        _agent = agent;
                }

                private IntPtr ClientMemoryBase
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return Marshal.ReadIntPtr(Marshal.ReadIntPtr(Marshal.ReadIntPtr(_db8location) + 0xe8) + (int) (4 * IdManager.GetId(_agent))) + 4;
                                }
                                catch (AccessViolationException)
                                {
                                        return IntPtr.Zero;
                                }
                        }
                }

                public float Speed
                {
                        get
                        {
                                float mx = MoveX;
                                float my = MoveY;

                                return (float) Math.Sqrt(mx * mx + my * my);
                        }
                }

                public short Plane
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return Marshal.ReadInt16(ClientMemoryBase + 0x5C);
                                }
                                catch (AccessViolationException)
                                {
                                        return 0;
                                }
                        }
                }

                public float X
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ClientMemoryBase + 116)), 0);
                                }
                                catch (AccessViolationException)
                                {
                                        return 0;
                                }
                        }
                }
                
                public float Y
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ClientMemoryBase + 120)), 0);
                                }
                                catch (AccessViolationException)
                                {
                                        return 0;
                                }
                        }
                }
                
                public float MoveX
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ClientMemoryBase + 160)), 0);
                                }
                                catch (AccessViolationException)
                                {
                                        return 0;
                                }
                        }
                }
                
                public float MoveY
                {
                        [HandleProcessCorruptedStateExceptions]
                        get
                        {
                                try
                                {
                                        return BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ClientMemoryBase + 164)), 0);
                                }
                                catch (AccessViolationException)
                                {
                                        return 0;
                                }
                        }
                }

                public Position Position
                {
                        get { return new Position(X, Y, Plane); }
                }
        }
}
