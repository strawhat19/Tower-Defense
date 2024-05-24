using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Waves))]
public class WavePointsEditor : Editor {
    Waves Waves => target as Waves;
    private Vector3 newWaypointsPoint;

    private void OnSceneGUI() {
        for (int i = 0; i < Waves.waypoints.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (Waves.waypoints.Length - 1);
            bool notFirstAndNotLast = i < Waves.waypoints.Length - 1;

            Handles.color = isFirst ? Color.green : isLast ? Color.red : Waves.handleColor;

            EditorGUI.BeginChangeCheck();

            // Create Handles
            Vector3 currentWaypointsPoint = GlobalData.waypointPosition + Waves.waypoints[i];
            var fmh_18_17_638518458882687444 = Quaternion.identity;
            
            // Adjust the size and appearance of the handle
            Vector3 handleSnap = new Vector3(0.2f, 0.2f, 0.2f); // Smaller snap increments

            if (Waves.handleShape == HandleShapes.Square) {
                newWaypointsPoint = Handles.FreeMoveHandle(
                    currentWaypointsPoint, 
                    Waves.handleSize, 
                    handleSnap, 
                    Handles.DotHandleCap
                );
            } else if (Waves.handleShape == HandleShapes.Circle) {
                newWaypointsPoint = Handles.FreeMoveHandle(
                    currentWaypointsPoint, 
                    Waves.handleSize,
                    handleSnap, 
                    Handles.CircleHandleCap
                );
            }

            // Create text
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = Waves.fontSize;
            textStyle.normal.textColor = Waves.fontColor;
            Vector3 textAlignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(GlobalData.waypointPosition + Waves.waypoints[i] + textAlignment,
                $"{i + 1}", textStyle);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Free Move Handle");
                Waves.waypoints[i] = newWaypointsPoint - GlobalData.waypointPosition;
            }
        }
    }
}