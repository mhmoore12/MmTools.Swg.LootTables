using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmTools.Swg.LootTables
{
    public class LootItem
    {
        public string ItemTemplate { get; set; }
        public long Weight { get; set; }
    }

    public class MasterLootGroup
    {
        public string Description { get; set; }        
        public List<LootItem> LootItems { get; set; }

        public static MasterLootGroup? ParseLootGroup(LuaTable lootTable)
        {
            if (lootTable == null || lootTable["description"] == null)
            {
                return null;
            }

            try
            { 
                MasterLootGroup mlg = new MasterLootGroup
                {
                    Description = (string)lootTable["description"],                    
                    LootItems = new List<LootItem>()
                };

                if (lootTable["lootItems"] is LuaTable[])
                {
                    foreach (LuaTable item in (LuaTable[])lootTable["lootItems"])
                    {
                        long itemWeight = 0;
                        if (item["weight"] is long)
                        {
                            itemWeight = (long)item["weight"];
                        }
                        if (item["weight"] is double)
                        {
                            itemWeight = (long)(double)item["weight"];
                        }
                        LootItem lootItem = new LootItem
                        {
                            ItemTemplate = (string)item["itemTemplate"] + (string)item["groupTemplate"],
                            Weight = itemWeight
                        };
                        mlg.LootItems.Add(lootItem);
                    }
                }
                if (lootTable["lootItems"] is LuaTable)
                {
                    var subTable = (LuaTable)lootTable["lootItems"];
                    var keys = subTable.Keys;
                    foreach (var key in keys)
                    {
                        var item = (LuaTable)subTable[key];
                        long itemWeight = 0;
                        if (item["weight"] is long)
                        {
                            itemWeight = (long)item["weight"];
                        }
                        if (item["weight"] is double)
                        {
                            itemWeight = (long)(double)item["weight"];
                        }
                        LootItem lootItem = new LootItem
                        {
                            ItemTemplate = (string)item["itemTemplate"] + (string)item["groupTemplate"],
                            Weight = itemWeight
                        };
                        mlg.LootItems.Add(lootItem);
                    }
                }
                    return mlg;
            }
            catch
            {
                return null;
            }
        }
    }
}