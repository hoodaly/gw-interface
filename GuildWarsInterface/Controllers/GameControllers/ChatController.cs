#region

using System.Collections.Generic;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Interaction;
using GuildWarsInterface.Logic;
using GuildWarsInterface.Networking.Protocol;

#endregion

namespace GuildWarsInterface.Controllers.GameControllers
{
        internal class ChatController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler((int)GameClientMessage.Chat, ChatHandler);
                }

                private void ChatHandler(List<object> objects)
                {
                        var totalMessage = (string) objects[2];

                        var channel = (Chat.Channel) totalMessage[0];

                        if (channel != Chat.Channel.Command)
                                if ((Chat.Channel) totalMessage[1] == Chat.Channel.InternalCommand)
                                        channel = Chat.Channel.InternalCommand;

                        int startIndex = channel == Chat.Channel.InternalCommand ? 2 : 1;

                        var message = new string(totalMessage.ToCharArray(), startIndex, totalMessage.Length - startIndex);

                        if (channel == Chat.Channel.InternalCommand)
                        {
                                Chat.HandleInternalCommand(message);
                        }
                        else
                        {
                                GameLogic.ChatMessage(message, channel);
                        }
                }
        }
}
