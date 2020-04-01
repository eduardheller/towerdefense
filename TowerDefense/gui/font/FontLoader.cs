using System;
using System.Collections.Generic;
using System.IO;

namespace TowerDefense.gui.font
{
    class FontLoader
    {
        private Dictionary<string, string> values;
        private Dictionary<char, FontCharacter> characters;
        private int _padding;
        internal Dictionary<char, FontCharacter> Characters
        {
            get
            {
                return characters;
            }

            set
            {
                characters = value;
            }
        }

        public FontLoader(string pathToFntFile, int padding)
        {
            Characters = new Dictionary<char, FontCharacter>();
            StreamReader reader = new StreamReader(pathToFntFile);
            reader.ReadLine();
            reader.ReadLine();
            reader.ReadLine();
            reader.ReadLine();
            _padding = padding;
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] equations = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                values = new Dictionary<string, string>();
                for (int i = 1; i<equations.Length; i++)
                {
                    string[] equation = equations[i].Split('=');
                    values.Add(equation[0], equation[1]);
                }

                addCharData();
            }

        }

        private void addCharData()
        {
            char character = (char)Int32.Parse(values["id"]);
            float x  = (Int32.Parse(values["x"]))/ 512.0f;
            float y = (Int32.Parse(values["y"]))/ 512.0f;
            float w = (Int32.Parse(values["width"]) ) / 512.0f;
            float h = (Int32.Parse(values["height"])) / 512.0f;
            float xoff = Int32.Parse(values["xoffset"]) / 512.0f;
            float yoff = Int32.Parse(values["yoffset"]) / 512.0f;
            float cursorwidth = Int32.Parse(values["xadvance"]) / 512.0f;



            float qwidth = w;
            float qheight = h;

            FontCharacter fntChar = new FontCharacter(character, x, y, w, h,qwidth, qheight, xoff, yoff, cursorwidth);
            Characters.Add(character,fntChar);
        }
    }
}
