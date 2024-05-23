using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Waves : MonoBehaviour {
    public float waveDelay = 5f;
    public GameObject[] waveSpawners;
    private int currentWaveIndex = 0;

    void Start() {
        for (int i = 0; i < waveSpawners.Length; i++) {
            waveSpawners[i].SetActive(i == currentWaveIndex);
        }
    }

    void Update() {
        bool readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        // if (readyForNextWave) Invoke("ActivateNextWave", waveDelay);
        if (readyForNextWave) ActivateNextWave();
    }

    // Call this method when the current wave is done
    public void ActivateNextWave() {
        if (currentWaveIndex < waveSpawners.Length - 1) {
            waveSpawners[currentWaveIndex].SetActive(false);
            currentWaveIndex++;
            GlobalData.currentWaveF = (float)currentWaveIndex + 1f;
            GlobalData.lastEnemyInWaveSpawned = false;
            GlobalData.lastEnemyInWaveDied = false;
            waveSpawners[currentWaveIndex].SetActive(true);
        } else {
            Debug.Log("All waves are complete!");
        }
    }
}