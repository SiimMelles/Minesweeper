using System;
using System.IO;
using System.Text.Json;
using GameEngine;

namespace Handler
{
    public class GameConfigHandler
    {
        private const string FileName = "gamesettings.json";
        private const string PathName = "";
        static string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);


        public static void SaveConfig(GameSettings settings, string fileName = FileName)
        {
            /*
            if (!Directory.Exists(destPath + "/configSaves/"))
            {
                Directory.CreateDirectory(destPath + "/configSaves");
            }
            */

            using (var writer = System.IO.File.CreateText(destPath + fileName))
            {
                Console.WriteLine(fileName);
                var jsonString = JsonSerializer.Serialize(settings);
                writer.Write(jsonString);
            }
        }
        public static GameSettings LoadConfig(string fileName = FileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                var jsonString = System.IO.File.ReadAllText(destPath + fileName); 
                var res = JsonSerializer.Deserialize<GameSettings>(jsonString);
                return res;
            }
            
            return new GameSettings();
        }
        
        
    }
}