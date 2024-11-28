using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherUnitHealth : MonoBehaviour
{
    public Slider healthSlider;    // Reference to the health slider UI
    public List<GameObject> soldiers;
    public int maxHealth = 1800;    // Maximum health of the unit
    private int currentHealth;     // The current total health of all soldiers

    public Transform healthBar;  // Reference to the health bar Canvas
    public Image bannerIcon;
    public float heightOffset = 2f;  // Offset to position health bar above the unit
    public float bannerOffset = 0.5f;
    public float bannerOppacity = 0.8f;

    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RedArcher") || child.CompareTag("BlueArcher"))
            {
                soldiers.Add(child.gameObject);
            }
        }
        currentHealth = CalculateTotalHealth();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        SetBannerOpacity(bannerOppacity);
        PositionHealthBar();
    }

    private void Update()
    {
        currentHealth = CalculateTotalHealth();

        if (healthSlider != null && healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (healthBar != null)
        {
            PositionHealthBar();
            PositionBannerIcon();
        }

        if (soldiers.Count == 0 || currentHealth <= 0)
        {
            RemoveHealthBar();

        }
    }

    private int CalculateTotalHealth()
    {
        int totalHealth = 0;

        for (int i = soldiers.Count - 1; i >= 0; i--)  
        {
            GameObject soldier = soldiers[i];

            if (soldier == null)
            {
                soldiers.RemoveAt(i);
                continue;
            }

            Archer soldierHealth = soldier.GetComponent<Archer>();
            if (soldierHealth != null)
            {
                totalHealth += soldierHealth.health;
            }
        }

        return totalHealth;
    }
    private void PositionHealthBar()
    {
        if (healthBar == null) return;

        Vector3 groupCenter = CalculateGroupCenter();

        healthBar.position = groupCenter + new Vector3(0, heightOffset, 0);
    }

    private void PositionBannerIcon()
    {
        if (bannerIcon == null || healthBar == null) return;

        bannerIcon.transform.position = healthBar.position + new Vector3(0, bannerOffset, 0);
    }
    private void SetBannerOpacity(float opacity)
    {
        if (bannerIcon != null)
        {
            Color color = bannerIcon.color;
            color.a = opacity;
            bannerIcon.color = color;
        }
    }
    private Vector3 CalculateGroupCenter()
    {
        if (soldiers.Count == 0) return transform.position;

        Vector3 center = Vector3.zero;
        foreach (GameObject soldier in soldiers)
        {
            center += soldier.transform.position;
        }
        return center / soldiers.Count;
    }
    private void RemoveHealthBar()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
    }
}
