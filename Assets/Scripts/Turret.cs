using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour {
    public float range = 5.0f; // Turret range
    public float fireRate = 1.0f; // Time between shots
    public GameObject projectilePrefab; // Projectile to shoot
    public Transform firePoint; // Point from where projectiles are fired

    private Transform target;
    [SerializeField] private LayerMask enemyMask;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private float fireCooldown = 0.0f;

    void Start() {
        // Adjust the range of the collider
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = range;
    }

    void Update() {
        if (target == null) {
            FindTarget();
            return;
        }

        RotateTowardsTarget();
        fireCooldown -= Time.deltaTime;

        // if (enemiesInRange.Count > 0 && fireCooldown <= 0f) {
        //     // Target the closest enemy
        //     GameObject target = GetClosestEnemy();
        //     if (target != null) {
        //         Shoot(target);
        //         fireCooldown = 1.0f / fireRate;
        //     }
        // }
    }

    private void FindTarget() {
        // RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2)transform.position, 0f, enemyMask);

        // if (hits.Length > 0) {
            // target = hits[0].transform;
        // } else {
            target = firePoint.transform;
        // }
    }

    private void RotateTowardsTarget() {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        transform.rotation = rotation;
    }

    void Shoot(GameObject target) {
        if (projectilePrefab != null && firePoint != null) {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null) {
                proj.Seek(target);
            }
        }
    }

    GameObject GetClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange) {
            if (enemy == null) continue; // Skip destroyed enemies
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) {
            enemiesInRange.Add(trigger.gameObject);
            bool enemy_in_range = trigger.bounds.size.magnitude < 5;
        }
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) {
            enemiesInRange.Remove(trigger.gameObject);
            bool enemy_in_range = trigger.bounds.size.magnitude < 5;
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}