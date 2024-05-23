using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoints : MonoBehaviour {
    [SerializeField] private Vector3[] points;
    public Vector3[] Points => points;
    public Vector3 CurrentPosition => _currentPosition;

    private Vector3 _currentPosition;
    private bool _gameStarted;

    void Start() {
        _gameStarted = true;
        _currentPosition = transform.position;
    }

    public Vector3 GetWaypointPosition(int index) {
        return CurrentPosition + Points[index];
    }

    private void OnDrawGizmosSelected() {
        if (points == null || points.Length == 0) return;
        if (!_gameStarted && transform.hasChanged) _currentPosition = transform.position;

        for (int i = 0; i < points.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (points.Length - 1);
            bool notFirstAndNotLast = i < points.Length - 1;
            Gizmos.color = isFirst ? Color.green : isLast ? Color.red : Color.black;
            Gizmos.DrawWireSphere(points[i] + _currentPosition, 0.45f);
            if (notFirstAndNotLast) {
                bool isLastLine = i >= points.Length - 2;
                Gizmos.color = isFirst ? Color.green : (isLast || isLastLine) ? Color.red : Color.black;
                Gizmos.DrawLine(points[i] + _currentPosition, points[i + 1] + _currentPosition);
            }
        }
    }
}