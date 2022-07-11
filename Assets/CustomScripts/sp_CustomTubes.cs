using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomTubes")]
public class sp_CustomTubes : ScriptableObject
{
    public static sp_CustomTubes instance = null;
    public Sprite lockedBackground;
    public Holder currentSelectPrefab;
    public Color selectedColor;
    public Color normalColor;

    
    public List<TubesDetails> tubesList;
    [System.Serializable]
    public class TubesDetails
    {
        public String name;
        public Holder tubesPreab;
        public long price;
        public bool isLocked = false;
        public bool isCurrentSelected = false;

    }
}
