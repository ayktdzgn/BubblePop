using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MatchController : MonoBehaviour
{
    Level _level;
    Grid _grid;

    List<Bubble> _matchBubbleList = new List<Bubble>();
    List<Vector2Int> _checkAfterIndexList = new List<Vector2Int>();
    List<Vector2Int> _checkedIndexList = new List<Vector2Int>();

    private void Awake()
    {
        _level = GetComponent<Level>();
        _grid = _level.Grid;
    }

    private void Start()
    {
        GameEvent.OnBubbleReachTargetEvent.AddListener(BubbleReachTarget);
    }

    void BubbleReachTarget(Vector2Int index)
    {
        var neighbours = _grid.NeighbourList(index);
        PushNeighbours(_grid.GetPositionForHexCordinate(index),neighbours);

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
        Bubble bubble= _grid.TileArr[index.x, index.y].GetComponentInChildren<Bubble>();

        //Debug.Log(bubble,bubble.gameObject);
        //Debug.Log(neighbours.Count);
        //foreach (var item in neighbours)
        //{
        //    Debug.Log(item);
        //}

        foreach (var neighbour in neighbours)
        {
            //If this index haven't checked before then check it
            if (!_checkedIndexList.Contains(neighbour))
            {
                //kontrol edilmişler listesine ekliyoruz ki bir daha kontrol edilmesin
                _checkedIndexList.Add(neighbour);
                Bubble neighbourBubble = _grid.TileArr[neighbour.x, neighbour.y].GetComponentInChildren<Bubble>();

                //Debug.Log(neighbour + " - Value : " + neighbourBubble.Value +" || Shooted Bubble Value : " + bubble.Value, neighbourBubble.gameObject);

                if (neighbourBubble != null)
                {
                    //Bubble'lar match oluyorsa match olan bubble'ı geri döndürüyor bize
                    var matchedBubble = CheckMatch(bubble, neighbourBubble);
                    if (matchedBubble != null)
                    {
                        //Daha önce eşleşmiş bubble değilse listeye ekliyoruz
                        if (!_matchBubbleList.Contains(bubble))
                            _matchBubbleList.Add(bubble);
                        if(!_matchBubbleList.Contains(matchedBubble))
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
            if(_matchBubbleList.Count > 0)
                MergeBubbles(_matchBubbleList);
        }
    }

    public void MergeBubbles(List<Bubble> bubbleList)
    {
        Debug.Log("In Merge!");
        //Find bubble index which was added last in the list
        int lastBubbleIndex = bubbleList.Count - 1;
        for (int i = 0; i < lastBubbleIndex; i++)
        {
            //First bubble moved first, if it reach then others too
            if (i == 0)
                bubbleList[i].transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.2f).OnComplete(() =>
                {
                    for (int i = 0; i < lastBubbleIndex; i++)
                    {
                        bubbleList[i].Pop();
                    }
                    int powerOfTwo = FindPowerOfTwo(bubbleList[lastBubbleIndex].Value) + 1;
                    int newValue = Mathf.RoundToInt(Mathf.Pow(2,powerOfTwo + lastBubbleIndex));
                    bubbleList[lastBubbleIndex].SetBubbleProperties(newValue);
                    //Merge olduktan sonra yeni oluşan bubble ile etrafında merge var mı kontrollü
                    CheckIndexForMatch(bubbleList[lastBubbleIndex].Index);
                });
            else
                bubbleList[i].transform.DOMove(bubbleList[lastBubbleIndex].transform.position, 0.2f);
        }
    }

    Bubble CheckMatch(Bubble bubble, Bubble neighbourBubble)
    {
        if (bubble.Value == neighbourBubble.Value)
        {
            //Debug.Log($"Bubble 1 : {bubble.Index} - {bubble.Value} | Bubble 2 : {neighbourBubble.Index} - {neighbourBubble.Value}");
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
