using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// [ExecuteAlways]
public class Turret : MonoBehaviour {
    public bool canAim = true;
    public bool canFire = true;
    public float critChance = 10.0f;
    public float critMultiplier = 2.0f;
    public float attackSpeed = 2.0f;
    public float damageMin = 15.0f;
    public float damageMax = 25.0f;
    public float cost = 100.0f;
    public float baseCost = 100.0f;

    public GameObject projectile;
    public AudioSource shootSound;
    public AudioSource hitSound;
    public Transform barrelOfTheGun;

    public GameObject preview;
    private AimAndFire aimAndFire;
    public GameObject aimAndFireObject;
    public GameObject[] costTexts;

    private Transform target;
    private float range = 1.0f;
    private Transform finishLine;
    private float cooldown = 0.0f;
    private bool canAfford = false;
    private GameObject rangeIndicator;
    private bool alwaysShowRangeIndicator = false;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Start() {
        SetAimAndFire();
        SetTurret();
    }

    void Update() {
        UpdateTurret();
    }

    public void EnableFiring(bool enable) {
        canFire = enable;
    }

    void SetAimAndFire() {
        if (aimAndFireObject != null) aimAndFire = aimAndFireObject.GetComponent<AimAndFire>();
    }

    public void ShowRange(bool show) {
        bool ifShowRange = alwaysShowRangeIndicator ? true : show;
        if (rangeIndicator != null) rangeIndicator.SetActive(ifShowRange);
        if (preview != null) preview.SetActive(show);
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

    // void ScaleCost() {
    //     float newCostScaledByWaveAndLevel = GlobalData.CalculateLevelScaled(baseCost);
    //     cost = baseCost * GlobalData.currentWave;
    //     SetCosts();
    // }

    void UpdateTurret() {
        // ScaleCost();
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
            float colliderRange = (float)collider.radius / 3.33f;
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
                    } else {
                        if (!transparent) canAfford = true;
                        color.a = transparent ? 0.35f : 0.75f;
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

    private Sprite CreateCircleSprite() {
        int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        float radius = size / 2f;
        Vector2 center = new Vector2(radius, radius);
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Vector2 pos = new Vector2(x, y);
                if (Vector2.Distance(pos, center) <= radius) {
                    texture.SetPixel(x, y, Color.cyan); // Set the color to cyan
                } else {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    void SetTurret() {
        // ScaleCost();
        SetCosts();
        GameObject finishLineObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishLineObject != null) finishLine = finishLineObject.GetComponent<Transform>();
        CircleCollider2D finishcollider = gameObject.AddComponent<CircleCollider2D>();
        finishcollider.isTrigger = true;
        finishcollider.radius = range;

        // Create and configure the range indicator
        rangeIndicator = new GameObject("RangeIndicator");
        rangeIndicator.transform.SetParent(transform);
        rangeIndicator.transform.localPosition = Vector3.zero;
        var lineRenderer = rangeIndicator.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;

        // Create a material and set it to the LineRenderer
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = new Color(0, 1, 1, 0.5f); // Cyan with semi-transparency
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineMaterial.color;
        lineRenderer.endColor = lineMaterial.color;

        int segments = 100;
        lineRenderer.positionCount = segments + 1;

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        float radius = collider.radius;

        for (int i = 0; i <= segments; i++) {
            float angle = i * (2f * Mathf.PI / segments);
            float x = Mathf.Cos(angle) * (radius / 3.33f);
            float y = Mathf.Sin(angle) * (radius / 3.33f);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }

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
}