using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;
    float maxHealth;

    public void InitHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        fill.fillAmount = 1.0f;
    }

    public void UpdateHealth(float health)
    {
        health = Mathf.Max(health, 0);
        float healthFrac = health / maxHealth;
        fill.fillAmount = healthFrac;
    }
}
