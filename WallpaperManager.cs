using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace MyWallpapersDownloader
{
    public class WallpaperManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
                      UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        private static readonly UInt32 SPI_SETDESKWALLPAPER =  0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE =    0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        private static readonly string serverUrl =             "https://my-wallpapers.github.io";
        private static readonly string indexUrl =              "https://raw.githubusercontent.com/PatrickStar8753/randomwallpaper/main/wallpapers.json";

        private Random random;
        private WebClient client;

        private string path;
        public JObject json;
        public List<string> wallpapers;
        public string wallpaper;
        public string wallpaperFile;

        private static JToken[] SearchKey(JArray jarray, string key, bool ignoreCase)
        {
            return jarray.Where(x => x is JObject ? ((JObject)x).ContainsKey(key) : false).ToArray();
        }

        public void LoadWallpaperIndex(dynamic jw, string path="")
        {
            if (jw is JObject && jw["wallpapers"] != null)
                foreach (dynamic elem in jw.wallpapers)
                    LoadWallpaperIndex(elem);
            else if (jw is JObject && jw["content"] != null)
                foreach (dynamic elem in jw.content)
                    LoadWallpaperIndex(elem, $"{path}/{jw.name}");
            else
                foreach (string elem in jw)
                    wallpapers.Add($"{path}/{elem}");
        }

        public void DownloadIndex()
        {
            Console.WriteLine("Downloading index...");
            client.DownloadFile(indexUrl, "wallpapers.json");
        }

        public void LoadJson()
        {

            if (!File.Exists(path))
                DownloadIndex();

            Console.WriteLine("Loading index...");

            using (StreamReader file = new StreamReader(path))
            {
                string content = file.ReadToEnd();
                json = JObject.Parse(content);

                LoadWallpaperIndex(json);
            }
        }
        public string GetRandomWallpaper()
        {
            return wallpaper = wallpapers[random.Next(wallpapers.Count)];
        }

        public void DownloadWallpaper()
        {
            Console.WriteLine("Downloading wallpaper...");
            wallpaperFile = $"wallpaper{Path.GetExtension(wallpaper)}";
            client.DownloadFile($"{serverUrl}/{wallpaper}", wallpaperFile);
        }

        public void SetWallpaper()
        {
            Console.WriteLine("Applying wallpaper...");
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, Path.GetFullPath(wallpaperFile),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public WallpaperManager(string indexPath="wallpapers.json")
        {
            path = indexPath;
            wallpapers = new List<string>();
            random = new Random();
            client = new WebClient();
        }
    }
}
