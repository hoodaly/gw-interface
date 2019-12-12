#region

using System;
using System.Collections.Generic;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Logic;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;
using GuildWarsInterface.Networking.Servers;

#endregion

namespace GuildWarsInterface.Controllers.GameControllers
{
        internal class MiscController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler((int)GameClientMessage.ExitToCharacterScreen, ExitToCharacterScreenHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.ExitToLoginScreen, ExitToLoginScreenHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.CommitMapChange, CommitMapChangeHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.ChangeMap, ChangeMapHandler);
                        controllerManager.RegisterHandler((int)GameClientMessage.StylistChangeAppearance, StylistChangeAppearanceHandler);
                }

                private void StylistChangeAppearanceHandler(List<object> objects)
                {
                        Game.Player.Character.Appearance = new PlayerAppearance((uint) objects[1]);

                        Network.GameServer.Send(GameServerMessage.AccountCurrency, (ushort) 101, (ushort) 1, (ushort) 0);
                        Network.GameServer.Send(GameServerMessage.AccountCurrency, (ushort) 102, (ushort) 1, (ushort) 0);
                        Network.GameServer.Send(GameServerMessage.OpenWindow,
                                0, //agent
                                (byte) 3, //windowType
                                0); //data

                        Console.WriteLine(Game.Player.Character.Appearance);
                }

                private void ExitToCharacterScreenHandler(List<object> objects)
                {
                        Game.State = GameState.CharacterScreen;

                        Network.GameServer.Disconnect();

                        GameLogic.ExitToCharacterScreen();
                }

                private void ExitToLoginScreenHandler(List<object> objects)
                {
                        Game.State = GameState.LoginScreen;

                        Network.GameServer.Disconnect();

                        GameLogic.ExitToLoginScreen();
                }

                private void ChangeMapHandler(List<object> objects)
                {
                        Network.GameServer.Send(GameServerMessage.ShowOutpostOnWorldMap,
                                                (ushort)objects[1],
                                                (byte)0); //unknown
                        GameLogic.ChangeMap((Map)(ushort)objects[1]);
                }

                private void CommitMapChangeHandler(List<object> objects)
                {
                        Network.GameServer.Disconnect();
                        Network.GameServer.Start();
                        Network.GameServer.ChangeMap();
                }
        }
}
