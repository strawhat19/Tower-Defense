using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
    public float dmg = 5f;
    public float speed = 10f;
    private GameObject target;
    private Enemy targetSettings;

    public void Seek(GameObject target, float damage) {
        this.target = target;
        dmg = damage; 
        // AdjustRotation(); // Adjust the rotation when the target is set
    }

    void Update() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame) {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        
        // Adjust rotation during movement
        // AdjustRotation();
    }

    void HitTarget() {
        Enemy enemySettings = target.GetComponent<Enemy>();
        // Logic for what happens when the projectile hits the target
        Destroy(gameObject); // Destroy the projectile
        if (enemySettings != null) {
            enemySettings.TakeDamage(dmg);
            // enemySettings.TriggerHitAnimation();
        }
    }

    private void AdjustRotation() {
        if (target != null) {
            Vector3 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (angle * 2.0f)));
        }
    }
}