using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour {
    [Header("Turret Settings")]
    public GameObject sellTurretButtonCard;
    public GameObject upgradeTurretButtonCard;

    [Header("Cursor Settings")]
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public Texture2D disabledCursorTexture;

    void Start() {
        SetCursor(defaultCursorTexture);
    }

    void Update() {
        if (GlobalData.activeTurret != null) {
            if (sellTurretButtonCard != null) {
                SellTurretCard sellTurretCard = sellTurretButtonCard.GetComponent<SellTurretCard>();
                if (sellTurretCard != null) sellTurretCard.SetCardForSale();
                sellTurretButtonCard.SetActive(true);
            }
            if (upgradeTurretButtonCard != null) {
                UpgradeTurret[] upgradeTurretOptions = upgradeTurretButtonCard.GetComponentsInChildren<UpgradeTurret>();
                if (upgradeTurretOptions != null) upgradeTurretOptions[0].SetUpgradeOptions();
                upgradeTurretButtonCard.SetActive(true);
            }
        } else {
            if (sellTurretButtonCard != null) sellTurretButtonCard.SetActive(false);
            if (upgradeTurretButtonCard != null) upgradeTurretButtonCard.SetActive(false);
        }
    }

    public void SetCursor(Texture2D textureToSet) {
        if (textureToSet != null) {
            Cursor.SetCursor(textureToSet, Vector2.zero, CursorMode.Auto);
        }
    }
}