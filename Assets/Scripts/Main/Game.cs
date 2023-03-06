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
        currentLevelNo = new ReactiveProperty<int>(0);
        score = new ReactiveProperty<int>(0);

        _view.Bind(this);
    }

    private void Start()
    {
        if (currentLevelNo.Value == _levels.Count)
        {
            currentLevelNo.Value = (currentLevelNo.Value % _levels.Count);
        }
        _currentLevel = Instantiate(_levels[currentLevelNo.Value].gameObject).GetComponent<LevelBase>();
        _currentLevel.Init(_view, this);
    }

    public void StartLevel()
    {
        _currentLevel.Play();
    }

    public void RetryLevel()
    {
        DestroyImmediate(_currentLevel.gameObject);
        _currentLevel = Instantiate(_levels[currentLevelNo.Value].gameObject).GetComponent<LevelBase>();

        _currentLevel.Init(_view,this);
    }

    public void NextLevel()
    {
        DestroyImmediate(_currentLevel.gameObject);
        currentLevelNo.Value++;

        if (currentLevelNo.Value == _levels.Count)
        {
            currentLevelNo.Value = (currentLevelNo.Value % _levels.Count);
        }
        _currentLevel = Instantiate(_levels[currentLevelNo.Value].gameObject).GetComponent<LevelBase>();

        _currentLevel.Init(_view, this);
    }
}
