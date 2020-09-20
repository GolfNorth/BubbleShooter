using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BubbleShooter
{
    public static class LevelLoader
    {
        private const int Columns = 11;

        public static Level Load(int index)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/Level-{index}");

            return levelAsset is null
                ? new Level()
                : ParseTextAsset(levelAsset);
        }
        
        private static Level ParseTextAsset (TextAsset levelAsset) {
            var colors = new List<BubbleColor[]>();
            var text = levelAsset.text;
            var lines = Regex.Split (text, "\n|\r|\r\n");
 
            foreach (var valueLine in lines)
            {
                if (string.IsNullOrWhiteSpace(valueLine)) continue;
                
                var values = Regex.Split(valueLine, ";");
                
                if (values.Length != Columns) continue;
                
                var row = new BubbleColor[Columns];

                for (var i = 0; i < Columns; i++)
                {
                    row[i] = Context.Instance.ColorCollection[values[i]];
                }
                
                colors.Add(row);
            }

            return new Level
            {
                Rows = colors.Count,
                Columns = Columns,
                Colors = colors.ToArray()
            };
        }
    }
}