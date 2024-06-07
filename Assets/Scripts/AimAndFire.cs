using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AimAndFire : MonoBehaviour {
    public bool canFire = false;
    private Transform target;
    public GameObject shells;
    public GameObject bullets;
    public AudioSource fireSound;
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
        enemiesInRange.RemoveAll(enemy => enemy == null || enemy.GetComponent<Enemy>().currentHealth <= 0);
        FindTarget();
        if (target != null) {
            Aim();
        }
    }

    // Not working right now 
    void OnParticleCollision(GameObject collidedWith) {
        // ParticleSystem bulletImpactParticles = bulletImpact.GetComponent<ParticleSystem>();
        // ParticlePhysicsExtensions.GetCollisionEvents(bulletImpactParticles, collidedWith, particleCollisionEvents);
        int evts = bulletParticles.GetCollisionEvents(collidedWith, particleCollisionEvents);

        for (int i = 0; i < evts; i++) {
            var objectCollidedWith = particleCollisionEvents[i].colliderComponent;
            if (objectCollidedWith.CompareTag("Enemy")) {
                Enemy enemySettings = objectCollidedWith.GetComponent<Enemy>();
                if (enemySettings != null) enemySettings.TakeDamage(0f, false);
            }   
        }

        Debug.Log("Collided with " + collidedWith.name);
    }
    
    void OnCollisionEnter2D(Collision2D collidedWith) {
        // ParticleSystem bulletImpactParticles = bulletImpact.GetComponent<ParticleSystem>();
        // ParticlePhysicsExtensions.GetCollisionEvents(bulletImpactParticles, collidedWith, particleCollisionEvents);
        // int evts = bulletParticles.GetCollisionEvents(collidedWith, particleCollisionEvents);

        // for (int i = 0; i < evts; i++) {
        //     var objectCollidedWith = particleCollisionEvents[i].colliderComponent;
        //     if (objectCollidedWith.CompareTag("Enemy")) {
        //         Enemy enemySettings = objectCollidedWith.GetComponent<Enemy>();
        //         if (enemySettings != null) enemySettings.TakeDamage(0f, false);
        //     }   
        // }

        Debug.Log("Collided with " + collidedWith.gameObject.name);
    }

    void OnParticleCollision2D(GameObject collidedWith) {
        // ParticleSystem bulletImpactParticles = bulletImpact.GetComponent<ParticleSystem>();
        // ParticlePhysicsExtensions.GetCollisionEvents(bulletImpactParticles, collidedWith, particleCollisionEvents);
        int evts = bulletParticles.GetCollisionEvents(collidedWith, particleCollisionEvents);

        for (int i = 0; i < evts; i++) {
            var objectCollidedWith = particleCollisionEvents[i].colliderComponent;
            if (objectCollidedWith.CompareTag("Enemy")) {
                Enemy enemySettings = objectCollidedWith.GetComponent<Enemy>();
                if (enemySettings != null) enemySettings.TakeDamage(0f, false);
            }   
        }

        Debug.Log("Collided with " + collidedWith.name);
    }

    public void StopFiring() {
        if (fireSound != null) fireSound.Stop();
        if (fireAnimation != null) fireAnimation.SetActive(false);
    }

    void PlayFireSound() {
        if (fireSound != null) fireSound.Play();
    }

    public void Fire() {
        if (canFire) {
            if (fireAnimation != null) fireAnimation.SetActive(true);
            PlayFireSound();
            bulletParticles.Emit(1);
            fireAnimation.GetComponent<ParticleSystem>().Emit(1);
            if (shells != null) shells.GetComponent<ParticleSystem>().Emit(1);
            if (bulletImpact != null) bulletImpact.GetComponent<ParticleSystem>().Emit(1);
        }
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