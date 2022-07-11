// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance{ get; private set; }
    public static event Action LevelCompleted;

    [SerializeField] private float _minXDistanceBetweenHolders;
    [SerializeField] private Camera _camera;
    public Holder _holderPrefab;

    [SerializeField] private AudioClip _winClip;

    public GameMode GameMode{ get; private set; } = GameMode.Easy;
    public Level Level{ get; private set; }

    private readonly List<Holder> _holders = new List<Holder>();

    private readonly Stack<MoveData> _undoStack = new Stack<MoveData>();

    public State CurrentState{ get; private set; } = State.None;

    public bool HaveUndo => _undoStack.Count > 0;

    public sp_CustomTubes customTubes;
    public sp_GameCoin gameCoin;

    public bool IsTransfer{ get; set; }


    private void Awake(){
        Instance = this;
        _holderPrefab = customTubes.currentSelectPrefab.GetComponent<Holder>();
        var loadGameData = GameManager.LoadGameData;
        GameMode = loadGameData.GameMode;
        Level = loadGameData.Level;
        LoadLevel(Level.map.Count);
        CurrentState = State.Playing;
    }

    private void LoadLevel(int totalTubes){
        var list = PositionsForHolders(totalTubes, out var width).ToList();
        _camera.orthographicSize = 0.5f * width * Screen.height / Screen.width;

        var levelMap = Level.LiquidDataMap;
        for (var i = 0; i < levelMap.Count; i++){
            var levelColumn = levelMap[i];
            var holder = Instantiate(_holderPrefab, list[i], Quaternion.identity);
            holder.Init(levelColumn);
            _holders.Add(holder);
        }
    }

    public void AddNewTube(){
        if (gameCoin.addTubeCount == 0){
            Debug.LogError("Buy");
            return;
        }

        gameCoin.addTubeCount--;
        sp_DataManager.Instance.SaveData(sp_DataManager.GameCoin, sp_DataManager.Type.Coins);
        Debug.Log("Coin Database Updated..");
        
        var list = PositionsForHolders(_holders.Count + 1, out var width).ToList();
        _camera.orthographicSize = 0.5f * width * Screen.height / Screen.width;
        var holder = Instantiate(_holderPrefab, list[_holders.Count - 1], Quaternion.identity);
        _holders.Add(holder);
        for (int i = 0; i < _holders.Count; i++){
            _holders[i].transform.position = list[i]; // Change current position
            _holders[i].OverridePosition(list[i]); // Change original position
        }
    }

    public void OnClickUndo(){
        
        if (gameCoin.tuneUndoCount == 0){
            Debug.LogError("Buy");
            return;
        }

        if (CurrentState != State.Playing || _undoStack.Count <= 0 || IsTransfer)
            return;
        
        gameCoin.tuneUndoCount--;
        sp_DataManager.Instance.SaveData(sp_DataManager.GameCoin, sp_DataManager.Type.Coins);
        Debug.Log("Coin Database Updated..");

        var moveData = _undoStack.Pop();

        foreach (var holder in _holders){
            if (holder.Equals(moveData.FromHolder)){
                holder.AddLiquid(moveData.Liquid.GroupId, moveData.TransferAmount);
            }

            if (holder.Equals(moveData.ToHolder)){
                holder.RemoveLiquid(holder.TopLiquid, moveData.TransferAmount);
            }
        }
    }

    private void Update(){
        if (CurrentState != State.Playing)
            return;

        if (Input.GetMouseButtonDown(0)){
            var collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition));
            if (collider != null){
                var holder = collider.GetComponent<Holder>();

                if (holder != null)
                    OnClickHolder(holder);
            }
        }
    }

    public void AddLatestMove(Holder fromHolder, Holder toHolder, Liquid liquid, float amount){
        Debug.Log("From Holder " + fromHolder + " --- " + "To Holder " + toHolder + " --- " + "Liquid " + liquid +
                  " --- " + "Amount " + amount);
        MoveData moveData = new MoveData();
        moveData.FromHolder = fromHolder;
        moveData.ToHolder = toHolder;
        moveData.Liquid = liquid;
        moveData.TransferAmount = amount;
        _undoStack.Push(moveData);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnClickHolder(Holder holder){
        // if (IsTransfer) // Enabling this only allows one tube to work at a time
        //     return;

        var pendingHolder = _holders.FirstOrDefault(h => h.IsPending);

        if (pendingHolder != null && pendingHolder != holder){
            if (holder.TopLiquid == null ||
                (pendingHolder.TopLiquid.GroupId == holder.TopLiquid.GroupId && !holder.IsFull)){
                // Save Last move 
                IsTransfer = true;
                StartCoroutine(SimpleCoroutine.CoroutineEnumerator(
                    pendingHolder.MoveAndTransferLiquid(holder, CheckAndGameOver), () => {
                        IsTransfer = false;
                        if (holder.IsCompleted()){
                            // Enable Particle Effect
                            // Add Completed Tag
                            // Add Cover
                        }
                    }));
            }
            else{
                pendingHolder.ClearPending();
                holder.StartPending();
            }
        }
        else if (holder.Liquids.Any()){
            if (!holder.IsPending){
                holder.StartPending();
            }
            else{
                holder.ClearPending();
            }
        }
    }

    private void CheckAndGameOver(){
        if (_holders.All(holder => {
                var liquids = holder.Liquids.ToList();
                return liquids.Count == 0 || liquids.Count == 1;
            }) && _holders.Where(holder => holder.Liquids.Any()).GroupBy(holder => holder.Liquids.First().GroupId)
                .All(holders => holders.Count() == 1)){
            OverTheGame();
        }
    }

    private void OverTheGame(){
        if (CurrentState != State.Playing)
            return;

        PlayClipIfCan(_winClip);
        CurrentState = State.Over;

        ResourceManager.CompleteLevel(GameMode, Level.no);
        LevelCompleted?.Invoke();
    }

    private void PlayClipIfCan(AudioClip clip, float volume = 0.35f){
        if (!AudioManager.IsSoundEnable || clip == null)
            return;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }

    public IEnumerable<Vector2> PositionsForHolders(int count, out float expectWidth){
        expectWidth = 4 * _minXDistanceBetweenHolders;
        if (count <= 6){
            var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                           Vector3.up * 1f;

            expectWidth = Mathf.Max(count * _minXDistanceBetweenHolders, expectWidth);

            return Enumerable.Range(0, count)
                .Select(i => (Vector2)minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
        }

        var aspect = (float)Screen.width / Screen.height;

        var maxCountInRow = Mathf.CeilToInt(count / 2f);

        if ((maxCountInRow + 1) * _minXDistanceBetweenHolders > expectWidth){
            expectWidth = (maxCountInRow + 1) * _minXDistanceBetweenHolders;
        }

        var height = expectWidth / aspect;


        var list = new List<Vector2>();
        var topRowMinPoint = transform.position + Vector3.up * height / 6f -
                             ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                             Vector3.up * 1f;
        list.AddRange(Enumerable.Range(0, maxCountInRow)
            .Select(i => (Vector2)topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

        var lowRowMinPoint = transform.position - Vector3.up * height / 6f -
                             (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                             Vector3.up * 1f;
        list.AddRange(Enumerable.Range(0, count - maxCountInRow)
            .Select(i => (Vector2)lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

        return list;
    }


    public enum State
    {
        None,
        Playing,
        Over
    }

    public struct MoveData
    {
        public Holder FromHolder{ get; set; }
        public Holder ToHolder{ get; set; }
        public Liquid Liquid{ get; set; }
        public float TransferAmount{ get; set; }
    }
}

[Serializable]
public struct LevelGroup : IEnumerable<Level>
{
    public List<Level> levels;

    public IEnumerator<Level> GetEnumerator(){
        return levels?.GetEnumerator() ?? Enumerable.Empty<Level>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator(){
        return GetEnumerator();
    }
}

[Serializable]
public struct Level
{
    public int no;
    public List<LevelColumn> map;

    public List<IEnumerable<LiquidData>> LiquidDataMap => map.Select(GetLiquidDatas).ToList();

    public static IEnumerable<LiquidData> GetLiquidDatas(LevelColumn column){
        var list = column.ToList();

        for (var j = 0; j < list.Count; j++){
            var currentGroup = list[j];
            var count = 0;
            for (; j < list.Count; j++){
                if (currentGroup == list[j]){
                    count++;
                }
                else{
                    j--;
                    break;
                }
            }

            yield return new LiquidData{
                groupId = currentGroup,
                value = count
            };
        }
    }
}

public struct LiquidData
{
    public int groupId;
    public float value;
}

[Serializable]
public struct LevelColumn : IEnumerable<int>
{
    public List<int> values;

    public IEnumerator<int> GetEnumerator(){
        return values?.GetEnumerator() ?? Enumerable.Empty<int>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator(){
        return GetEnumerator();
    }
}