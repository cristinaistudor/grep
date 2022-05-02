using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace grep
{
    class Program
    {
        static void Search(string[] text, string path, string word, bool reverse, bool caseSensitive)
        {
            word = caseSensitive ? word.ToLower() : word;
            foreach (string line in text)
            {
                var lineToSearch = caseSensitive ? line.ToLower() : line;
                if (reverse == !lineToSearch.Contains(word))
                {
                    Console.WriteLine(path + ": " + line);
                }
            }
        }

        static void FileSearch(string fileNo, string word, bool reverse, bool caseSensitive)
        {
            try
            {
                string[] lines = File.ReadAllLines(fileNo);
                fileNo = fileNo.Substring(fileNo.LastIndexOf(@"\") + 1);
                Search(lines, fileNo, word, reverse, caseSensitive);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        static void URLSearch(string url, string word, bool reverse, bool caseSensitive)
        {
            try
            {
                List<string> lines = new List<string>();
                var wc = new System.Net.WebClient();
                using (var stream = wc.OpenRead(url))
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                string[] auxLines = lines.ToArray();
                Search(auxLines, url, word, reverse, caseSensitive);
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        static void FolderSearch(string folderPath, string word, bool reverse, bool caseSensitive)
        {
            try
            {
                foreach (string file in Directory.EnumerateFiles(folderPath, "*.txt"))
                {
                    FileSearch(file, word, reverse, caseSensitive);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            bool reverseFlag = Array.Exists(arguments, arg => arg == "-v");
            bool caseSensitiveFlag = Array.Exists(arguments, arg => arg == "-i");

            int argumentsNo = arguments.Count();
            if (arguments.Count() < 3)
            {
                Console.WriteLine("Usage: grep [-v reverse search] [-i case insensitive] word paths");
                return;
            }

            string input = arguments[argumentsNo - 1];
            string word = arguments[argumentsNo - 2];

            foreach (string itemInput in input.Split(','))
            {
                int inputNo = itemInput.IndexOf(":");
                if (inputNo == -1)
                {
                    Console.WriteLine("incorrect inputPath");
                    return;
                }
                string inputType = itemInput.Substring(0, inputNo);
                string path = itemInput.Substring(inputNo + 1);

                switch (inputType)
                {
                    case "file":
                        FileSearch(path, word, reverseFlag, caseSensitiveFlag);
                        break;
                    case "url":
                        URLSearch(path, word, reverseFlag, caseSensitiveFlag);
                        break;
                    case "folder":
                        FolderSearch(path, word, reverseFlag, caseSensitiveFlag);
                        break;
                    default:
                        Console.WriteLine("Incorrect InputPathType");
                        break;
                }
            }
        }
    }
}
