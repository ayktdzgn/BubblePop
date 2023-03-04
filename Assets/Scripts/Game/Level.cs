using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : LevelBase
{
    [SerializeField] int _goalScore = 1000;
    [SerializeField] int _createdBubbleRowCount = 5;
    [SerializeField] Grid _grid;
    [SerializeField] Bubble _bubble;

    public Grid Grid { get => _grid; }

    public override void Init(GameView gameView, Game game)
    {
        base.Init(gameView,game);
        _player.OnLevelInit(this);
    }

    private void Start()
    {
        _grid.CreateGrid();
        BubbleCreateOnStart(_createdBubbleRowCount);
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _grid.AddNewRow();
            CreateRandomBubbleRow(_grid.gridSize.y-1,_grid.gridSize.x);
        }
    }

    private void BubbleCreateOnStart(int rowCount)
    {
        for (int i = 1; i <= rowCount; i++)
        {
            if(i <= _grid.gridSize.y)
            {
                int y = _grid.gridSize.y - i;
                CreateRandomBubbleRow(y, _grid.gridSize.x);
            }
        }
    }

    private void CreateRandomBubbleRow(int row, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newBubble = CreateRandomBubble(_grid.TileArr[i, row].transform);
            newBubble.SpawnScaleAnimation();
            newBubble.Index = new Vector2Int(i,row);
        }
    }

    public Bubble CreateRandomBubble(Transform parent)
    {
        var bubble = Instantiate(_bubble, parent);
        bubble.SetBubbleProperties(Mathf.RoundToInt(Mathf.Pow(2, Random.Range(1, 11))));

        return bubble;
    }
}
