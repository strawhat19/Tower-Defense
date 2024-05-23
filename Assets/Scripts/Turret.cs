using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour {
    private Transform target;
    private Transform finishLine;
    private GameObject rangeIndicator;
    private float fireCooldown = 0.0f;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private float range = 1.0f;
    public bool canFire = true;
    public float cost = 100.0f;
    public float rateOfFire = 1.0f;
    public float damageMin = 5.0f;
    public float damageMax = 15.0f;

    public GameObject projectile;
    public AudioSource shootSound;
    public AudioSource hitSound;
    public Transform barrelOfTheGun;

    void Start() {
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

    void Update() {
        if (canFire) {
            FindTarget();

            if (target != null) {
                RotateTowardsTarget();
                fireCooldown -= Time.deltaTime;

                if (fireCooldown <= 0f) {
                    Shoot(target.gameObject);
                    fireCooldown = 1.0f / rateOfFire;
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

    void Shoot(GameObject target) {
        if (projectile != null && barrelOfTheGun != null) {
            GameObject projectileObject = Instantiate(projectile, barrelOfTheGun.position, barrelOfTheGun.rotation);
            Projectile proj = projectileObject.GetComponent<Projectile>();
            if (proj != null) {
                float damage = Random.Range(damageMin, damageMax);
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

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) {
            enemiesInRange.Add(trigger.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) {
            enemiesInRange.Remove(trigger.gameObject);
        }
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

        // Show the range indicator only if not fully opaque
        ShowRange(!fullyOpaque);
    }

    public void ShowRange(bool show) {
        if (rangeIndicator != null) {
            rangeIndicator.SetActive(show);
        }
    }

    public void EnableFiring(bool enable) {
        canFire = enable;
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
}