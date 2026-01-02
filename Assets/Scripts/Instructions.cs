using UnityEngine;
using static PlayerData;

public class Instructions : MonoBehaviour
{
    public GameObject instructCont;
    InputActions inputs;
    bool open = true;

    void Awake()
    {
        inputs = new();
    }

    void Start()
    {
        InitOverlay();
        InitInputs();
    }

    void InitOverlay()
    {
        if (!instructionsSeen)
        {
            open = true;
            instructCont.SetActive(true);
            instructionsSeen = true;
        }
        else
        {
            open = false;
            instructCont.SetActive(false);
        }
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

    void OnPause()
    {
        if (open) Close();
    }

    public void Close()
    {
        open = false;
        instructCont.SetActive(false); 
    }

    public void Open()
    {
        open = true;
        instructCont.SetActive(true);
    }
}
