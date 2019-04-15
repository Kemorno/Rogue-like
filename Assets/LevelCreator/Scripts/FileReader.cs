using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Resources;
using Enums;

public class FileReader : MonoBehaviour
{
    private void Awake()
    {
        ImportFiles();
    }

    public void ImportFiles()
    {
        string path = Application.dataPath + @"\GameData";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo p in info)
            Read(p.FullName);
    }

    void Read(string path)
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

            line = line.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", string.Empty);

            if (line.Contains("\\"))
                line = line.Remove(line.IndexOf("\\"));
            if (line.Contains("##"))
                line = line.Remove(line.IndexOf("##"));
            if (line.Contains("//"))
                line = line.Remove(line.IndexOf("//"));

            switch (state)
            {
                case state.None:
                    switch (line.ToUpper())
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
                    switch (line.ToUpper())
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
                        switch (separatedLine[0].ToUpper())
                        {
                            case "NAME":
                                effect.SetName(separatedLine[1]);
                                break;
                            case "FREQUENCY":
                                switch (separatedLine[1].ToUpper())
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
                                effect.SetDamage(float.Parse(separatedLine[1]));
                                break;
                            case "DURATION":
                                effect.SetDuration(float.Parse(separatedLine[1]));
                                break;
                            case "INTERVAL":
                                effect.SetInterval(float.Parse(separatedLine[1]));
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
            }
        }

        foreach (Effect e in ListOfEffects.Values)
            Debug.Log(e.ToLongString());
    }
}
