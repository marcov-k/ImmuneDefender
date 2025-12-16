using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject background;
    public static bool paused = false;
    Settings settings;

    void Awake()
    {
        settings = FindFirstObjectByType<Settings>();
    }

    void Start()
    {
        paused = false;
        background.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnPause()
    {
        if (!Settings.open)
        {
            paused = !paused;
            background.SetActive(paused);
            if (paused) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        if (!Settings.open)
        {
            paused = false;
            background.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowSettings()
    {
        settings.ShowSettings();
    }
}
