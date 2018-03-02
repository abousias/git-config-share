using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourcePath = Environment.ExpandEnvironmentVariables(@"%userprofile%\.gitconfig");
            var sourceDic = GetEntries(File.ReadAllLines(sourcePath));
            var targetPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\.gitconfig2");
            var targetDic = GetEntries(File.ReadAllLines(targetPath));

            if (targetDic.ContainsKey("[alias]")) 
            {
                targetDic["[alias]"] = sourceDic["[alias]"];
            }
            else 
            {
                targetDic.Add("[alias]", sourceDic["[alias]"]);
            }

            using (var targetFile = File.CreateText(targetPath)) 
            {
                foreach (var section in targetDic)
                {
                    targetFile.WriteLine(section.Key);
                    foreach (var line in section.Value)
                    {
                        targetFile.WriteLine($"    {line}");
                    }
                }

                targetFile.Flush();
            }
        }

        private static Dictionary<string, List<string>> GetEntries(IEnumerable<string> source)
        {
            var dic = new Dictionary<string, List<string>>();

            string currentTag = null;
            foreach (var line in source)
            {
                var trimmed = line.Trim();

                if (Regex.IsMatch(trimmed, @"^\[.*\]$"))
                {
                    currentTag = trimmed;
                    dic.Add(currentTag, new List<string>());
                }
                else if (currentTag != null && !string.IsNullOrWhiteSpace(line))
                {
                    dic[currentTag].Add(line.Trim());
                }
            }

            return dic;
        }
    }
}
