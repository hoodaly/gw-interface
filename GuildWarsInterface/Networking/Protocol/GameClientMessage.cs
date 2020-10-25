namespace GuildWarsInterface.Networking.Protocol
{
        internal enum GameClientMessage
        {
                ExitToCharacterScreen = 2,
                ExitToLoginScreen = 3,
                CommitMapChange = 9,//na, //Disconnect
                DropItem = 51,
                EquipItem = 55,//42,
                ChangeWeaponset = 57,//44,
                Attack = 58,//45,
                KeyboardMove = 68,//55,
                MouseMove = 69,//56,
                ItemPickup = 70,
                CastSkill = 77,//64,
                BuyItem = 84,//71,
                UnEquipItem = 86,//73,
                KeyboardStopMoving = 78,
                StylistChangeAppearance = 79,
                EquipSkill = 99,//85,
                SwapSkill = 101,//87,
                MoveSkillToEmptySlot = 102,//88,
                CharacterCreateUpdateCampaignAndProfession = 103,//89,
                Chat = 107,//94,
                MoveBag = 111,
                UnEquipBag = 120,
                MoveItem = 121,//108,
                CreateNewCharacter = 144,//131,
                ValidateNewCharacter = 146,//133,
                InstanceLoadRequestMapData = 143,
                InstanceLoadRequestPlayerData = 152,
                InstanceLoadRequestItems = 153,
                EnterFight = 173,
                AcceptJoin = 164,//150, //controllerManager.RegisterHandler(150, AcceptJoinRequestHandler);
                KickInvite = 165,//151, //controllerManager.RegisterHandler(151, KickInviteHandler);
                KickJoinRequest = 166,//152, //controllerManager.RegisterHandler(152, KickJoinRequestHandler);
                Invite = 168,//154, //controllerManager.RegisterHandler(154, InviteHandler);
                Leave = 170,//156, //controllerManager.RegisterHandler(156, LeaveHandler);
                KickMember = 177,//163, //controllerManager.RegisterHandler(163, KickMemberHandler);*/
                ChangeMap = 185,//171,
        }
}
