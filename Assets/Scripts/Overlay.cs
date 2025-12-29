using UnityEngine;
using UnityEngine.InputSystem;

public class Overlay : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    public bool open = false;
    InputActions inputs;

    void Awake()
    {
        inputs = new();
    }

    void Start()
    {
        inputs.Player.Pause.performed += ctx => OnPause();
        open = false;
        overlay.SetActive(false);
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    public void Show()
    {
        open = true;
        overlay.SetActive(true);
    }

    public void Hide()
    {
        open = false;
        overlay.SetActive(false);
    }

    void OnPause()
    {
        if (open) Hide();
    }
}
