using System.IO;
using System.Collections.Generic;

public static class configReader
{
    public static void Read(string path)
    {
        string[] lines = File.ReadAllLines(path);
        List<string> thread = new List<string>();

        for(int i = 0; i < lines.GetLength(0); i++)
        {
            if (lines[i] == "{")
                thread.Add(lines[i]);
        }
    }
}