using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Waves : MonoBehaviour {
    public float waveDelay = 5f;
    public GameObject[] waves;
    private int currentWaveIndex = 0;

    void Start() {
        GlobalData.maxWaves = waves.Length;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GlobalData.currentLevel = currentSceneIndex + 1;
        for (int i = 0; i < waves.Length; i++) {
            waves[i].SetActive(i == currentWaveIndex);
        }
    }

    void Update() {
        bool readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        // if (readyForNextWave) Invoke("ActivateNextWave", waveDelay);
        if (readyForNextWave) ActivateNextWave();
    }

    // Call this method when the current wave is done
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