using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Transform[] _levelPart;
    [SerializeField] Vector3 _nextPartPosition;
    [SerializeField] float _distanceToSpawn;
    [SerializeField] float _distanceToDelete;

    void Update()
    {
        DeletePlatform();
        GeneratePlatform();
    }

    void GeneratePlatform()
    {
        while (Vector2.Distance(_player.transform.position, _nextPartPosition) < _distanceToSpawn)
        {
            Transform part = _levelPart[Random.Range(0, _levelPart.Length)];

            Vector2 newPosition = new Vector2(_nextPartPosition.x - part.Find("StartPoint").position.x, 0);

            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);

            _nextPartPosition = newPart.Find("EndPoint").position;
        }
    }

    void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);

            if (Vector2.Distance(_player.transform.position, partToDelete.transform.position) > _distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }

}
