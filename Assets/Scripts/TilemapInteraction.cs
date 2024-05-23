using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInteraction : MonoBehaviour {
    private Color originalColor;
    private GameObject activePreviewTurret;
    private Vector3Int previousMousePos = new Vector3Int();

    public Tilemap tilemap;
    public GameObject turret;
    public GameObject[] turrets;
    public Color highlightColor = Color.cyan;

    void Start() {
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
    }

    void Update() {
        Camera.main.orthographic = true;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0; // Ensure the z position is 0 for 2D
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        if (tilemap.HasTile(cellPos)) {
            if (activePreviewTurret == null) {
                // Instantiate the semi-transparent turret preview
                activePreviewTurret = Instantiate(turret, worldPos, Quaternion.identity);
                Turret turretComponent = activePreviewTurret.GetComponent<Turret>();
                turretComponent.SetTransparency(true);
                turretComponent.ShowRange(true);
                turretComponent.EnableFiring(false);
            } else {
                // Move the semi-transparent turret preview to follow the mouse
                activePreviewTurret.transform.position = worldPos;
                activePreviewTurret.SetActive(true);

                // Check affordability and adjust transparency
                Turret turretComponent = activePreviewTurret.GetComponent<Turret>();
                if (GlobalData.startCoins >= turretComponent.cost) {
                    turretComponent.SetTransparency(false);  // Less transparent
                } else {
                    turretComponent.SetTransparency(true);  // More transparent
                }
            }
        } else {
            if (activePreviewTurret != null) {
                // Hide the semi-transparent turret preview if not over a tile
                activePreviewTurret.SetActive(false);
            }
        }

        if (cellPos != previousMousePos) {
            if (tilemap.HasTile(previousMousePos)) {
                ResetTileColor(previousMousePos);
            }
            if (tilemap.HasTile(cellPos)) {
                HighlightTile(cellPos);
            }
            previousMousePos = cellPos;
        }

        if (Input.GetMouseButtonDown(0) && tilemap.HasTile(cellPos)) {
            OnTileClicked(worldPos);
        }
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
        // Get the cost of the turret from its component
        float turretCost = turret.GetComponent<Turret>().cost;

        // Check if the player can afford the turret
        if (GlobalData.startCoins >= turretCost) {
            // Instantiate the turret at the clicked position
            GameObject newTurret = Instantiate(turret, worldPos, Quaternion.identity);
            // Enable the firing logic for the placed turret
            Turret turretComponent = newTurret.GetComponent<Turret>();
            turretComponent.SetTransparency(false, true);
            turretComponent.ShowRange(false);
            turretComponent.EnableFiring(true);
            // Deduct the cost from the player's coins
            GlobalData.startCoins -= turretCost;

            // Destroy the semi-transparent turret preview
            if (activePreviewTurret != null) {
                Destroy(activePreviewTurret);
            }

            // Debug log for successful placement
            Debug.Log("Turret placed at: " + worldPos + ". Remaining coins: " + GlobalData.startCoins);
        } else {
            // Debug log for insufficient funds
            Debug.Log("Cannot afford turret. Cost: " + turretCost + ", Available: " + GlobalData.startCoins);
        }
    }
}