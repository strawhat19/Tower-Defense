using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimpleTurret : MonoBehaviour {
    private Transform target;
    public GameObject shells;
    public GameObject bullets;
    public GameObject rotatingGun;
    public GameObject bulletImpact;
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
        }
    }

    void OnParticleCollision(GameObject collidedWith) {
        ParticleSystem bulletImpactParticles = bulletImpact.GetComponent<ParticleSystem>();
        ParticlePhysicsExtensions.GetCollisionEvents(bulletImpactParticles, collidedWith, particleCollisionEvents);
        ParticlePhysicsExtensions.GetCollisionEvents(bulletParticles, collidedWith, particleCollisionEvents);
        Debug.Log("Collided with " + collidedWith.name);
    }

    public void StopFiring() {
        if (fireAnimation != null) fireAnimation.SetActive(false);
    }

    public void Fire() {
        if (fireAnimation != null) fireAnimation.SetActive(true);
        bulletParticles.Emit(1);
        fireAnimation.GetComponent<ParticleSystem>().Emit(1);
        if (shells != null) shells.GetComponent<ParticleSystem>().Emit(1);
        if (bulletImpact != null) bulletImpact.GetComponent<ParticleSystem>().Emit(1);
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
            Fire();
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

    private void OnDrawGizmos() {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null) {
            Handles.color = Color.cyan;
            float colliderRange = (float)collider.radius;
            Handles.DrawWireDisc(transform.position, Vector3.forward, colliderRange);
        }
    }
}