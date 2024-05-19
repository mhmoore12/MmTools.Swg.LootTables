using NLua;

namespace MmTools.Swg.LootTables
{
    internal class Program
    {
        static void Main(string[] args)
        {

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

            lua.DoString("pikemanmaster = 1");
            lua.DoString("fencermaster = 2");
            lua.DoString("brawlermaster = 4");
            lua.DoString("BEEP = 16");

            var creaturePath = @"C:\Users\mhmoo\source\repos\public\MMOCoreORB\bin\scripts\mobile\creatures.lua";
            lua.DoFile(creaturePath);

            List<Creature> list = new List<Creature>(); 
            var luaFileDirectory = @"C:\Users\mhmoo\source\repos\public\MMOCoreORB\bin\scripts\mobile\dantooine\";
            foreach (var file in Directory.GetFiles(luaFileDirectory))
            {
                var luaFilePath = file;
                var fileContents = File.ReadAllText(luaFilePath);
                fileContents = String.Join('\n', fileContents.Split("\n").Where(a => !a.Trim().StartsWith("includeFile")));
                if (fileContents.Contains("Creature:new"))
                {
                    lua.DoString(fileContents);

                    var creatureTableName = luaFilePath.Replace(luaFileDirectory, "").Replace(".lua", "");
                    Console.WriteLine("Getting table: " + creatureTableName);
                    LuaTable creatureTable = lua[creatureTableName] as LuaTable;
                    Creature creature = Creature.ParseCreature(creatureTable);
                    list.Add(creature);
                    Console.WriteLine("Object Name: " + creature.ObjectName);
                }
            }
            var jantaScout = list.FirstOrDefault(a => a.ObjectName.Contains("janta_loreweaver")).LootGroups;
            Console.WriteLine("Hello, World!");
            Console.ReadLine();
        }
    }
}
