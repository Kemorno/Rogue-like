using System.IO;
using System.Collections.Generic;

public class SerializeINI
{
    public static void Serialize(string name, string GamePath)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("name=" + name);
        System.IO.StreamWriter writer = new System.IO.StreamWriter(GamePath);
        writer.Write(sb.ToString());
        writer.Close();
    }

    public static string DeSerialize(string name, string GamePath)
    {
        System.IO.StreamReader reader = new System.IO.StreamReader(GamePath);
        string line;
        while ((line = reader.ReadLine()) != string.Empty)
        {
            string[] id_value = line.Split('=');
            switch (id_value[0])
            {
                case "name":
                    name = id_value[1].ToString();
                    break;
            }
        }
        reader.Close();
        return name;
    }
}
public class TextHandler
{
}