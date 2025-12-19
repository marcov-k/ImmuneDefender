using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefIndicator : MonoBehaviour
{
    [SerializeField] Image[] icons;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] RectTransform indicator;
    [SerializeField] float indicatorOffset;
    [SerializeField] float fadeOpacity = 0.5f;
    int defence;

    public void InitIndicator(int totalDefs)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i <= totalDefs)
            {
                icons[i].color = new(icons[i].color.r, icons[i].color.g, icons[i].color.b, fadeOpacity);
                texts[i].color = new(texts[i].color.r, texts[i].color.g, texts[i].color.b, fadeOpacity);
            }
            else
            {
                icons[i].color = new(icons[i].color.r, icons[i].color.g, icons[i].color.b, 0);
                texts[i].color = new(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0);
            }
        }
    }

    public void UpdateIndicator(int newDefence)
    {
        icons[defence].color = new(icons[defence].color.r, icons[defence].color.g, icons[defence].color.b, fadeOpacity);
        texts[defence].color = new(texts[defence].color.r, texts[defence].color.g, texts[defence].color.b, fadeOpacity);

        defence = newDefence;

        icons[defence].color = new(icons[defence].color.r, icons[defence].color.g, icons[defence].color.b, 1);
        texts[defence].color = new(texts[defence].color.r, texts[defence].color.g, texts[defence].color.b, 1);

        float offset = indicatorOffset * Screen.width;
        Vector2 indPos = new(icons[defence].rectTransform.position.x + offset, icons[defence].rectTransform.position.y);
        indicator.position = indPos;
    }
}
