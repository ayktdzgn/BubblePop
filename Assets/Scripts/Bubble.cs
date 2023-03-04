using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Bubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _valueText;
    [SerializeField] Color[] _colors;

    SpriteRenderer _spriteRenderer;
    Vector2Int _index;
    int _value;

    public Vector2Int Index { get => _index; set => _index = value; }
    public int Value { get => _value; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SpawnScaleAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.25f);
    }

    public void SetBubbleProperties(int value)
    {
        _value = value;
        _valueText.text = value.ToString();

        if (value == 2)
        {
            _spriteRenderer.color = _colors[0];
        }
        else
        {
            _spriteRenderer.color = _colors[Mathf.RoundToInt(FindPowerOfTwo(value))];
        }
    }

    public void Pop()
    {
        Destroy(gameObject);
    }

    int FindPowerOfTwo(int value)
    {
        int tempVal = value;
        int pow = 0;
        while (tempVal > 2)
        {
            tempVal /= 2;
            pow++;
        }

        return pow;
    }
}
