using NLua;
using System.Text.RegularExpressions;

namespace MmTools.Swg.LootTables
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(200, 60);
            Lua lua = new Lua();
            lua.DoString("NONE = 0");
            lua.DoString("PACK = 1");
            lua.DoString("HERD = 2");
            lua.DoString("KILLER = 4");
            lua.DoString("AGGRESSIVE = 16");
            lua.DoString("ATTACKABLE = 32");
            lua.DoString("ENEMY = 64");
            lua.DoString("AIENABLED = 128");
            lua.DoString("JTLINTERESTING = 256");
            lua.DoString("STALKER = 512");
            lua.DoString("HEALER = 1024");
            lua.DoString("CONVERSABLE = 2048");
            lua.DoString("INVULNERABLE = 4096");
            lua.DoString("OVERT = 8192");
            lua.DoString("INTERESTING = 16384");

            lua.DoString("pikemanmaster = 1");
            lua.DoString("fencermaster = 2");
            lua.DoString("brawlermaster = 4");
            lua.DoString("BEEP = 16");

            var creaturePath = @"C:\Users\mhmoo\source\repos\swginfinity\public\MMOCoreORB\bin\scripts\mobile\creatures.lua";
            var fileContentsBase = File.ReadAllText(creaturePath);
            fileContentsBase = String.Join('\n', fileContentsBase.Split("\n").Where(a => !a.Trim().StartsWith("includeFile")));
            lua.DoString(fileContentsBase);

            List<Creature> list = new List<Creature>();
            var luaFileDirectory = @"C:\Users\mhmoo\source\repos\swginfinity\public\MMOCoreORB\bin\scripts\mobile\";
            foreach (var file in Directory.GetFiles(luaFileDirectory, @"*.lua", SearchOption.AllDirectories))
            {
                try
                {
                    var luaFilePath = file;
                    var fileContents = File.ReadAllText(luaFilePath);
                    var fileInfo = new FileInfo(file);
                    var fileName = fileInfo.Name;
                    fileContents = String.Join('\n', fileContents.Split("\n").Where(a => !a.Trim().StartsWith("includeFile")));
                    if (fileContents.Contains("@mob") && fileName != "creatures.lua")
                    {

                        lua.DoString(fileContents);

                        var creatureTableName = fileName.Replace(".lua", "");
                        LuaTable creatureTable = lua[creatureTableName] as LuaTable;
                        Creature creature = Creature.ParseCreature(creatureTable);
                        if (creature != null)
                        {
                            list.Add(creature);
                        }

                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
            Dictionary<string, MasterLootGroup> mlgDict = new Dictionary<string, MasterLootGroup>();
            var luaMlgFileDirectory = @"C:\Users\mhmoo\source\repos\swginfinity\public\MMOCoreORB\bin\scripts\loot\groups\";
            foreach (var file in Directory.GetFiles(luaMlgFileDirectory, @"*.lua", SearchOption.AllDirectories))
            {
                try
                {
                    var luaFilePath = file;
                    var fileContents = File.ReadAllText(luaFilePath);
                    var fileInfo = new FileInfo(file);
                    var fileName = fileInfo.Name;
                    fileContents = String.Join('\n', fileContents.Split("\n").Where(a => !a.Trim().StartsWith("includeFile"))
                        .Where(a => !a.Trim().StartsWith("addLootGroupTemplate")));
                    lua.DoString(fileContents);

                    var mlgTableName = fileName.Replace(".lua", "");
                    LuaTable mlgTable = lua[mlgTableName] as LuaTable;
                    MasterLootGroup mlg = MasterLootGroup.ParseLootGroup(mlgTable);
                    if (mlg != null)
                    {
                        mlgDict.Add(mlgTableName, mlg);
                    }
                }
                catch { }

            }
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Mob name (leave blank for wildcard): ");
                Console.ForegroundColor = ConsoleColor.Gray;
                var mobSearch = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Item name (leave blank for wildcard): ");
                Console.ForegroundColor = ConsoleColor.Gray;
                var itemSearch = Console.ReadLine();
                var jantas = list.Where(a => a.ObjectName.Contains(mobSearch));

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(" Mob".PadRight(65) + "\t" + $"| {"Item".PadRight(35)}\t| {"Drop Rate".PadRight(25)}");
                Console.WriteLine(new String('-', 180));
                Console.ForegroundColor = ConsoleColor.Gray;
                int i = 0;
                foreach (var janta in jantas)
                {


                    foreach (var lg in janta.LootGroups)
                        if (lg.Groups != null && lg.Groups.Count > 0)
                            foreach (var group in lg.Groups)
                            {
                                var totalGroupPerc = lg.Groups.Sum(a => (double)a.Chance);
                                //Console.WriteLine("" + group.Group + " | " + group.Chance);
                                if (mlgDict.ContainsKey(group.Group))
                                {
                                    foreach (var item in mlgDict[group.Group].LootItems)
                                    {
                                        if (item.ItemTemplate.Contains(itemSearch))
                                        {
                                            if (i % 2 == 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                            }
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.White;
                                            }
                                            var totalPerc = mlgDict[group.Group].LootItems.Sum(a => (double)a.Weight);
                                            Console.WriteLine(janta.ObjectName.PadRight(65) + "\t| " + item.ItemTemplate.PadRight(35) + $"\t| {(item.Weight / totalPerc) * (group.Chance / totalGroupPerc):P2}");
                                            i++;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"MISSING LOOT GROUP {group.Group}");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                }
                            }
                }
                Console.WriteLine("Hit any key to start new search...");
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
