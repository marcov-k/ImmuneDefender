using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] Color winTextColor;
    [SerializeField] Color lossTextColor;
    Color starColor;
    [SerializeField] List<GameObject> stars = new();
    [SerializeField] float starDelay;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject scoreIcon;
    [SerializeField] GameObject background;

    void Start()
    {
        SetupScreen();
    }

    void SetupScreen()
    {
        winText.text = string.Empty;
        Image image;
        foreach (var star in stars)
        {
            image = star.GetComponent<Image>();
            starColor = image.color;
            image.color = new(0, 0, 0, 0);
        }
        scoreIcon.SetActive(false);
        scoreText.text = string.Empty;
        background.SetActive(false);
    }

    public void ShowResults(bool won, int starCount, float score, float maxScore)
    {
        background.SetActive(true);
        switch (won)
        {
            case true:
                winText.text = "Victory!";
                winText.color = winTextColor;
                break;
            case false:
                winText.text = "Defeat!";
                winText.color = lossTextColor;
                break;
        }
        StartCoroutine(ResultsCoroutine(starCount, score, maxScore));
    }

    IEnumerator ResultsCoroutine(int starCount, float score, float maxScore)
    {
        for (int i = 0; i < starCount; i++)
        {
            yield return new WaitForSecondsRealtime(starDelay);
            stars[i].GetComponent<Image>().color = starColor;
        }
        yield return new WaitForSecondsRealtime(starDelay);
        scoreIcon.SetActive(true);
        scoreText.text = $"{score} / {maxScore}";
    }
}
