using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GridController : MonoBehaviour
{
    [SerializeField] Transform _failLine;
    Level _level;
    Grid _grid;

    List<Bubble> _matchBubbleList = new List<Bubble>();
    List<Vector2Int> _checkAfterIndexList = new List<Vector2Int>();
    List<Vector2Int> _checkedIndexList = new List<Vector2Int>();

    private void Awake()
    {
        _level = GetComponentInParent<Level>();
        _grid = GetComponent<Grid>();
    }

    private void Start()
    {
        GameEvent.OnBubbleReachTargetEvent.AddListener(BubbleReachTarget);
        GameEvent.OnCheckForFallingEvent.AddListener(CheckForFallingNeighbours);
    }

    void BubbleReachTarget(Vector2Int index)
    {
        var neighbours = _grid.NeighbourList(index);
        PushNeighbours(_grid.GetPositionForHexCordinate(index), neighbours);

        if (_grid.GetPositionForHexCordinate(index).y < _failLine.position.y)
            GameEvent.OnLevelFailEvent?.Invoke();

        CheckIndexForMatch(index);
    }

    void CheckIndexForMatch(Vector2Int index, int combo = 0)
    {
        _matchBubbleList.Clear();
        _checkedIndexList.Clear();
        _checkAfterIndexList.Clear();

        //Others shouldn't check this index
        _checkedIndexList.Add(index);
        CheckNeighboursMatch(index,combo);
    }

    void CheckNeighboursMatch(Vector2Int index ,int combo = 0)
    {
        //Get Neighbours
        var neighbours = _grid.NeighbourList(index);
        Bubble bubble = _grid.TileArr[index.x, index.y].Bubble;

        foreach (var neighbour in neighbours)
        {
            //If this index haven't checked before then check it
            if (!_checkedIndexList.Contains(neighbour))
            {
                //Add it to checkList because don't want to check again
                _checkedIndexList.Add(neighbour);
                Bubble neighbourBubble = _grid.TileArr[neighbour.x, neighbour.y].Bubble;

                if (neighbourBubble != null && bubble != null)
                {
                    //It returns matched bubbles
                    var matchedBubble = CheckMatch(bubble, neighbourBubble);
                    if (matchedBubble != null)
                    {
                        //If it haven't matched before, add to list
                        if (!_matchBubbleList.Contains(bubble))
                            _matchBubbleList.Add(bubble);
                        if (!_matchBubbleList.Contains(matchedBubble))
                            _matchBubbleList.Add(matchedBubble);
                    }
                }
            }
        }
        //Is there any checkable neighbours after match
        //if, Get that index and re-Func
        //else, Merge mathed bubbles
        if (_checkAfterIndexList.Count > 0)
        {
            Vector2Int checkIndex = _checkAfterIndexList[0];
            _checkAfterIndexList.RemoveAt(0);
            CheckNeighboursMatch(checkIndex,combo);
        }
        else
        {
            if (_matchBubbleList.Count > 0)
                StartCoroutine(MergeBubblesIE(_matchBubbleList,combo));
        }
    }

    IEnumerator MergeBubblesIE(List<Bubble> bubbleList, int combo = 0)
    {
        int newCombo = combo; 
        //Find bubble index which was added last in the list
        int mergedCount = 0;
        int lastBubbleIndex = bubbleList.Count - 1;
        bool isNeedNewRow = false;
        bool isBigMergeNeedNewRow = false;
        for (int i = 0; i < lastBubbleIndex; i++)
        {
            //Set Tile occupied false
            _grid.TileArr[bubbleList[i].Index.x, bubbleList[i].Index.y].SetTileEmpty();

            //If TopRow has a empty spot need to spawn new row
            if (bubbleList[i].Index.y >= _grid.gridSize.y - 1)
                isNeedNewRow = true;
            //First bubble moved first, if it reach then others too
            bubbleList[i].transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.35f).OnComplete(() => { mergedCount++; });
            bubbleList[i].PopEffect();
        }
        yield return new WaitUntil(()=> mergedCount >= bubbleList.Count - 1);

        int powerOfTwo = FindPowerOfTwo(bubbleList[lastBubbleIndex].Value) + 1;
        int newValue = Mathf.RoundToInt(Mathf.Pow(2, powerOfTwo + lastBubbleIndex));
        bubbleList[lastBubbleIndex].SetBubbleProperties(newValue);

        newCombo++;
        GameEvent.OnMergeEvent?.Invoke(newValue, newCombo);

        for (int i = 0; i < lastBubbleIndex; i++)
        {
            bubbleList[i].Pop();
        }

        //2048 or bigger
        if (newValue >= 2048)
        {
            isBigMergeNeedNewRow = BigMerge(bubbleList[lastBubbleIndex]);
        }
        else
        {
            //Check if there any merge around new bubble
            CheckIndexForMatch(bubbleList[lastBubbleIndex].Index, newCombo);
        }

        if (isNeedNewRow || isBigMergeNeedNewRow)
            CreateNewRow();

        yield return new WaitForEndOfFrame();
        CheckLowLevel();
    }

    //Return true if it needs newRow
    bool BigMerge(Bubble bubble)
    {
        bool isNeedNewRow=false;
        bubble.PopEffect();
        bubble.Pop();

        _grid.TileArr[bubble.Index.x, bubble.Index.y].SetTileEmpty();

        if (bubble.Index.y >= _grid.gridSize.y - 1)
            isNeedNewRow = true;

        //Explode
        foreach (var item in _grid.NeighbourList(bubble.Index))
        {
            if (!_grid.TileArr[item.x, item.y].IsOccupied) continue;
            _grid.TileArr[item.x, item.y].Bubble.PopEffect();
            _grid.TileArr[item.x, item.y].Bubble.Pop();

            _grid.TileArr[item.x, item.y].SetTileEmpty();

            if (item.y >= _grid.gridSize.y - 1)
                isNeedNewRow = true;
        }

        return isNeedNewRow;
    }

    void CheckForFallingNeighbours(Vector2Int index)
    {
        var neighbours = _grid.NeighbourList(index);

        foreach (var neighbour in neighbours)
        {
            if(_grid.TileArr[neighbour.x, neighbour.y].IsOccupied)
                CheckForFalling(neighbour);
        }
    }

    void CheckForFalling(Vector2Int index)
    {
        if (index.y >= _grid.gridSize.y-1) return;
        bool isFallen = false;

        //Check For Vertical
        //if even row-> -1 & itself
        //if odd row-> itself & +1
        if (index.y % 2 == 0)//even
        {
            if (index.x - 1 >= 0)
            {
                isFallen = !(_grid.TileArr[index.x - 1, index.y + 1].IsOccupied || _grid.TileArr[index.x, index.y + 1].IsOccupied);
            }else
                isFallen = !_grid.TileArr[index.x, index.y + 1].IsOccupied;

        }
        else//odd
        {
            if (index.x + 1 < _grid.gridSize.x)
            {
                isFallen = !(_grid.TileArr[index.x + 1, index.y + 1].IsOccupied || _grid.TileArr[index.x, index.y + 1].IsOccupied);
            }else
                isFallen = !_grid.TileArr[index.x, index.y + 1].IsOccupied;
        }

        //Check for Horizontal
        //if (index.x > 0 && index.x < _grid.gridSize.x - 1)
        //{
        //    //-1 & +1
        //    isFallen = !_grid.TileArr[index.x - 1, index.y].IsOccupied;
        //    isFallen = !_grid.TileArr[index.x + 1, index.y].IsOccupied;
        //}
        //else if(index.x <= 0)
        //{
        //    //+1 
        //}
        //else
        //{
        //    //-1 
        //}

        if (isFallen)
        {
            _grid.TileArr[index.x, index.y].Bubble.Fall();
            _grid.TileArr[index.x, index.y].SetTileEmpty();
        }
    }

    Bubble CheckMatch(Bubble bubble, Bubble neighbourBubble)
    {
        if (bubble.Value == neighbourBubble.Value)
        {
            //Daha sonra komşularını kontrol etmek için listeye ekliyoruz
            _checkAfterIndexList.Add(neighbourBubble.Index);
            return neighbourBubble;
        }
        return null;
    }

    void CheckLowLevel()
    {
        bool hasBubble = false;
        int lowerY = int.MaxValue;
        for (int x = 0; x < _grid.gridSize.x; x++)
        {
            for (int y = 0; y < _grid.gridSize.y; y++)
            {
                //Check grid.Y -1 (because of create new row after top row pop)
                if (y< _grid.gridSize.y - 1 && _grid.TileArr[x, y].IsOccupied)
                    hasBubble = true;
                if (_grid.TileArr[x, y].IsOccupied && y < lowerY)
                    lowerY = y;
            }
        }
        if (!hasBubble)
            GameEvent.OnPerfectEvent?.Invoke();

        if (_grid.gridSize.y - lowerY <= 2)
            CreateNewRow();

        if (_grid.TileArr[0, lowerY].transform.position.y < _failLine.position.y)
            GameEvent.OnLevelFailEvent?.Invoke();
    }

    void CreateNewRow()
    {
        _grid.AddNewRow(CheckLowLevel);
        _level.CreateRandomBubbleRow(_grid.gridSize.y - 1, _grid.gridSize.x);
    }

    void PushNeighbours(Vector3 origin, List<Vector2Int> neighbourList)
    {
        foreach (var neighbour in neighbourList)
        {
            if (_grid.TileArr[neighbour.x, neighbour.y].transform.childCount > 0)
            {
                var bubbleTransfom = _grid.TileArr[neighbour.x, neighbour.y].transform.GetChild(0);
                bubbleTransfom.DOLocalMove((bubbleTransfom.position - origin).normalized * 0.1f, 0.05f).SetLoops(2, LoopType.Yoyo);
            }
        }
    }

    int FindPowerOfTwo(int value)
    {
        int tempVal = value;
        int pow = 0;
        while (tempVal > 2)
        {
            tempVal /= 2;
            pow++;
        }

        return pow;
    }
}
