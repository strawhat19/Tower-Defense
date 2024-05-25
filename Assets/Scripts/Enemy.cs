using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum HealthDisplays {
    ShowPercentages = 1,
    ShowHealthPoints = 2,
}

public class Enemy : MonoBehaviour {
    public int waveMax = 15;
    public int wavePosition = 1;
    public float speed = GlobalData.defaultSpeed;
    public float damage = GlobalData.defaultDamage;
    public float reward = GlobalData.defaultReward;
    public float maxHealth = GlobalData.defaultHealth;
    public float currentHealth = GlobalData.defaultHealth;
    public GameObject health;
    public TextMeshProUGUI healthText;
    public GameObject damageTextContainer;
    public TextMeshProUGUI damageText;
    public RectTransform healthBarRect;
    public HealthDisplays HealthDisplay = HealthDisplays.ShowHealthPoints;

    public Waves waves;
    private Animator animator;
    private bool isDead = false;
    private int waypointIndex = 0;
    private SpriteRenderer spriteRenderer;

// [`bat`,`mushroom`, `ghost`, `rocks`, `skulls`, `slime`, `turtle`]
    void Die() {
        if (wavePosition == waveMax) {
            // Debug.Log("Last Enemy #" + wavePosition + " In Wave Died");
            GlobalData.lastEnemyInWaveDied = true;
        }
        Destroy(gameObject);
    }

    void AddCoins() {
        GlobalData.startCoins = GlobalData.startCoins + reward;
    }

    private IEnumerator KillAnimation() {
        AddCoins();
        currentHealth = 0;
        animator.SetBool("Dead", true);

        // Wait for the death animation to complete
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        Destroy(gameObject);
    }

    public void StartKill() {
        StartCoroutine(KillAnimation());
    }

    void ScaleUp() {
        if (isDead) transform.localScale = new Vector3(1f, 1f, 1f) * 3f;
    }

    void Kill() {
        if (isDead) return;
        isDead = true;
        if (health != null) health.SetActive(false);
        animator.SetBool("Dead", true);
        currentHealth = 0;
        AddCoins();
        Invoke("ScaleUp", 0.375f);
        Invoke("Die", 0.55f);
        if (wavePosition == waveMax) {
            // Debug.Log("Last Enemy #" + wavePosition + " In Wave Killed");
            GlobalData.lastEnemyInWaveDied = true;
        }
    }

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        RemoveDamageText();
        if (healthText != null) {
            if (HealthDisplay == HealthDisplays.ShowPercentages) {
                healthText.text = $"Health: 100%";
            } else {
                healthText.text = $"{currentHealth}";
            }
        }
    }

    void RemoveDamageText() {
        // damageText.SetActive(false);
        if (damageTextContainer != null) damageTextContainer.SetActive(false);
    }
    
    public void TakeDamage(float damage) {
        // Debug.Log(currentHealth + " - " + damage + " Dmg = " + (currentHealth - damage));
        TriggerHitAnimation();
        currentHealth -= damage;
        if (damageText != null) {
            damageText.text = $"- {GlobalData.RemoveDotZeroZero(damage.ToString("F2"))}";
            if (damageTextContainer != null) damageTextContainer.SetActive(true);
            Invoke("RemoveDamageText", 0.5f);
            // float damagePosX = Random.Range(-0.75f, 0.75f);
            // float damagePosY = Random.Range(-0.5f, 0.5f);
            // GameObject damageMarkerInstance = Instantiate(damageMarkerPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            // damageMarkerInstance.SetActive(true);
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
        StartCoroutine(SmoothTransitionToNewHealth(currentHealth - damage, true));
    }

    void Update() {
        if (waves != null && waves.waypoints.Length > 0) {
            Move();
        }

        if (!isDead && currentHealth <= 0) {
            Invoke("Kill", 0.5f);
        }
    }

    void TriggerHitAnimation() {
        animator.SetBool("Hit", true);
        Invoke("StopHitAnimation", 0.1f);
    }

    void StopHitAnimation() {
        animator.SetBool("Hit", false);
    }

    void Move() {
        Vector3 targetPosition = waves.GetWaypointPosition(waypointIndex);
        Vector3 direction = targetPosition - transform.position;

        // Flip the sprite based on the direction of movement
        if (direction.x < 0) {
            spriteRenderer.flipX = false;
        } else if (direction.x > 0) {
            spriteRenderer.flipX = true;
        }

        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Check if the enemy is close to the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
            waypointIndex++;
            int amountOfWaypoints = waves.waypoints.Length;
            if (waypointIndex >= amountOfWaypoints) {
                // Reached the final waypoint, destroy the enemy or handle end of path
                GlobalData.startLives = GlobalData.startLives - damage;
                Die();
            }
        }
    }

    IEnumerator SmoothTransitionToNewHealth(float newHealth, bool damage) {
        float timeToChange = 0.5f; // The duration of the change
        float elapsed = 0f;
        
        float currentWidth = healthBarRect.sizeDelta.x;
        float targetWidth = (float)currentHealth / maxHealth; // Calculate new width based on current health
        
        while (elapsed < timeToChange) {
            elapsed += Time.deltaTime;
            float updatedHealth = Mathf.Lerp(currentWidth, targetWidth, elapsed / timeToChange);
            healthBarRect.sizeDelta = new Vector2(updatedHealth, healthBarRect.sizeDelta.y);
            string updatedHealthString = GlobalData.RemoveDotZeroZero(currentHealth.ToString("F2"));
            string updatedHealthPercentageString = GlobalData.RemoveDotZeroZero((updatedHealth * 100).ToString("F2"));
            if (healthText != null) {
                if (HealthDisplay == HealthDisplays.ShowPercentages) {
                    healthText.text = $"Health: {updatedHealthPercentageString}%";
                } else {
                    healthText.text = $"{updatedHealthString}";
                    // if (currentHealth > 999) {
                    //     healthText.text = $"{updatedHealthString} / {maxHealth}";
                    // } else {
                    //     healthText.text = $"HP: {updatedHealthString} / {maxHealth}";
                    // }
                }
            }
            yield return null;
        }
        
        healthBarRect.sizeDelta = new Vector2(targetWidth, healthBarRect.sizeDelta.y);
    }
}