using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Bubble : MonoBehaviour
{
    [SerializeField] ParticleSystem _popParticle;
    [SerializeField] TextMeshProUGUI _valueText;
    [SerializeField] Color[] _colors;

    [Header("Fall")]
    [SerializeField] float _yFallPower = 0.6f;
    [SerializeField] float _xFallPower = 0.4f;

    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidbody;
    CircleCollider2D _collider;
    Vector2Int _index;
    int _value;

    public Vector2Int Index { get => _index; set => _index = value; }
    public int Value { get => _value; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
    }

    public void SpawnScaleAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f);
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
            _spriteRenderer.color = _colors[Mathf.RoundToInt(FindPowerOfTwo(value)) % _colors.Length];
        }
    }

    public void Fall()
    {
        _collider.isTrigger = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        transform.DOLocalMove(new Vector3(Random.Range(-_xFallPower, _xFallPower), _yFallPower, 0),Random.Range(0.2f,0.4f)).OnComplete(()=>
        {
            gameObject.layer = LayerMask.NameToLayer("Fall");
        });
    }

    public void PopEffect()
    {
        if (_popParticle != null)
        {
            var particle = Instantiate(_popParticle,transform.position,Quaternion.identity);
            particle.startColor = _spriteRenderer.color;
            particle.Play();
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
