using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : PlayerBase
{
    [SerializeField] LayerMask _raycasLayer;
    [SerializeField] Transform _shootPoint;
    [SerializeField] int _reflectionCount;
    [SerializeField] float _maxRange;

    private LineRenderer _lr;
    Bubble _currentBubble;
    Ray ray;
    //RaycastHit hit;

    protected override void Awake()
    {
        base.Awake();
        _lr = GetComponent<LineRenderer>();
    }

    public void SetShootAim(Vector3 shootPos)
    {
        var direction = (shootPos - _shootPoint.position).normalized;
        ray = new Ray(_shootPoint.position,direction);

        _lr.positionCount = 1;
        _lr.SetPosition(0,_shootPoint.position);
        float remainingLenght = _maxRange;

        for (int i = 0; i < _reflectionCount; i++)
        {
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction, remainingLenght, _raycasLayer);
            var sortedHits = hits.OrderBy(h => Vector2.Distance(h.point, transform.position)).ToList();
            if (sortedHits.Count <= 0) break;

            var hit = sortedHits[0];
            if (hit)
            {
                _lr.positionCount += 1;
                _lr.SetPosition(_lr.positionCount -1 ,hit.point);
                remainingLenght -= Vector2.Distance(ray.origin,hit.point);
                ray = new Ray(hit.point , Vector2.Reflect(ray.direction,hit.normal));
                if (hit.collider.tag != "Wall")
                    break;
            }
            else
            {
                _lr.positionCount += 1;
                _lr.SetPosition(_lr.positionCount-1,ray.origin + ray.direction * remainingLenght);
            }
        }
    }

    public void Shoot()
    {

    }
}
