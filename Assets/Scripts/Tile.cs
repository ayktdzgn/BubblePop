using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    bool _isOccupied;
    Bubble _bubble;
    Vector2Int _index;

    public bool IsOccupied { get => _isOccupied; set => _isOccupied = value; }
    public Bubble Bubble { get => _bubble; set => _bubble = value; }

    public void SetTileEmpty()
    {
        if (!_isOccupied && _bubble == null) return;

        _index = _bubble.Index;
        _isOccupied = false;
        _bubble = null;

        GameEvent.OnCheckForFallingEvent?.Invoke(_index);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isOccupied ? new Color(0,1,0,0.6f) : new Color(1, 0, 0, 0.6f);
        Gizmos.DrawCube(transform.position,Vector3.one*0.5f);
    }
}
