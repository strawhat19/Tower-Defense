using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
    private GameObject target;
    private AudioSource hitSound;
    private float damageToDeal = 5f;
    private bool criticalStrike = false;
    private float speedOfProjectile = 10f;

    private Turret shootingTurret;
    // private ParticleSystem particleSystem;
    // private bool triggerCollision = false;

    public void Seek(GameObject target, float damage, bool isCriticalStrike, AudioSource hitSnd, Turret shooter) {
        this.target = target;
        damageToDeal = damage;
        hitSound = hitSnd;
        criticalStrike = isCriticalStrike;
        shootingTurret = shooter; 
    }

    void PlayHitSound() {
        if (hitSound != null) hitSound.Play();
    }

    void Update() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = speedOfProjectile * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame) {
            PlayHitSound();
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void TriggerParticleCollisionEvt() {
        if (shootingTurret != null && shootingTurret.aimAndFireObject != null) {
            ParticleSystem[] particleSystems = shootingTurret.aimAndFireObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems) {
                ps.Stop();
                // ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
                // int numParticlesAlive = ps.GetParticles(particles);

                // for (int i = 0; i < numParticlesAlive; i++) {
                //     particles[i].remainingLifetime = 0.01f;
                // }

                // ps.SetParticles(particles, numParticlesAlive);
            }
        }
    }

    void HitTarget() {
        Enemy enemySettings = target.GetComponent<Enemy>();
        if (enemySettings != null) enemySettings.TakeDamage(damageToDeal, criticalStrike);
        TriggerParticleCollisionEvt();
        Destroy(gameObject); // Destroy the projectile
    }

    private void AdjustRotation() {
        if (target != null) {
            Vector3 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (angle * 2.0f)));
        }
    }
}