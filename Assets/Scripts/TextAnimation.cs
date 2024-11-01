using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextAnimation : MonoBehaviour {
    public Sprite critStrikeSprite;
    public Image textAnimationIcon;
    private TextMeshPro textComponent;
    public bool isCriticalStrike = false;
    public string textToShow = "100 Damage";
    public float critScaleMultiplier = 1.25f;

    void Start() {
        textComponent = GetComponent<TextMeshPro>();
        if (textComponent != null) textComponent.text = $"{textToShow}";
        if (textAnimationIcon != null) {
            if (isCriticalStrike) {
                gameObject.transform.localScale = Vector3.one * critScaleMultiplier;
                if (critStrikeSprite != null) {
                    textAnimationIcon.sprite = critStrikeSprite;
                }
            }
        } 
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }
}