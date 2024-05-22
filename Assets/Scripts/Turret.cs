using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour {
    private Transform target;
    private float fireCooldown = 0.0f;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private float range = 1.0f;
    public float cost = 100.0f;
    public float rateOfFire = 1.0f;
    public float damageMin = 15.0f;
    public float damageMax = 25.0f;

    private Transform finishLine;
    public GameObject projectile;
    public AudioSource shootSound;
    public Transform barrelOfTheGun;

    void Start() {
        GameObject finishLineObject = GameObject.FindGameObjectWithTag("Finish");
        if (finishLineObject != null) finishLine = finishLineObject.GetComponent<Transform>();
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = range;
    }

    void Update() {
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
        // Adjusted to face the correct direction
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
                proj.Seek(target, damage);
            }
        }
    }

    GameObject GetClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            // Skip destroyed or inactive enemies
            if (enemy == null || !enemy.activeInHierarchy) continue;
            float distanceToFinishLine = Vector2.Distance(enemy.transform.position, finishLine.position);
            if (distanceToFinishLine < shortestDistance) {
                shortestDistance = distanceToFinishLine;
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
}