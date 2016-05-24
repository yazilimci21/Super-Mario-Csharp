using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Mario.SampleGame
{
    public class LevelLoader
    {
        private static class Items
        {
            static Hashtable items = new Hashtable();

            static Items()
            {
                items.Add("M", typeof(SampleGame.Players.Mario));
                items.Add("P", typeof(SampleGame.Players.Pacman));
                items.Add("X", typeof(SampleGame.Materials.Platform));
                items.Add("S", typeof(SampleGame.Materials.MovablePlatform));
                items.Add("C", typeof(SampleGame.Materials.Coin));
                items.Add("Q", typeof(SampleGame.Materials.QuestionBlock));
                items.Add("R", typeof(SampleGame.Materials.RotatingBlock));
                items.Add("#", typeof(SampleGame.Materials.Tile));
                items.Add("&", typeof(SampleGame.Materials.Stone));
                items.Add("k", typeof(SampleGame.Monsters.Koopa));
                items.Add("g", typeof(SampleGame.Monsters.Goomba));
                items.Add("G", typeof(SampleGame.Guns.StandardGun));
                items.Add("s", typeof(SampleGame.Guns.Ship));
                items.Add("F", typeof(SampleGame.Materials.FinishLevel));
            }

            public static Core.Material find(string shutcutName)
            {
                foreach(DictionaryEntry entry in items)
                {
                    if (entry.Key.ToString() == shutcutName)
                    {
                        try
                        {
                            return (Core.Material)Activator.CreateInstance((Type)entry.Value);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                return null;
            }
        }

        public static Core.Screen GameScreen;
        public static string[] LevelFiles;
        private static List<string> map = new List<string>();

        public static void LoadLevel(int levelint,Size charSize,ref int loading)
        {
            map.Clear();
            StreamReader fstream = new StreamReader(LevelFiles[levelint]);
            string line;
            int mapload = 0, load = 0;
            while(!string.IsNullOrEmpty(line=fstream.ReadLine()))
            {
                mapload += line.Length;
                map.Add(line);
            }
            fstream.Close();
            fstream.Dispose();
            GC.SuppressFinalize(fstream);

            for(int y=0;y<map.Count;y++)
            {
                char[] xchars = map[y].ToCharArray();
                for(int x=0;x<xchars.Length;x++)
                {
                    if (xchars[x] != ' ')
                    {
                        Core.Material material = Items.find(xchars[x].ToString());
                        if (material == null)
                        {
                            continue;
                        }
                        RectangleF draw = findPoint(material, y, x, charSize);
                        material.Properties.CollisionRect = draw;
                        GameScreen.Materials.Add(material);
                    }
                    load++;
                    loading = ((load * 100) / mapload);
                }
            }
        }

        private static RectangleF findPoint(Core.Material material, int y,int x, Size charSize)
        {
            float x0 = x*charSize.Width;
            float y0 = y*charSize.Height;
            if (x != 0)
            {
                x0 -= (material.Properties.CollisionRect.Width - charSize.Width);
            }
            if (y != 0)
            {
                y0 -= (material.Properties.CollisionRect.Height - charSize.Height);
            }

            RectangleF rect = new RectangleF(x0, y0, material.Properties.CollisionRect.Width,
                material.Properties.CollisionRect.Height);
            return rect;
        }
    }
}
