using System.Collections;
using System.Collections.Generic;
using Game;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : ShowHidable
{
    [SerializeField] private Text _toastTxt;
    [SerializeField]private List<string> _toasts = new List<string>();

    protected override void OnShowCompleted()
    {
        base.OnShowCompleted();

        switch (LevelManager.Instance.GameMode){
            case  GameMode.Easy:
                sp_GameCoinManager.Instance.AddCoin(sp_GameCoinManager.Instance.gameCoin.easyCoin);
                break;
            case GameMode.Normal:
                sp_GameCoinManager.Instance.AddCoin(sp_GameCoinManager.Instance.gameCoin.mediumCoin);
                break;
            case GameMode.Hard:
                sp_GameCoinManager.Instance.AddCoin(sp_GameCoinManager.Instance.gameCoin.hardCoin);
                break;
            
        }
        
        _toastTxt.text = _toasts.GetRandom();
        _toastTxt.gameObject.SetActive(true);

        AdsManager.ShowOrPassAdsIfCan();
    }


    public void OnClickContinue()
    {
        UIManager.Instance.LoadNextLevel();
    }
}