using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// [ExecuteAlways]
public class Turret : MonoBehaviour {
    public bool canAim = true;
    public bool canFire = true;
    public bool turretPlaced = false;
    public bool alwaysShowRangeIndicator = false;
    public int level = 1;
    public int unlockedAfterWave = 1;
    public float critChance = 10.0f;
    public float critMultiplier = 2.0f;
    public float attackSpeed = 2.0f;
    public float damageMin = 15.0f;
    public float damageMax = 25.0f;
    public float radiusModifier = 3.33f;
    public float cost = 100.0f;
    public float baseCost = 100.0f;

    public string displayName = "Bullet Gun";
    public GameObject projectile;
    public GameObject preview;
    public GameObject aimAndFireObject;
    public AudioSource shootSound;
    public AudioSource hitSound;
    public Transform barrelOfTheGun;
    public SpriteRenderer levelIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    public Vector3Int cellPos;

    public GameObject[] costTexts;
    public Sprite[] levelIcons;

    private Transform target;
    private float range = 1.0f;
    private Transform finishLine;
    private AimAndFire aimAndFire;
    private float cooldown = 0.0f;
    private bool canAfford = false;
    private GameObject rangeIndicator;
    private GameSettings gameSettings;
    private TilemapInteraction shopTerrain;
    private GameObject sellTurretButtonCard;
    private static List<Turret> allTurrets = new List<Turret>();
    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Start() {
        sellTurretButtonCard = GameObject.FindGameObjectWithTag("SellTurretButtonCard");
        shopTerrain = FindObjectOfType<TilemapInteraction>();
        gameSettings = FindObjectOfType<GameSettings>();
        allTurrets.Add(this);
        SetAimAndFire();
        SetTurret();
    }

    void OnDestroy() {
        allTurrets.Remove(this);
    }

    void Update() {
        if (canAim) {
            enemiesInRange.RemoveAll(enemy => enemy == null || enemy.GetComponent<Enemy>().currentHealth <= 0);
            FindTarget();
            if (target != null) {
                RotateTowardsTarget();
                if (canFire) {
                    ShootAtTarget();
                }
            }
        }
    }

    public void EnableFiring(bool enable) {
        canFire = enable;
    }

    void SetAimAndFire() {
        if (aimAndFireObject != null) aimAndFire = aimAndFireObject.GetComponent<AimAndFire>();
    }

    public void ToggleUI() {
        bool show = !alwaysShowRangeIndicator;
        ShowUI(show);
        GlobalData.activeTurret = show && turretPlaced ? this : null;
        if (show) CloseOtherTurretsUI();
    }

    public void ShowUI(bool show = true) {
        alwaysShowRangeIndicator = show;
        ShowRange(alwaysShowRangeIndicator);
    }

    public void ShowRange(bool show = true) {
        bool ifShowRange = alwaysShowRangeIndicator ? true : show;
        if (preview != null) preview.SetActive(show);
        if (rangeIndicator != null) {
            rangeIndicator.transform.localPosition = new Vector3(0, 0, -0.001f);
            rangeIndicator.SetActive(ifShowRange);
        }
    }

    private void CloseOtherTurretsUI() {
        foreach (var turret in allTurrets) {
            if (turret != this) {
                turret.ShowUI(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Add(trigger.gameObject);
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Remove(trigger.gameObject);
    }

    void SetCosts() {
        string costString = GlobalData.RemoveDotZeroZero(cost.ToString("F2"));
        if (preview != null) {
            bool userProvidedCostTexts = costTexts != null || costTexts.Length > 0 || costTexts[0] != null;
            if (userProvidedCostTexts) {
                for (int i = 0; i < costTexts.Length; i++) {
                    costTexts[i].GetComponent<TextMeshProUGUI>().text = costString;
                }
            }
        }
    }

    void UpdateTurretNameAndCost() {
        if (nameText != null) nameText.text = $"{displayName}";
        if (costText != null) costText.text = $"{baseCost * level}";
    }

    void UpdateTurretLevelIcon() {
        if (levelIcon != null) {
            if (levelIcons != null && levelIcons.Length > 0) {
                levelIcon.sprite = levelIcons[level - 1];
            }
        }
    }

    private void FindTarget() {
        GameObject closestEnemy = GetClosestEnemy();
        if (closestEnemy != null) {
            target = closestEnemy.transform;
        } else {
            target = null;
        }
    }

    private void RotateTowardsTarget() {
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.rotation = rotation;
    }

    void ShootAtTarget() {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f) {
            Shoot(target.gameObject);
            cooldown = 1.0f / attackSpeed;
        }
    }

    void Shoot(GameObject target) {
        if (aimAndFire != null) aimAndFire.canFire = true;
        if (projectile != null && barrelOfTheGun != null) {
            GameObject projectileObject = Instantiate(projectile, barrelOfTheGun.position, barrelOfTheGun.rotation);
            Projectile proj = projectileObject.GetComponent<Projectile>();
            if (proj != null) {
                bool isCriticalStrike = false;
                float randomValueInRange = Random.Range(0f, 100f);
                float damageInRange = Random.Range(damageMin, damageMax);
                if (randomValueInRange < critChance) {
                    isCriticalStrike = true;
                    damageInRange *= critMultiplier;
                }
                // float damage = GlobalData.CalculateLevelScaled(damageInRange);
                if (shootSound != null) shootSound.Play();
                proj.Seek(target, damageInRange, isCriticalStrike, hitSound);
            }
        }
    }

    GameObject GetClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemiesInRange) {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            float distanceToFinishLineX = Mathf.Abs(enemy.transform.position.x - (finishLine == null ? GlobalData.finishLineX : finishLine.position.x));
            if (distanceToFinishLineX < shortestDistance) {
                shortestDistance = distanceToFinishLineX;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void OnDrawGizmos() {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null) {
            Handles.color = Color.cyan;
            float colliderRange = (float)collider.radius / radiusModifier;
            Handles.DrawWireDisc(transform.position, Vector3.forward, colliderRange);
        }
    }

    void SetHaloTransparency(float alpha) {
        if (rangeIndicator != null) {
            var lineRenderer = rangeIndicator.GetComponent<LineRenderer>();
            if (lineRenderer != null) {
                Color newColor = lineRenderer.startColor;
                newColor.a = alpha;
                lineRenderer.startColor = newColor;
                lineRenderer.endColor = newColor;
            }
        }
    }

    public void SetAffordability(bool transparent, bool turretIsPlaced = false) {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            foreach (Material mat in renderer.materials) {
                if (mat.HasProperty("_Color")) { // Check if the material has a _Color property
                    Color color = mat.color;
                    if (turretIsPlaced) {
                        color.a = 1.0f;
                        turretPlaced = true;
                    } else {
                        if (transparent) {
                            canAfford = true;
                            color.a = 0.35f;
                        } else {
                            // float grayscale = (color.r + color.g + color.b) / 3f;
                            // color = new Color(grayscale, grayscale, grayscale);
                            color.a = 0.75f;
                        }
                    }
                    mat.color = color;
                }
            }
        }

        bool showLogs = false;
        if (canAfford && showLogs == true) Debug.Log("Can now afford " + gameObject.name);

        SetHaloTransparency(transparent ? 0.35f : 0.75f);
        ShowRange(!turretIsPlaced); // Show the range indicator only if not fully opaque

        // Show or hide the red 'X' based on affordability
        // Transform redX = rangeIndicator.transform.Find("RedX");
        // if (redX != null) {
        //     redX.gameObject.SetActive(!canAfford);
        // }
    }

    // private Sprite CreateCircleSprite() {
    //     int size = 256;
    //     Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
    //     float radius = size / 2f;
    //     Vector2 center = new Vector2(radius, radius);
    //     for (int y = 0; y < size; y++) {
    //         for (int x = 0; x < size; x++) {
    //             Vector2 pos = new Vector2(x, y);
    //             if (Vector2.Distance(pos, center) <= radius) {
    //                 texture.SetPixel(x, y, Color.cyan); // Set the color to cyan
    //             } else {
    //                 texture.SetPixel(x, y, Color.clear);
    //             }
    //         }
    //     }
    //     texture.Apply();
    //     return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    // }

    void SetRangeIndicator() {
        if (rangeIndicator == null) {
            // Create and configure the range indicator
            rangeIndicator = new GameObject("RangeIndicator");
            rangeIndicator.transform.SetParent(transform);
            rangeIndicator.transform.localPosition = Vector3.zero;
            var lineRenderer = rangeIndicator.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = false;
        }

        var existingLineRenderer = rangeIndicator.GetComponent<LineRenderer>();

        // Create a material and set it to the LineRenderer
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        Color cyanColor = new Color(0, 1, 1, 0.5f);
        Color greenColor = new Color(0, 1, 0, 0.5f);
        Color redColor = new Color(1, 0, 0, 0.5f);
        Color colorToUse = level == 1 ? cyanColor : level >= 3 ? redColor : greenColor;
        lineMaterial.color = colorToUse;
        existingLineRenderer.material = lineMaterial;
        existingLineRenderer.startColor = lineMaterial.color;
        existingLineRenderer.endColor = lineMaterial.color;

        int segments = 100;
        existingLineRenderer.positionCount = segments + 1;

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        float radius = collider.radius;

        for (int i = 0; i <= segments; i++) {
            float angle = i * (2f * Mathf.PI / segments);
            float x = Mathf.Cos(angle) * (radius / radiusModifier);
            float y = Mathf.Sin(angle) * (radius / radiusModifier);
            existingLineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    void SetTurret() {
        // ScaleCost();

        displayName = gameObject.name.Replace("(Clone)", "");
        SetCosts();
        GameObject finishLineObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishLineObject != null) finishLine = finishLineObject.GetComponent<Transform>();
        CircleCollider2D finishcollider = gameObject.AddComponent<CircleCollider2D>();
        finishcollider.isTrigger = true;
        finishcollider.radius = range;

        SetRangeIndicator();

        bool ifShowRange = alwaysShowRangeIndicator ? true : false;
        rangeIndicator.SetActive(ifShowRange);

        // Create the red 'X' GameObject
        // GameObject redX = new GameObject("RedX");
        // redX.transform.SetParent(rangeIndicator.transform);
        // redX.transform.localPosition = Vector3.zero;

        // LineRenderer xLineRenderer = redX.AddComponent<LineRenderer>();
        // xLineRenderer.startWidth = 0.1f;
        // xLineRenderer.endWidth = 0.1f;
        // xLineRenderer.useWorldSpace = false;
        // xLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        // xLineRenderer.material.color = Color.red;

        // xLineRenderer.positionCount = 4;
        // xLineRenderer.SetPosition(0, new Vector3(-0.5f, -0.5f, 0));
        // xLineRenderer.SetPosition(1, new Vector3(0.5f, 0.5f, 0));
        // xLineRenderer.SetPosition(2, new Vector3(0.5f, -0.5f, 0));
        // xLineRenderer.SetPosition(3, new Vector3(-0.5f, 0.5f, 0));

        // redX.SetActive(false); // Initially hide the red 'X'
        
        // Bubble Range
        // // Create and configure the range indicator as a bubble
        // rangeIndicator = new GameObject("RangeIndicator");
        // rangeIndicator.transform.SetParent(transform);
        // rangeIndicator.transform.localPosition = Vector3.zero;
        // var spriteRenderer = rangeIndicator.AddComponent<SpriteRenderer>();
        // spriteRenderer.sprite = CreateCircleSprite();
        // spriteRenderer.color = new Color(0, 1, 1, 0.5f); // Cyan with semi-transparent

        // CircleCollider2D collider = GetComponent<CircleCollider2D>();
        // float colliderRange = (float)collider.radius * 2f;
        // rangeIndicator.transform.localScale = new Vector3(colliderRange, colliderRange, 1);
        // rangeIndicator.SetActive(false);
    }

    public void Sell() {
        if (shopTerrain != null && cellPos != null) shopTerrain.VacantGridCell(cellPos);
        float upgradeCostOfTrt = baseCost * level;
        GlobalData.startCoins = GlobalData.startCoins + upgradeCostOfTrt;
        GlobalData.activeTurret = null;
        Destroy(gameObject);
        string turretPlacedMessage = displayName + $" Sold! +{upgradeCostOfTrt}";
        GlobalData.Message = turretPlacedMessage;
    }

    public void Upgrade(Dictionary<string, object> upgradedTurretStats) {
        level = level + 1;
        displayName = (string)upgradedTurretStats["Name"];
        GlobalData.startCoins = GlobalData.startCoins - (float)upgradedTurretStats["Cost"];
        damageMin = (float)upgradedTurretStats["DamageMin"];
        damageMax = (float)upgradedTurretStats["DamageMax"];
        attackSpeed = (float)upgradedTurretStats["AttackSpeed"];
        critChance = (float)upgradedTurretStats["CriticalStrike"];
        float newOuterRange = (float)upgradedTurretStats["MaxRange"];
        CircleCollider2D maxRangeIndicator = gameObject.GetComponent<CircleCollider2D>();
        if (aimAndFireObject != null) {
            CircleCollider2D aimAndFireCollider = aimAndFireObject.GetComponent<CircleCollider2D>();
                float newRadius = CalculateProportionalValue(maxRangeIndicator.radius, aimAndFireCollider.radius, newOuterRange);
                if (aimAndFireCollider != null) {
                    aimAndFireCollider.radius = (float)newRadius;
                }
        }
        if (maxRangeIndicator != null) maxRangeIndicator.radius = newOuterRange;
        UpdateTurretNameAndCost();
        UpdateTurretLevelIcon();
        SetRangeIndicator();
        string turretPlacedMessage = displayName + $" Upgraded! -{upgradedTurretStats["Cost"]}";
        GlobalData.Message = turretPlacedMessage;
    }

    public float CalculateProportionalValue(float originalFirstValue, float originalSecondValue, float newFirstValue) {
        // Calculate the ratio
        float ratio = originalSecondValue / originalFirstValue;
        // Calculate the new second value based on the ratio
        float newSecondValue = newFirstValue * ratio;
        return newSecondValue;
    }
}