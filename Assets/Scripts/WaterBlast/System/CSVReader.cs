using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace WaterBlast.System
{
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static Dictionary<int, string> Read(string file)
        {
            var dic = new Dictionary<int, string>();
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return dic;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i] == "," || lines[i] == "") continue;
                if (lines[i].Replace(" ", "").Substring(0, 2) == "//")
                {
                    DebugX.Log(lines[i]);
                }
                else
                {
                    var values = Regex.Split(lines[i], SPLIT_RE);
                    if (values.Length == 0 || values[0] == "") continue;

                    int key = -1;
                    string val = string.Empty;
                    for (var j = 0; j < header.Length && j < values.Length; j++)
                    {
                        string value = values[j];
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        value = value.Replace("</ br>", "\n");
                        value = value.Replace("</ comma>", ",");
                        int n;
                        if (int.TryParse(value, out n))
                        {
                            key = n;
                        }
                        else
                        {
                            val = value;
                        }
                    }
                    dic.Add(key, val);
                }
            }
            return dic;
        }
    }
}