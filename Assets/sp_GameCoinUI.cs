using TMPro;
using UnityEngine;

public class sp_GameCoinUI : MonoBehaviour
{
    public static sp_GameCoinUI Instance;

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
    }

    public TMP_Text text;

    private void Start(){
        ReloadCoin();
    }

    public void ReloadCoin(){
        text.text = $"{sp_GameCoinManager.Instance.gameCoin.coin}";
    }
}