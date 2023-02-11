using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] GameObject _coinPrefab;

    int _amountOfCoins;
    [SerializeField] int _minCoins;
    [SerializeField] int _maxCoins;
    [SerializeField] float _chanceToSpawn;

    [SerializeField] SpriteRenderer[] _coinImage;

    void Start()
    {
        for (int i = 0; i < _coinImage.Length; i++)
        {
            _coinImage[i].sprite = null;
        }

        _amountOfCoins = Random.Range(_minCoins, _maxCoins);
        int additionalOffset = _amountOfCoins / 2;

        for (int i = 0; i < _amountOfCoins; i++)
        {
            bool canSpawn = _chanceToSpawn > Random.Range(0, 100);
            Vector3 offset = new Vector2(i - additionalOffset, 0);

            if (canSpawn == true)
                Instantiate(_coinPrefab, transform.position + offset, Quaternion.identity, transform);
        }
    }
}
