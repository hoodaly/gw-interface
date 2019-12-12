#region

using System.Collections.Generic;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Networking.Protocol;

#endregion

namespace GuildWarsInterface.Controllers.GameControllers
{
        internal class MovementController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler((int)GameClientMessage.KeyboardMove, KeyboardMoveHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.MouseMove, MouseMoveHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.KeyboardStopMoving, KeyboardStopMovingHandler);
                }

                private void KeyboardMoveHandler(List<object> objects)
                {
                        Game.Player.Character.Transformation.MovementType = (MovementType) (uint) objects[4];
                }

                private void MouseMoveHandler(List<object> objects)
                {
                        Game.Player.Character.Transformation.MovementType = MovementType.Forward;

                        var pos = (float[]) objects[1];
                        var plane = (short) (uint) objects[2];

                        Game.Player.Character.ShootProjectile(new Position(pos[0], pos[1], plane), 1F, 0x8F);
                        Game.Player.Character.Transformation.SetGoal(pos[0], pos[1], plane);
                }

                private void KeyboardStopMovingHandler(List<object> objects)
                {
                        Game.Player.Character.Transformation.MovementType = MovementType.Stop;
                }
        }
}
