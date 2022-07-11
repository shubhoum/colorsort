using System;
using TMPro;
using UnityEngine;

public class sp_GameCoinManager : MonoBehaviour
{
    public sp_GameCoin gameCoin;
    public static sp_GameCoinManager Instance;

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public void SubtractCoin(long amount){
        gameCoin.coin -= amount;
    }

    public void AddCoin(long amount){
        Debug.Log("Added "+ amount + " Coins");
        gameCoin.coin += amount;
    }
}