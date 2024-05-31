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
    [Header("Wave Settings")]
    public bool flipSprite = true;
    public bool flipInverse = false;
    public Waves waves;
    public int waveMax = 15;
    public int wavePosition = 1;
    
    [Header("Stats")]
    public float speed = GlobalData.defaultSpeed;
    public float reward = GlobalData.defaultReward;
    public GameObject goldTextAnimation;
    public GameObject goldPopupLocation;

    [Header("Health")]
    public float maxHealth = GlobalData.defaultHealth;
    public float currentHealth = GlobalData.defaultHealth;
    public HealthDisplays HealthDisplay = HealthDisplays.ShowHealthPoints;
    public GameObject health;
    public TextMeshProUGUI healthText;
    public RectTransform healthBarRect;

    [Header("Damage")]
    public float damage = GlobalData.defaultDamage;
    public TextMeshProUGUI damageText;
    public GameObject damageTextContainer;
    public GameObject damageTextAnimation;
    private GameObject[] damagePopupLocations;
    public GameObject damagePopupLocationsParent;

    private Animator animator;
    private bool isDead = false;
    private int waypointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private Transform previousTextAnimationLocation;

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

    void Update() {
        if (waves != null && waves.waypoints.Length > 0) {
            Move();
        }

        if (!isDead && currentHealth <= 0) {
            Invoke("Kill", 0.5f);
        }
    }

    void Move() {
        Vector3 targetPosition = waves.GetWaypointPosition(waypointIndex);
        Vector3 direction = targetPosition - transform.position;

        // Flip the sprite based on the direction of movement
        if (flipSprite == true) {
            if (direction.x < 0) {
                spriteRenderer.flipX = flipInverse ? true : false;
            } else if (direction.x > 0) {
                spriteRenderer.flipX = flipInverse ? false : true;
            }
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

    public void TakeDamage(float damage, bool criticalStrike) {
        TriggerHitAnimation();
        currentHealth -= damage;
        ShowDamageTextAnimation(damage, criticalStrike);
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
        StartCoroutine(SmoothTransitionToNewHealth(currentHealth - damage, true));
    }
    
    void ShowDamageTextAnimation(float damageToSet, bool criticalStrike) {
        if (damageTextAnimation != null) {
            if ((damagePopupLocations == null || damagePopupLocations.Length == 0 || damagePopupLocations[0] == null) && damagePopupLocationsParent != null) {
                List<GameObject> dmgPopupLocations = new List<GameObject>();
                foreach (Transform child in damagePopupLocationsParent.transform) {
                    dmgPopupLocations.Add(child.gameObject);
                }
                damagePopupLocations = dmgPopupLocations.ToArray();
            }

            if (damagePopupLocations != null && damagePopupLocations.Length > 0) {
                int randomIndex = -1;
                Transform randomLocation = null;

                // Ensure the new location is different from the previous one
                do {
                    randomIndex = Random.Range(0, damagePopupLocations.Length);
                    randomLocation = damagePopupLocations[randomIndex].transform;
                } while (randomLocation == previousTextAnimationLocation && damagePopupLocations.Length > 1);

                // Store the current location as the previous location for next time
                previousTextAnimationLocation = randomLocation;
                
                GameObject damageTextPopup = Instantiate(damageTextAnimation, randomLocation);

                if (damageTextPopup != null) {
                    // Set the damage text to show
                    string damageToShow = $"{GlobalData.RemoveDotZeroZero(damageToSet.ToString("F2"))}";
                    TextAnimation damageTextAnim = damageTextPopup.GetComponent<TextAnimation>();
                    if (damageTextAnim != null) {
                        damageTextAnim.textToShow = damageToShow;
                        damageTextAnim.isCriticalStrike = criticalStrike;
                    }
                }

                Invoke("ShowGoldPopup", 0.75f);
            }
        }
    }

    void ShowGoldPopup() {
        if (currentHealth <= 0 && goldPopupLocation != null) {
            ShowGoldTextAnimation(goldPopupLocation.transform);
        }
    }

    private IEnumerator ShowGoldTextAnimationDelayed(float delay, Transform location) {
        yield return new WaitForSeconds(delay);
        ShowGoldTextAnimation(location);
    }

    void ShowGoldTextAnimation(Transform location) {
        if (isDead) return;
        if (goldTextAnimation != null) {
            GameObject goldTextPopup = Instantiate(goldTextAnimation, location);

            if (goldTextPopup != null) {
                string goldToShow = $"{GlobalData.RemoveDotZeroZero(reward.ToString("F2"))}";
                GoldTextAnimation goldTextAnim = goldTextPopup.GetComponent<GoldTextAnimation>();
                if (goldTextAnim != null) {
                    goldTextAnim.textToShow = goldToShow;
                }
            }
        }
    }

    void TriggerHitAnimation() {
        animator.SetBool("Hit", true);
        Invoke("StopHitAnimation", 0.1f);
    }

    void StopHitAnimation() {
        animator.SetBool("Hit", false);
    }

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
        if (wavePosition == waveMax) GlobalData.lastEnemyInWaveDied = true;
        string enemyName = gameObject.name.Replace("(Clone)", "");
        GlobalData.Message = enemyName + " #" + wavePosition + " Killed. +" + reward + " Coins";
    }

    void RemoveDamageText() {
        if (damageTextContainer != null) damageTextContainer.SetActive(false);
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