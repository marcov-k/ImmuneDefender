using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    Vector2 move;
    public float3 padding = new(0.1f, 0.8f, 0.1f); // left/right, top, bottom
    float4 bounds; // left, top, bottom, right
    float2 dims;
    public float health = 20.0f;
    LevelManager manager;
    [SerializeField] Shooter[] shooters;
    int defences;
    int selectedDef;
    Shooter[] availableDefs;
    DefIndicator defenceIndicator;
    HealthBar healthBar;
    InputActions inputs;
    Animator animator;
    ShakeSystem shakeSystem;
    AudioSource hitSound;

    void Awake()
    {
        inputs = new();
        animator = GetComponent<Animator>();
        hitSound = GetComponent<AudioSource>();
    }

    void Start()
    {
        InitValues();
        InitInputs();
        StartCoroutine(InitDefenceIndicator());
    }

    void Update()
    {
        Move();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void InitValues()
    {
        shakeSystem = gameObject.AddComponent<ShakeSystem>();
        manager = FindFirstObjectByType<LevelManager>();
        defenceIndicator = FindFirstObjectByType<DefIndicator>();
        healthBar = FindFirstObjectByType<HealthBar>();

        float3 margins = new(padding.x * Screen.width, padding.y * Screen.height, padding.z * Screen.height);
        float4 edges = new(margins.x, Screen.height - margins.y, margins.z, Screen.width - margins.x); // left, top, bottom, right
        bounds = new(Camera.main.ScreenToWorldPoint(new(edges.x, 0)).x, Camera.main.ScreenToWorldPoint(new(0, edges.y)).y,
            Camera.main.ScreenToWorldPoint(new(0, edges.z)).y, Camera.main.ScreenToWorldPoint(new(edges.w, 0)).x);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        dims = new(renderer.bounds.extents.x, renderer.bounds.extents.y);

        shooters[0].active = true;
        List<Sprite> icons = new() { shooters[0].data.icon };
        for (int i = 1; i < shooters.Length; i++)
        {
            shooters[i].active = false;
            icons.Add(shooters[i].data.icon);
        }
        defences = manager.GetDefencesUnlocked() - 1;
        selectedDef = 0;
        availableDefs = new Shooter[defences + 1];
        for (int i = 0; i < availableDefs.Length; i++)
        {
            availableDefs[i] = shooters[i];
        }
        defenceIndicator.InitIndicator(defences, icons);
        healthBar.InitHealth(health);
    }

    void InitInputs()
    {
        inputs.Player.ChangeDef.performed += ctx => OnChangeDef(ctx);
    }

    void Move()
    {
        move = inputs.Player.Move.ReadValue<Vector2>();
        transform.position = new(transform.position.x + move.x * moveSpeed * Time.deltaTime, transform.position.y + move.y * moveSpeed * Time.deltaTime);
        if (transform.position.y + dims.y > bounds.y)
        {
            transform.position = new(transform.position.x, bounds.y - dims.y);
        }
        else if (transform.position.y - dims.y < bounds.z)
        {
            transform.position = new(transform.position.x, bounds.z + dims.y);
        }
        if (transform.position.x - dims.x < bounds.x)
        {
            transform.position = new(bounds.x + dims.x, transform.position.y);
        }
        else if (transform.position.x + dims.x > bounds.w)
        {
            transform.position = new(bounds.w - dims.x, transform.position.y);
        }
    }

    void UpdateDefence(float input)
    {
        if (input != 0)
        {
            availableDefs[selectedDef].active = false;
            if (input < 0)
            {
                if (selectedDef < defences) selectedDef++;
                else selectedDef = 0;
            }
            else if (input > 0)
            {
                if (selectedDef > 0) selectedDef--;
                else selectedDef = defences;
            }
            availableDefs[selectedDef].active = true;
            UpdateDefenceIndicator();
        }
    }

    IEnumerator InitDefenceIndicator()
    {
        yield return new WaitForEndOfFrame();
        UpdateDefenceIndicator();
    }

    void UpdateDefenceIndicator()
    {
        defenceIndicator.UpdateIndicator(selectedDef);
    }

    public void OnChangeDef(InputAction.CallbackContext input)
    {
        UpdateDefence(input.ReadValue<float>());
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        shakeSystem.Shake(damage);
        healthBar.UpdateHealth(health);
        hitSound.Play();
        if (health <= 0)
        {
            manager.PlayerKilled();
            Destroy(gameObject);
        }
    }

    public void Shoot()
    {
        animator.SetTrigger("Shoot");
    }
}
