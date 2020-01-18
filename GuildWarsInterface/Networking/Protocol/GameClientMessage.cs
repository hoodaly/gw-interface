namespace GuildWarsInterface.Networking.Protocol
{
        internal enum GameClientMessage
        {
                ExitToCharacterScreen = 2,
                ExitToLoginScreen = 3,
                CommitMapChange = 9,//na,
                EquipItem = 54,//42,
                ChangeWeaponset = 56,//44,
                Attack = 57,//45,
                KeyboardMove = 67,//55,
                MouseMove = 68,//56,
                CastSkill = 76,//64,
                BuyItem = 83,//71,
                UnEquipItem = 85,//73,
                KeyboardStopMoving = 77,
                StylistChangeAppearance = 78,
                EquipSkill = 98,//85,
                SwapSkill = 100,//87,
                MoveSkillToEmptySlot = 101,//88,
                CharacterCreateUpdateCampaignAndProfession = 102,//89,
                Chat = 106,//94,
                MoveBag = 110,
                UnEquipBag = 119,
                MoveItem = 120,//108,
                CreateNewCharacter = 143,//131,
                ValidateNewCharacter = 145,//133,
                InstanceLoadRequestMapData = 142,
                InstanceLoadRequestPlayerData = 151,
                InstanceLoadRequestItems = 152,
                EnterFight = 172,
                AcceptJoin = 163,//150, //controllerManager.RegisterHandler(150, AcceptJoinRequestHandler);
                KickInvite = 164,//151, //controllerManager.RegisterHandler(151, KickInviteHandler);
                KickJoinRequest = 165,//152, //controllerManager.RegisterHandler(152, KickJoinRequestHandler);
                Invite = 167,//154, //controllerManager.RegisterHandler(154, InviteHandler);
                Leave = 169,//156, //controllerManager.RegisterHandler(156, LeaveHandler);
                KickMember = 176,//163, //controllerManager.RegisterHandler(163, KickMemberHandler);*/
                ChangeMap = 184,//171,
        }
}
