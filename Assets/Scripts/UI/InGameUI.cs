using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _distanceText;
    [SerializeField] TextMeshProUGUI _coinsText;
    [SerializeField] Image _heartEmpty;
    [SerializeField] Image _heartFull;

    float _distance;
    int _coins;

    Player _player;

    void Start()
    {
        _player = GameManager.instance.player;
        InvokeRepeating("UpdateInfo", 0, 0.05f);
    }

    void UpdateInfo()
    {
        _distance = GameManager.instance.distance;
        _coins = GameManager.instance.coins;

        if (_distance > 0)
            _distanceText.text = _distance.ToString("#,#");

        if (_coins > 0)
            _coinsText.text = GameManager.instance.coins.ToString("#,#");

        _heartEmpty.enabled = !_player.extraLife;
        _heartFull.enabled = _player.extraLife;
    }
}
