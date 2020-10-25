using System;
using System.Text;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Datastructures.Base;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;

namespace GuildWarsInterface.Datastructures.Agents
{
        public abstract class Agent : Creatable, IIdentifyable
        {
                public readonly AgentClientMemory AgentClientMemory;
                public readonly AgentTransformation Transformation;
                private string _name;
                private float _speed;

                protected Agent()
                {
                        AgentClientMemory = new AgentClientMemory(this);

                        Name = "N/A";
                        Transformation = new AgentTransformation(this);
                        Speed = 288;
                }

                public string Name
                {
                        get { return _name; }
                        set
                        {
                                _name = value;

                                OnNameChanged();
                        }
                }

                public float Speed
                {
                        get { return _speed; }
                        set
                        {
                                _speed = value;

                                if (Game.State == GameState.Playing && Created)
                                {
                                        Network.GameServer.Send(GameServerMessage.AgentSpeed, IdManager.GetId(this), Speed);
                                }
                        }
                }

                protected abstract void OnNameChanged();

                internal void SendAgentPropertyInt(AgentProperty identifier, uint value)
                {
                        Network.GameServer.Send(GameServerMessage.AgentPropertyInt, (uint) identifier, IdManager.GetId(this), value);
                }

                internal void SendAgentTargetPropertyInt(AgentProperty identifier, Agent target, uint value)
                {
                        Network.GameServer.Send(GameServerMessage.AgentTargetPropertyInt, (uint) identifier, IdManager.GetId(target), IdManager.GetId(this), value);
                }

                internal void SendAgentPropertyFloat(AgentProperty identifier, float value)
                {
                        Network.GameServer.Send(GameServerMessage.AgentPropertyFloat, (uint) identifier, IdManager.GetId(this), value);
                }

                internal void SendAgentTargetPropertyFloat(AgentProperty identifier, Agent target, float value)
                {
                        Network.GameServer.Send(GameServerMessage.AgentTargetPropertyFloat, (uint) identifier, IdManager.GetId(target), IdManager.GetId(this), value);
                }

                protected override void OnDestruction()
                {
                        Network.GameServer.Send(GameServerMessage.DespawnAgent, IdManager.GetId(this));
                }

                protected void Spawn(uint agentType, bool allied = true, uint secondaryId = 0)
                {
                        uint monster = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("MONS"), 0);
                        uint player = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("yalp"), 0);
                        uint item = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("\x00\x00\x00\x34"), 0);
                        
                        uint type = 0;
                        uint id = IdManager.GetId(this);
                        uint _secondaryId = id;

                        switch (agentType)
                        {
                                case 0:
                                        type = item;
                                        _secondaryId = secondaryId;
                                        break;
                                case 0x30000000:
                                        type = player;
                                        break;
                                case 0x20000000:
                                        type = monster;
                                        break;
                        }

                        _secondaryId |= agentType;

                        Network.GameServer.Send(GameServerMessage.SpawnAgent,
                                                id,
                                                _secondaryId,
                                                (byte) (agentType > 0 ? 1 : 4),
                                                (byte) 5,
                                                Transformation.Position.X,
                                                Transformation.Position.Y,
                                                (ushort) 0, //positionPlane
                                                (float) 0, //directionRotationX
                                                (float) 0, //directionRotationY
                                                (byte) 1, //unknown
                                                Speed,
                                                float.PositiveInfinity, //unknown
                                                0, //unknown
                                                type, //spawnType
                                                0, //unknown
                                                0, //unknown
                                                0, //unknown
                                                0, //unknown
                                                0, //unknown
                                                (float) 0, //unknown
                                                (float) 0, //unknown
                                                float.PositiveInfinity, //unknown
                                                float.PositiveInfinity, //unknown
                                                (ushort) 0, //unknown
                                                0, //unknown
                                                float.PositiveInfinity, //unknown
                                                float.PositiveInfinity, //unknown
                                                (ushort) 0); //unknown
                }

                public void AttackFailed(Agent target, AttackFailType type)
                {
                        SendAgentTargetPropertyInt(AgentProperty.AttackFailed, target, (uint) type);
                }

                public override string ToString()
                {
                        return string.Format("[{0}] {1}", GetType().Name, Name);
                }
        }
}
