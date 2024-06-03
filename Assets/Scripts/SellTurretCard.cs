using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class SellTurretCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Turret activeTrt;
    private Button cardButton;
    private GameSettings gameSettings;

    public GameObject[] turretSprites;

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        cardButton = gameObject.GetComponent<Button>();
    }

    public void SellTurret() {
        if (GlobalData.activeTurret != null) GlobalData.activeTurret.Sell();
    }

    public void SetCardForSale() {
        activeTrt = GlobalData.activeTurret;
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");
        SetTexts($"Sell {activeTrt.displayName} for {activeTrt.baseCost * activeTrt.level}");
        if (turretSprites != null && turretSprites.Length > 0) {
            foreach (var turretSprite in turretSprites) {
                if (turretSprite.name == $"{activeTurretName} Sprite") {
                    turretSprite.SetActive(true);
                } else {
                    turretSprite.SetActive(false);
                }
            }
        }
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