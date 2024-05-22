using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoints))]
public class WaypointsEditor : Editor {
    Waypoints Waypoints => target as Waypoints;

    private void OnSceneGUI() {
        Handles.color = Color.cyan;
        for (int i = 0; i < Waypoints.Points.Length; i++) {
            EditorGUI.BeginChangeCheck();

            // Create Handles
            Vector3 currentWaypointsPoint = Waypoints.CurrentPosition + Waypoints.Points[i];
            var fmh_18_17_638518458882687444 = Quaternion.identity; Vector3 newWaypointsPoint = Handles.FreeMoveHandle(currentWaypointsPoint, 0.7f,
                new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);

            // Create text
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.white;
            Vector3 textAlignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(Waypoints.CurrentPosition + Waypoints.Points[i] + textAlignment,
                $"{i + 1}", textStyle);

            EditorGUI.EndChangeCheck();

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Free Move Handle");
                Waypoints.Points[i] = newWaypointsPoint - Waypoints.CurrentPosition;
            }
        }
    }
}