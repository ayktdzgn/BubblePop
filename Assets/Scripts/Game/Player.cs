using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Player : PlayerBase
{
    [SerializeField] Transform _bubbleIndicator;
    [SerializeField] Transform _nextBubbleTransform;
    [SerializeField] Transform _shootPoint;
    [SerializeField] int _reflectionCount;
    [SerializeField] float _lineMaxRange;

    private LineRenderer _lr;
    Bubble _currentBubble;
    Bubble _nextBubble;
    Vector2Int _indicatorTartgetIndex;
    bool _isIndicatorTargeted;
    bool _isAbleToShoot = true;
    List<Vector3> Points = new List<Vector3>();
    int currentReflections;

    TweenerCore<Vector3, Vector3, VectorOptions> indicatorTween { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _lr = GetComponent<LineRenderer>();
    }

    public override void OnLevelStart()
    {
        base.OnLevelStart();
        CreateNextBubble();
        SetCurrentBubble();
    }

    void SetCurrentBubble()
    {
        if (_currentBubble != null)
            return;

        _nextBubble.transform.DOScale(Vector3.one,0.2f);
        _nextBubble.transform.DOMove(_shootPoint.position,0.4f).OnComplete(()=>
        {
            _currentBubble = _nextBubble;
            _currentBubble.transform.SetParent(_shootPoint);
            _nextBubble = null;
            CreateNextBubble();

            _isAbleToShoot = true;
        });
    }

    void CreateNextBubble()
    {
        if (_nextBubble != null)
            return;

        _nextBubble = _level.CreateRandomBubble(_nextBubbleTransform);
        _nextBubble.transform.localScale = Vector3.one * 0.8f;
        _nextBubble.gameObject.layer = 2; //Ignore Raycast index
    }

    void AimHitBubble(Bubble bubble,Vector3 hitPoint)
    {
        _indicatorTartgetIndex = _level.Grid.GetClosestNeighbour(hitPoint,bubble.Index);
        SetIndicatior(_level.Grid.GetPositionForHexCordinate(_indicatorTartgetIndex));
    }

    void SetIndicatior(Vector3 pos)
    {
        if (!Vector3.Equals(_bubbleIndicator.position,pos))
        {
            _bubbleIndicator.localScale = Vector3.zero;
            _bubbleIndicator.position = pos;
            indicatorTween = _bubbleIndicator.DOScale(Vector3.one, 0.2f);
            _isIndicatorTargeted = true;
        }
    }

    void ResetIndicator()
    {
        indicatorTween.Kill();
        _isIndicatorTargeted = false;
        _bubbleIndicator.localScale = Vector3.zero;
        _bubbleIndicator.position = _shootPoint.position;
    }

    void ResetLineRenderer()
    {
        _lr.positionCount = 0;
        _lr.SetPositions(new Vector3[] { });
    }

    public void SetShootAim(Vector3 shootPos)
    {
        if (!_isAbleToShoot) return;

        var hitData = Physics2D.Raycast(_shootPoint.position, (shootPos - _shootPoint.position).normalized, _lineMaxRange);

        currentReflections = 0;
        Points.Clear();
        Points.Add(_shootPoint.position);

        if (hitData)
        {
            if (hitData.collider.tag != "Wall")
                Points.Add(hitData.point);
            else
                ReflectFurther(_shootPoint.position, hitData);

            if (hitData.collider.tag == "Ball")
            {
                AimHitBubble(hitData.collider.GetComponent<Bubble>(), hitData.point);
            }
        }
        else
        {
            Points.Add(_shootPoint.position + (shootPos - _shootPoint.position).normalized * 999);
        }

        _lr.positionCount = Points.Count;
        _lr.SetPositions(Points.ToArray());
    }

    private void ReflectFurther(Vector2 origin, RaycastHit2D hitData)
    {
        if (currentReflections > _reflectionCount) return;

        Points.Add(hitData.point);
        currentReflections++;

        Vector2 inDirection = (hitData.point - origin).normalized;
        Vector2 newDirection = Vector2.Reflect(inDirection, hitData.normal);

        var newHitData = Physics2D.Raycast(hitData.point + (newDirection * 0.0001f), newDirection * 100, _lineMaxRange);

        if (newHitData)
        {
            if (newHitData.collider.tag != "Wall")
                Points.Add(newHitData.point);
            else
                ReflectFurther(hitData.point, newHitData);

            if (newHitData.collider.tag == "Ball")
            {
                AimHitBubble(newHitData.collider.GetComponent<Bubble>(), newHitData.point);
            }
        }
        else
        {
            Points.Add(hitData.point + newDirection * _lineMaxRange);
        }
    }

    public void Shoot()
    {
        if (!_isAbleToShoot) return;
        //Shoot
        if (_isIndicatorTargeted)
        {
            _isAbleToShoot = false;
            Points.RemoveAt(Points.Count - 1);
            Points.Add(_bubbleIndicator.position);
            Vector3[] path = Points.ToArray();
            _currentBubble.transform.DOPath(path, 0.4f, PathType.Linear, PathMode.Ignore).OnComplete(()=>
            {
                _currentBubble.gameObject.layer = 0;
                _currentBubble.transform.SetParent(_level.Grid.TileArr[_indicatorTartgetIndex.x, _indicatorTartgetIndex.y].transform);
                _currentBubble.Index = _indicatorTartgetIndex;
                _currentBubble = null;
                SetCurrentBubble();

                GameEvent.OnBubbleReachTargetEvent?.Invoke(_indicatorTartgetIndex);
            });
        }

        ResetIndicator();
        ResetLineRenderer();
    }
}
