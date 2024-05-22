using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
    public GameObject objectToPool; // Assign your prefab in the Inspector
    private Queue<GameObject> pool = new Queue<GameObject>();

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