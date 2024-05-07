using System;

namespace MyWallpapersDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool flag_downloadIndex = true;
            bool flag_applyWallpaper = true;

            foreach (string arg in args)
                switch (arg)
                {
                    case "/i":
                        flag_downloadIndex = false;
                        break;
                    case "/a":
                        flag_applyWallpaper = false;
                        break;
                }

            WallpaperManager wm = new WallpaperManager();

            if (flag_downloadIndex)
                wm.DownloadIndex();

            wm.LoadJson();
            wm.GetRandomWallpaper();

            wm.DownloadWallpaper();
            if (flag_applyWallpaper)
                wm.SetWallpaper();
        }
    }
}
