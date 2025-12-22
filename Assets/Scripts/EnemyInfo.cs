using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI speedText;

    public void ShowEnemy(EnemyData enemy)
    {
        icon.sprite = enemy.icon;
        nameText.text = enemy.invaderName;
        descText.text = enemy.description;
        typeText.text = $"Type: {enemy.type}";
        damageText.text = $"Damage: {enemy.damage}";
        healthText.text = $"Health: {enemy.health}";
        speedText.text = $"Speed: {enemy.speed}";
    }
}
