using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class sp_DataManager : MonoBehaviour
{
    public static sp_DataManager Instance;

    [SerializeField] private sp_CustomTubes customTubesObject;
    [SerializeField] private sp_CustomBackground customBackgroundObject;
    [SerializeField] private sp_GameCoin gameCoin;

    #region PlayerPref Const Key Name

    public static readonly String GameCoin = "GameCoin";
    public static readonly String CustomTubes = "CustomTubes";
    public static readonly String CustomBackground = "CustomBackground";

    #endregion

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    bool IsSaveFile(string filepath) => Directory.Exists(Application.persistentDataPath + filepath);

    public void SaveData(string filepath, Type type){
        if (IsSaveFile(filepath)){
            Directory.CreateDirectory(Application.persistentDataPath + filepath);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + filepath + $"{type}.txt");
        string json = String.Empty;
        switch (type){
            case Type.Backgrounds:
                json = JsonUtility.ToJson(customBackgroundObject);
                break;
            case Type.Tubes:
                json = JsonUtility.ToJson(customTubesObject);
                break;
            case Type.Coins:
                json = JsonUtility.ToJson(gameCoin);
                break;
        }

        bf.Serialize(file, json);
        file.Close();
        
        Debug.Log(type + " is Saved..");

    }

    public void LoadData(string filepath, Type type){
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + filepath + $"{type}.txt")){
            FileStream file = File.Open(Application.persistentDataPath + filepath + $"{type}.txt",
                FileMode.Open);

            switch (type){
                case Type.Backgrounds:
                    JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), customBackgroundObject);
                    break;
                case Type.Tubes:
                    JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), customTubesObject);
                    break;
                case Type.Coins:
                    JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), gameCoin);
                    break;
            }

            file.Close();
        }
        else{
            Debug.LogError("File doesn't exits..");
        }
        Debug.Log(type + " is Loaded");
    }

    public enum Type
    {
        Backgrounds,
        Tubes,
        Coins
    }
}