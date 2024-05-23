using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WavesCount : MonoBehaviour {
    public TextMeshProUGUI wavesCount;
    private int waves = GlobalData.currentWave;

    void Start() {
        SetCount();
        UpdateCount();
    }

    void Update() {
        UpdateCount();
    }

    void SetCount() {
        if (wavesCount == null) wavesCount = GetComponent<TextMeshProUGUI>();
    }

    void UpdateCount() {
        if (wavesCount != null) wavesCount.text = $"Wave {GlobalData.currentWave} / {GlobalData.maxWaves}";
    }
}