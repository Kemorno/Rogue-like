using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LogHandler
{
    private static string Path;

    public static void ClearLog()
    {
        if (Path == string.Empty || Path == null || string.IsNullOrWhiteSpace(Path))
            Path = Application.dataPath + @"\Log.txt";
        if (!File.Exists(Path))
            File.CreateText("Log");

        StreamWriter sr = new StreamWriter(Path);

        sr.Write("");
        sr.Close();
    }

    public static void NewEntry(string LogEntry, bool isError = false)
    {
        if (Path == string.Empty || Path == null || string.IsNullOrWhiteSpace(Path))
            Path = Application.dataPath + @"\Log.txt";
        if (!File.Exists(Path))
            File.CreateText("Log");


        StreamWriter sr = new StreamWriter(Path, true);

        string line = StartString(isError) + LogEntry;
        sr.WriteLine(line);
        sr.Close();
    }

    public static string StartString(bool isError)
    {
        return "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]" + " " + (isError ? " ERROR " : "");
    }
}