using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Lean.Touch;

public class GameView : MonoBehaviour
{
    [SerializeField] CanvasGroup _taptoPlay;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _currentLevelText;
    [SerializeField] TextMeshProUGUI _nextLevelText;
    [SerializeField] Image _progressBar;

    private Game _game;

    public void Bind(Game game)
    {
        _game = game;
        ShowTapToPlay();
    }

    private void Start()
    {
        _game.score.Subscribe((score) => ChangeScoreText(score));
    }

    private void ShowTapToPlay()
    {
        _taptoPlay.gameObject.SetActive(true);
        _taptoPlay.alpha = 1;

        LeanTouch.OnFingerDown += OnFirstTouch;
    }

    private void OnFirstTouch(LeanFinger finger)
    {
        _taptoPlay.gameObject.SetActive(false);
        _game.StartLevel();

        LeanTouch.OnFingerDown -= OnFirstTouch;
    }

    public void UpdateProgress(float val)
    {
        _progressBar.fillAmount = val;
    }

    private void ChangeScoreText(int value)
    {
        _scoreText.text = ChangeScoreFormat(value);
    }

    private string ChangeScoreFormat(int money)
    {
        if (money >= 100000000)
            return (money / 1000000).ToString("#,0M");

        if (money >= 10000000)
            return (money / 1000000).ToString("0.#") + "M";

        if (money >= 100000)
            return (money / 1000).ToString("#,0K");

        if (money >= 10000)
            return (money / 1000).ToString("0.#") + "K";

        return money.ToString();
    }

}
