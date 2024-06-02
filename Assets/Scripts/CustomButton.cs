using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CustomButton : MonoBehaviour {
    private Turret parentTurret;
    private Transform parentObj;
    private GameSettings gameSettings;

    void Start() {
        parentObj = gameObject.transform.parent;
        gameSettings = FindObjectOfType<GameSettings>();
        if (parentObj != null) parentTurret = parentObj.GetComponent<Turret>();
    }

    void OnMouseDown() {
        if (parentTurret != null) {
            parentTurret.ToggleUI();
        }
    }
}