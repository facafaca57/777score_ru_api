using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public static class SaveSystem
{
    public static void SaveData(List<DataJSON> dataJSON, string path) {
        string json = "";
        string AllinOne = "";
        for (int i = 0; i < dataJSON.Count; i++)
        {   
            json += JsonUtility.ToJson(dataJSON[i]) + "|";
        }
        AllinOne = json + DateTime.Now;

        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, AllinOne);
        stream.Close();
    }

    public static string LoadData() {
        string path = Application.persistentDataPath + "/data.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            var save = (string)formatter.Deserialize(stream);
            stream.Close();
            return save;
        }
        else 
        {
            Debug.LogError("Save File not found " + path);
            return null;
        }
    }
}
