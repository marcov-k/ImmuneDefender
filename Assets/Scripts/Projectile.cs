using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class Projectile : MonoBehaviour
{
    [SerializeField] string projName;
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] bool piercing;
    [SerializeField] bool area;
    [SerializeField] float areaDamageInterval;
    [SerializeField] float range;
    float distTraveled;
    readonly List<Enemy> damageEnemies = new();
    float screenTop;

    void Start()
    {
        InitValues();
        if (area)
        {
            StartCoroutine(AreaDamageCoroutine());
        }
    }

    void Update()
    {
        Move();
    }

    void InitValues()
    {
        var renderer = GetComponent<SpriteRenderer>();
        screenTop = Camera.main.ScreenToWorldPoint(new(0, Screen.height)).y + renderer.bounds.extents.y;
    }

    void Move()
    {
        float move = speed * Time.deltaTime;
        transform.position += move * transform.up;
        distTraveled += move;
        if (distTraveled >= range || transform.position.y >= screenTop)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (area)
            {
                damageEnemies.Add(enemy);
            }
            else if (piercing)
            {
                enemy.ApplyCytokine();
            }
            else
            {
                enemy.TakeDamage(damage, projName);
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (area && other.TryGetComponent<Enemy>(out var enemy))
        {
            damageEnemies.Remove(enemy);
        }
    }

    IEnumerator AreaDamageCoroutine()
    {
        while (true)
        {
            foreach (var enemy in damageEnemies.ToList())
            {
                enemy.TakeDamage(damage, projName);
                if (enemy.gameObject.IsDestroyed())
                {
                    damageEnemies.Remove(enemy);
                }
            }
            yield return new WaitForSeconds(areaDamageInterval);
        }
    }
}
