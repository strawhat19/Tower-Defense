using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro;  // Include this if you're using TextMeshPro

public class Turret : MonoBehaviour {
    public bool canAim = true;
    public bool canFire = true;
    public float baseCost = 100.0f;
    public float cost;
    public float rateOfFire = 1.0f;
    public float damageMin = 5.0f;
    public float damageMax = 15.0f;
    public string displayName = "Turret";

    public GameObject projectile;
    public AudioSource shootSound;
    public AudioSource hitSound;
    public Transform barrelOfTheGun;
    
    public TextMeshProUGUI costText;  // Reference to the TextMeshPro component for displaying the cost

    private Transform target;
    private float range = 1.0f;
    private Transform finishLine;
    private float cooldown = 0.0f;
    private GameObject rangeIndicator;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Start() {
        SetTurret();
    }

    void Update() {
        UpdateTurret();
    }

    public void EnableFiring(bool enable) {
        canFire = enable;
    }

    public void ShowRange(bool show) {
        if (rangeIndicator != null) rangeIndicator.SetActive(show);
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Add(trigger.gameObject);
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Remove(trigger.gameObject);
    }

    void ScaleCost() {
        // float newCostScaledByWaveAndLevel = GlobalData.CalculateLevelScaled(baseCost);
        cost = baseCost * GlobalData.currentWave;
        if (costText != null) costText.text = GlobalData.RemoveDotZeroZero(cost.ToString("F2"));
    }

    void UpdateTurret() {
        ScaleCost();
        if (canAim) {
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
            cooldown = 1.0f / rateOfFire;
        }
    }

    void Shoot(GameObject target) {
        if (projectile != null && barrelOfTheGun != null) {
            GameObject projectileObject = Instantiate(projectile, barrelOfTheGun.position, barrelOfTheGun.rotation);
            Projectile proj = projectileObject.GetComponent<Projectile>();
            if (proj != null) {
                float damageInRange = Random.Range(damageMin, damageMax);
                float damage = GlobalData.CalculateLevelScaled(damageInRange);
                if (shootSound != null) shootSound.Play();
                proj.Seek(target, damage, hitSound);
            }
        }
    }

    GameObject GetClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemiesInRange) {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            float distanceToFinishLineX = Mathf.Abs(enemy.transform.position.x - finishLine.position.x);
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

    public void SetTransparency(bool transparent, bool fullyOpaque = false) {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            foreach (Material mat in renderer.materials) {
                Color color = mat.color;
                if (fullyOpaque) {
                    color.a = 1.0f;
                } else {
                    color.a = transparent ? 0.35f : 0.75f;
                }
                mat.color = color;
            }
        }
        ShowRange(!fullyOpaque); // Show the range indicator only if not fully opaque
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
        ScaleCost();
        GameObject finishLineObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishLineObject != null) finishLine = finishLineObject.GetComponent<Transform>();
        CircleCollider2D finishcollider = gameObject.AddComponent<CircleCollider2D>();
        finishcollider.isTrigger = true;
        finishcollider.radius = range;

        // Create and configure the range indicator
        rangeIndicator = new GameObject("RangeIndicator");
        rangeIndicator.transform.SetParent(transform);
        rangeIndicator.transform.localPosition = Vector3.zero;
        var spriteRenderer = rangeIndicator.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = new Color(0, 1, 1, 0.5f); // Cyan with semi-transparent

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        float colliderRange = (float)collider.radius * 2f;
        rangeIndicator.transform.localScale = new Vector3(colliderRange, colliderRange, 1);
        rangeIndicator.SetActive(false);
    }
}