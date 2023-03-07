using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Lean.Touch;
using DG.Tweening;

public class GameView : MonoBehaviour
{
    [SerializeField] CanvasGroup _taptoPlay;
    [SerializeField] CanvasGroup _levelWin;
    [SerializeField] CanvasGroup _levelFail;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _currentLevelText;
    [SerializeField] TextMeshProUGUI _nextLevelText;
    [SerializeField] Image _progressBar;

    DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _comboTween;
    private Game _game;

    public void Bind(Game game)
    {
        _game = game;
        UpdateLevelTexts();
    }

    private void Start()
    {
        GameEvent.OnComboEvent.AddListener(ShowCombo);
        _game.score.Subscribe((score) => ChangeScoreText(score));
    }

    public void ShowTapToPlay()
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

    private void ShowCombo(int combo)
    {
        if (_comboTween != null &&_comboTween.IsPlaying())
        {
            _comboTween.Kill();
            _comboText.transform.localScale = Vector3.zero;
        }
        _comboText.text = "x"+combo.ToString();
        _comboTween = _comboText.transform.DOScale(Vector3.one * 2, 1f).OnComplete(() => { _comboText.transform.localScale = Vector3.zero; });
    }

    public void ResetLevelEndScreens()
    {
        _levelWin.alpha = 0;
        _levelWin.blocksRaycasts = false;

        _levelFail.alpha = 0;
        _levelFail.blocksRaycasts = false;
    }

    public void UpdateLevelTexts()
    {
        _currentLevelText.text = (_game.currentLevelNo.Value + 1).ToString();
        _nextLevelText.text = (_game.currentLevelNo.Value + 2).ToString();
    }

    public void LevelWin()
    {
        _levelWin.alpha = 1;
        _levelWin.blocksRaycasts = true;
        _levelWin.GetComponent<Animator>().SetTrigger("Play");
    }

    public void LevelFail()
    {
        _levelFail.alpha = 1;
        _levelFail.blocksRaycasts = true;
        _levelFail.GetComponent<Animator>().SetTrigger("Play");
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
