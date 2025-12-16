using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] List<Button> levelButtons = new();
    [SerializeField] Color completeColor;
    [SerializeField] Color unlockedColor;
    [SerializeField] Color lockedColor;

    void Start()
    {
        InitButtons();
    }

    void InitButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (i < PlayerData.lastLevel)
            {
                levelButtons[i].GetComponent<Image>().color = completeColor;
                levelButtons[i].interactable = true;
            }
            else if (i == PlayerData.lastLevel)
            {
                levelButtons[i].GetComponent<Image>().color = unlockedColor;
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].GetComponent<Image>().color = lockedColor;
                levelButtons[i].interactable = false;
            }
        }
    }

    public void LoadLevel(int index)
    {
        PlayerData.startedLevel = index;
        SceneManager.LoadScene("Loading");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
