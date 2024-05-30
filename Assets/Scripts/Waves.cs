using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum HandleShapes {
    Circle = 1,
    Square = 2,
}

public class Waves : MonoBehaviour {
    [Header("Waves Settings")]
    public float delay = 5f;
    public GameObject[] waves;
    private int currentWaveIndex = 0;
    private bool gameStarted = false;
    public bool wavesStarted = false;
    private bool isWaitingForNextWave = false;

    [Header("Waypoints Settings")]
    public bool alwaysShowPath = true;
    public Vector3[] waypoints;
    public int fontSize = 16;
    public float handleSize = 0.25f;
    public Color fontColor = Color.white;
    public Color pathColor = Color.black;
    public Color handleColor = Color.black;
    public Color wireCircleColor = Color.black;
    public HandleShapes handleShape = HandleShapes.Circle;

    void Start() {
        SetPath();
        SetWaves();
        StartWaves();
    }

    // void Update() {
        // UpdateWaves();
    // }

    void SetPath() {
        gameStarted = true;
        GlobalData.waypointPosition = transform.position;
        GlobalData.finishLineX = waypoints[waypoints.Length - 1].x;
    }

    public Vector3 GetWaypointPosition(int index) {
        return GlobalData.waypointPosition + waypoints[index];
    }

    void SetWaves() {
        bool userNotProvidedWaves = waves == null || waves.Length == 0 || waves[0] == null;
        if (userNotProvidedWaves) {
            List<GameObject> waveList = new List<GameObject>();
            foreach (Transform child in transform) {
                if (child.GetComponent<Wave>() != null) {
                    waveList.Add(child.gameObject);
                }
            }
            waves = waveList.ToArray();
        }
    }

    void StartWaves() {
        GlobalData.maxWaves = waves.Length;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GlobalData.currentLevel = currentSceneIndex + 1;
        for (int i = 0; i < waves.Length; i++) {
            waves[i].SetActive(false);
        }
    }

    void UpdateWaves() {
        bool readyForNextWave = GlobalData.lastEnemyInWaveSpawned && GlobalData.lastEnemyInWaveDied;
        if (readyForNextWave && !isWaitingForNextWave) {
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    private IEnumerator StartNextWaveAfterDelay() {
        isWaitingForNextWave = true;
        if (currentWaveIndex < waves.Length - 1) {
            Debug.Log("Starting Next Wave In " + delay + " seconds");
            // GlobalData.startCoins = GlobalData.startCoins + (100f * GlobalData.currentWave);
        }
        yield return new WaitForSeconds(delay);
        ActivateNextWave();
        isWaitingForNextWave = false;
    }

    public void ActivateNextWave() {
        if (currentWaveIndex < waves.Length - 1) {
            if (wavesStarted == true) {
                // waves[currentWaveIndex].SetActive(false);
                currentWaveIndex++;
            }
            GlobalData.currentWave = currentWaveIndex + 1;
            GlobalData.lastEnemyInWaveSpawned = false;
            GlobalData.lastEnemyInWaveDied = false;
            waves[currentWaveIndex].SetActive(true);
            wavesStarted = true;
            GlobalData.Message = "Wave Started";
        } else {
            GlobalData.Message = "All waves are complete!";
        }
    }

    private void OnDrawGizmos() {
        if (!alwaysShowPath) return;
        if (waypoints == null || waypoints.Length == 0) return;
        if (!gameStarted && transform.hasChanged) GlobalData.waypointPosition = transform.position;

        for (int i = 0; i < waypoints.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (waypoints.Length - 1);
            bool notFirstAndNotLast = i < waypoints.Length - 1;
            Gizmos.color = isFirst ? Color.green : isLast ? Color.red : wireCircleColor;
            Gizmos.DrawWireSphere(waypoints[i] + GlobalData.waypointPosition, 0.45f);
            if (notFirstAndNotLast) {
                bool isLastLine = i >= waypoints.Length - 2;
                Gizmos.color = isFirst ? Color.green : (isLast || isLastLine) ? Color.red : pathColor;
                Gizmos.DrawLine(waypoints[i] + GlobalData.waypointPosition, waypoints[i + 1] + GlobalData.waypointPosition);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (alwaysShowPath) return;
        if (waypoints == null || waypoints.Length == 0) return;
        if (!gameStarted && transform.hasChanged) GlobalData.waypointPosition = transform.position;

        for (int i = 0; i < waypoints.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (waypoints.Length - 1);
            bool notFirstAndNotLast = i < waypoints.Length - 1;
            Gizmos.color = isFirst ? Color.green : isLast ? Color.red : wireCircleColor;
            Gizmos.DrawWireSphere(waypoints[i] + GlobalData.waypointPosition, 0.45f);
            if (notFirstAndNotLast) {
                bool isLastLine = i >= waypoints.Length - 2;
                Gizmos.color = isFirst ? Color.green : (isLast || isLastLine) ? Color.red : pathColor;
                Gizmos.DrawLine(waypoints[i] + GlobalData.waypointPosition, waypoints[i + 1] + GlobalData.waypointPosition);
            }
        }
    }
}