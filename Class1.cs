using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using InfinityScript;

namespace MapRotation
{
    public class MapRotation : BaseScript
    {
        private static string NextDSR, CurrentDSR, CurrentDSPL = Function.Call<string>("getdvar", "sv_maprotation"), NextMap, CurrentMap = Function.Call<string>("getdvar", "mapname");
        private static Random rand = new Random();
        private static string _MapRotation_ = "1.0.0.0";
        public MapRotation()
        {
            if (!File.Exists(@"scripts\\MapRotation.txt"))
                File.AppendAllLines(@"scripts\\MapRotation.txt", new string[]
                {
                    "[DSR]=TDM_default,TDEF_default",
                    "[Map]=mp_dome,mp_alpha,mp_seatown"
                });
            string[] lines2 = File.ReadAllLines(@"scripts\\MapRotation.txt");
            foreach (string line in lines2)
            {
                if (line.StartsWith("//")) continue;
                if (line.StartsWith("[Map]="))
                {
                    string l = line.Replace("[Map]=", "");
                    string[] array = l.Split(',');
                    GetNextMap(array);
                }
                if (line.StartsWith("[DSR]="))
                {
                    string l = line.Replace("[DSR]=", "");
                    string[] array = l.Split(',');
                    GetNextDSR(array);
                }
            }
            Log.Debug("MapRotation " + _MapRotation_ + " by Mahyar has been loaded.");
            Log.Debug("Next dsr: " + NextDSR);
            Log.Debug("Next map: " + NextMap);
            WriteDSPL();
        }
        public void WriteDSPL()
        {
            if (File.Exists(@"admin\\" + NextDSR + ".dsr") || File.Exists(@"players2\\" + NextDSR + ".dsr"))
            {
                if (File.Exists(@"players2\\" + CurrentDSPL + ".dspl"))
                {
                    File.Delete(@"players2\\" + CurrentDSPL + ".dspl");
                }
                AfterDelay(600, delegate
                {
                    File.WriteAllLines(@"players2\\" + CurrentDSPL + ".dspl", new string[]{
                        "//////////////// MapRotation by Mahyar ////////////////",
                        NextMap + "," + NextDSR + ",1"
                    });
                });
            }
        }
        private static void GetNextMap(string[] array)
        {
            GetMap:
            int choose = rand.Next(0, array.Length);
            if (array[choose].ToLower().Trim() == CurrentMap) goto GetMap;
            NextMap = array[choose].ToLower().Trim();
        }
        private static void GetNextDSR(string[] array)
        {
            GetDSR:
            int choose = rand.Next(0, array.Length);
            NextDSR = array[choose];
            if (!File.Exists(@"admin\\" + NextDSR + ".dsr") && !File.Exists(@"players2\\" + NextDSR + ".dsr"))
                goto GetDSR;
        }
        public override EventEat OnSay2(Entity player, string name, string message)
        {
            string msg = message.ToLower().Trim();
            if (!msg.StartsWith("!")) return EventEat.EatNone;
            if(msg.StartsWith("!nextgame"))
            {
                Utilities.SayTo(player, "^7Next Game: ^1Map ^7" + NextMap + ", ^1GameType ^7" + NextDSR);
                return EventEat.EatGame;
            }
            return EventEat.EatNone;
        }
    }
}
