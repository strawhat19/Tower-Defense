using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimpleTurret : MonoBehaviour {
    private Transform target;
    public GameObject bullets;
    public GameObject rotatingGun;
    public GameObject fireAnimation;
    private ParticleSystem bulletParticles;
    private List<ParticleCollisionEvent> particleCollisionEvents;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Start() {
        if (bullets != null) {
            bulletParticles = bullets.GetComponent<ParticleSystem>();
            if (bulletParticles != null) {
                particleCollisionEvents = new List<ParticleCollisionEvent>();
            }
        }
    }

    void Update() {
        FindTarget();
        if (target != null) {
            Aim();
            // Fire();
        }
    }

    void OnParticleCollision(GameObject collidedWith) {
        ParticlePhysicsExtensions.GetCollisionEvents(bulletParticles, collidedWith, particleCollisionEvents);
    }

    public void StopFiring() {
        fireAnimation.SetActive(false);
    }

    public void Fire() {
        fireAnimation.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Add(trigger.gameObject);
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.CompareTag("Enemy")) enemiesInRange.Remove(trigger.gameObject);
    }

    private void FindTarget() {
        GameObject closestEnemy = GetClosestEnemy();
        if (closestEnemy != null) {
            target = closestEnemy.transform;
        } else {
            target = null;
            StopFiring();
        }
    }

    GameObject GetClosestEnemy() {
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemiesInRange) {
            if (enemy == null || !enemy.activeInHierarchy) continue;
            float distanceToFinishLineX = Mathf.Abs(enemy.transform.position.x - GlobalData.finishLineX);
            if (distanceToFinishLineX < shortestDistance) {
                shortestDistance = distanceToFinishLineX;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void Aim(bool useY = false) {
        if (target == null) return;
        // if (rotatingGun != null) {
        //     Vector3 direction = (target.position - transform.position).normalized;
        //     float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        //     Quaternion rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
        //     Vector3 currentEulerAngles = rotatingGun.transform.rotation.eulerAngles;
        //     rotatingGun.transform.rotation = Quaternion.Euler(currentEulerAngles.x, rotation.eulerAngles.y, currentEulerAngles.z);
        // } else {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            if (useY) {
                Vector3 currentEulerAngles = rotatingGun.transform.rotation.eulerAngles;
                Quaternion rotation = Quaternion.Euler(currentEulerAngles.x, angle, currentEulerAngles.z);
                rotatingGun.transform.rotation = rotation;
            } else {
                Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                transform.rotation = rotation;
            }
        // }
    }


}