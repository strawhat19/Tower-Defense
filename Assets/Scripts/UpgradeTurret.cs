using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class UpgradeTurret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image shortIcon;
    public Image longIcon;
    public TextMeshProUGUI upgradeCost;
    public TextMeshProUGUI currentNameText;
    public GameObject currentCostGraphicsContainer;
    public TextMeshProUGUI currentDamageText;
    public TextMeshProUGUI currentAttackSpeedText;
    public TextMeshProUGUI currentMaxRangeText;
    public TextMeshProUGUI currentCritChanceText;
    public TextMeshProUGUI upgradedNameText;
    public GameObject upgradedCostGraphicsContainer;
    public TextMeshProUGUI upgradedDamageText;
    public TextMeshProUGUI upgradedAttackSpeedText;
    public TextMeshProUGUI upgradedMaxRangeText;
    public TextMeshProUGUI upgradedCritChanceText;
    public GameObject upgradeStatTextContainer;
    public Sprite[] shortIcons;
    public Sprite[] longIcons;
    public GameObject[] costGraphics;

    private Button cardButton;
    private Turret activeTrt;
    private GameObject activeTrtObj;
    private GameSettings gameSettings;
    private bool buttonEnabled = false;
    private float costToUpgrade = 100f;
    private bool hasActiveTurret = false;
    private bool upgradeUnlocked = false;
    private bool upgradeAffordable = false;
    private string activeTurretType = "Turret";

    // Turret Upgrades
    public Dictionary<string, Dictionary<string, object>> TurretLevels = new Dictionary<string, Dictionary<string, object>> {
        { "Bullets_Gun", new Dictionary<string, object> { 
            { "Level", 2 }, 
            { "Cost", 200f }, 
            { "Type", "Bullet Gun" }, 
            { "Name", "Bullets Gun" }, 
            { "DamageMin", 20f }, 
            { "DamageMax", 30f }, 
            { "AttackSpeed", 2.33f },
            { "MaxRange", 17.5f },
            { "CriticalStrike", 20f },
        }},
        { "Bullets_Gunner", new Dictionary<string, object> { 
            { "Level", 3 }, 
            { "Cost", 400f }, 
            { "Type", "Bullet Gun" }, 
            { "Name", "Bullets Gunner" }, 
            { "DamageMin", 30f }, 
            { "DamageMax", 35f }, 
            { "AttackSpeed", 2.5f },
            { "MaxRange", 19f },
            { "CriticalStrike", 25f },
        }},
        { "Mortar_Launcher", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 400f }, 
            { "Type", "Missile Launcher" }, 
            { "Name", "Mortar Launcher" }, 
            { "DamageMin", 55f }, 
            { "DamageMax", 95f }, 
            { "AttackSpeed", 1.5f },
            { "MaxRange", 22f },
            { "CriticalStrike", 30f },
        }},
        { "MOAB", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 800f }, 
            { "Type", "Missile Launcher" }, 
            { "Name", "MOAB" }, 
            { "DamageMin", 105f }, 
            { "DamageMax", 155f }, 
            { "AttackSpeed", 2f },
            { "MaxRange", 25f },
            { "CriticalStrike", 40f },
        }},
        { "Photon_Gunner", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 600f }, 
            { "Type", "Photon Blaster" }, 
            { "Name", "Photon Gunner" }, 
            { "DamageMin", 10f }, 
            { "DamageMax", 15f }, 
            { "AttackSpeed", 12f },
            { "MaxRange", 15f },
            { "CriticalStrike", 15f },
        }},
        { "Photon_Beams", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 1200f }, 
            { "Type", "Photon Blaster" }, 
            { "Name", "Photon Beams" }, 
            { "DamageMin", 15f }, 
            { "DamageMax", 25f }, 
            { "AttackSpeed", 15f },
            { "MaxRange", 17.5f },
            { "CriticalStrike", 20f },
        }},
        { "Machine_Guns", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 800f }, 
            { "Type", "Machine Gun" }, 
            { "Name", "Machine Guns" }, 
            { "DamageMin", 15f }, 
            { "DamageMax", 25f }, 
            { "AttackSpeed", 18f },
            { "MaxRange", 12.5f },
            { "CriticalStrike", 10f },
        }},
        { "Machine_Gunner", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 1600f }, 
            { "Type", "Machine Gun" }, 
            { "Name", "Machine Gunner" }, 
            { "DamageMin", 25f }, 
            { "DamageMax", 35f }, 
            { "AttackSpeed", 20f },
            { "MaxRange", 15f },
            { "CriticalStrike", 15f },
        }},
        { "Laser_Phaser", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 1000f }, 
            { "Type", "Laser Cannon" }, 
            { "Name", "Laser Phaser" }, 
            { "DamageMin", 50f }, 
            { "DamageMax", 75f }, 
            { "AttackSpeed", 7.5f },
            { "MaxRange", 20f },
            { "CriticalStrike", 15f },
        }},
        { "Disintegration", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 2000f }, 
            { "Type", "Laser Cannon" }, 
            { "Name", "Disintegration" }, 
            { "DamageMin", 75f }, 
            { "DamageMax", 100f }, 
            { "AttackSpeed", 10f },
            { "MaxRange", 25f },
            { "CriticalStrike", 20f },
        }},
    };

    void Start() {
        SetAndUpdateRequiredParameters();
        cardButton = gameObject.GetComponent<Button>();
        gameSettings = FindObjectOfType<GameSettings>();
    }

    void SetAndUpdateRequiredParameters() {
        hasActiveTurret = GlobalData.activeTurret != null;
        activeTrt = hasActiveTurret ? GlobalData.activeTurret : null;
        activeTrtObj = activeTrt != null ? activeTrt.gameObject : null;
        activeTurretType = activeTrt != null ? activeTrt.name.Replace("(Clone)", "") : "Turret";
    }

    // On Upgrade Button Click
    public void UpgradeActiveTurret() {
        if (hasActiveTurret && activeTurretType != "Turret") {
            int levelToGoTo = activeTrt.level;
            if (activeTrt.level < 3) levelToGoTo = activeTrt.level + 1; 
            var upgradedTurretStats = GetTurretLevel(activeTurretType, levelToGoTo);
            if (upgradedTurretStats != null) {
                if (buttonEnabled) {
                    activeTrt.Upgrade(upgradedTurretStats, costToUpgrade);
                } else {
                    string cantUpgradeYetMessage = "Can't Upgrade Yet.";
                    GlobalData.Message = cantUpgradeYetMessage;
                }
            }
        }
    }

    public Dictionary<string, object> GetTurretLevel(string type, int level) {
        if (level > 3) return null;
        foreach (var turret in TurretLevels) {
            var turretData = turret.Value;
            if ((string)turretData["Type"] == type && (int)turretData["Level"] == level) {
                return turretData;
            }
        }
        return null;
    }

    public void SetUpgradeOptions() {
        SetAndUpdateRequiredParameters();
        SetCurrentCard();
        SetUpgradeCard();
        SetUpgradeCosts();
        SetUpgradeLook();
        SetButtonEnabled();
    }

    public void SetButtonEnabled() {
        upgradeUnlocked = GlobalData.currentWave > activeTrt.level;
        upgradeAffordable = costToUpgrade <= GlobalData.startCoins;
        buttonEnabled = upgradeAffordable && upgradeUnlocked;

        cardButton.interactable = buttonEnabled;
        GlobalData.SetGameObjectTransparency(gameObject, buttonEnabled ? 1f : 0.5f);
    }

    public Color HexToColor(string hex) {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color)) {
            return color;
        } else {
            Debug.LogWarning("Invalid hex color code");
            return Color.white;
        }
    }

    public void SetUpgradeLook() {
        if (hasActiveTurret) {
            int levelToUse = activeTrt.level - 1;
            if (shortIcon != null) {
                if (shortIcons != null && shortIcons.Length > 0) {
                    int indexToCheck = levelToUse >= 0 && levelToUse <= (shortIcons.Length - 1) ? levelToUse : (shortIcons.Length - 1);
                    shortIcon.sprite = shortIcons[indexToCheck];
                }
            }
            if (longIcon != null) {
                if (longIcons != null && longIcons.Length > 0) {
                    int indexToCheck = levelToUse >= 0 && levelToUse <= (longIcons.Length - 1) ? levelToUse : (longIcons.Length - 1);
                    longIcon.sprite = longIcons[indexToCheck];
                }
            }
        }
        if (upgradeStatTextContainer != null) {
            if (activeTrt.level > 1) {
                SetTextColors(Color.red, upgradeStatTextContainer);
            } else {
                Color customGreenColor = HexToColor("#C3FF00");
                SetTextColors(customGreenColor, upgradeStatTextContainer);
            }
        }
    }

    public void SetCurrentCard() {
        if (hasActiveTurret) {
            string maxRangeString = "15";
            CircleCollider2D maxRangeIndicator = activeTrtObj.GetComponent<CircleCollider2D>();
            if (maxRangeIndicator != null) maxRangeString = $"{maxRangeIndicator.radius}";

            if (currentNameText != null) currentNameText.text = $"{activeTrt.displayName}";
            if (currentDamageText != null) {
                currentDamageText.text = $"{activeTrt.damageMin} - {activeTrt.damageMax} Damage";
            }
            if (currentAttackSpeedText != null) {
                currentAttackSpeedText.text = $"{activeTrt.attackSpeed}% Attack Speed";
            }
            if (currentMaxRangeText != null) currentMaxRangeText.text = $"{maxRangeString} Max Range";
            if (currentCritChanceText != null) {
                currentCritChanceText.text = $"{activeTrt.critChance}% Critical Strike";
            }
        }
    }

    public void SetUpgradeCard() {
        if (hasActiveTurret) {
            if (activeTrt.level < 3) {
                var upgradedTurretStats = GetTurretLevel(activeTurretType, activeTrt.level + 1);
                if (upgradedTurretStats != null) SetUpgradeCardStats(upgradedTurretStats);
            } else {
                var upgradedTurretStats = GetTurretLevel(activeTurretType, activeTrt.level);
                if (upgradedTurretStats != null) SetUpgradeCardStats(upgradedTurretStats);
            }
        }
    }

    public void SetUpgradeCardStats(Dictionary<string, object> upgradedTurretStats) {
        if (hasActiveTurret) {
            if (upgradedNameText != null) upgradedNameText.text = $"{upgradedTurretStats["Name"]}";
            if (upgradedDamageText != null) {
                string damageText = $"{upgradedTurretStats["DamageMin"]} - {upgradedTurretStats["DamageMax"]} Damage";
                upgradedDamageText.text = damageText;
            }
            if (upgradedAttackSpeedText != null) {
                upgradedAttackSpeedText.text = $"{upgradedTurretStats["AttackSpeed"]}% Attack Speed";
            }
            if (upgradedMaxRangeText != null) upgradedMaxRangeText.text = $"{upgradedTurretStats["MaxRange"]} Max Range";
            if (upgradedCritChanceText != null) {
                upgradedCritChanceText.text = $"{upgradedTurretStats["CriticalStrike"]}% Critical Strike";
            }
        }
    }

    public void SetUpgradeCosts() {
        if (hasActiveTurret) {
            if (costGraphics != null && costGraphics.Length > 0) {
                foreach (var costGraphic in costGraphics) {
                    if (costGraphic.name == $"{activeTurretType} Cost Graphic") {
                        if (costGraphic.transform.parent != null) {
                            string parentName = costGraphic.transform.parent.name.ToLower();
                            if (parentName.Contains("upgrade")) {
                                costToUpgrade = activeTrt.baseCost * (activeTrt.level * gameSettings.UpgradeTurretCostPercentage);
                                string upgradeCostText = $"{costToUpgrade}";
                                string upgradeCostTextOnCard = $"{activeTrt.cost + costToUpgrade}";
                                if (upgradeCost != null) upgradeCost.text = upgradeCostText;
                                SetTexts(upgradeCostTextOnCard, costGraphic);
                            } else {
                                SetTexts($"{activeTrt.cost}", costGraphic);
                            }
                        }
                        costGraphic.SetActive(true);
                    } else {
                        costGraphic.SetActive(false);
                    }
                }
            }
        }
    }

    public void SetTexts(string textToSet, GameObject gameObj) {
        TextMeshProUGUI[] texts = gameObj.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) {
            text.text = textToSet;
        }
    }
    
    public void SetTextColors(Color colorToSet, GameObject gameObj) {
        TextMeshProUGUI[] texts = gameObj.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) {
            text.color = colorToSet;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (gameSettings != null) {
            if (cardButton != null && cardButton.interactable == true) {
                GlobalData.overrideCursor = true;
                gameSettings.SetCursor(gameSettings.hoverCursorTexture);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (gameSettings != null) {
            GlobalData.overrideCursor = false;
            gameSettings.SetCursor(gameSettings.defaultCursorTexture);
        }
    }
}