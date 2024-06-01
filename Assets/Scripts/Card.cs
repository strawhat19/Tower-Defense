using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public GameObject turret;
    private Button cardButton;
    private bool turretUnlocked;
    public Sprite coinIconSprite;
    public Sprite lockIconSprite;
    private Turret turretSettings;
    public GameObject costContainer;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    private GameSettings gameSettings;
    public TextMeshProUGUI critChanceText;
    public GameObject buttonIsActiveVisual;
    public TilemapInteraction turretShop;
    public TextMeshProUGUI attackSpeedText;
    private SpriteRenderer spriteRenderer;

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        cardButton = gameObject.GetComponent<Button>();
        SetCard();
    }

    void Update() {
        UpdateCardLook();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (gameSettings != null) {
            if (turretUnlocked) {
                gameSettings.SetCursor(gameSettings.hoverCursorTexture);
            } else {
                gameSettings.SetCursor(gameSettings.disabledCursorTexture);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (gameSettings != null) {
            gameSettings.SetCursor(gameSettings.defaultCursorTexture);
        }
    }

    public void OnCardClicked() {
        if (turretUnlocked) {
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
        }
    }

    void UpdateCardLook() {
        if (turret != null && turretShop != null) {
            bool isActiveTurret = turretShop.turret == turret;
            if (turretSettings != null) {
                int turretUnlockedAfterWave = turretSettings.unlockedAfterWave;
                turretUnlocked = turretUnlockedAfterWave <= GlobalData.currentWave;
                // bool readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
                // bool turretIsReady = (turretUnlockedAfterWave + 1) <= GlobalData.currentWave;
                if (cardButton != null) cardButton.interactable = turretUnlocked;
                if (turretUnlocked) {
                    SetStatusIcon(coinIconSprite);
                    SetCost(turretSettings.baseCost);
                    if (turretSettings.baseCost > GlobalData.startCoins) {
                        GlobalData.SetGameObjectTransparency(gameObject, 0.55f);
                    } else {
                        GlobalData.SetGameObjectTransparency(gameObject, 1f);
                    }
                } else {
                    SetStatusIcon(lockIconSprite);
                    SetCost(turretSettings.unlockedAfterWave);
                    GlobalData.SetGameObjectTransparency(gameObject, 0.35f);
                }
            }
            if (buttonIsActiveVisual != null) {
                GlobalData.SetGameObjectTransparency(buttonIsActiveVisual, 0.55f);
                buttonIsActiveVisual.SetActive(isActiveTurret);
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

    void SetStatusIcon(Sprite spriteToPut) {
        if (costContainer != null) {
            Image statusIconContainer = costContainer.GetComponent<Image>();
            if (statusIconContainer != null) {
                if (spriteToPut != null) {
                    statusIconContainer.sprite = spriteToPut;
                }
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

    // void MovePositionXAsTextGrows() {
    //     if (costContainer != null) {
    //         if (costString.Length == 3) {
    //             rectTransform.localPosition = initialPosition; 
    //         } else if (costString.Length > 3) {
    //             float adjustment = rectTransform.localPosition.x + (costString.Length * 0.03f);
    //             rectTransform.localPosition = initialPosition + new Vector3(-adjustment, 0, 0); 
    //         }
    //     }
    // }

    // void ScaleTurretDamageAndCost() {
    //     Turret turretSettings = turret.GetComponent<Turret>();
    //     if (turretSettings != null) {
    //         SetCost(turretSettings.baseCost * GlobalData.currentWave);
    //         if (GlobalData.currentWave > 1 && damageText != null) {
    //             float scaledDamageMin = GlobalData.CalculateScaled(turretSettings.damageMin);
    //             float scaledDamageMax = GlobalData.CalculateScaled(turretSettings.damageMax);
    //             string scaledDamageMinString = GlobalData.RemoveDotZeroZero(scaledDamageMin.ToString("F2"));
    //             string scaledDamageMaxString = GlobalData.RemoveDotZeroZero(scaledDamageMax.ToString("F2"));
    //             damageText.text = $"Damage: {scaledDamageMinString} - {scaledDamageMaxString}";
    //         }
    //     }
    // }
}