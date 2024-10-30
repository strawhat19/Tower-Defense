using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class GlobalData {

    // Start Values
    public static Turret activeTurret;
    public static bool overrideCursor = false;
    public static float defaultHealth = 100f;
    public static float defaultReward = 10f;
    public static float defaultDamage = 1f;
    public static float defaultSpeed = 2f;
    public static string Message = "Click Start Waves To Start Game.";
    public static float startLives = GameSettings.Instance != null ? GameSettings.Instance.startLives : 20f;
    public static float startCoins = GameSettings.Instance != null ? GameSettings.Instance.startCoins : 500f;

    // Wave Management
    public static int killed = 0;
    public static Wave activeWave;
    public static int maxWaves = 3;
    public static int currentWave = 1;
    public static int currentLevel = 1;
    public static float finishLineX = 0;
    public static Vector3 waypointPosition;
    public static bool hasActiveTurret = false;
    public static bool lastEnemyInWaveSpawned = false;
    public static bool lastEnemyInWaveDied = false;

    public static string RemoveDotZeroZero(string input) {
        if (decimal.TryParse(input, out decimal number)) {
            return number.ToString("0.##");
        }
        return input;
    }

    public static float CalculateScaled(float initialVal) {
        float calculatedLevelScalingValue = (float)(initialVal * currentLevel) * (currentWave > 1 ? (currentWave / currentWave + 1) : currentWave);
        float updatedValue = (float)(calculatedLevelScalingValue / 2) + initialVal;
        return updatedValue;
    }

    public static float CalculateLevelScaled(float initialVal) {
        float calculatedLevelScalingValue = (float)(initialVal * currentLevel) * (currentWave > 1 ? (currentWave / currentWave + 1) : currentWave);
        return calculatedLevelScalingValue;
    }

    public static void SetGameObjectTransparency(GameObject gameObj, float alpha) {
        Image[] images = gameObj.GetComponentsInChildren<Image>();
        foreach (Image img in images) {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }

        TextMeshProUGUI[] texts = gameObj.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts) {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }

        SpriteRenderer[] sprites = gameObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }
    }
}