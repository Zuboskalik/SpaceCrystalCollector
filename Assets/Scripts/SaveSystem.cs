using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    [HideInInspector] public float scoreTop = 0.0f;
    [HideInInspector] public int adUnitCd = 0;
    [HideInInspector] public bool isMusicEnabled = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.scoreTop = scoreTop;
        data.adUnitCd = adUnitCd;
        data.isMusicEnabled = isMusicEnabled;

        bf.Serialize(file, data);

        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            scoreTop = data.scoreTop;
            adUnitCd = data.adUnitCd;
            isMusicEnabled = data.isMusicEnabled;

            GameSystem.instance.updateScore(0);
        }
    }
}

[Serializable]
class PlayerData 
{
    public float scoreTop;
    public int adUnitCd;
    public bool isMusicEnabled;
}
