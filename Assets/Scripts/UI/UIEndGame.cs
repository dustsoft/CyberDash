using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEndGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _distance;
    [SerializeField] TextMeshProUGUI _coins;
    [SerializeField] TextMeshProUGUI _score;

    void Start()
    {
        GameManager gameManager = GameManager.instance;

        Time.timeScale = 0;

        if (gameManager.distance <= 0)
            return;

        if (gameManager.coins <= 0)
            return;
            
        _distance.text = "Distance: " + gameManager.distance.ToString("#,#") + " Meters";
        _coins.text = "Coins: " + gameManager.coins.ToString("#,#");
        _score.text = "Score: " + gameManager.score.ToString("#,#");
    }

}
