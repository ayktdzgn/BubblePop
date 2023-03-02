using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerBase
{
    [SerializeField] Transform _shootPoint;
    [SerializeField] int _reflectionCount;
    [SerializeField] float _maxRange;

    private LineRenderer _lr;
    Bubble _currentBubble;
    Ray ray;
    RaycastHit hit;

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
            if (Physics.Raycast(ray.origin,ray.direction,out hit, remainingLenght))
            {
                _lr.positionCount += 1;
                _lr.SetPosition(_lr.positionCount -1 ,hit.point);
                remainingLenght -= Vector3.Distance(ray.origin,hit.point);
                ray = new Ray(hit.point , Vector3.Reflect(ray.direction,hit.normal));
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
