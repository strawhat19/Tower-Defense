using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapInteraction : MonoBehaviour {
    public bool buildingTurret = true;
    public Tilemap tilemap;
    public GameObject turret;
    // public GameObject[] turrets;
    public Color highlightColor = Color.cyan;

    private Color originalColor;
    private bool turretUnlocked;
    private bool canAfford = false;
    private GameSettings gameSettings;
    public GameObject activePreviewTurret;
    private Vector3Int previousMousePos = new Vector3Int();
    private HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();

    void Start() {
        gameSettings = FindObjectOfType<GameSettings>();
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
    }

    void Update() {
        if (!buildingTurret) {
            if (activePreviewTurret != null) {
                Destroy(activePreviewTurret);
            }
            return;
        }

        Camera.main.orthographic = true;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0; // Ensure the z position is 0 for 2D
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cellPos);

        if (tilemap.HasTile(cellPos) && !occupiedTiles.Contains(cellPos)) {
            if (activePreviewTurret == null) {
                // Instantiate the semi-transparent turret preview
                activePreviewTurret = Instantiate(turret, cellCenterPos, Quaternion.identity);
                Turret turretComponent = activePreviewTurret.GetComponent<Turret>();
                turretComponent.SetAffordability(true);
                turretComponent.ShowRange(true);
                turretComponent.EnableFiring(false);
                
                // Set the sorting order of the turret's SpriteRenderer
                SpriteRenderer sr = activePreviewTurret.GetComponent<SpriteRenderer>();
                if (sr != null) {
                    sr.sortingOrder = 10; // Set to a value higher than the terrain
                }
            } else {
                // Move the semi-transparent turret preview to follow the mouse
                activePreviewTurret.transform.position = cellCenterPos;
                activePreviewTurret.SetActive(true);

                // Check affordability and adjust transparency
                Turret turretComponent = activePreviewTurret.GetComponent<Turret>();
                int turretUnlockedAfterWave = turretComponent.unlockedAfterWave;
                turretUnlocked = turretUnlockedAfterWave <= GlobalData.currentWave;

                if (turretUnlocked) {
                    if (GlobalData.startCoins >= turretComponent.baseCost) {
                        turretComponent.SetAffordability(false);  // Less transparent
                        if (gameSettings != null) {
                            gameSettings.SetCursor(gameSettings.hoverCursorTexture);
                        }
                    } else {
                        turretComponent.SetAffordability(true);  // More transparent
                        if (gameSettings != null) {
                            gameSettings.SetCursor(gameSettings.disabledCursorTexture);
                        }
                    }
                } else {
                    turretComponent.SetAffordability(false);
                    if (gameSettings != null) {
                        gameSettings.SetCursor(gameSettings.hoverCursorTexture);
                    }
                }
            }
        } else {
            if (activePreviewTurret != null) {
                // Hide the semi-transparent turret preview if not over a tile
                activePreviewTurret.SetActive(false);
            }
            if (gameSettings != null) {
                gameSettings.SetCursor(gameSettings.defaultCursorTexture);
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

        if (Input.GetMouseButtonDown(0) && tilemap.HasTile(cellPos) && !occupiedTiles.Contains(cellPos)) {
            OnTileClicked(cellCenterPos, cellPos);
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

    void OnTileClicked(Vector3 worldPos, Vector3Int cellPos) {
        // Get the cost of the turret from its component
        float turretCost = turret.GetComponent<Turret>().baseCost;
        int turretUnlockedAfterWave = turret.GetComponent<Turret>().unlockedAfterWave;
        turretUnlocked = turretUnlockedAfterWave <= GlobalData.currentWave;

        // Check if the player can afford the turret
        canAfford = GlobalData.startCoins >= turretCost;
        if (turretUnlocked && canAfford) {
            // Instantiate the turret at the clicked position
            GameObject newTurret = Instantiate(turret, worldPos, Quaternion.identity);
            // Enable the firing logic for the placed turret
            Turret turretComponent = newTurret.GetComponent<Turret>();
            
            turretComponent.SetAffordability(false, canAfford);
            turretComponent.ShowRange(false);
            turretComponent.EnableFiring(true);
            // Deduct the cost from the player's coins
            GlobalData.startCoins -= turretCost;

            // Set the sorting order of the turret's SpriteRenderer
            SpriteRenderer sr = newTurret.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sortingOrder = 10; // Set to a value higher than the terrain
            }

            // Mark the tile as occupied
            occupiedTiles.Add(cellPos);

            // Destroy the semi-transparent turret preview
            if (activePreviewTurret != null) Destroy(activePreviewTurret);

            string turretPlacedMessage = turret.name + " placed";
            GlobalData.Message = turretPlacedMessage;
        } else if (!turretUnlocked) {
            GlobalData.Message = turret.name + " will be unlocked after wave #" + turretUnlockedAfterWave;
        } else {
            string cantAffordTurretMessage = "Cannot afford " + turret.name + ". Cost: " + turretCost + ", Available: " + GlobalData.startCoins;
            GlobalData.Message = cantAffordTurretMessage;
        }
    }
}