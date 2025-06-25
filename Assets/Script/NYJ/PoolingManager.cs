using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingManager
{
    private static PoolingManager _instance;

    public static PoolingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PoolingManager();
            }

            return _instance;
        }
    }

    private Dictionary<string, ObjectPool<GameObject>> _objectsPools =
        new Dictionary<string, ObjectPool<GameObject>>();

    private GameObject _prefab;

    public void CreatePool(string key, GameObject prefab, int poolSize)
    {
        _prefab = prefab;

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePoolObj,
            OnGetPoolObj, OnReleasePoolObj, OnDestroyPoolObj, true, poolSize);

        GameObject parent = new GameObject(key);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CreatePoolObj();
            obj.transform.parent = parent.transform;
            pool.Release(obj);
        }

        _objectsPools.Add(key, pool);
    }

    public GameObject Pop(string key)
    {
        return _objectsPools[key].Get();
    }

    public void Release(string key, GameObject poolObj)
    {
        _objectsPools[key].Release(poolObj);
    }

    private GameObject CreatePoolObj()
    {
        return Object.Instantiate(_prefab);
    }

    private void OnGetPoolObj(GameObject poolObj)
    {
        poolObj.SetActive(true);
    }

    private void OnReleasePoolObj(GameObject poolObj)
    {
        poolObj.SetActive(false);
    }

    private void OnDestroyPoolObj(GameObject poolObj)
    {
        Object.Destroy(poolObj);
    }
}