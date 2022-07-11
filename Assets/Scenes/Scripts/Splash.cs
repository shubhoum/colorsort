// /*
// Created by Darsan
// */

using System;
using System.Collections;
using MyGame;
using UnityEngine;

public class Splash : MonoBehaviour
{
    private IEnumerator Start(){
        try{
            LoadDatabaseData();
        }
        catch{
            Debug.LogError("dATABASE lOAD fAILED");
        }

        if (!AdsManager.HaveSetupConsent){
            SharedUIManager.ConsentPanel.Show();
            yield return new WaitUntil(() => !SharedUIManager.ConsentPanel.Showing);
        }

        GameManager.LoadScene("MainMenu");
    }

    void LoadDatabaseData(){
        sp_DataManager.Instance.LoadData(sp_DataManager.CustomBackground, sp_DataManager.Type.Backgrounds);
        sp_DataManager.Instance.LoadData(sp_DataManager.CustomTubes, sp_DataManager.Type.Tubes);
        sp_DataManager.Instance.LoadData(sp_DataManager.GameCoin, sp_DataManager.Type.Coins);
    }
}