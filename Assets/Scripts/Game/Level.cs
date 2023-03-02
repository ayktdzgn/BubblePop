using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : LevelBase
{
    [SerializeField] int _goalScore = 1000;
    [SerializeField] int _levelStartHeight = 5;
    [SerializeField] Grid _grid;
    [SerializeField] Bubble _bubble;

    private void Start()
    {
        _grid.CreateGrid();
        StartBubbleCreate(_levelStartHeight);
    }

   

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _grid.AddNewRow();
            CreateRandomBubbleRow(_grid.gridSize.y-1,_grid.gridSize.x);
        }
    }

    private void StartBubbleCreate(int levelStartHeight)
    {
        for (int i = 0; i < levelStartHeight; i++)
        {
            CreateRandomBubbleRow(i , _grid.gridSize.x);
        }
    }

    private void CreateRandomBubbleRow(int y, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newBubble = Instantiate(_bubble, _grid.TileArr[i, y].transform);
            newBubble.SetBubbleProperties(Mathf.RoundToInt(Mathf.Pow(2, Random.Range(1,11))));
        }
    }
}
