using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Resources;

public static class LogHandler
{
    private static string LogPath;
    private static string SeedBLPath;

    public static void ClearLog()
    {
        if (LogPath == string.Empty || LogPath == null || string.IsNullOrWhiteSpace(LogPath))
            LogPath = Application.dataPath + @"\Log.txt";
        if (!File.Exists(LogPath))
            File.CreateText("Log");

        StreamWriter sr = new StreamWriter(LogPath);

        sr.Write("");
        sr.Close();
    }

    public static void NewEntry(string LogEntry, bool isError = false)
    {
        if (LogPath == string.Empty || LogPath == null || string.IsNullOrWhiteSpace(LogPath))
            LogPath = Application.dataPath + @"\Log.txt";
        if (!File.Exists(LogPath))
            File.CreateText("Log");


        StreamWriter sr = new StreamWriter(LogPath, true);

        string line = StartString(isError) + LogEntry;
        sr.WriteLine(line);
        sr.Close();
    }

    public static string StartString(bool isError)
    {
        return "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]" + " " + (isError ? " ERROR " : "");
    }

    public static void BlacklistSeed(string Seed)
    {
        if (SeedBLPath == string.Empty || SeedBLPath == null || string.IsNullOrWhiteSpace(SeedBLPath))
            SeedBLPath = Application.dataPath + @"\BlacklistedSeeds.txt";
        if (!File.Exists(SeedBLPath))
            File.CreateText("BlacklistedSeeds");
        StreamWriter sr = new StreamWriter(SeedBLPath, true);

        sr.Write(Seed);
        sr.Close();
    } 

    public static Dictionary<int, string> GetBlacklistedSeeds()
    {
        if (SeedBLPath == string.Empty || SeedBLPath == null || string.IsNullOrWhiteSpace(SeedBLPath))
            SeedBLPath = Application.dataPath + @"\BlacklistedSeeds.txt";
        if (!File.Exists(SeedBLPath))
            File.CreateText("BlacklistedSeeds");

        Dictionary<int, string> Dict = new Dictionary<int, string>();
        
        string[] lines = File.ReadAllLines(SeedBLPath);

        foreach(string s in lines)
        {
            if (!String.IsNullOrEmpty(s))
            {
                Seed se = new Seed(s.Substring(0, 9).Replace(" ", string.Empty));
                Dict.Add(se.GetHashCode(), se.ToString());
            }
        }
        return Dict;
    }
}