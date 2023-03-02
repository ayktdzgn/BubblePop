using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grid : MonoBehaviour
{
    public Vector2Int gridSize;
    public float tileSize = 0.9f;
    GameObject[,] _tileArr;

    public GameObject[,] TileArr { get => _tileArr;}

    public void CreateGrid()
    {
        _tileArr = new GameObject[gridSize.x, gridSize.y];
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Tile_{x}-{y}");
                _tileArr[x, y] = tile;
                tile.transform.position = GetPositionForHexCordinate(new Vector2Int(x,y));
                tile.transform.SetParent(transform);
            }
        }
    }

    public void AddNewRow()
    {
        gridSize = new Vector2Int(gridSize.x, gridSize.y + 1);
        GameObject[,] tempArr = _tileArr;    
        _tileArr = new GameObject[gridSize.x,gridSize.y];

        ArrayTransfer(ref _tileArr,tempArr,gridSize.x,gridSize.y-1);

        for (int x = 0; x < gridSize.x; x++)
        {
            GameObject tile = new GameObject($"Tile_{x}-{gridSize.y}");
            _tileArr[x, gridSize.y-1] = tile;
            tile.transform.position = GetPositionForHexCordinate(new Vector2Int(x, gridSize.y-1));
            tile.transform.SetParent(transform);
        }

        Vector3 newPos = transform.position + new Vector3(0, -tileSize * 0.75f, 0);
        transform.DOMove(newPos , 0.2f); 
    }

    void ArrayTransfer(ref GameObject[,] target, GameObject[,] source, int xSize , int ySize)
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                target[x, y] = source[x, y];
            }
        }
    }

    Vector3 GetPositionForHexCordinate(Vector2Int pos)
    {
        int columb = pos.x;
        int row = pos.y;

        bool shouldOffset;

        shouldOffset = (row % 2) == 1;

        Vector3 worldPos = new Vector3(columb, 0, 0) * tileSize + new Vector3(0, row, 0) * tileSize * 0.75f + (shouldOffset ? Vector3.right * tileSize * 0.5f : Vector3.zero);

        return worldPos + transform.position;
    }

    public List<Vector2Int> NeighbourList(Vector2Int index)
    {
        bool hasOffset = (index.y % 2) == 1;

        return new List<Vector2Int>
        {
            index + new Vector2Int(-1,0),
            index + new Vector2Int(1,0),

            index + new Vector2Int(hasOffset ? 1 : -1,1),
            index + new Vector2Int(0,1),

            index + new Vector2Int(hasOffset ? 1 : -1, -1),
            index + new Vector2Int(0,-1)
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(GetPositionForHexCordinate(Vector2Int.zero) , GetPositionForHexCordinate(new Vector2Int(0,gridSize.y)));
        Gizmos.DrawLine(GetPositionForHexCordinate(Vector2Int.zero), GetPositionForHexCordinate(new Vector2Int(gridSize.x, 0)));

        Gizmos.DrawLine(GetPositionForHexCordinate(new Vector2Int(gridSize.x, gridSize.y)), GetPositionForHexCordinate(new Vector2Int(gridSize.x, 0)));
        Gizmos.DrawLine(GetPositionForHexCordinate(new Vector2Int(gridSize.x, gridSize.y)), GetPositionForHexCordinate(new Vector2Int(0, gridSize.y)));

    }
}
