using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField] float _activeTime = 0.1f;
    float _timeActivated;
    float _alpha;
    [SerializeField] float _alphaSet = 0.8f;
    [SerializeField] float _alphaMultipler = 0.85f;

    Transform _player;
    SpriteRenderer _spriteRenderer;
    SpriteRenderer _playerSpriteRenderer;
    Color _color;

    void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerSpriteRenderer = _player.GetComponent<SpriteRenderer>();

        _alpha = _alphaSet;
        _spriteRenderer.sprite = _playerSpriteRenderer.sprite;
        transform.position = _player.position;
        transform.rotation = _player.rotation;
        _timeActivated = Time.time;

    }

    void Update()
    {
        _alpha *= _alphaMultipler;
        _color = new Color(1f, 1f, 1f, _alpha);
        _spriteRenderer.color = _color;

        if (Time.time >= (_timeActivated + _activeTime))
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
