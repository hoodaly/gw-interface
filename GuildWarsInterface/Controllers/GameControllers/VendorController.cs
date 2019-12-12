#region

using System.Collections.Generic;
using GuildWarsInterface.Controllers.Base;
using GuildWarsInterface.Networking.Protocol;

#endregion

namespace GuildWarsInterface.Controllers.GameControllers
{
        internal class VendorController : IController
        {
                public void Register(IControllerManager controllerManager)
                {
                        controllerManager.RegisterHandler((int)GameClientMessage.BuyItem, BuyItemHandler);
                }

                private void BuyItemHandler(List<object> objects)
                {
                }
        }
}
