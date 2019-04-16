using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Resources;
using System;
using Enums;

public static class FileHandler
{

    public static string Numeric = "0123456789";
    public static string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static IGame ImportFiles(IGame game)
    {
        string path = Application.dataPath + @"\GameData";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] text = dir.GetFiles("*.txt");

        foreach (FileInfo p in text)
            game = ReadText(p.FullName, game);

        return game;
    }

    private static IGame ReadText(string path, IGame game)
    {
        string[] lines = File.ReadAllLines(path);

        state state = state.None;

        List<state> StateHist = new List<state>();

        Dictionary<int, Effect> ListOfEffects = new Dictionary<int, Effect>();

        int curEffect = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line.Contains("}"))
                if (StateHist.Count > 1)
                {
                    state = StateHist[StateHist.Count - 1];
                    StateHist.RemoveAt(StateHist.Count - 1);
                }

            line = line.Replace("\t", "").Replace("\r", "").Replace("\n", "");

            if(line.LastIndexOf(' ') > -1)
                line = line.Remove(line.LastIndexOf(' '));

            if (line.Contains("\\"))
                line = line.Remove(line.IndexOf("\\"));
            if (line.Contains("##"))
                line = line.Remove(line.IndexOf("##"));
            if (line.Contains("//"))
                line = line.Remove(line.IndexOf("//"));

            Debug.Log(line);

            switch (state)
            {
                case state.None:
                    switch (line.ToUpper().Replace(" ", string.Empty))
                    {
                        case "DEFINITION":
                            StateHist.Add(state);
                            state = state.Definition;
                            break;
                        default:
                            break;
                    }
                    break;
                case state.Definition:
                    switch (line.ToUpper().Replace(" ", string.Empty))
                    {
                        case "EFFECT":
                            StateHist.Add(state);
                            state = state.NewEffect;
                            curEffect++;
                            ListOfEffects.Add(curEffect, new Effect());
                            break;
                        case "MODIFIER":
                            break;
                        default:
                            break;
                    }
                    break;
                case state.NewEffect:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split('=');
                        switch (separatedLine[0].ToUpper().Replace(" ", string.Empty))
                        {
                            case "NAME":
                                effect.SetName(separatedLine[1]);
                                break;
                            case "FREQUENCY":
                                switch (separatedLine[1].ToUpper().Replace(" ", string.Empty))
                                {
                                    case "PERIODICAL":
                                        effect.SetActivation(ActivationType.Periodical);
                                        break;
                                    case "ONCE":
                                        effect.SetActivation(ActivationType.Once);
                                        break;
                                    case "TRIGERRED":
                                        effect.SetActivation(ActivationType.Triggered);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "DAMAGE":
                                effect.SetDamage(float.Parse(separatedLine[1].Replace(" ", string.Empty)));
                                break;
                            case "DURATION":
                                effect.SetDuration(float.Parse(separatedLine[1].Replace(" ", string.Empty)));
                                break;
                            case "INTERVAL":
                                effect.SetInterval(float.Parse(separatedLine[1].Replace(" ", string.Empty)));
                                break;
                            case "SPRITE":
                                StateHist.Add(state);
                                state = state.SetSprite;
                                break;
                            case "MODIFIEDBY":
                                StateHist.Add(state);
                                state = state.SetModifiedBy;
                                break;
                            case "INFLICTWHEN":
                                StateHist.Add(state);
                                state = state.SetInflictIfConditions;
                                break;
                            case "TRIGGERWHEN":
                                StateHist.Add(state);
                                state = state.SetTriggerIfConditions;
                                break;
                            case "STOPWHEN":
                                StateHist.Add(state);
                                state = state.SetStopIfConditions;
                                break;
                            case "REMOVEWHEN":
                                StateHist.Add(state);
                                state = state.SetRemoveIfConditions;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case state.SetModifiedBy:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper())
                        {
                            case "DAMAGE":
                                effect.ModifiedBy.Add(separatedLine[1] + " DAMAGE", new System.Tuple<Modifiable, float>(Modifiable.Damage, float.Parse(separatedLine[2])));
                                break;
                            case "DURATION":
                                effect.ModifiedBy.Add(separatedLine[1] + " DURATION", new System.Tuple<Modifiable, float>(Modifiable.Duration, float.Parse(separatedLine[2])));
                                break;
                            case "INTERVAL":
                                effect.ModifiedBy.Add(separatedLine[1] + " INTERVAL", new System.Tuple<Modifiable, float>(Modifiable.Interval, float.Parse(separatedLine[2])));
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case state.SetInflictIfConditions:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper())
                        {
                            case "EFFECT":
                                effect.InflictIf.Add(separatedLine[2], new System.Tuple<string, OperatorType, string>(separatedLine[0], separatedLine[1].Contains("Have") ? OperatorType.Have : OperatorType.NotHave, separatedLine[2]));
                                break;
                            case "HEALTH":
                                switch (separatedLine[1])
                                {
                                    case ">":
                                        effect.InflictIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerThan, separatedLine[2]));
                                        break;
                                    case ">=":
                                        effect.InflictIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "=":
                                        effect.InflictIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.EqualsTo, separatedLine[2]));
                                        break;
                                    case "<=":
                                        effect.InflictIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "<":
                                        effect.InflictIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerThan, separatedLine[2]));
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case state.SetTriggerIfConditions:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper())
                        {
                            case "EFFECT":
                                effect.TriggerIf.Add(separatedLine[2], new System.Tuple<string, OperatorType, string>(separatedLine[0], separatedLine[1].Contains("Have") ? OperatorType.Have : OperatorType.NotHave, separatedLine[2]));
                                break;
                            case "HEALTH":
                                switch (separatedLine[1])
                                {
                                    case ">":
                                        effect.TriggerIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerThan, separatedLine[2]));
                                        break;
                                    case ">=":
                                        effect.TriggerIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "=":
                                        effect.TriggerIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.EqualsTo, separatedLine[2]));
                                        break;
                                    case "<=":
                                        effect.TriggerIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "<":
                                        effect.TriggerIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerThan, separatedLine[2]));
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case state.SetStopIfConditions:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper())
                        {
                            case "EFFECT":
                                effect.StopIf.Add(separatedLine[2], new System.Tuple<string, OperatorType, string>(separatedLine[0], separatedLine[1].Contains("Have") ? OperatorType.Have : OperatorType.NotHave, separatedLine[2]));
                                break;
                            case "HEALTH":
                                switch (separatedLine[1])
                                {
                                    case ">":
                                        effect.StopIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerThan, separatedLine[2]));
                                        break;
                                    case ">=":
                                        effect.StopIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "=":
                                        effect.StopIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.EqualsTo, separatedLine[2]));
                                        break;
                                    case "<=":
                                        effect.StopIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "<":
                                        effect.StopIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerThan, separatedLine[2]));
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case state.SetRemoveIfConditions:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper())
                        {
                            case "EFFECT":
                                effect.RemoveIf.Add(separatedLine[2], new System.Tuple<string, OperatorType, string>(separatedLine[0], separatedLine[1].Contains("Have") ? OperatorType.Have : OperatorType.NotHave, separatedLine[2]));
                                break;
                            case "HEALTH":
                                switch (separatedLine[1])
                                {
                                    case ">":
                                        effect.RemoveIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerThan, separatedLine[2]));
                                        break;
                                    case ">=":
                                        effect.RemoveIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.BiggerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "=":
                                        effect.RemoveIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.EqualsTo, separatedLine[2]));
                                        break;
                                    case "<=":
                                        effect.RemoveIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerOrEqualsThan, separatedLine[2]));
                                        break;
                                    case "<":
                                        effect.RemoveIf.Add("HEALTH", new System.Tuple<string, OperatorType, string>(separatedLine[0], OperatorType.LowerThan, separatedLine[2]));
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case state.SetSprite:
                    {
                        Effect effect = ListOfEffects[curEffect];
                        string[] separatedLine = line.Split(':');
                        switch (separatedLine[0].ToUpper().Replace(" ", string.Empty))
                        {
                            case "ANIMATED":
                                effect.Sprite.SetAnimated(bool.Parse(separatedLine[1]));
                                break;
                            case "FILE":
                                string TexturePath = path.Remove(path.LastIndexOf('/')+1);
                                TexturePath += separatedLine[1];

                                effect.Sprite.SetPath(TexturePath);
                                break;
                            case "SIZE":
                                StateHist.Add(state);
                                state = state.GetSpriteSizes;
                                break;
                        }
                    }
                    break;
                case state.GetSpriteSizes:
                    {
                        Effect effect = ListOfEffects[curEffect];

                        line = FilterString(line, Numeric);

                        int Size = int.Parse(line);

                        effect.Sprite.Sprites.Add(Size, null);
                    }
                    break;
            }
        }

        foreach (Effect e in ListOfEffects.Values)
            Debug.Log(e.ToLongString());

        game.Effects.AddRange(ListOfEffects.Values);

        return game;
    }

    private static Texture2D ReadImage(string path)
    {
        /* 
         * 512*512, 256*256, 128*128, 64*64, 32*32, 16*16
         * 512*512, 768*256, 896*128, 960*64, 992*32, 1008*16
        */

        Texture2D Texture = new Texture2D(1024, 512)
        {
            alphaIsTransparency = true,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        Texture.LoadImage(File.ReadAllBytes(path));
        Texture.Apply();

        return Texture;
    }

    public static Dictionary<int, Sprite> GetStaticSprites(Dictionary<int, Sprite> Sprites, string path)
    {
        Texture2D text = ReadImage(path);

        foreach (int t in Sprites.Keys)
        {
            Texture2D SpriteTexture = new Texture2D(t, t)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                alphaIsTransparency = true
            };

            SpriteTexture.SetPixels(text.GetPixels(t, text.height - t, t, t));
            SpriteTexture.Apply();
        }




        return Sprites;
    }

    private static string RemoveString(string String, string Remove)
    {
        string s = String;

        for (int i = 0; i < s.Length; i++)
        {
            if (Remove.Contains(s[i].ToString()))
            {
                s = s.Remove(s.IndexOf(s[i]), 1);
                i--;
            }
        }

        return s;
    }
    private static string FilterString(string String, string Filter)
    {
        string s = String;

        for (int i = 0; i < s.Length; i++)
        {
            if (!Filter.Contains(s[i].ToString()))
            {
                s = s.Remove(s.IndexOf(s[i]), 1);
                i--;
            }
        }

        return s;
    }
}