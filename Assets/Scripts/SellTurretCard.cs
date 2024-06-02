using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class SellTurretCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button cardButton;
    private GameSettings gameSettings;

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        cardButton = gameObject.GetComponent<Button>();
    }

    public void SellTurret() {
        if (GlobalData.activeTurret != null) GlobalData.activeTurret.Sell();
    }

    public void SetTexts(string textToSet) {
        TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) {
            text.text = textToSet;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (gameSettings != null) {
            GlobalData.overrideCursor = true;
            gameSettings.SetCursor(gameSettings.hoverCursorTexture);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (gameSettings != null) {
            GlobalData.overrideCursor = false;
            gameSettings.SetCursor(gameSettings.defaultCursorTexture);
        }
    }
}