using UnityEngine;
using System.Collections.Generic;

public static class Game
{
    public class Mod
    {
        public string Name = "Mod_Name";
        public int MajorVersion = 1;
        public int MinorVersion = 0;
        public int Patch = 0;
        public string Author = "Unknown";

        public Mod()
        {

        }
        public Mod(string _ModName)
        {
            Name = _ModName;
        }
        public Mod(string _ModName, string _Author)
        {
            Name = _ModName;
            Author = _Author;
        }
        public Mod(string _ModName, int _MajorVersion, int _MinorVersion = 0, int _Patch = 0)
        {
            Name = _ModName;
            MajorVersion = _MajorVersion;
            MinorVersion = _MinorVersion;
            Patch = _Patch;
        }
        public Mod(string _ModName, string _Author, int _MajorVersion, int _MinorVersion = 0, int _Patch = 0)
        {
            Name = _ModName;
            Author = _Author;
            MajorVersion = _MajorVersion;
            MinorVersion = _MinorVersion;
            Patch = _Patch;
        }
    }
    public class Tile
    {
        public string name = "tile_default";
        public float WalkSpeed = 1f;
        public Mod Mod = new Mod();
        public Sprite[] sprites = new Sprite[9];
        public List<Object> objects = new List<Object>();
        
        public Tile(string _Name)
        {
            name = _Name;
        }
        public Tile(string _Name, Mod _Mod)
        {
            name = _Name;
            Mod = _Mod;
        }
    }
}