using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StartWaves : MonoBehaviour {
    public Waves waves;
    private bool readyForNextWave;
    public TextMeshProUGUI startWavesButtonText;

    void Start() {
        if (startWavesButtonText != null) startWavesButtonText.text = "Start Waves";
    }
    
    public void OnStartWavesClicked() {
        if (waves != null) {
            readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
            if (waves.wavesStarted == false || readyForNextWave) {
                waves.ActivateNextWave();
                if (startWavesButtonText != null) startWavesButtonText.text = "Start Next Wave";
            } else {
                Debug.Log("Not Ready For Next Wave");
            }
        }
    }
}