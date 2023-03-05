using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    bool _isOccupied;
    Bubble _bubble;

    public bool IsOccupied { get => _isOccupied; set => _isOccupied = value; }
    public Bubble Bubble { get => _bubble; set => _bubble = value; }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isOccupied ? new Color(0,1,0,0.6f) : new Color(1, 0, 0, 0.6f);
        Gizmos.DrawCube(transform.position,Vector3.one*0.5f);
    }
}
