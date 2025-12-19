using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
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

    void Start()
    {
        InitValues();
    }

    void Update()
    {
        Move();
    }

    void InitValues()
    {
        manager = FindFirstObjectByType<LevelManager>();
        float3 margins = new(padding.x * Screen.width, padding.y * Screen.height, padding.z * Screen.height);
        float4 edges = new(margins.x, Screen.height - margins.y, margins.z, Screen.width - margins.x); // left, top, bottom, right
        bounds = new(Camera.main.ScreenToWorldPoint(new(edges.x, 0)).x, Camera.main.ScreenToWorldPoint(new(0, edges.y)).y,
            Camera.main.ScreenToWorldPoint(new(0, edges.z)).y, Camera.main.ScreenToWorldPoint(new(edges.w, 0)).x);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        dims = new(renderer.bounds.extents.x, renderer.bounds.extents.y);

        shooters[0].active = true;
        for (int i = 1; i < shooters.Length; i++)
        {
            shooters[i].active = false;
        }
        defences = manager.GetDefencesUnlocked();
    }

    void Move()
    {
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

    public void OnMove(InputValue input)
    {
        move = input.Get<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            manager.PlayerKilled();
            Destroy(gameObject);
        }
    }
}
