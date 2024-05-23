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
    private EnemySettings defaultsVsOverride = EnemySettings.UseOverrideValues;

    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxObjects = 10;
    
    public GameObject objectToSpawn;
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
            if (_objectsSpawned >= maxObjects) GlobalData.lastEnemyInWaveSpawned = true;
            if (_objectsSpawned < maxObjects) {
                _objectsSpawned++;
                SpawnObject(_objectsSpawned);
            }
        }
    }

    private void SpawnObject(int wavePosition) {
        if (wavePosition == maxObjects) Debug.Log("Last Enemy #" + wavePosition + " In Wave Spawned");
        GameObject newInstanceOfObjectToSpawn = _pooler.GetInstanceFromPool();
        if (defaultsVsOverride == EnemySettings.UseOverrideValues) {
            Enemy enemyObjectSettings = newInstanceOfObjectToSpawn.GetComponent<Enemy>();
            if (enemyObjectSettings != null) {
                enemyObjectSettings.speed = objectSpeed;
                enemyObjectSettings.waveMax = maxObjects;
                enemyObjectSettings.damage = objectDamage;
                enemyObjectSettings.reward = objectReward;
                enemyObjectSettings.maxHealth = objectHealth;
                enemyObjectSettings.wavePosition = wavePosition;
                enemyObjectSettings.currentHealth = objectHealth;
            }
        }
        newInstanceOfObjectToSpawn.SetActive(true);
    }
}