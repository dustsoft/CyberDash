using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Components")]
    public UIManager UI;
        
    public Player player;

    [Header ("Score Info")]
    public int coins;
    public float distance;
    public float score;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (player.transform.position.x > distance)
            distance = player.transform.position.x;
    }

    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", savedCoins + coins); //may need to change

        score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if (PlayerPrefs.GetFloat("HighScore") < score)
            PlayerPrefs.SetFloat("HighScore", score);
        
    }

    public void UnLockPlayer() => player.runStarted = true;

    public void RestartLevel() => SceneManager.LoadScene(0);
    
    public void GameEnded()
    {
        SaveInfo();
        UI.OpenEndGameUI();
    }
}
