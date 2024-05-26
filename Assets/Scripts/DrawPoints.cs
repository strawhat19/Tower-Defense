using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawPoints : MonoBehaviour {
    public Vector3 currentPosition;
    public bool alwaysShowPoints = true;
    public Vector3[] points;
    public int fontSize = 16;
    public float radius = 0.45f;
    public float handleSize = 0.25f;
    public Color fontColor = Color.white;
    public Color pathColor = Color.black;
    public Color handleColor = Color.black;
    public Color wireCircleColor = Color.black;
    public HandleShapes handleShape = HandleShapes.Circle;

    private bool gameStarted;

    void Start() {
        gameStarted = true;
        currentPosition = transform.position;
    }

    public Vector3 GetPointPosition(int index) {
        return currentPosition + points[index];
    }

    private void OnDrawGizmos() {
        if (!alwaysShowPoints) return;
        DrawPointsInSceneView();
    }

    private void OnDrawGizmosSelected() {
        if (alwaysShowPoints) return;
        DrawPointsInSceneView();
    }

    void DrawPointsInSceneView() {
        if (points == null || points.Length == 0) return;
        if (!gameStarted && transform.hasChanged) currentPosition = transform.position;

        for (int i = 0; i < points.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (points.Length - 1);
            bool notFirstAndNotLast = i < points.Length - 1;
            Gizmos.color = isFirst ? Color.green : isLast ? Color.red : wireCircleColor;
            Gizmos.DrawWireSphere(points[i] + currentPosition, radius);
            if (notFirstAndNotLast) {
                bool isLastLine = i >= points.Length - 2;
                Gizmos.color = isFirst ? Color.green : (isLast || isLastLine) ? Color.red : pathColor;
                Gizmos.DrawLine(points[i] + currentPosition, points[i + 1] + currentPosition);
            }
        }
    }
}