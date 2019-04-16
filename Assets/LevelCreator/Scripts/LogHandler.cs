using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LogHandler
{
    private static string Path;

    public static void NewEntry(string LogEntry, bool isError = false)
    {
        if (Path == string.Empty || Path == null || string.IsNullOrWhiteSpace(Path))
        {
            Path = Application.dataPath + @"\GameData\";
            File.CreateText("Log");
            Path += "Log.txt";
        }
        StreamWriter sr = new StreamWriter(Path);

        string line = StartString(isError) + LogEntry;

        sr.WriteLine(line);
    }

    public static string StartString(bool isError)
    {
        return "\n[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]" + ": " + (isError ? " ERROR " : "");
    }
}