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
        if (startWavesButtonText != null) startWavesButtonText.text = "Start Waves";
        UpdateButtonState();
    }

    void Update() {
        UpdateButtonState();
    }

    void SetButtonText() {
        if (startWavesButtonText != null) startWavesButtonText.text = "Start Next Wave";
    }

    void UpdateButtonState() {
        readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        if (waves != null) startWavesButtonEnabled = GlobalData.currentWave != GlobalData.maxWaves && (waves.wavesStarted == false || readyForNextWave);
        GlobalData.SetGameObjectTransparency(gameObject, startWavesButtonEnabled ? 1f : 0.35f);
        if (buttonIsActiveVisual != null) {
            GlobalData.SetGameObjectTransparency(buttonIsActiveVisual, 0.35f);
            buttonIsActiveVisual.SetActive(true);
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

    // void SetGameObjectTransparency(float alpha) {
    //     Image[] images = gameObject.GetComponentsInChildren<Image>();
    //     foreach (Image img in images) {
    //         Color color = img.color;
    //         color.a = alpha;
    //         img.color = color;
    //     }

    //     TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
    //     foreach (TextMeshProUGUI text in texts) {
    //         Color color = text.color;
    //         color.a = alpha;
    //         text.color = color;
    //     }

    //     SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
    //     foreach (SpriteRenderer sprite in sprites) {
    //         Color color = sprite.color;
    //         color.a = alpha;
    //         sprite.color = color;
    //     }
    // }
}