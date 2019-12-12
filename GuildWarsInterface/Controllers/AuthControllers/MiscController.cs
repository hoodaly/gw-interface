#region

using System;
using System.Collections.Generic;
using System.Linq;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Components;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Logic;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;

#endregion

namespace GuildWarsInterface.Controllers.AuthControllers
{
        internal class MiscController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler(7, DeleteCharacterHandler);
                        controllerManager.RegisterHandler(9, Packet9Handler);
                        controllerManager.RegisterHandler(10, SelectCharacterHandler);
                        controllerManager.RegisterHandler(12, MoveFriendListEntryHandler);
                        controllerManager.RegisterHandler(14, UpdatePlayerStatusHandler);
                        controllerManager.RegisterHandler(32, Packet32Handler);
                        controllerManager.RegisterHandler(41, PlayRequest);
                        controllerManager.RegisterHandler(53, Packet53Handler);
                        controllerManager.RegisterHandler(13, LogoutHandler);
                        controllerManager.RegisterHandler(26, AddFriendListEntryHandler);
                }

                private void UpdatePlayerStatusHandler(List<object> objects)
                {
                        Game.Player.Status = (PlayerStatus) (uint) objects[1];
                }

                private void MoveFriendListEntryHandler(List<object> objects)
                {
                        bool success = AuthLogic.MoveFriend((string)objects[3], (FriendList.Type)(uint)objects[2]);

                        Network.AuthServer.TransactionCounter = (uint) objects[1];
                        Network.AuthServer.SendTransactionSuccessCode(success ? TransactionSuccessCode.Success : TransactionSuccessCode.Aborted);
                }

                private void AddFriendListEntryHandler(List<object> objects)
                {
                        bool success = AuthLogic.AddFriend((FriendList.Type)(uint)objects[2], (string)objects[3], (string)objects[4]);

                        Network.AuthServer.TransactionCounter = (uint)objects[1];
                        Network.AuthServer.SendTransactionSuccessCode(success ? TransactionSuccessCode.Success : TransactionSuccessCode.Aborted);
                }

                private void LogoutHandler(List<object> objects)
                {
                        Game.State = GameState.LoginScreen;

                        AuthLogic.Logout();
                }

                private void Packet9Handler(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint) objects[1];

                        Network.AuthServer.SendTransactionSuccessCode(TransactionSuccessCode.Success);
                }

                private void SelectCharacterHandler(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint) objects[1];

                        var selectedCharacterName = (string) objects[2];
                        PlayerCharacter selectedCharacter = Game.Player.Account.Characters.FirstOrDefault(chara => chara.Name.Equals(selectedCharacterName));

                        if (selectedCharacter == null)
                        {
                                Debug.ThrowException(new Exception("character with name " + selectedCharacterName + " does not belong to the account"));
                        }

                        Game.Player.Character = selectedCharacter;

                        Network.AuthServer.SendTransactionSuccessCode(TransactionSuccessCode.Success);
                }

                private void Packet32Handler(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint)objects[1];

                        Network.AuthServer.SendTransactionSuccessCode(TransactionSuccessCode.Success);
                }

                private void Packet35Handler(List<object> objects)
                {

                        Network.AuthServer.Send((AuthServerMessage)23, (ushort) 0);
                }

                private void PlayRequest(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint) objects[1];
                        var mapId = (uint) objects[3];
                        if (mapId != 0)
                        {
                                AuthLogic.Play((Map) mapId);
                        }
                        else
                        {
                                Game.State = GameState.CharacterCreation;

                                Network.AuthServer.Send(AuthServerMessage.Dispatch,
                                                Network.AuthServer.TransactionCounter,
                                                0,
                                                mapId,
                                                new byte[]
                                                        {
                                                                0x02, 0x00, 0x23, 0x98, 0x22, 0x22, 0x22, 0x22,
                                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                                                        },
                                                0);
                        }
                }

                private void Packet53Handler(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint) objects[1];

                        Network.AuthServer.Send(AuthServerMessage.Response53, Network.AuthServer.TransactionCounter, 0);
                        Network.AuthServer.SendTransactionSuccessCode(TransactionSuccessCode.Success);
                }

                private void DeleteCharacterHandler(List<object> objects)
                {
                        Network.AuthServer.TransactionCounter = (uint) objects[1];

                        var nameOfCharacterToDelete = (string) objects[2];

                        PlayerCharacter characterToDelete = Game.Player.Account.Characters.FirstOrDefault(character => character.Name.Equals(nameOfCharacterToDelete));

                        if (characterToDelete == null)
                        {
                                Debug.ThrowException(new Exception("trying to delete character not belonging to this account"));
                        }

                        DeleteCharacterSucceeded(characterToDelete);
                }

                private void DeleteCharacterSucceeded(PlayerCharacter characterToDelete)
                {
                        if (!Game.Player.Account.Characters.Contains(characterToDelete))
                        {
                                Debug.ThrowException(new Exception("account does not contain " + characterToDelete));
                        }

                        AuthLogic.DeleteCharacter(characterToDelete);
                        Game.Player.Account.RemoveCharacter(characterToDelete);

                        Network.AuthServer.SendTransactionSuccessCode(TransactionSuccessCode.Success);
                }
        }
}
