using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shield : MonoBehaviour {
    public bool activated;
    public int health;
    public float damageCooldown;

    public Image progressImage;
    public Button button;

    protected float cooldownTimer;
    protected int currentHealth;

    void Update() {
        if (cooldownTimer > 0) {
            if (progressImage != null) {
                progressImage.fillAmount = (damageCooldown - cooldownTimer) / damageCooldown;
            }
            cooldownTimer -= Time.deltaTime;
            if (button != null) {
                button.interactable = false;
            }
        } else if (progressImage != null && progressImage.fillAmount != 0) {
            progressImage.fillAmount = 0;
            if (button != null) {
                button.interactable = true;
            }
            currentHealth = health;
        }
    }

    public void Toggle() {
        if (cooldownTimer > 0) return;
        activated = !activated;
        renderer.enabled = !renderer.enabled;
        collider.enabled = !collider.enabled;
    }

    public void Enable() {
        if (cooldownTimer > 0) return;
        activated = true;
        renderer.enabled = true;
        collider.enabled = true;
    }

    public void Disable() {
        activated = false;
        renderer.enabled = false;
        collider.enabled = false;
    }

    public void MissileHit() {
        if (cooldownTimer > 0) return;
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Disable();
            cooldownTimer = damageCooldown;
        }
    }
}