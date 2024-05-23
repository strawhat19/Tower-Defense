using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Waves : MonoBehaviour {
    public float delay = 5f;
    public GameObject[] waves;
    private int currentWaveIndex = 0;
    private bool isWaitingForNextWave = false;

    void Start() {
        SetWaves();
        StartWaves();
    }

    void Update() {
        UpdateWaves();
    }

    void SetWaves() {
        bool userNotProvidedWaves = waves == null || waves.Length == 0 || waves[0] == null;
        if (userNotProvidedWaves) {
            List<GameObject> waveList = new List<GameObject>();
            foreach (Transform child in transform) {
                if (child.GetComponent<Spawner>() != null) {
                    waveList.Add(child.gameObject);
                }
            }
            waves = waveList.ToArray();
        }
    }

    void StartWaves() {
        GlobalData.maxWaves = waves.Length;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GlobalData.currentLevel = currentSceneIndex + 1;
        for (int i = 0; i < waves.Length; i++) {
            waves[i].SetActive(i == currentWaveIndex);
        }
    }

    void UpdateWaves() {
        bool readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        if (readyForNextWave && !isWaitingForNextWave) {
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    private IEnumerator StartNextWaveAfterDelay() {
        isWaitingForNextWave = true;
        Debug.Log("Starting Next Wave In " + delay + " seconds");
        yield return new WaitForSeconds(delay);
        ActivateNextWave();
        isWaitingForNextWave = false;
    }

    public void ActivateNextWave() {
        if (currentWaveIndex < waves.Length - 1) {
            waves[currentWaveIndex].SetActive(false);
            currentWaveIndex++;
            GlobalData.currentWave = currentWaveIndex + 1;
            GlobalData.lastEnemyInWaveSpawned = false;
            GlobalData.lastEnemyInWaveDied = false;
            waves[currentWaveIndex].SetActive(true);
        } else {
            Debug.Log("All waves are complete!");
        }
    }
}