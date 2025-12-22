using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class Projectile : MonoBehaviour
{
    float speed;
    float damage;
    bool piercing;
    bool area;
    float areaDamageInterval;
    float range;
    DefenceData data;
    float distTraveled;
    readonly List<Enemy> damageEnemies = new();
    float screenTop;

    void Update()
    {
        Move();
    }

    void InitValues()
    {
        var renderer = GetComponent<SpriteRenderer>();
        screenTop = Camera.main.ScreenToWorldPoint(new(0, Screen.height)).y + renderer.bounds.extents.y;
        speed = data.speed;
        damage = data.damage;
        piercing = data.piercing;
        area = data.area;
        areaDamageInterval = data.areaDamageInterval;
        range = data.range;
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
                enemy.TakeDamage(damage, data);
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
                enemy.TakeDamage(damage, data);
                if (enemy.gameObject.IsDestroyed())
                {
                    damageEnemies.Remove(enemy);
                }
            }
            yield return new WaitForSeconds(areaDamageInterval);
        }
    }

    public void SetData(DefenceData newData)
    {
        data = newData;
        InitValues();
        if (area)
        {
            StartCoroutine(AreaDamageCoroutine());
        }
    }
}
