using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField] GameObject _afterImagePrefab;

    Queue<GameObject> _availableObjects = new Queue<GameObject>();

    public static AfterImagePool Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        GrowPool();
    }

    void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(_afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        _availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (_availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = _availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
