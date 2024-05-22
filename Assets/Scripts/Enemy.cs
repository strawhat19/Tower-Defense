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
    private Animator animator;
    private int waypointIndex = 0;
    private SpriteRenderer spriteRenderer;

    public float speed = GlobalData.defaultSpeed;
    public float damage = GlobalData.defaultDamage;
    public float reward = GlobalData.defaultReward;
    public float maxHealth = GlobalData.defaultHealth;
    public float currentHealth = GlobalData.defaultHealth;
    public TextMeshProUGUI healthText;
    public GameObject damageTextContainer;
    public TextMeshProUGUI damageText;
    public RectTransform healthBarRect;
    [SerializeField] private Waypoints waypoints;
    public HealthDisplays HealthDisplay = HealthDisplays.ShowHealthPoints;

    void Die() {
        Destroy(gameObject);
    }

    void AddCoins() {
        GlobalData.startCoins = GlobalData.startCoins + reward;
    }

    void Kill() {
        currentHealth = 0;
        Destroy(gameObject);
        AddCoins();
    }

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        RemoveDamageText();
        if (waypoints == null) waypoints = GameObject.FindObjectOfType<Waypoints>();
        if (healthText != null) {
            if (HealthDisplay == HealthDisplays.ShowPercentages) {
                healthText.text = $"Health: 100%";
            } else {
                healthText.text = $"{currentHealth}";
                // healthText.text = $"HP: {currentHealth} / {maxHealth}";
            }
        }
    }

    void RemoveDamageText() {
        // damageText.SetActive(false);
        if (damageTextContainer != null) damageTextContainer.SetActive(false);
    }
    
    public void TakeDamage(float damage) {
        // Debug.Log(currentHealth + " - " + damage + " Dmg = " + (currentHealth - damage));
        currentHealth -= damage;
        if (damageText != null) {
            damageText.text = $"- {damage.ToString()}";
            if (damageTextContainer != null) damageTextContainer.SetActive(true);
            Invoke("RemoveDamageText", 0.3f);
            // float damagePosX = Random.Range(-0.75f, 0.75f);
            // float damagePosY = Random.Range(-0.5f, 0.5f);
            // GameObject damageMarkerInstance = Instantiate(damageMarkerPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            // damageMarkerInstance.SetActive(true);
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
        StartCoroutine(SmoothTransitionToNewHealth(currentHealth - damage, true));
    }

    void Update() {
        if (waypoints != null && waypoints.Points.Length > 0) {
            Move();
        }

        SimulateTakingDamage();

        if (currentHealth <= 0) {
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

    void SimulateTakingDamage() {
        if (Input.GetButtonDown("Fire1")) {
            TriggerHitAnimation();
            if (healthBarRect != null) {
                float damage = Random.Range(5f, 15f);
                damage = Mathf.Round(damage * 100f) / 100f;
                TakeDamage(damage);
            }
        }
    }

    void Move() {
        Vector3 targetPosition = waypoints.GetWaypointPosition(waypointIndex);
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
            if (waypointIndex >= waypoints.Points.Length) {
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