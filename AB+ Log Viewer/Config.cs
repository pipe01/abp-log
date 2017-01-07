using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB__Log_Viewer
{
    class Config
    {
        public static Config Inst;


        public string LogPath;



        public static void Load()
        {
            if (!File.Exists("config.json"))
            {
                Save();
            }

            Inst = JsonConvert.DeserializeObject<Config>("config.json");
        }

        public static void Save()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Inst));
        }
    }
}
