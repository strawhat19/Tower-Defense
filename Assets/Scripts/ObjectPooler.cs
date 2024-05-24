using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
    private Wave wave;
    private GameObject objectToPool;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start() {
        wave = GetComponent<Wave>();
        if (objectToPool == null) {
            if (wave != null) {
                objectToPool = wave.enemy;
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