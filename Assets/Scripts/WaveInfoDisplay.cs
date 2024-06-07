using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WaveInfoDisplay : MonoBehaviour {
    private Waves waves;
    private Wave activeWave;

    [HideInInspector]
    public GameSettings gameSettings;
    public GameObject enemyContainer;

    public TextMeshProUGUI waveNum;
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI maxHealth;
    public TextMeshProUGUI maxEnemies;
    public TextMeshProUGUI spawnDelay;
    public TextMeshProUGUI bountyReward;
    public TextMeshProUGUI movementSpeed;

    void Start() {
        waves = FindObjectOfType<Waves>();
        gameSettings = FindObjectOfType<GameSettings>();
    }

    public void SetActiveWave(Wave waveInfoToDisplay, GameObject enemyToShow) {
        if (waveInfoToDisplay != null) activeWave = waveInfoToDisplay;
        SetEnemyContainer(enemyToShow);
        SetActiveWaveTexts();
    }

    void SetActiveWaveTexts() {
        if (waveNum != null) waveNum.text = $"Wave {GlobalData.currentWave}";
        if (activeWave != null) {
            if (maxHealth != null) maxHealth.text = $"{activeWave.health} Max Health";
            if (maxEnemies != null) maxEnemies.text = $"{activeWave.maxObjects} Enemies Total";
            // if (spawnDelay != null) spawnDelay.text = $"Every {activeWave.spawnDelay} Seconds";
            if (bountyReward != null) bountyReward.text = $"{activeWave.reward} Coins Per Kill";
            if (movementSpeed != null) {
                float moveSpdPerc = activeWave.speed * 33.33f;
                string moveSpdPercString = GlobalData.RemoveDotZeroZero(moveSpdPerc.ToString("F2"));
                movementSpeed.text = $"{moveSpdPercString}% Movement Speed";
            }
            if (enemyName != null && activeWave.enemy != null) {
                enemyName.text = $"{activeWave.name}";
            }
        }
    }

    void SetEnemyContainer(GameObject enemyToShow) {
        if (enemyContainer != null && waves != null) {
            foreach (Transform child in enemyContainer.transform) {
                if (child && child.gameObject) {
                    Destroy(child.gameObject);
                }
            }

            if (enemyToShow != null) {
                GameObject newEnemy = Instantiate(enemyToShow, enemyContainer.transform);
                newEnemy.transform.localPosition = Vector3.zero;

                foreach (Transform child in newEnemy.transform) {
                    if (child && child.gameObject) Destroy(child.gameObject);
                }

                Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                if (enemyScript != null) enemyScript.enabled = false;

                BoxCollider2D boxCollider = newEnemy.GetComponent<BoxCollider2D>();
                if (boxCollider != null) boxCollider.enabled = false;

                CircleCollider2D circleCollider = newEnemy.GetComponent<CircleCollider2D>();
                if (circleCollider != null) circleCollider.enabled = false;

                Rigidbody2D rigidbody2D = newEnemy.GetComponent<Rigidbody2D>();
                if (rigidbody2D != null) Destroy(rigidbody2D);
            }
        }
    }
}