using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header ("Game Objects")]
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _endGameMenu;

    [Header ("Score Info")]
    [SerializeField] TextMeshProUGUI _lastScoreText;
    [SerializeField] TextMeshProUGUI _highScoreText;
    [SerializeField] TextMeshProUGUI _coinsText;

    bool _gameIsPaused;

    void Start()
    {
        SwitchMenuTo(_mainMenu);

        _lastScoreText.text = "LAST SCORE: " + PlayerPrefs.GetFloat("LastScore").ToString("#,#");
        _highScoreText.text = "HIGH SCORE: " + PlayerPrefs.GetFloat("HighScore").ToString("#,#");
    }

    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        uiMenu.SetActive(true);

        _coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");
    }

    public void StartGame() => GameManager.instance.UnLockPlayer();

    public void PauseGame()
    {
        if (_gameIsPaused == true)
        {
            Time.timeScale = 1;
            _gameIsPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            _gameIsPaused = true;
        }
    }

    public void RestartGame() => GameManager.instance.RestartLevel();

    public void OpenEndGameUI()
    {
        SwitchMenuTo(_endGameMenu);
    }
}
