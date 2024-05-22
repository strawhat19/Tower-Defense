using Pooling;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemySettings {
    UseEnemyDefaults = 1,
    UseOverrideValues = 2,
}

public class Spawner : MonoBehaviour {
    private float _spawnTimer;
    private int _objectsSpawned;
    private ObjectPooler _pooler;

    // [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxObjects = 10;
    [SerializeField] private GameObject objectToSpawn;

    // [Header("Override Values")]
    private EnemySettings defaultsVsOverride = EnemySettings.UseOverrideValues;
    public float objectHealth = GlobalData.defaultHealth;
    public float objectReward = GlobalData.defaultReward;
    public float objectDamage = GlobalData.defaultDamage;
    public float objectSpeed = GlobalData.defaultSpeed;

    private void Start() {
        _pooler = GetComponent<ObjectPooler>();
    }

    private void Update() {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0) {
            _spawnTimer = spawnDelay;
            if (_objectsSpawned < maxObjects) {
                _objectsSpawned++;
                SpawnObject();
            }
        }
    }

    private void SpawnObject() {
        GameObject newInstanceOfObjectToSpawn = _pooler.GetInstanceFromPool();
        if (defaultsVsOverride == EnemySettings.UseOverrideValues) {
            Enemy objectSettings = newInstanceOfObjectToSpawn.GetComponent<Enemy>();
            if (objectSettings != null) {
                objectSettings.speed = objectSpeed;
                objectSettings.damage = objectDamage;
                objectSettings.reward = objectReward;
                objectSettings.maxHealth = objectHealth;
                objectSettings.currentHealth = objectHealth;
            }
        }
        newInstanceOfObjectToSpawn.SetActive(true);
    }
}