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

    public void Save()
    {
        PlayerPrefs.SetFloat("scoreTop", scoreTop);
        PlayerPrefs.SetInt("adUnitCd", adUnitCd);
        PlayerPrefs.SetInt("isMusicEnabledInt", isMusicEnabled ? 1 : 0);
    }

    public void Load()
    {
        scoreTop = PlayerPrefs.GetFloat("scoreTop");
        adUnitCd = PlayerPrefs.GetInt("adUnitCd");
        isMusicEnabled = PlayerPrefs.GetInt("isMusicEnabledInt") > 0 ? true : false;
    }
}

[Serializable]
class PlayerData 
{
    public float scoreTop;
    public int adUnitCd;
    public bool isMusicEnabled;
}
