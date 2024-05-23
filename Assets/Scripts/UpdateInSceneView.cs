using UnityEngine;
[ExecuteAlways] // Ensures the script runs in the editor without playing the game
public class UpdateOnValueChange : MonoBehaviour {

    private void OnValidate() {
        UpdateSceneView();
    }

    private void UpdateSceneView() {
        // Your code to update the scene view based on myFloat
        // For example, changing the scale of the GameObject
    }
}