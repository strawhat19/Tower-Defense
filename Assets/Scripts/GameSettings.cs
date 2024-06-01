using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour {
    [Header("Cursor Settings")]
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public Texture2D disabledCursorTexture;

    void Start() {
        SetCursor(defaultCursorTexture);
    }

    public void SetCursor(Texture2D textureToSet) {
        if (textureToSet != null) {
            Cursor.SetCursor(textureToSet, Vector2.zero, CursorMode.Auto);
        }
    }
}