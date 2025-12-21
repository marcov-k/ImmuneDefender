using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject background;
    public static bool paused = false;
    static bool permPause = false;
    Settings settings;
    InputActions inputs;

    void Awake()
    {
        settings = FindFirstObjectByType<Settings>();
        inputs = new();
    }

    void Start()
    {
        InitInputs();
        paused = false;
        background.SetActive(false);
        Time.timeScale = 1;
    }

    void InitInputs()
    {
        inputs.Player.Pause.performed += ctx => OnPause();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    public void OnPause()
    {
        if (!Settings.open && !permPause)
        {
            paused = !paused;
            background.SetActive(paused);
            if (paused) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        if (!Settings.open && !permPause)
        {
            paused = false;
            background.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void PermPause(bool pause)
    {
        permPause = pause;
        paused = permPause;
        if (permPause) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
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
