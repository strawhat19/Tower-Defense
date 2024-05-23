using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// [ExecuteAlways]
public class Card : MonoBehaviour {
    public GameObject turret;
    public GameObject costContainer;
    public TextMeshProUGUI costText;
    private SpriteRenderer spriteRenderer;

    void Start() {
        SetCard();
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

    void SetCard() {
        if (turret != null) {
            SpriteRenderer turretSpriteRenderer = turret.GetComponent<SpriteRenderer>();
            if (turretSpriteRenderer != null) {
                SetCardImage(turretSpriteRenderer.sprite);
            }

            Turret turretSettings = turret.GetComponent<Turret>();
            if (turretSettings != null) {
                SetCost(turretSettings.cost);
            }
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