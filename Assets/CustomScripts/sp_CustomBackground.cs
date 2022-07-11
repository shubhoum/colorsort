using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "CustomBackground")]
public class sp_CustomBackground : ScriptableObject
{
    public static sp_CustomBackground instance = null;
    public Sprite lockedBackground;
    public Sprite currentSelectBackground;
    public Color selectedColor;
    public Color normalColor;

    
    public List<BackgroundDetails> BackgroundList;
    [Serializable]
    public class BackgroundDetails
    {
        public String name;
        public Sprite background;
        public long price;
        public bool isLocked = false;
        public bool isCurrentSelected = false;

    }
}
