using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StartWaves : MonoBehaviour {
    public Waves waves;
    private bool readyForNextWave;
    private bool startWavesButtonEnabled;
    public GameObject buttonIsActiveVisual;
    public TextMeshProUGUI startWavesButtonText;

    void Start() {
        if (startWavesButtonText != null) startWavesButtonText.text = "Start First Wave";
        UpdateButtonState();
    }

    void Update() {
        UpdateButtonState();
    }

    void SetButtonText() {
        if (startWavesButtonText != null) startWavesButtonText.text = "Start Next Wave";
    }

    void UpdateButtonState() {
        bool wavesFinished = GlobalData.currentWave == GlobalData.maxWaves;
        readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        if (readyForNextWave) GlobalData.Message = "Ready for Next Wave";
        if (wavesFinished && readyForNextWave && startWavesButtonText != null) {
            string wavesFinishedMessage = "Waves Finished";
            startWavesButtonText.text = wavesFinishedMessage;
            GlobalData.Message = wavesFinishedMessage;
        }
        if (waves != null) {
            startWavesButtonEnabled = wavesFinished == false && (waves.wavesStarted == false || readyForNextWave);
        }
        GlobalData.SetGameObjectTransparency(gameObject, startWavesButtonEnabled ? 1f : 0.35f);
        if (buttonIsActiveVisual != null) {
            GlobalData.SetGameObjectTransparency(buttonIsActiveVisual, 0.35f);
            buttonIsActiveVisual.SetActive(startWavesButtonEnabled);
        }
    }
    
    public void OnStartWavesClicked() {
        if (waves != null) {
            if (startWavesButtonEnabled) {
                waves.ActivateNextWave();
                SetButtonText();
            } else {
                if (startWavesButtonText != null) startWavesButtonText.text = "Not Ready Yet!";
                Invoke("SetButtonText", 1.5f);
            }
        }
    }
}