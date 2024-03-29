using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : LevelBase
{
    [SerializeField] int _goalScore = 1000;
    [SerializeField] int _createdBubbleRowCount = 5;
    [SerializeField] Grid _grid;
    [SerializeField] Bubble _bubble;
    [SerializeField] BubbleSpawner _bubbleSpawnerGrid;
    [SerializeField] BubbleSpawner _bubbleSpawnerShoot;
    [SerializeField] AudioSource _audioSource;
    int _comboCount = 0;
    int _score=0;

    public Grid Grid { get => _grid; }

    public override void Init(GameView gameView, Game game)
    {
        base.Init(gameView,game);
        _player.OnLevelInit(this);
        _gameView.UpdateProgress((float)_score / (float)_goalScore);
    }

    private void Start()
    {
        GameEvent.OnMergeEvent.AddListener(BubbleMerge);

        _grid.CreateGrid();
        BubbleCreateOnStart(_createdBubbleRowCount);
    }

    private void BubbleMerge(int value, int combo)
    {
        if (!_isPlaying) return;

        if (value >= 2048)
            Taptic.Heavy();
        else if (combo > 2)
            Taptic.Medium();
        else
            Taptic.Light();

        _audioSource.pitch = Mathf.Clamp( 1f + (float)combo * 0.25f,1f,2.1f);
        _audioSource.Play();

        Game.score.Value += value * combo;
        _score += value * combo;

        _gameView.UpdateProgress((float)_score / (float)_goalScore);

        if (combo > 1)
            GameEvent.OnComboEvent?.Invoke(combo);

        if (_score >= _goalScore)
            GameEvent.OnLevelWinEvent?.Invoke();
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

    public void CreateRandomBubbleRow(int row, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newBubble = CreateRandomBubble(new Vector2Int(i, row));
            newBubble.SpawnScaleAnimation();
        }
    }

    public Bubble CreateRandomBubble(Vector2Int gridIndex)
    {       
        var bubble = Instantiate(_bubble, _grid.TileArr[gridIndex.x, gridIndex.y].transform);
        bubble.Index = gridIndex;
        bubble.SetBubbleProperties(_bubbleSpawnerGrid.GetRandomSpawnedValue());

        _grid.TileArr[gridIndex.x, gridIndex.y].IsOccupied = true;
        _grid.TileArr[gridIndex.x, gridIndex.y].Bubble = bubble;

        return bubble;
    }

    public Bubble CreateRandomBubble(Transform transform)
    {
        var bubble = Instantiate(_bubble, transform);
        bubble.SetBubbleProperties(_bubbleSpawnerShoot.GetRandomSpawnedValue());

        return bubble;
    }
}
