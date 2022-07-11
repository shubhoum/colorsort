using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class sp_ChangeBG : MonoBehaviour
{
    [FormerlySerializedAs("SpCustomSkin")] public sp_CustomBackground spCustomBackground;

    private void Awake(){
        try{
            GetComponent<Image>().sprite = spCustomBackground.currentSelectBackground;
        }
        catch{
            GetComponent<SpriteRenderer>().sprite = spCustomBackground.currentSelectBackground;
        }
    }
}