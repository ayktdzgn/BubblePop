using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBase : MonoBehaviour
{
    protected bool _isPlaying;
    protected GameView _gameView;
    protected PlayerBase _player;
    private Game _game;

    public Game Game { get => _game; set => _game = value; }

    protected virtual void Awake()
    {
        _player = GetComponentInChildren<PlayerBase>();

        GameEvent.OnLevelWinEvent.AddListener(Win);
        GameEvent.OnLevelFailEvent.AddListener(Fail);
    }

    private void OnDestroy()
    {
        GameEvent.OnLevelWinEvent.RemoveListener(Win);
        GameEvent.OnLevelFailEvent.RemoveListener(Fail);
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Fail();
        }
    }

    public virtual void Init(GameView gameView, Game game)
    {
        gameObject.SetActive(true);
        _game = game;
        _gameView = gameView;
        gameView.ResetLevelEndScreens();
        gameView.ShowTapToPlay();
    }

    public virtual void Play()
    {
        _isPlaying = true;
        _player.OnLevelStart();
    }

    protected virtual void End()
    {
        _isPlaying = false;
        _player.OnLevelEnd();
    }

    private void Win()
    {
        if (!_isPlaying)
            return;

        End();
        _gameView.LevelWin();
    }

    private void Fail()
    {
        if (!_isPlaying)
            return;

        End();
        _gameView.LevelFail();
    }
}
