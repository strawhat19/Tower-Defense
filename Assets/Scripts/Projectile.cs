using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
    public float speed = 10f;
    private GameObject target;

    public void Seek(GameObject target) {
        this.target = target;
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
    }

    void HitTarget() {
        // Logic for what happens when the projectile hits the target
        Destroy(gameObject); // Destroy the projectile
        Destroy(target); // Destroy the target
    }
}