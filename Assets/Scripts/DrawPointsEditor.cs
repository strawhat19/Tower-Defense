using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DrawPoints))]
public class DrawPointsEditor : Editor {
    DrawPoints DrawPoints => target as DrawPoints;
    private Vector3 newDrawPoint;

    private void OnSceneGUI() {
        for (int i = 0; i < DrawPoints.points.Length; i++) {
            bool isFirst = i == 0;
            bool isLast = i == (DrawPoints.points.Length - 1);
            bool notFirstAndNotLast = i < DrawPoints.points.Length - 1;

            Handles.color = isFirst ? Color.green : isLast ? Color.red : DrawPoints.handleColor;

            EditorGUI.BeginChangeCheck();

            // Create Handles
            Vector3 currentDrawPoint = DrawPoints.currentPosition + DrawPoints.points[i];
            var fmh_18_17_638518458882687444 = Quaternion.identity;
            
            // Adjust the size and appearance of the handle
            Vector3 handleSnap = new Vector3(0.2f, 0.2f, 0.2f); // Smaller snap increments

            if (DrawPoints.handleShape == HandleShapes.Square) {
                newDrawPoint = Handles.FreeMoveHandle(
                    currentDrawPoint, 
                    DrawPoints.handleSize, 
                    handleSnap, 
                    Handles.DotHandleCap
                );
            } else if (DrawPoints.handleShape == HandleShapes.Circle) {
                newDrawPoint = Handles.FreeMoveHandle(
                    currentDrawPoint, 
                    DrawPoints.handleSize,
                    handleSnap, 
                    Handles.CircleHandleCap
                );
            }

            // Create text
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = DrawPoints.fontSize;
            textStyle.normal.textColor = DrawPoints.fontColor;
            Vector3 textAlignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(DrawPoints.currentPosition + DrawPoints.points[i] + textAlignment,
                $"{i + 1}", textStyle);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Free Move Handle");
                DrawPoints.points[i] = newDrawPoint - DrawPoints.currentPosition;
            }
        }
    }
}