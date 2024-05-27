using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// [ExecuteAlways]
public class Card : MonoBehaviour {
    public GameObject turret;
    private Turret turretSettings;
    public GameObject costContainer;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI critChanceText;
    public TextMeshProUGUI attackSpeedText;
    public TilemapInteraction turretShop;
    private SpriteRenderer spriteRenderer;

    void Start() {
        SetCard();
    }

    // void Update() {
        // SetCard();
    // }

    public void OnCardClicked() {
        OperateTurretShop();
    }

    void OperateTurretShop() {
        if (turretShop != null) {
            if (turretShop.turret == turret) {
                if (turretShop.activePreviewTurret != null) Destroy(turretShop.activePreviewTurret);
                turretShop.buildingTurret = true;
            } else {
                turretShop.turret = turret;
                if (turretShop.activePreviewTurret != null) Destroy(turretShop.activePreviewTurret);
                turretShop.buildingTurret = true;
            }
        }
    }

    void SetCardImage(Sprite spriteToSet) {
        if (costContainer != null) {
            spriteRenderer = costContainer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.sprite = spriteToSet;
            }
        }
    }

    void SetCost(float costToSet) {
        if (costText != null) {
            costText = costText.GetComponent<TextMeshProUGUI>();
            string costString = GlobalData.RemoveDotZeroZero(costToSet.ToString("F2"));
            costText.text = $"{costString}";
        }
    }

    void SetDamage(float damageMinToSet, float damageMaxToSet) {
        if (damageText != null) {
            string damageMinString = GlobalData.RemoveDotZeroZero(damageMinToSet.ToString("F2"));
            string damageMaxString = GlobalData.RemoveDotZeroZero(damageMaxToSet.ToString("F2"));
            damageText.text = $"{damageMinString} - {damageMaxString} Damage";
        }
    }

    void SetAttackSpeed(float attackSpeed) {
        if (attackSpeedText != null) {
            string attackSpeedString = GlobalData.RemoveDotZeroZero(attackSpeed.ToString("F1"));
            attackSpeedText.text = $"{attackSpeedString}% Attack Speed";
        }
    }

    void SetCritChance(float criticalStrikeChance, float criticalCritMultiplier) {
        if (critChanceText != null) {
            string critChanceString = GlobalData.RemoveDotZeroZero(criticalStrikeChance.ToString("F2"));
            critChanceText.text = $"{critChanceString}% Critical Strike";
        }
    }

    void SetCard() {
        if (turret != null) {
            turretSettings = turret.GetComponent<Turret>();
            if (turretSettings != null) {
                SetCost(turretSettings.baseCost);
                SetAttackSpeed(turretSettings.attackSpeed);
                SetDamage(turretSettings.damageMin, turretSettings.damageMax);
                SetCritChance(turretSettings.critChance, turretSettings.critMultiplier);
            }

            SpriteRenderer turretSpriteRenderer = turret.GetComponent<SpriteRenderer>();
            if (turretSpriteRenderer != null) {
                SetCardImage(turretSpriteRenderer.sprite);
            }

            // Turret turretSettings = turret.GetComponent<Turret>();
            // if (turretSettings != null) {
            //     // SetCost(turretSettings.baseCost * GlobalData.currentWave);
            //     if (GlobalData.currentWave > 1 && damageText != null) {
            //         float scaledDamageMin = GlobalData.CalculateScaled(turretSettings.damageMin);
            //         float scaledDamageMax = GlobalData.CalculateScaled(turretSettings.damageMax);
            //         string scaledDamageMinString = GlobalData.RemoveDotZeroZero(scaledDamageMin.ToString("F2"));
            //         string scaledDamageMaxString = GlobalData.RemoveDotZeroZero(scaledDamageMax.ToString("F2"));
            //         damageText.text = $"Damage: {scaledDamageMinString} - {scaledDamageMaxString}";
            //     }
            // }
        }
    }

    // Adjust Position X Based on Character Count of Cost On Scene Edit
    // void AdjustPositionX() {
    //     if (costContainer != null) {
    //         if (costString.Length == 3) {
    //             rectTransform.localPosition = initialPosition; 
    //         } else if (costString.Length > 3) {
    //             float adjustment = rectTransform.localPosition.x + (costString.Length * 0.03f); // Adjust this value to fit your needs
    //             rectTransform.localPosition = initialPosition + new Vector3(-adjustment, 0, 0); 
    //         }
    //     }
    // }
}