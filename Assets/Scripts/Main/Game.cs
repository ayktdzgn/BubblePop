using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Game : MonoBehaviour
{
    [HideInInspector] public ReactiveProperty<int> currentLevelNo;
    [HideInInspector] public ReactiveProperty<int> score;

    [SerializeField] private GameView _view;
    [SerializeField] private List<LevelBase> _levels;

    private LevelBase _currentLevel;  

    private void Awake()
    {
        _view.Bind(this);
    }

    private void Start()
    {
        currentLevelNo = new ReactiveProperty<int>(0);
        score = new ReactiveProperty<int>(0);

        currentLevelNo.Value = PlayerPrefs.GetInt("Level", 0);
        currentLevelNo.Subscribe(ev => { PlayerPrefs.SetInt("Level", ev); _view.UpdateLevelTexts(); });

        score.Value = PlayerPrefs.GetInt("Score",0);
        score.Subscribe(ev => { PlayerPrefs.SetInt("Score",ev); });

        GetCurrentLevel();

        _currentLevel.Init(_view, this);
    }

    public void StartLevel()
    {
        _currentLevel.Play();
    }

    public void RetryLevel()
    {
        DestroyImmediate(_currentLevel.gameObject);
        GetCurrentLevel();
        _currentLevel.Init(_view,this);
    }

    public void NextLevel()
    {
        DestroyImmediate(_currentLevel.gameObject);
        currentLevelNo.Value++;

        GetCurrentLevel();

        _currentLevel.Init(_view, this);
    }

    private void GetCurrentLevel()
    {
        if (currentLevelNo.Value >= _levels.Count)
        {
            int nextLevel = (currentLevelNo.Value % _levels.Count);
            _currentLevel = Instantiate(_levels[nextLevel].gameObject).GetComponent<LevelBase>();
        }
        else
        {
            _currentLevel = Instantiate(_levels[currentLevelNo.Value].gameObject).GetComponent<LevelBase>();
        }
    }
}
