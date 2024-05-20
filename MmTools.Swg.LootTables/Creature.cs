using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmTools.Swg.LootTables
{

    public class Creature
    {
        public string? ObjectName { get; set; }
        public string? RandomNameType { get; set; }
        public bool? RandomNameTag { get; set; }
        public string? MobType { get; set; }
        public string? SocialGroup { get; set; }
        public string? Faction { get; set; }
        public long? Level { get; set; }
        //public double? ChanceHit { get; set; }
        public long? DamageMin { get; set; }
        public long? DamageMax { get; set; }
        public long? BaseXp { get; set; }
        public long? BaseHam { get; set; }
        public long? BaseHamMax { get; set; }
        public long? Armor { get; set; }
        public List<long?>? Resists { get; set; }
        public string? MeatType { get; set; }
        public long? MeatAmount { get; set; }
        public string? HideType { get; set; }
        public long? HideAmount { get; set; }
        public string? BoneType { get; set; }
        public long? BoneAmount { get; set; }
        public long? Milk { get; set; }
        //public long? TamingChance { get; set; }
        public long? Ferocity { get; set; }
        //public string? PvpBitmask { get; set; }
        //public string? CreatureBitmask { get; set; }
        //public string? OptionsBitmask { get; set; }
        public string? Diet { get; set; }
        public List<string>? Templates { get; set; }
        public List<LootGroup>? LootGroups { get; set; }
        public string? PrimaryWeapon { get; set; }
        public string? SecondaryWeapon { get; set; }
        public string? ConversationTemplate { get; set; }
        public List<string>? PrimaryAttacks { get; set; }
        public List<string>? SecondaryAttacks { get; set; }
        public static Creature? ParseCreature(LuaTable creatureTable)
        {
            if (creatureTable == null)
                return null;
            Creature creature = new Creature
            {
                ObjectName = creatureTable["objectName"] as string,
                RandomNameType = creatureTable["randomNameType"] as string,
                RandomNameTag = (bool?)creatureTable["randomNameTag"],
                MobType = creatureTable["mobType"] as string,
                SocialGroup = creatureTable["socialGroup"] as string,
                Faction = creatureTable["faction"] as string,
                Level = (long?)creatureTable["level"],
                //ChanceHit = (double?)creatureTable["chanceHit"],
                DamageMin = (long?)creatureTable["damageMin"],
                DamageMax = (long?)creatureTable["damageMax"],
                BaseXp = (long?)creatureTable["baseXp"],
                BaseHam = (long?)creatureTable["baseHAM"],
                BaseHamMax = (long?)creatureTable["baseHAMmax"],
                Armor = (long?)creatureTable["armor"],
                Resists = ParseResists(creatureTable["resists"] as LuaTable),
                MeatType = creatureTable["meatType"] as string,
                MeatAmount = (long?)creatureTable["meatAmount"],
                HideType = creatureTable["hideType"] as string,
                HideAmount = (long?)creatureTable["hideAmount"],
                BoneType = creatureTable["boneType"] as string,
                BoneAmount = (long?)creatureTable["boneAmount"],
                Milk = (long?)creatureTable["milk"],
                //TamingChance = (long?)creatureTable["tamingChance"],
                Ferocity = (long?)creatureTable["ferocity"],
                //PvpBitmask = creatureTable["pvpBitmask"] as string,
                //CreatureBitmask = creatureTable["creatureBitmask"] as string,
                //OptionsBitmask = creatureTable["optionsBitmask"] as string,
                Diet = creatureTable["diet"] as string,
                Templates = ParseTemplates(creatureTable["templates"] as LuaTable),
                LootGroups = ParseLootGroups(creatureTable["lootGroups"] as LuaTable),
                PrimaryWeapon = creatureTable["primaryWeapon"] as string,
                SecondaryWeapon = creatureTable["secondaryWeapon"] as string,
                ConversationTemplate = creatureTable["conversationTemplate"] as string,
                PrimaryAttacks = ParseAttacks(creatureTable["primaryAttacks"] as LuaTable),
                SecondaryAttacks = ParseAttacks(creatureTable["secondaryAttacks"] as LuaTable)
            };

            return creature;
        }

        private static List<long?>? ParseResists(LuaTable? resistsTable)
        {
            if (resistsTable == null) return null;

            List<long?> resists = new List<long?>();
            foreach (var value in resistsTable.Values)
            {
                if(value != null)
                resists.Add((long?)(value));
            }
            return resists;
        }

        private static List<string>? ParseTemplates(LuaTable? templatesTable)
        {
            if (templatesTable == null) return null;

            List<string> templates = new List<string>();
            foreach (var value in templatesTable.Values)
            {
                templates.Add(value as string);
            }
            return templates;
        }

        private static List<LootGroup>? ParseLootGroups(LuaTable? lootGroupsTable)
        {
            if (lootGroupsTable == null) return null;

            List<LootGroup> lootGroups = new List<LootGroup>();
            foreach (LuaTable lootGroupTable in lootGroupsTable.Values)
            {
                long lootChance = 0;
                if(lootGroupTable["lootChance"] is long)
                {
                    lootChance = (long)lootGroupTable["lootChance"];                    
                }
                LootGroup lootGroup = new LootGroup
                {
                    Groups = ParseLoot(lootGroupTable["groups"] as LuaTable),
                    LootChance = lootChance 
                };
                lootGroups.Add(lootGroup);
            }
            return lootGroups;
        }

        private static List<Loot>? ParseLoot(LuaTable? lootTable)
        {
            if (lootTable == null) return null;

            List<Loot> lootList = new List<Loot>();
            foreach (LuaTable loot in lootTable.Values)
            {
                 long lootChance = 0;
                if(loot["chance"] is long)
                {
                    lootChance = (long)loot["chance"];                    
                }
                Loot lootItem = new Loot
                {
                    Group = loot["group"] as string,
                    Chance = lootChance
                };
                lootList.Add(lootItem);
            }
            return lootList;
        }

        private static List<string>? ParseAttacks(LuaTable? attacksTable)
        {
            if (attacksTable == null) return null;

            List<string> attacks = new List<string>();
            foreach (var value in attacksTable.Values)
            {
                attacks.Add(value as string);
            }
            return attacks;
        }
    }

    public class LootGroup
    {
        public List<Loot>? Groups { get; set; }
        public double? LootChance { get; set; }
    }

    public class Loot
    {
        public string? Group { get; set; }
        public double? Chance { get; set; }
    }
}