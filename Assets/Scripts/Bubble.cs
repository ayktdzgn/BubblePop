using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _valueText;
    [SerializeField] Color[] _colors;

    SpriteRenderer _spriteRenderer;
    int _value;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
