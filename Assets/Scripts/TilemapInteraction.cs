using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInteraction : MonoBehaviour {
    public Tilemap tilemap;
    public GameObject turret;
    public Color highlightColor = Color.cyan;
    private Color originalColor;
    private Vector3Int previousMousePos = new Vector3Int();

    void Start() {
        if (tilemap == null) {
            tilemap = GetComponent<Tilemap>();
        }
    }

    void Update() {
        Camera.main.orthographic = true;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0; // Ensure the z position is 0 for 2D
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        if (cellPos != previousMousePos) {
            if (tilemap.HasTile(previousMousePos)) {
                ResetTileColor(previousMousePos);
            }
            if (tilemap.HasTile(cellPos)) {
                HighlightTile(cellPos);
            }
            previousMousePos = cellPos;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (tilemap.HasTile(cellPos)) {
                OnTileClicked(worldPos); // Use world position here
            }
        }

        // Debug logs
        // Debug.Log("World Position: " + worldPos);
        // Debug.Log("Cell Position: " + cellPos);
    }

    void HighlightTile(Vector3Int cellPos) {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile != null) {
            originalColor = tilemap.GetColor(cellPos);
            tilemap.SetColor(cellPos, highlightColor);
        }
    }

    void ResetTileColor(Vector3Int cellPos) {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile != null) {
            tilemap.SetColor(cellPos, originalColor);
        }
    }

    void OnTileClicked(Vector3 worldPos) {
        float turretCost = turret.GetComponent<Turret>().cost;
        if (GlobalData.startCoins >= turretCost) {
            Instantiate(turret, worldPos, Quaternion.identity);
            GlobalData.startCoins -= turretCost;
            Debug.Log("Turret placed at: " + worldPos + ". Remaining coins: " + GlobalData.startCoins);
        } else {
            Debug.Log("Cannot afford turret. Cost: " + turretCost + ", Available: " + GlobalData.startCoins);
        }
    }
}