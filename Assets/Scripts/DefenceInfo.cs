using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DefenceInfo : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI strengthText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI shotText;
    [SerializeField] TextMeshProUGUI firerateText;

    public void ShowDefence(DefenceData defence)
    {
        icon.sprite = defence.icon;
        nameText.text = defence.defenceName;
        descText.text = defence.description;

        var strengths = "Strong against: ";
        for (int i = 0; i < defence.strengths.Count; i++)
        {
            if (i != 0) strengths += ", ";
            strengths += defence.strengths[i];
        }
        strengthText.text = strengths;

        damageText.text = $"Damage: {defence.damage}";
        speedText.text = $"Speed: {defence.speed}";
        shotText.text = $"Projectiles: {defence.shotCount}";
        firerateText.text = $"Firerate: {defence.firerate}";
    }
}
