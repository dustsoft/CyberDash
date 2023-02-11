using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameObject _graphic;
        

    private void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();

        

    }

    private void Update()
    {
        if (_playerScript._doubleJump == true)
            _graphic.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _playerScript._doubleJump = true;
            Destroy(gameObject);
        }
    }
}
