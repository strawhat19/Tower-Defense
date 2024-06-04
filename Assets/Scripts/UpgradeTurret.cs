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
    private GameSettings gameSettings;
    private float costToUpgrade = 100f;

    public Dictionary<string, Dictionary<string, object>> TurretLevels = new Dictionary<string, Dictionary<string, object>> {
        { "Bullet_Gun", new Dictionary<string, object> {
            { "Level", 1 }, 
            { "Cost", 100f },  
            { "Type", "Bullet Gun" }, 
            { "Name", "Bullet Gun" }, 
            { "DamageMin", 15f }, 
            { "DamageMax", 25f }, 
            { "AttackSpeed", 2f },
            { "MaxRange", 15f },
            { "CriticalStrike", 12f },
        }},
        { "Bullets_Gun", new Dictionary<string, object> { 
            { "Level", 2 }, 
            { "Cost", 200f }, 
            { "Type", "Bullet Gun" }, 
            { "Name", "Bullets Gun" }, 
            { "DamageMin", 20f }, 
            { "DamageMax", 30f }, 
            { "AttackSpeed", 3f },
            { "MaxRange", 17.5f },
            { "CriticalStrike", 15f },
        }},
        { "Bullets_Gunner", new Dictionary<string, object> { 
            { "Level", 3 }, 
            { "Cost", 300f }, 
            { "Type", "Bullet Gun" }, 
            { "Name", "Bullets Gunner" }, 
            { "DamageMin", 30f }, 
            { "DamageMax", 35f }, 
            { "AttackSpeed", 3.5f },
            { "MaxRange", 19f },
            { "CriticalStrike", 18f },
        }},
        { "Missile_Launcher", new Dictionary<string, object> {
            { "Level", 1 }, 
            { "Cost", 200f }, 
            { "Type", "Missile Launcher" }, 
            { "Name", "Missile Launcher" }, 
            { "DamageMin", 55f }, 
            { "DamageMax", 85f }, 
            { "AttackSpeed", 1f },
            { "MaxRange", 20f },
            { "CriticalStrike", 15f },
        }},
        { "Mortar_Launcher", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 400f }, 
            { "Type", "Missile Launcher" }, 
            { "Name", "Mortar Launcher" }, 
            { "DamageMin", 75f }, 
            { "DamageMax", 95f }, 
            { "AttackSpeed", 1.5f },
            { "MaxRange", 22f },
            { "CriticalStrike", 20f },
        }},
        { "MOAB", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 600f }, 
            { "Type", "Missile Launcher" }, 
            { "Name", "MOAB" }, 
            { "DamageMin", 85f }, 
            { "DamageMax", 115f }, 
            { "AttackSpeed", 2f },
            { "MaxRange", 25f },
            { "CriticalStrike", 25f },
        }},
        { "Photon_Blaster", new Dictionary<string, object> {
            { "Level", 1 }, 
            { "Cost", 300f }, 
            { "Type", "Photon Blaster" }, 
            { "Name", "Photon Blaster" }, 
            { "DamageMin", 5f }, 
            { "DamageMax", 10f }, 
            { "AttackSpeed", 10f },
            { "MaxRange", 12f },
            { "CriticalStrike", 8f },
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
            { "CriticalStrike", 10f },
        }},
        { "Photon_Beams", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 900f }, 
            { "Type", "Photon Blaster" }, 
            { "Name", "Photon Beams" }, 
            { "DamageMin", 15f }, 
            { "DamageMax", 25f }, 
            { "AttackSpeed", 15f },
            { "MaxRange", 17.5f },
            { "CriticalStrike", 15f },
        }},
        { "Machine_Gun", new Dictionary<string, object> {
            { "Level", 1 }, 
            { "Cost", 400f }, 
            { "Type", "Machine Gun" }, 
            { "Name", "Machine Gun" }, 
            { "DamageMin", 10f }, 
            { "DamageMax", 15f }, 
            { "AttackSpeed", 15f },
            { "MaxRange", 10f },
            { "CriticalStrike", 5f },
        }},
        { "Machine_Guns", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 800f }, 
            { "Type", "Machine Gun" }, 
            { "Name", "Machine Guns" }, 
            { "DamageMin", 15f }, 
            { "DamageMax", 25f }, 
            { "AttackSpeed", 17.5f },
            { "MaxRange", 12.5f },
            { "CriticalStrike", 10f },
        }},
        { "Machine_Gunner", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 1200f }, 
            { "Type", "Machine Gun" }, 
            { "Name", "Machine Gunner" }, 
            { "DamageMin", 25f }, 
            { "DamageMax", 35f }, 
            { "AttackSpeed", 20f },
            { "MaxRange", 15f },
            { "CriticalStrike", 15f },
        }},
        { "Laser_Cannon", new Dictionary<string, object> {
            { "Level", 1 }, 
            { "Cost", 500f }, 
            { "Type", "Laser Cannon" }, 
            { "Name", "Laser Cannon" }, 
            { "DamageMin", 35f }, 
            { "DamageMax", 50f }, 
            { "AttackSpeed", 3.5f },
            { "MaxRange", 18f },
            { "CriticalStrike", 10f },
        }},
        { "Laser_Phaser", new Dictionary<string, object> {
            { "Level", 2 }, 
            { "Cost", 1000f }, 
            { "Type", "Laser Cannon" }, 
            { "Name", "Laser Phaser" }, 
            { "DamageMin", 50f }, 
            { "DamageMax", 75f }, 
            { "AttackSpeed", 5f },
            { "MaxRange", 20f },
            { "CriticalStrike", 12f },
        }},
        { "Disintegration", new Dictionary<string, object> {
            { "Level", 3 }, 
            { "Cost", 1500f }, 
            { "Type", "Laser Cannon" }, 
            { "Name", "Disintegration" }, 
            { "DamageMin", 75f }, 
            { "DamageMax", 100f }, 
            { "AttackSpeed", 7.5f },
            { "MaxRange", 25f },
            { "CriticalStrike", 15f },
        }},
    };

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        cardButton = gameObject.GetComponent<Button>();
    }

    public void UpgradeActiveTurret() {
        Turret activeTrt = GlobalData.activeTurret;
        GameObject activeTrtObj = activeTrt.gameObject;
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");

        int levelToGoTo = activeTrt.level;
        if (activeTrt.level < 3) levelToGoTo = activeTrt.level + 1; 
        var upgradedTurretStats = GetTurretLevel(activeTurretName, levelToGoTo);
        if (upgradedTurretStats != null) {
            if (GlobalData.startCoins >= costToUpgrade) {
                activeTrt.Upgrade(upgradedTurretStats, costToUpgrade);
            } else {
                string cantAffordUpgradeMessage = "Cannot Afford This Upgrade";
                GlobalData.Message = cantAffordUpgradeMessage;
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
        return null; // Return null if no match is found
    }

    public void SetUpgradeOptions() {
        SetCurrentCard();
        SetUpgradeCard();
        SetUpgradeCosts();
        SetUpgradeLook();
    }

    public Color HexToColor(string hex) {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color)) {
            return color;
        } else {
            Debug.LogWarning("Invalid hex color code");
            return Color.white; // Return white if hex code is invalid
        }
    }

    public void SetUpgradeLook() {
        Turret activeTrt = GlobalData.activeTurret;
        if (activeTrt != null) {
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
        Turret activeTrt = GlobalData.activeTurret;
        GameObject activeTrtObj = activeTrt.gameObject;
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");

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

    public void SetUpgradeCard() {
        Turret activeTrt = GlobalData.activeTurret;
        GameObject activeTrtObj = activeTrt.gameObject;
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");

        if (activeTrt.level < 3) {
            var upgradedTurretStats = GetTurretLevel(activeTurretName, activeTrt.level + 1);
            if (upgradedTurretStats != null) SetUpgradeCardStats(upgradedTurretStats);
        } else {
            var upgradedTurretStats = GetTurretLevel(activeTurretName, activeTrt.level);
            if (upgradedTurretStats != null) SetUpgradeCardStats(upgradedTurretStats);
        }
    }

    public void SetUpgradeCardStats(Dictionary<string, object> upgradedTurretStats) {
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

    public void SetUpgradeCosts() {
        Turret activeTrt = GlobalData.activeTurret;
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");
        if (costGraphics != null && costGraphics.Length > 0) {
            foreach (var costGraphic in costGraphics) {
                if (costGraphic.name == $"{activeTurretName} Cost Graphic") {
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
        bool upgradeAffordable = costToUpgrade <= GlobalData.startCoins;
        cardButton.interactable = upgradeAffordable;
        GlobalData.SetGameObjectTransparency(gameObject, upgradeAffordable ? 1f : 0.5f);
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