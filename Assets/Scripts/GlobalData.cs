using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GlobalData {
    public static float defaultHealth = 100f;
    public static float defaultReward = 10f;
    public static float defaultDamage = 1f;
    public static float defaultSpeed = 2f;
    public static float startCoins = 0f;
    public static float startLives = 20f;
    public static string RemoveDotZeroZero(string input) {
        if (input.EndsWith(".00")) {
            return input.Substring(0, input.Length - 3);  // Remove the last three characters
        }
        return input;
    }
}