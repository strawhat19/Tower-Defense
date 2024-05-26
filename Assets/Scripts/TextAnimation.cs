using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextAnimation : MonoBehaviour {
    private TextMeshPro textComponent;
    public string textToShow = "100 Damage";

    void Start() {
        textComponent = GetComponent<TextMeshPro>();
        if (textComponent != null) textComponent.text = $"{textToShow}";
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }
}