#region

using System;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;

#endregion

namespace GuildWarsInterface.Datastructures.Agents.Components
{
        public sealed class AgentTransformation
        {
                private readonly Agent _agent;
                private MovementType _cacheType = MovementType.Stop;
                private float _cacheVelocity = 1F;
                private Position _goal;
                private MovementType _movementType;
                private float _orientation;

                private Position _position;
                private float _speed;

                public AgentTransformation(Agent agent)
                {
                        _agent = agent;
                        _position = new Position(0, 0, 0);
                        _goal = _position;
                }

                public Position Position
                {
                        get { return _position; }
                        set
                        {
                                Position oldPosition = Position;
                                _position = value;

                                if (Position.DistanceTo(oldPosition) > 100000)
                                {
                                        if (Game.State == GameState.Playing && _agent.Created)
                                        {
                                                Network.GameServer.Send(GameServerMessage.UpdateAgentPosition, IdManager.GetId(_agent), Position.X, Position.Y, Position.Plane);
                                        }
                                }
                        }
                }

                public float Orientation
                {
                        get { return _orientation; }
                        set
                        {
                                _orientation = value;

                                if (Game.State == GameState.Playing && _agent.Created)
                                {
                                        Network.GameServer.Send(GameServerMessage.AgentRotation,
                                                IdManager.GetId(_agent),
                                                Orientation, //cos
                                                5); //sin
                                }
                        }
                }

                public Position Goal { get; set; }
                public MovementType MovementType { get; set; }
                public float SpeedModifier { get; set; } = 1;

                public void SetGoal(float x, float y, short plane)
                {
                        if (Game.State == GameState.Playing)
                        {
                                Goal = new Position(x, y, plane);
                                Network.GameServer.Send(GameServerMessage.MovementSpeed,
                                                        IdManager.GetId(_agent),
                                                        SpeedModifier,
                                                        (byte)MovementType);

                                Network.GameServer.Send(GameServerMessage.Move,
                                                        IdManager.GetId(_agent),
                                                        x,
                                                        y,
                                                        plane, //currentPlane
                                                        plane); //nextPlane
                        }
                }

                public void Move(Position goal, float speedModifier, MovementType movementType)
                {
                        Network.GameServer.Send(GameServerMessage.MovementSpeed,
                                                IdManager.GetId(_agent),
                                                speedModifier,
                                                (byte) movementType);

                        Network.GameServer.Send(GameServerMessage.Move,
                                                IdManager.GetId(_agent),
                                                goal.X,
                                                goal.Y,
                                                goal.Plane, //currentPlane
                                                goal.Plane); //nextPlane
                }
        }
}
