namespace GuildWarsInterface.Networking.Protocol
{
        internal enum GameServerMessage
        {
                AccountCurrency = 4,
                Tick = 31,//19,
                SpawnAgent = 33,//21,
                DespawnAgent = 34,//22,
                ControlledCharacter = 35,//23,
                KeyboardMove = 38,//26,
                AgentSpeed = 40,//28,
                Move = 42,//30,
                Goto = 43,//31,
                MovementSpeed = 44,//32,
                UpdateAgentPosition = 45,//33,
                AgentRotation = 47,//35,
                Attack = 54,//42,
                SetAttributePoints = 56,//44,
                UpdateAttributePoints1 = 57,//45,
                UpdateAttributePoints2 = 58,//46,
                UpdateAttributes = 59,//47,
                SkillEffectTimed = 66,//54,
                SkillEffectRemove = 68,//56,
                NpcStats = 86,//74,
                NpcModel = 87,//75,
                UpdateAgentAppearance = 89,//77,
                Message = 93,//81,
                MessageSenderAnonymous = 94,//82,
                MessageSenderWithTag = 96,//84,
                MessageSender = 97,//85,
                UpdateFullEquipment = 110,//98,
                DialogButton = 114,
                DialogBody = 116,
                DialogSender = 117,
                OpenWindow = 119,
                VendorWindowItems = 120,
                MapExploration126 = 138,//126,
                MapExploration127 = 139,//127,
                ShowOutpostOnWorldMap = 154,//141,
                NpcName = 156,//143,
                AgentMorale = 157,//144,
                AgentPropertyInt = 160,//147,
                AgentTargetPropertyInt = 161,//148,
                AgentPropertyFloat = 163,//150,
                AgentTargetPropertyFloat = 164,//151,
                Projectile = 165,//152,
                SpeechBubble = 166,//153,
                AgentProfessions = 167,//154,
                UpdateAvailableProfessions = 183,//170,
                UpdatePrivateProfessions = 184,//171,
                RefreshAgentAppearance = 191,//178,
                VendorWindowSender = 197,//184,
                UpdateSkillBar = 219,//206,
                UpdateAvailableSkills = 220,//207,
                SkillRechargedVisualAutoAfterRecharge = 227,//214,
                SkillRechargedVisual = 228,//215,
                SkillRecharging = 230,//217,
                SkillRecharged = 231,//218,
                PlayerData221 = 234,//221,
                AgentStatus = 241,//228,
                PlayerData230 = 243,//230,
                ManipulateMapObject = 260,
                ChownItem = 317,
                EquipBag = 319,//306,
                ItemLocation = 321,//308,
                CreateInventoryPage = 322,//309,
                CreateInventory = 327,//314,
                DestroyInventory = 328,//315,
                CreateBag = 329,//316,
                UpdateWeaponsetWeapons = 330,//317,
                UpdateActiveWeaponset = 331,//318,
                MoveItem = 334,//321,
                RemoveItem = 323,
                MoveBag = 335,
                SwapItems = 341,//328,
                UnEquipBag = 343,//329,
                CreateItem = 357,//343,
                InstanceLoadHead = 385,//= 370,
                InstanceLoadCharName = 386,//= 371,
                CharacterCreated = 397,//393,//378,
                BeginCharacterCreation = 398,//394,//379,
                CharacterCreation380 = 399,//395,//380,
                CharacterCreationValidationFailed = 400,//396,//381,
                InstanceLoadMapData = 410,//391,
                ConnectionStatus = 412,//393,
                InstanceLoadDistrictInfo = 414,//= 395,
                Dispatch = 426,//406,
                ShowPartyWindow = 439,//419,
                InviteParty = 457,//437,
                AddPartyJoinRequest = 458,//438,
                KickInvitedParty = 459,//439,
                KickPartyJoinRequest = 461,//441,
                AddPartyMember = 464,//444,
                RemovePartyMember = 469,//449,
                CreateParty1 = 471,//451,
                CreateParty2 = 472,//452,
                StatusText = 478,//458,
        }
}
