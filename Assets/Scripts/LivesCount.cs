using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LivesCount : MonoBehaviour {
    public float lives = GlobalData.startLives;
    public TextMeshProUGUI livesCount;

    void Start() {
        if (livesCount == null) livesCount = GetComponent<TextMeshProUGUI>();
        UpdateCount();
    }

    void UpdateCount() {
        if (livesCount != null) {
            livesCount.text = $"{GlobalData.startLives}";
        }
    }

    void Update() {
        UpdateCount();
    }
}