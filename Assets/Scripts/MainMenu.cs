using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Settings settings;
    [SerializeField] Overlay controlsOverlay;

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

    public void ShowControls()
    {
        controlsOverlay.Show();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
