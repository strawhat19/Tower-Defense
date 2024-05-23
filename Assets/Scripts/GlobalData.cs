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

    // Wave Management
    public static int maxWaves = 3;
    public static int currentWave = 1;
    public static int currentLevel = 1;
    public static bool lastEnemyInWaveSpawned = false;
    public static bool lastEnemyInWaveDied = false;

    public static string RemoveDotZeroZero(string input) {
        if (input.EndsWith(".00")) {
            return input.Substring(0, input.Length - 3);  // Remove the last three characters
        }
        return input;
    }
}