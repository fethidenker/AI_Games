using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour
{
    public List<GameObject> soldiers;
    public int maxHealth = 1800;    
    private int currentHealth;

    public Slider healthSlider;
    public Transform healthBar;  
    public Image bannerIcon;
    public float heightOffset = 2f;  
    public float bannerOffset = 0.5f;
    public float bannerOppacity = 0.8f;

    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        soldiers = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("RedSoldier") || child.CompareTag("BlueSoldier"))
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

        // Iterate over all soldiers, skipping any that have been destroyed
        for (int i = soldiers.Count - 1; i >= 0; i--)  // Loop backward to safely remove elements
        {
            GameObject soldier = soldiers[i];

            // If soldier is destroyed, remove it from the list
            if (soldier == null)
            {
                soldiers.RemoveAt(i);
                continue;
            }

            SoldierHealth soldierHealth = soldier.GetComponent<SoldierHealth>();
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
