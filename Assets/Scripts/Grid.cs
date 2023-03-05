using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grid : MonoBehaviour
{
    const float TILE_HEIGHT_OFFSET_RATIO = 0.9f;

    public Vector2Int gridSize;
    public float tileSize = 0.9f;
    [SerializeField] Tile _tilePrefab;
    Tile[,] _tileArr;

    public Tile[,] TileArr { get => _tileArr;}

    //Create Grid At Start [0,size.y-1]
    public void CreateGrid()
    {
        //Give extra row for Aim
        gridSize.y += 1;

        _tileArr = new Tile[gridSize.x, gridSize.y]; // new GameObject[gridSize.x, gridSize.y];
        for (int y = gridSize.y-1; y >=0 ; y--)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Tile tile = Instantiate(_tilePrefab);//new GameObject($"Tile_{x}-{y}");
                _tileArr[x, y] = tile;
                tile.name = $"Tile_{x}-{y}";
                tile.transform.position = GetPositionForHexCordinate(new Vector2Int(x,y));
                tile.transform.SetParent(transform);
            }
        }
    }

    public void AddNewRow()
    {
        gridSize = new Vector2Int(gridSize.x, gridSize.y + 1);
        Tile[,] tempArr = _tileArr;    
        _tileArr = new Tile[gridSize.x,gridSize.y];

        ArrayTransfer(ref _tileArr,tempArr,gridSize.x,gridSize.y-1);

        for (int x = 0; x < gridSize.x; x++)
        {
            Tile tile = Instantiate(_tilePrefab); //GameObject tile = new GameObject($"Tile_{x}-{gridSize.y}");
            tile.name = $"Tile_{x}-{gridSize.y}";           
            _tileArr[x, gridSize.y-1] = tile;
            tile.transform.position = GetPositionForHexCordinate(new Vector2Int(x, gridSize.y-1));
            tile.transform.SetParent(transform);
        }

        Vector3 newPos = transform.position + new Vector3(0, -tileSize * TILE_HEIGHT_OFFSET_RATIO, 0);
        transform.DOMove(newPos , 0.2f); 
    }

    void RemoveOldRow()
    {

    }

    void ArrayTransfer(ref Tile[,] target, Tile[,] source, int xSize , int ySize)
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                target[x, y] = source[x, y];
            }
        }
    }

    public Vector3 GetPositionForHexCordinate(Vector2Int pos)
    {
        int columb = pos.x;
        int row = pos.y;

        bool shouldOffset;

        shouldOffset = (row % 2) == 1;

        Vector3 worldPos = new Vector3(columb, 0, 0) * tileSize + new Vector3(0, row, 0) * tileSize * TILE_HEIGHT_OFFSET_RATIO + (shouldOffset ? Vector3.right * tileSize * 0.5f : Vector3.zero);

        return worldPos + transform.position;
    }

    public Vector2Int GetClosestNeighbour(Vector3 worldPos, Vector2Int index)
    {
        float closest = 1f;
        Vector2Int closestNeighbour = Vector2Int.one;

        foreach (var neighbour in NeighbourList(index))
        {
            float distance = Vector3.Distance(GetPositionForHexCordinate(neighbour), worldPos);
            if (distance < closest)
            {
                closest = distance;
                closestNeighbour = neighbour;
            }
        }
        return closestNeighbour;
    }

    public List<Vector2Int> NeighbourList(Vector2Int index)
    {
        bool hasOffset = (index.y % 2) == 1;

        var tempList = new List<Vector2Int>
        {
            index + new Vector2Int(-1,0),
            index + new Vector2Int(1,0),

            index + new Vector2Int(hasOffset ? 1 : -1,1),
            index + new Vector2Int(0,1),

            index + new Vector2Int(hasOffset ? 1 : -1, -1),
            index + new Vector2Int(0,-1)
        };

        List<Vector2Int> neighbourList = new List<Vector2Int>();
        foreach (var neighbour in tempList)
        {
            if ((neighbour.x >= 0 && neighbour.x < gridSize.x) && (neighbour.y >= 0 && neighbour.y < gridSize.y))
            {
                neighbourList.Add(neighbour);
            }
        }

        return neighbourList;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(GetPositionForHexCordinate(Vector2Int.zero) , GetPositionForHexCordinate(new Vector2Int(0,gridSize.y)));
        Gizmos.DrawLine(GetPositionForHexCordinate(Vector2Int.zero), GetPositionForHexCordinate(new Vector2Int(gridSize.x, 0)));

        Gizmos.DrawLine(GetPositionForHexCordinate(new Vector2Int(gridSize.x, gridSize.y)), GetPositionForHexCordinate(new Vector2Int(gridSize.x, 0)));
        Gizmos.DrawLine(GetPositionForHexCordinate(new Vector2Int(gridSize.x, gridSize.y)), GetPositionForHexCordinate(new Vector2Int(0, gridSize.y)));

    }
}
