using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameCoin")]
public class sp_GameCoin : ScriptableObject
{
    public long coin = 1000;

    public long easyCoin = 10;
    public long mediumCoin = 20;
    public long hardCoin = 30;
    
    public int tuneUndoCount;
    public int addTubeCount;
    



}