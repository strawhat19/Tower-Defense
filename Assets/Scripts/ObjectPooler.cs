using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
    private Spawner spawner;
    private GameObject objectToPool;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start() {
        spawner = GetComponent<Spawner>();
        if (objectToPool == null) {
            if (spawner != null) {
                objectToPool = spawner.objectToSpawn;
            }
        }
    }

    public GameObject GetInstanceFromPool() {
        if (pool.Count == 0) {
            AddObjects(1);
        }

        return pool.Dequeue();
    }

    private void AddObjects(int count) {
        for (int i = 0; i < count; i++) {
            var newObject = Instantiate(objectToPool);
            newObject.SetActive(false);
            pool.Enqueue(newObject);
        }
    }
}