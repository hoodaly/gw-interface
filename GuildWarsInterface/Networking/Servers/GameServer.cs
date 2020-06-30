#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Controllers.GameControllers;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Networking.Protocol;
using GuildWarsInterface.Networking.Servers.Base;

#endregion

namespace GuildWarsInterface.Networking.Servers
{
        internal class GameServer : Server
        {
                internal const short PORT = 9112;

                public GameServer()
                {
                        RegisterController(new AbilitiesController());
                        RegisterController(new CharacterCreationController());
                        RegisterController(new ChatController());
                        RegisterController(new InstanceLoadController());
                        RegisterController(new InventoryController());
                        RegisterController(new MiscController());
                        RegisterController(new MovementController());
                        RegisterController(new PartyController());
                        RegisterController(new SkillController());
                        RegisterController(new VendorController());
                        RegisterController(new AttackController());
                }

                protected override short Port
                {
                        get { return PORT; }
                }

                protected override void Received(byte[] data)
                {
                        if (data.Length <= 0)
                        {
                                return;
                        }
                        switch (BitConverter.ToUInt16(data, 0))
                        {
                                case 1280:
                                        return;
                                case 16896:
                                        Network.GameServer.Send(5633, new byte[20]);

                                        ChangeMap();

                                        return;
                                default:
                                        base.Received(data);
                                        break;
                        }
                }

                public void ChangeMap()
                {
                        if (Game.State != GameState.CharacterCreation)
                        {
                                Network.GameServer.Send(new List<KeyValuePair<GameServerMessage, object[]>> {
                                                        new KeyValuePair<GameServerMessage, object[]>(GameServerMessage.InstanceLoadHead, new object[] {
                                                                        (byte)0x1F,
                                                                        (byte)0x1F,
                                                                        (byte)0,
                                                                        (byte)0 }),
                                                        new KeyValuePair<GameServerMessage, object[]>(GameServerMessage.InstanceLoadCharName, new object[] {
                                                                        Game.Player.Character.Name}),
                                                        new KeyValuePair<GameServerMessage, object[]>(GameServerMessage.InstanceLoadDistrictInfo, new object[] {
                                                                        IdManager.GetId(Game.Player.Character),
                                                                        (ushort) Game.Zone.Map,
                                                                        (byte) (Game.Zone.IsExplorable ? 1 : 0),
                                                                        (uint) 1,
                                                                        (byte) 0,
                                                                        (byte) 0 })
                                                        });
                        }
                        else
                        {
                                Network.GameServer.Send(GameServerMessage.InstanceLoadHead,
                                                        (byte)0x1F,
                                                        (byte)0x1F,
                                                        (byte)0,
                                                        (byte)0);
                                Network.GameServer.Send(GameServerMessage.BeginCharacterCreation);
                        }
                }

                private void RegisterController(IController controller)
                {
                        controller.Register(this);
                }

                public void Send(GameServerMessage message, params object[] parameters)
                {
                        Send((int)message, parameters);
                }

                public void Send(List<KeyValuePair<GameServerMessage, object[]>> args)
                {
                        var newargs = args.Select((a) => new KeyValuePair<int, object[]>((int)a.Key, a.Value)).ToList();
                        Send(newargs);
                }
        }
}
