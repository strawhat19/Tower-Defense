using Pooling;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemySettings {
    UseEnemyDefaults = 1,
    UseOverrideValues = 2,
}

public class Wave : MonoBehaviour {
    private Waves waves;
    private float _spawnTimer;
    private int _objectsSpawned;
    private ObjectPooler _pooler;
    private EnemySettings defaultsVsOverride = EnemySettings.UseOverrideValues;

    public float spawnDelay;
    public int maxObjects = 10;
    public float health = GlobalData.defaultHealth;
    public float speed = GlobalData.defaultSpeed;
    public float reward = GlobalData.defaultReward;
    public float damage = GlobalData.defaultDamage;
    public GameObject enemy;

    private void Start() {
        _pooler = GetComponent<ObjectPooler>();
        if (waves == null) waves = gameObject.transform.parent.GetComponent<Waves>();
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
        // if (wavePosition == maxObjects) Debug.Log("Last Enemy #" + wavePosition + " In Wave Spawned");
        GameObject newInstanceOfObjectToSpawn = _pooler.GetInstanceFromPool();
        if (defaultsVsOverride == EnemySettings.UseOverrideValues) {
            Enemy enemyObjectSettings = newInstanceOfObjectToSpawn.GetComponent<Enemy>();
            if (enemyObjectSettings != null) {
                enemyObjectSettings.speed = speed;
                enemyObjectSettings.waves = waves;
                enemyObjectSettings.damage = damage;
                enemyObjectSettings.reward = reward;
                enemyObjectSettings.maxHealth = health;
                enemyObjectSettings.waveMax = maxObjects;
                enemyObjectSettings.currentHealth = health;
                enemyObjectSettings.wavePosition = wavePosition;
            }
        }
        newInstanceOfObjectToSpawn.SetActive(true);
    }
}