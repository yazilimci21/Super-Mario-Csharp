using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace Mario.SampleGame
{
    public static class ImageLoader
    {
        static Hashtable items = new Hashtable();

        public static Image LoadImage(string ImageFile)
        {
            foreach (DictionaryEntry entry in items)
            {
                if (entry.Key.ToString() == ImageFile)
                {
                    return (Image)(((Image)entry.Value).Clone());
                }
            }
            if(File.Exists(ImageFile))
            {
                Image img = Image.FromFile(ImageFile);
                items.Add(ImageFile, (Image)img.Clone());
                return img;
            }
            throw new Exception("Image file not found");
        }
    }
}
