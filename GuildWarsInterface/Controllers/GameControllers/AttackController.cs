using System.Collections.Generic;
using System.Threading;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Interaction;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;

namespace GuildWarsInterface.Controllers.GameControllers
{
        internal class AttackController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler((int)GameClientMessage.Attack, AttackHandler);
                }

                private void AttackHandler(List<object> objects)
                {
                        Creature target;
                        if (!IdManager.TryGet((uint) objects[1], out target)) return;

                        Chat.ShowMessage(string.Format("attacking {0}!", target));

                        Network.GameServer.Send(GameServerMessage.Attack, IdManager.GetId(Game.Player.Character), 1.75F, 0xF);

                        Game.Player.Character.ShootProjectile(target.Transformation.Position, 0, 1);

                        Game.Player.Character.PerformAnimation(CreatureAnimation.None);
                        for (var i = 0; i < 10; i++)
                        {
                                for (var j = 1; j < 5; j++)
                                {
                                        Game.Player.Character.ShootProjectile(target.Transformation.Position, (float)j / 5F, 0x8F);
                                        Thread.Sleep(100);
                                }
                        }
                }
        }
}
