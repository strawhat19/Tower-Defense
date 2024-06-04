using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour {
    [Header("Turret Settings")]
    public float UpgradeTurretCostPercentage = 0.5f;
    public GameObject sellTurretButtonCard;
    public GameObject currentStatsCard;
    public GameObject upgradeButton;
    public GameObject upgradeStatsCard;
    public GameObject upgradeTurretButtonCard;

    [Header("Cursor Settings")]
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public Texture2D disabledCursorTexture;

    void Start() {
        SetCursor(defaultCursorTexture);
    }

    void Update() {
        SetSellCardOptions();
        SetUpgradeCardOptions();
    }

    public void SetSellCardOptions() {
        if (GlobalData.activeTurret != null) {
            if (sellTurretButtonCard != null) {
                SellTurretCard sellTurretCard = sellTurretButtonCard.GetComponent<SellTurretCard>();
                if (sellTurretCard != null) sellTurretCard.SetCardForSale();
                sellTurretButtonCard.SetActive(true);
            }
        } else {
            if (sellTurretButtonCard != null) sellTurretButtonCard.SetActive(false);
        }
    }

    public void SetUpgradeEnabled(bool enabled) {
        float alphaOpacity = enabled == true ? 1f : 0f;
        Button buttonToInteract = upgradeButton.GetComponent<Button>();
        buttonToInteract.interactable = enabled;
        GlobalData.SetGameObjectTransparency(upgradeButton, alphaOpacity);
    }

    public void SetUpgradeCardOptions() {
        if (GlobalData.activeTurret != null) {
            UpgradeTurret[] upgradeTurretOptions = null;
            if (upgradeTurretButtonCard != null) {
                upgradeTurretOptions = upgradeTurretButtonCard.GetComponentsInChildren<UpgradeTurret>();
                if (upgradeTurretOptions != null && upgradeTurretOptions.Length > 0) {
                    upgradeTurretOptions[0].SetUpgradeOptions();
                }
            }
            if (GlobalData.activeTurret.level >= 3) {
                if (currentStatsCard != null) currentStatsCard.SetActive(true);
                if (upgradeButton != null) SetUpgradeEnabled(false);
                if (upgradeStatsCard != null) GlobalData.SetGameObjectTransparency(upgradeStatsCard, 0f);
            } else {
                if (currentStatsCard != null) currentStatsCard.SetActive(true);
                if (upgradeButton != null) SetUpgradeEnabled(true);
                if (upgradeStatsCard != null) GlobalData.SetGameObjectTransparency(upgradeStatsCard, 1f);
            }
        } else {
            if (currentStatsCard != null) currentStatsCard.SetActive(false);
            if (upgradeButton != null) SetUpgradeEnabled(false);
            if (upgradeStatsCard != null) GlobalData.SetGameObjectTransparency(upgradeStatsCard, 0f);
        }
    }

    public void SetCursor(Texture2D textureToSet) {
        if (textureToSet != null) {
            Cursor.SetCursor(textureToSet, Vector2.zero, CursorMode.Auto);
        }
    }
}