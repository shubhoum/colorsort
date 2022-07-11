// /*
// Created by Darsan
// */

using System;
using Game;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanel : ShowHidable
{
    [SerializeField] private Text _lvlTxt;

    private void Start()
    {
        _lvlTxt.text = $"Level {LevelManager.Instance.Level.no}";
    }

    public void OnClickUndo()
    {
        LevelManager.Instance.OnClickUndo();
    }

    public void OnClickRestart()
    {
        GameManager.LoadGame(new LoadGameData
        {
            Level = LevelManager.Instance.Level,
            GameMode = LevelManager.Instance.GameMode,
        },false);
    }

    public void OnClickSkip()
    {
        if (!AdsManager.IsVideoAvailable())
        {
            SharedUIManager.PopUpPanel.ShowAsInfo("Connection?","Sorry no video ads available.Check your internet connection!");
            return;
        }

        SharedUIManager.PopUpPanel.ShowAsConfirmation("Skip","Watch Video to skip this level", success =>
        {
            if(!success)
                return;

            AdsManager.ShowVideoAds(true, s =>
            {
                if(!s)
                    return;
                ResourceManager.CompleteLevel(LevelManager.Instance.GameMode, LevelManager.Instance.Level.no);
                UIManager.Instance.LoadNextLevel();
            });
          
        });
    }

    public void OnClickMenu()
    {
        SharedUIManager.PopUpPanel.ShowAsConfirmation("Exit?","Are you sure want to exit the game?", success =>
        {
            if(!success)
                return;

            GameManager.LoadScene("MainMenu");
        });
    }

    private void Update()
    {
    }
}