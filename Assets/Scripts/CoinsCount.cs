using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoinsCount : MonoBehaviour {
    public float coins = GlobalData.startCoins;
    public TextMeshProUGUI coinsCount;

    void Start() {
        if (coinsCount == null) coinsCount = GetComponent<TextMeshProUGUI>();
        UpdateCount();
    }

    void UpdateCount() {
        if (coinsCount != null) {
            coinsCount.text = $"{GlobalData.startCoins}";
        }
    }

    void Update() {
        UpdateCount();
    }
}