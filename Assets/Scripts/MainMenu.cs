using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Settings settings;

    void Start()
    {
        settings = FindFirstObjectByType<Settings>();
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void ShowSettings()
    {
        settings.ShowSettings();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
