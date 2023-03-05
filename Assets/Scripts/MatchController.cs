using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MatchController : MonoBehaviour
{
    Grid _grid;

    List<Bubble> _matchBubbleList = new List<Bubble>();
    List<Vector2Int> _checkAfterIndexList = new List<Vector2Int>();
    List<Vector2Int> _checkedIndexList = new List<Vector2Int>();

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    private void Start()
    {
        GameEvent.OnBubbleReachTargetEvent.AddListener(BubbleReachTarget);
    }

    void BubbleReachTarget(Vector2Int index)
    {
        var neighbours = _grid.NeighbourList(index);
        PushNeighbours(_grid.GetPositionForHexCordinate(index), neighbours);

        CheckIndexForMatch(index);
    }

    void CheckIndexForMatch(Vector2Int index)
    {
        _matchBubbleList.Clear();
        _checkedIndexList.Clear();
        _checkAfterIndexList.Clear();

        //Others shouldn't check this index
        _checkedIndexList.Add(index);
        CheckNeighboursMatch(index);
    }

    void CheckNeighboursMatch(Vector2Int index)
    {
        //Get Neighbours
        var neighbours = _grid.NeighbourList(index);
        Bubble bubble = _grid.TileArr[index.x, index.y].Bubble;

        foreach (var neighbour in neighbours)
        {
            //If this index haven't checked before then check it
            if (!_checkedIndexList.Contains(neighbour))
            {
                //kontrol edilmişler listesine ekliyoruz ki bir daha kontrol edilmesin
                _checkedIndexList.Add(neighbour);
                Bubble neighbourBubble = _grid.TileArr[neighbour.x, neighbour.y].Bubble;

                if (neighbourBubble != null && bubble != null)
                {
                    //Bubble'lar match oluyorsa match olan bubble'ı geri döndürüyor bize
                    var matchedBubble = CheckMatch(bubble, neighbourBubble);
                    if (matchedBubble != null)
                    {
                        //Daha önce eşleşmiş bubble değilse listeye ekliyoruz
                        if (!_matchBubbleList.Contains(bubble))
                            _matchBubbleList.Add(bubble);
                        if (!_matchBubbleList.Contains(matchedBubble))
                            _matchBubbleList.Add(matchedBubble);
                    }
                }
            }
        }
        //Komşuları Kontrol edilecek match olmuş bubble var mı
        //varsa, o index'i alıp tekrardan fonksiyonu çalıştır
        //yoksa, match olan bubble'ları birleştir 
        if (_checkAfterIndexList.Count > 0)
        {
            Vector2Int checkIndex = _checkAfterIndexList[0];
            _checkAfterIndexList.RemoveAt(0);
            CheckNeighboursMatch(checkIndex);
        }
        else
        {
            if (_matchBubbleList.Count > 0)
                StartCoroutine(MergeBubblesIE(_matchBubbleList));//MergeBubbles(_matchBubbleList);
        }
    }

    public void MergeBubbles(List<Bubble> bubbleList)
    {
        //Find bubble index which was added last in the list
        int lastBubbleIndex = bubbleList.Count - 1;
        for (int i = 0; i < lastBubbleIndex; i++)
        {
            //Set Tile occupied false
            _grid.TileArr[bubbleList[i].Index.x, bubbleList[i].Index.y].IsOccupied = false;
            _grid.TileArr[bubbleList[i].Index.x, bubbleList[i].Index.y].Bubble = null;

            //First bubble moved first, if it reach then others too
            if (i == 0)
                bubbleList[i].transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.2f).OnComplete(() =>
                {
                    bubbleList[i].Pop();

                    int powerOfTwo = FindPowerOfTwo(bubbleList[lastBubbleIndex].Value) + 1;
                    int newValue = Mathf.RoundToInt(Mathf.Pow(2, powerOfTwo + lastBubbleIndex));
                    bubbleList[lastBubbleIndex].SetBubbleProperties(newValue);

                    //Merge olduktan sonra yeni oluşan bubble ile etrafında merge var mı kontrollü
                    CheckIndexForMatch(bubbleList[lastBubbleIndex].Index);
                });
            else
            {
                var bubble = bubbleList[i];
                bubble.transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.2f).OnComplete(() => { bubble.Pop(); });

            }
        }

        for (int i = 0; i < lastBubbleIndex; i++)
        {
            //todo: Warn neighbours for falling
            CheckForFallingNeighbours(bubbleList[i].Index);
        }
    }

    IEnumerator MergeBubblesIE(List<Bubble> bubbleList)
    {
        //Find bubble index which was added last in the list
        int mergedCount = 0;
        int lastBubbleIndex = bubbleList.Count - 1;
        for (int i = 0; i < lastBubbleIndex; i++)
        {
            //Set Tile occupied false
            _grid.TileArr[bubbleList[i].Index.x, bubbleList[i].Index.y].IsOccupied = false;
            _grid.TileArr[bubbleList[i].Index.x, bubbleList[i].Index.y].Bubble = null;

            //First bubble moved first, if it reach then others too
            bubbleList[i].transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.35f).OnComplete(() => { mergedCount++; });
            bubbleList[i].PopEffect();
        }
        yield return new WaitUntil(()=> mergedCount >= bubbleList.Count - 1);
        Debug.Log(mergedCount + "/" + bubbleList.Count);

        int powerOfTwo = FindPowerOfTwo(bubbleList[lastBubbleIndex].Value) + 1;
        int newValue = Mathf.RoundToInt(Mathf.Pow(2, powerOfTwo + lastBubbleIndex));
        bubbleList[lastBubbleIndex].SetBubbleProperties(newValue);

        for (int i = 0; i < lastBubbleIndex; i++)
        {
            bubbleList[i].Pop();
            //todo: Warn neighbours for falling
            CheckForFallingNeighbours(bubbleList[i].Index);
        }

        //Merge olduktan sonra yeni oluşan bubble ile etrafında merge var mı kontrollü
        Debug.Log(bubbleList[lastBubbleIndex].Index, bubbleList[lastBubbleIndex].gameObject);
        CheckIndexForMatch(bubbleList[lastBubbleIndex].Index);

        yield return new WaitForEndOfFrame();
    }

    void CheckForFallingNeighbours(Vector2Int index)
    {
        var neighbours = _grid.NeighbourList(index);
        bool isNeighbourFallen = false;
        Vector2Int needToCheck = new Vector2Int(int.MaxValue , int.MaxValue);

        foreach (var neighbour in neighbours)
        {
            if(_grid.TileArr[neighbour.x, neighbour.y].IsOccupied)
                isNeighbourFallen = CheckForFalling(neighbour);
            if (isNeighbourFallen)
            {
                needToCheck = neighbour;
                break;
            }
        }
        if (isNeighbourFallen)
            CheckForFallingNeighbours(needToCheck);
    }

    bool CheckForFalling(Vector2Int index)
    {
        if (index.y >= _grid.gridSize.y-1) return false;
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
            if (index.x + 1 < _grid.gridSize.x-1)
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
            _grid.TileArr[index.x, index.y].IsOccupied = false;
            _grid.TileArr[index.x, index.y].Bubble = null;
        }

        return isFallen;
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
