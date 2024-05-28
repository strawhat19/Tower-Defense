using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StartWaves : MonoBehaviour {
    public Waves waves;
    public TextMeshProUGUI startWavesButtonText;

    public void OnStartWavesClicked() {
        if (waves != null) {
            waves.ActivateNextWave();
            if (startWavesButtonText != null) {
                if (GlobalData.currentWave > 1) {
                    startWavesButtonText.text = "Start Next Wave";
                } else {
                    startWavesButtonText.text = "Start Waves";
                }
            }
        }
    }
}