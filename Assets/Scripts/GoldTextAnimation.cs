using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoldTextAnimation : MonoBehaviour {
    private TextMeshPro textComponent;
    public string textToShow = "+10";

    void Start() {
        textComponent = GetComponent<TextMeshPro>();
        if (textComponent != null) textComponent.text = $"{textToShow}";
        // gameObject.transform.localScale = Vector3.one * 1.25f;
    }

    public void DestroyAfterAnimation() {
        Destroy(gameObject);
    }
}