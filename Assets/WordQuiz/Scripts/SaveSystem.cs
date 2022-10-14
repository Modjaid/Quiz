using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class  SaveSystem 
{
    public static void SaveSetting(DataSetting startGame)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.txt";

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, startGame);
        }
    }

    public static DataSetting LoadData()
    {
        string path = Application.persistentDataPath + "/Player.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                DataSetting data = formatter.Deserialize(stream) as DataSetting;

                return data;
            }
        }
        else
        {
            return null;
        }
    }
}
