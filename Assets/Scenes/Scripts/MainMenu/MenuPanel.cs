// /*
// Created by Darsan
// */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        public void OnClickPlay(){
            sp_GameCoinUI.Instance.ReloadCoin();
            UIManager.Instance.GameModePanel.Show();
        }
        public void OnClickExit()
        {

        }
        public void OpenScene(String openScene){
            SceneManager.LoadScene(openScene);
        }

    }
}