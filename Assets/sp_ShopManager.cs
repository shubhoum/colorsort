using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class sp_ShopManager : MonoBehaviour
{
    public sp_CustomBackground customBackground;
    public sp_CustomTubes customTubes;
    public sp_GameCoin gameCoin;

    #region Public

    public GameObject[] scrollBars;
    public GameObject popUI;
    public TMP_Text title;
    private ShopItems _shopItems;

    public static sp_ShopManager instance = null;

    public sp_BackgroundCard[] spBackgroundCards;
    public sp_TubesCard[] SpTubesCards;

    #endregion

    private void Awake(){
        if (instance == null){
            instance = this;
        }
    }

    public void OnExit(){
        sp_DataManager.Instance.SaveData(sp_DataManager.CustomBackground, sp_DataManager.Type.Backgrounds);
        sp_DataManager.Instance.SaveData(sp_DataManager.CustomTubes, sp_DataManager.Type.Tubes);
        sp_DataManager.Instance.SaveData(sp_DataManager.GameCoin, sp_DataManager.Type.Coins);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnBGCardClick(int index_info){
        if (customBackground.BackgroundList[index_info].isLocked){
            if (customBackground.BackgroundList[index_info].price <= sp_GameCoinManager.Instance.gameCoin.coin){
                OnUnlock(customBackground, index_info, scrollBars[(int)ShopItems.Backgrounds].GetComponent<ScrollRect>
                    ().content);
                return;
            }

            Debug.Log("No Enough Money");
            return;
        }

        for (int index = 0; index < customBackground.BackgroundList.Count; index++){
            if (index == index_info){
                if (!customBackground.BackgroundList[index].isCurrentSelected){
                    customBackground.BackgroundList[index].isCurrentSelected = true;
                    spBackgroundCards[index].bgColor.color = customBackground.selectedColor;
                    customBackground
                        .currentSelectBackground = spBackgroundCards[index].backgroundImage.sprite;
                }
            }

            else{
                customBackground.BackgroundList[index].isCurrentSelected = false;
                spBackgroundCards[index].bgColor.color = customBackground.normalColor;
            }
        }
    }

    public void OnUnlock(sp_CustomBackground customBackgrounds, int index, Transform contents){
        sp_GameCoinManager.Instance.SubtractCoin(customBackgrounds.BackgroundList[index].price);
        // customBackgrounds.BackgroundList[index].price = 0; // Make it free
        customBackgrounds.BackgroundList[index].isLocked = false; // Make it free

        var spCard = contents.GetChild(index).GetComponent<sp_BackgroundCard>();

        spCard.purchased.SetActive(true);
        spCard.lockedObj.SetActive(false);
        spCard.notPurchased.SetActive(false);
    }

    public void OnUnlock(sp_CustomTubes customTubes, int index, Transform contents){
        sp_GameCoinManager.Instance.SubtractCoin(customTubes.tubesList[index].price);
        // customTubes.tubesList[index].price = 0; // Make it free
        customTubes.tubesList[index].isLocked = false; // Make it free

        var spCard = contents.GetChild(index).GetComponent<sp_TubesCard>();

        spCard.purchased.SetActive(true);
        spCard.lockedObj.SetActive(false);
        spCard.notPurchased.SetActive(false);
    }

    public void OnTubeCardClick(int index_info){
        if (customTubes.tubesList[index_info].isLocked){
            if (customTubes.tubesList[index_info].price <= sp_GameCoinManager.Instance.gameCoin.coin){
                OnUnlock(customTubes, index_info, scrollBars[(int)ShopItems.Tubes].GetComponent<ScrollRect>
                    ().content);
                return;
            }

            Debug.Log("No Enough Money");
            return;
        }

        for (int index = 0; index < customTubes.tubesList.Count; index++){
            if (index == index_info){
                if (!customTubes.tubesList[index].isCurrentSelected){
                    customTubes.tubesList[index].isCurrentSelected = true;
                    SpTubesCards[index].bgColor.color = customTubes.selectedColor;
                    customTubes
                        .currentSelectPrefab = SpTubesCards[index].tubesModel;
                }
            }

            else{
                customTubes.tubesList[index].isCurrentSelected = false;
                SpTubesCards[index].bgColor.color = customTubes.normalColor;
            }
        }
    }

    public void OnClick(int index){
        if (!popUI.activeSelf) popUI.SetActive(true);

        for (int i = 0; i < scrollBars.Length; i++){
            if (i == index){
                title.text = ((ShopItems)i).ToString().ToUpper();
                scrollBars[i].SetActive(true);

                if (i == 2) // no need for coins
                    return;
                LoadData(scrollBars[i], i);
            }
            else{
                scrollBars[i].SetActive(false);
            }
        }
    }

    void SetBGCardData(int i, Transform contents){
        var background = customBackground.BackgroundList[i];
        var spCard = contents.GetChild(i).GetComponent<sp_BackgroundCard>();

        spCard.backgroundImage.sprite = background.background;

        if (!background.isLocked){
            spCard.purchased.SetActive(true);
            spCard.lockedObj.SetActive(false);
            spCard.notPurchased.SetActive(false);

            if (background.isCurrentSelected)
                customBackground.currentSelectBackground = spCard.backgroundImage.sprite;
        }
        else{
            spCard.purchased.SetActive(false);
            spCard.lockedObj.GetComponent<Image>().sprite = customBackground.lockedBackground;
            spCard.lockedObj.SetActive(true);
            spCard.notPurchased.SetActive(true);
            spCard.price.text = $"{background.price}";
        }

        spCard.bgColor.color = background.isCurrentSelected
            ? customBackground.selectedColor
            : customBackground
                .normalColor;
    }

    void SetTubesCardData(int i, Transform contents){
        var tubes = customTubes.tubesList[i];
        var spCard = contents.GetChild(i).GetComponent<sp_TubesCard>();

        spCard.tubesModel = tubes.tubesPreab;
        if (!tubes.isLocked){
            spCard.purchased.SetActive(true);
            spCard.lockedObj.SetActive(false);
            spCard.notPurchased.SetActive(false);

            if (tubes.isCurrentSelected)
                customTubes.currentSelectPrefab = spCard.tubesModel;
        }
        else{
            spCard.purchased.SetActive(false);
            spCard.lockedObj.GetComponent<Image>().sprite = customTubes.lockedBackground;
            spCard.lockedObj.SetActive(true);
            spCard.notPurchased.SetActive(true);
            spCard.price.text = $"{tubes.price}";
        }

        spCard.bgColor.color = tubes.isCurrentSelected
            ? customTubes.selectedColor
            : customTubes
                .normalColor;
    }

    void LoadData(GameObject parentObj, int index){
        switch (index){
            case 0:
                for (int i = 0; i < customBackground.BackgroundList.Count; i++){
                    SetBGCardData(i, parentObj.GetComponent<ScrollRect>().content);
                }

                break;

            case 1:
                for (int i = 0; i < customTubes.tubesList.Count; i++){
                    SetTubesCardData(i, parentObj.GetComponent<ScrollRect>().content);
                }

                break;
        }
    }

    private enum ShopItems
    {
        Backgrounds = 0,
        Tubes = 1,
        Coins = 2,
    }
}