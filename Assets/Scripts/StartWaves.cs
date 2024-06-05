using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class StartWaves : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Waves waves;
    private bool readyForNextWave;
    private Button startWavesButton;
    private GameSettings gameSettings;
    private bool startWavesButtonEnabled;
    public GameObject buttonIsActiveVisual;
    public TextMeshProUGUI startWavesButtonText;

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        startWavesButton = gameObject.GetComponent<Button>();
        if (startWavesButtonText != null) startWavesButtonText.text = "Start First Wave";
        UpdateButtonState();
    }

    void Update() {
        UpdateButtonState();
    }

    void SetButtonText() {
        if (startWavesButtonText != null) startWavesButtonText.text = "Start Next Wave";
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (gameSettings != null) {
            if (startWavesButtonEnabled) {
                gameSettings.SetCursor(gameSettings.hoverCursorTexture);
            } else {
                gameSettings.SetCursor(gameSettings.disabledCursorTexture);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (gameSettings != null) {
            gameSettings.SetCursor(gameSettings.defaultCursorTexture);
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

    void UpdateButtonState() {
        bool wavesFinished = GlobalData.currentWave == GlobalData.maxWaves;
        readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        if (wavesFinished && readyForNextWave && startWavesButtonText != null) {
            string wavesFinishedMessage = "Waves Finished.";
            startWavesButtonText.text = wavesFinishedMessage;
        }
        if (waves != null) {
            startWavesButtonEnabled = wavesFinished == false && (waves.wavesStarted == false || readyForNextWave);
        }
        GlobalData.SetGameObjectTransparency(gameObject, startWavesButtonEnabled ? 1f : 0.5f);
        if (buttonIsActiveVisual != null) {
            GlobalData.SetGameObjectTransparency(buttonIsActiveVisual, 0.5f);
            buttonIsActiveVisual.SetActive(startWavesButtonEnabled);
        }
        if (startWavesButton != null) {
            startWavesButton.interactable = startWavesButtonEnabled;
        }
    }
}