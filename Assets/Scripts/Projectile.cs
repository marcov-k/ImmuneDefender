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

    void Start()
    {
        if (area)
        {
            StartCoroutine(AreaDamageCoroutine());
        }
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position += speed * Time.deltaTime * transform.up;
        distTraveled += speed * Time.deltaTime;
        if (distTraveled >= range)
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
                enemy.disableResist = true;
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
