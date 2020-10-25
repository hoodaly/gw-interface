using System;
using System.Collections.Generic;
using System.Linq;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Components;
using GuildWarsInterface.Debugging;
using GuildWarsInterface.Declarations;
using GuildWarsInterface.Misc;
using GuildWarsInterface.Networking;
using GuildWarsInterface.Networking.Protocol;

namespace GuildWarsInterface.Datastructures.Items
{
        public sealed class DroppedItem : Agent
        {

                public Item Item { get; set; }

                public DroppedItem()
                {
                }

                public DroppedItem(Item item)
                {
                        Item = item;
                }

                protected override void OnCreation()
                {
                        if (!Item.Created)
                        {
                                Item.Create();
                        }
                        Spawn(0, true, IdManager.GetId(Item));
                }

                protected override void OnNameChanged()
                {
                        // TODO
                }

                public DroppedItem Clone()
                {
                        return new DroppedItem(Item);
                }

                public override string ToString()
                {
                        return string.Format("[DroppedItem] {0}", Name);
                }
        }
}
