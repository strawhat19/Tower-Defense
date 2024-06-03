using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class UpgradeTurret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
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
    public GameObject[] costGraphics;

    private Button cardButton;
    private GameSettings gameSettings;
    private float costToUpgrade = 200f;

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        cardButton = gameObject.GetComponent<Button>();
    }

    public void UpgradeActiveTurret() {
        Debug.Log("Upgrade Turret!");
    }

    public void SetUpgradeOptions() {
        SetCurrentCard();
        SetUpgradeCard();
        SetUpgradeCosts();
    }

    public void SetCurrentCard() {
        Turret activeTrt = GlobalData.activeTurret;
        GameObject activeTrtObj = activeTrt.gameObject;
        string maxRangeString = "15 Max Range";
        CircleCollider2D maxRangeIndicator = activeTrtObj.GetComponent<CircleCollider2D>();
        if (maxRangeIndicator != null) maxRangeString = $"{maxRangeIndicator.radius} Max Range";
        string activeTurretName = activeTrt.name.Replace("(Clone)", "");
        if (currentNameText != null) currentNameText.text = $"{activeTurretName}";
        if (currentDamageText != null) {
            currentDamageText.text = $"{activeTrt.damageMin} - {activeTrt.damageMax} Damage";
        }
        if (currentAttackSpeedText != null) currentAttackSpeedText.text = $"{activeTrt.attackSpeed}% Attack Speed";
        if (currentMaxRangeText != null) currentMaxRangeText.text = $"{maxRangeString}";
        if (currentCritChanceText != null) currentCritChanceText.text = $"{activeTrt.critChance}% Critical Strike";
    }

    public void SetUpgradeCard() {

    }

    public void SetUpgradeCosts() {
        string activeTurretName = GlobalData.activeTurret.name.Replace("(Clone)", "");
        if (costGraphics != null && costGraphics.Length > 0) {
            foreach (var costGraphic in costGraphics) {
                if (costGraphic.name == $"{activeTurretName} Cost Graphic") {
                    if (costGraphic.transform.parent != null) {
                        string parentName = costGraphic.transform.parent.name.ToLower();
                        if (parentName.Contains("upgrade")) {
                            costToUpgrade = GlobalData.activeTurret.cost * 2;
                            string upgradeCostText = $"{costToUpgrade}";
                            if (upgradeCost != null) upgradeCost.text = upgradeCostText;
                            SetTexts(upgradeCostText, costGraphic);
                        } else {
                            SetTexts($"{GlobalData.activeTurret.cost}", costGraphic);
                        }
                    }
                    costGraphic.SetActive(true);
                } else {
                    costGraphic.SetActive(false);
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